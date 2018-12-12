
using System;
using System.Collections.Generic;
using System.Text;
using Dfc.ProviderPortal.Providers;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Dfc.ProviderPortal.UKRLP;
using Newtonsoft.Json;


namespace UKRLP.Storage
{
    public class ProviderStorage
    {
        /// <summary>
        /// CosmosDB client and collection
        /// </summary>
        static private DocumentClient docClient = StorageFactory.DocumentClient;
        static private DocumentCollection Collection = StorageFactory.DocumentCollection;

        /// <summary>
        /// Public constructor
        /// </summary>
        public ProviderStorage() { }

        /// <summary>
        /// Inserts passed objects as documents into CosmosDB collection
        /// </summary>
        /// <param name="providers">Provider data from service</param>
        /// <param name="log">ILogger for logging info/errors</param>
        public async Task<bool> InsertDocs(IEnumerable<ProviderService.ProviderRecordStructure> providers,
                                           ILogger log,
                                           bool EmptyCollectionFirst = false)
        {
            // Insert documents into collection
            try {
                // If we're initialising by syncing all providers, delete all collection docs first
                if (EmptyCollectionFirst)
                    await TruncateCollectionAsync(log);

                //Task<ResourceResponse<Document>> task = null;
                //Task[] tasks = new Task[providers.Count()];
                //int i = 0;

                // Insert each provider in turn as a document
                string database = SettingsHelper.Database;
                string collection = SettingsHelper.Collection;
                log.LogInformation($"Inserting {providers.Count()} provider documents");
                foreach (ProviderService.ProviderRecordStructure p in providers)
                {
                    DateTime? dateUpdated = null;
                    string whoUpdated = null;
                    Status status = Status.Registered; // default value

                    // Check for existing documents (unless we already know we've just deleted them all)
                    if (!EmptyCollectionFirst)
                    {
                        // TODO: Change to use faster Upsert (which currently errors as doesn't like using UKPRN as PartitionKey)
                        // Any docs with this PRN already in the database? Then delete them before re-adding the provider.
                        IEnumerable<Document> docs = docClient.CreateDocumentQuery<Document>(Collection.SelfLink,
                                                                               new SqlQuerySpec("SELECT * FROM ukrlp p WHERE p.UnitedKingdomProviderReferenceNumber = @UKPRN",
                                                                                                new SqlParameterCollection(new[] {
                                                                                                        new SqlParameter { Name = "@UKPRN", Value = p.UnitedKingdomProviderReferenceNumber }
                                                                                                })),
                                                            new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                                                           //.Where(s => s.GetPropertyValue<string>("UnitedKingdomProviderReferenceNumber") == p.UnitedKingdomProviderReferenceNumber)  // bang!
                                                           .AsEnumerable();

                        // Get CD field values from any existing docs then delete them
                        if (docs.Any()) {
                            foreach (Document d in docs) {
                                dateUpdated = d.GetPropertyValue<DateTime>("DateUpdated");
                                whoUpdated = d.GetPropertyValue<string>("UpdatedBy");
                                status = (Status)d.GetPropertyValue<int>("Status");
                                await docClient.DeleteDocumentAsync(d.SelfLink);
                            }
                        }
                    }

                    // Add provider doc to collection
                    //Task<ResourceResponse<Document>> task = client.UpsertDocumentAsync(Collection.SelfLink,
                    //                                                                   p,
                    //                                                                   new RequestOptions { PartitionKey = new PartitionKey(p.UnitedKingdomProviderReferenceNumber) });

                    // Set CD field values, including Status, appropriately for deactivated/deactivating providers
                    CDProviderStructure provider = new CDProviderStructure()
                    {
                        DateDownloaded = DateTime.Now,
                        ExpiryDate = p.ExpiryDate,
                        ExpiryDateSpecified = p.ExpiryDateSpecified,
                        ProviderAliases = p.ProviderAliases,
                        ProviderAssociations = p.ProviderAssociations,
                        ProviderContact = p.ProviderContact,
                        ProviderName = p.ProviderName,
                        ProviderStatus = p.ProviderStatus,
                        ProviderVerificationDate = p.ProviderVerificationDate,
                        ProviderVerificationDateSpecified = p.ProviderVerificationDateSpecified,
                        UnitedKingdomProviderReferenceNumber = p.UnitedKingdomProviderReferenceNumber,
                        VerificationDetails = p.VerificationDetails
                    };
                    if (!string.IsNullOrWhiteSpace(whoUpdated))
                        provider.UpdatedBy = whoUpdated;
                    if (dateUpdated.HasValue && dateUpdated.Value > DateTime.MinValue)
                        provider.DateUpdated = dateUpdated.Value;
                    if (p.ProviderStatus == "PD1" || p.ProviderStatus == "PD2")
                        provider.Status = Status.Unregistered;
                    else
                        provider.Status = status; // Status.Registered;

                    // Insert document
                    Task<ResourceResponse<Document>> task = docClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(database, collection),
                                                                                          provider);

                    // TODO: Change to asynch operation
                    // If we make too many attempts too quickly we and use Task.WaitAll below then hundreds of "Request rate is large" exceptions thrown
                    task.Wait();
                    //tasks[i++] = task;
                }

                // Wait for all tasks to complete
                //Task.WaitAll(tasks);
            }
            catch (DocumentClientException ex) {
                Exception be = ex.GetBaseException();
                log.LogError(ex, $"Exception rasied at: {DateTime.Now}\n {be.Message}");
                throw;
            } catch (Exception ex) {
                Exception be = ex.GetBaseException();
                log.LogError(ex, $"Exception rasied at: {DateTime.Now}\n {be.Message}");
                throw;
            }
            return true;
        }

        async private Task<bool> TruncateCollectionAsync(ILogger log)
        {
            try {
                log.LogInformation("Deleting all docs from providers collection");
                IEnumerable<Document> docs = docClient.CreateDocumentQuery<Document>(Collection.SelfLink,
                                                                                     new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                                                      .ToList();
                log.LogInformation($"Deleting {docs.Count()} documents");
                foreach (Document d in docs)
                    await docClient.DeleteDocumentAsync(d.SelfLink);
            } catch (Exception ex) {
                throw ex;
            }
            return true;
        }

        /// <summary>
        /// Gets all documents from the collection and returns the data as Provider objects
        /// </summary>
        /// <param name="log">ILogger for logging info/errors</param>
        public async Task<IEnumerable<Provider>> GetAll(ILogger log)
        {
            try {
                // Get all provider documents in the collection
                string token = null;
                Task<FeedResponse<dynamic>> task = null;
                List<dynamic> docs = new List<dynamic>();
                log.LogInformation("Getting all providers from collection");

                // Read documents in batches, using continuation token to make sure we get them all
                //Task<FeedResponse<dynamic>> task = client.ReadDocumentFeedAsync(Collection.SelfLink, new FeedOptions { MaxItemCount = -1 });
                //FeedResponse<dynamic> response = await task;
                do {
                    task = docClient.ReadDocumentFeedAsync(Collection.SelfLink, new FeedOptions { MaxItemCount = -1, RequestContinuation = token });
                    token = task.Result.ResponseContinuation;
                    log.LogInformation("Collating results");
                    docs.AddRange(task.Result.ToList());
                } while (token != null);


                // Collections are schema-less and can therefore hold any data, even though we're only storing Provider docs
                // So we can cast the returned data by serializing to json and then deserialising into Provider objects
                log.LogInformation($"Serializing data for {docs.LongCount()} providers");
                string json = JsonConvert.SerializeObject(docs);
                return JsonConvert.DeserializeObject<IEnumerable<Provider>>(json);

            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Gets all documents with matching PRN from the collection and returns the data as Provider objects
        /// </summary>
        /// <param name="PRN">UKPRN to search by</param>
        /// <param name="log">ILogger for logging info/errors</param>
        public Provider GetByPRN(string PRN, ILogger log)
        {
            try {
                //// Get matching provider by PRN from the collection //
                //log.Info($"Getting providers from collection with PRN {PRN}");

                //string uri = SettingsHelper.StorageURI;
                //log.Info($"Using URI ending {uri.Substring(uri.Length - 15)}");

                //string pk = SettingsHelper.PrimaryKey;
                ////log.Info($"Using PK ending {pk.Substring(pk.Length - 6)}");

                //string dbname = SettingsHelper.Database;
                //log.Info($"Using database starting {dbname.Substring(0,3)}");

                //string colname = SettingsHelper.Collection;
                //log.Info($"Using collection starting {colname.Substring(0, 3)}");

                //DocumentClient cli = new DocumentClient(new Uri(uri), pk);
                //log.Info($"Using DocumentClient with hash {cli.GetHashCode().ToString()}");

                //Task<ResourceResponse<DocumentCollection>> task = cli.ReadDocumentCollectionAsync(
                //                                                        UriFactory.CreateDocumentCollectionUri(dbname, colname));
                //task.Wait();
                //DocumentCollection dc = task.Result;
                //log.Info($"Using DocumentCollection with SelfLink {dc?.SelfLink}");

                //FeedOptions fo = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };
                //log.Info($"Using FeedOptions with hash {fo?.GetHashCode().ToString()}");

                //IOrderedQueryable<Provider> q = cli.CreateDocumentQuery<Provider>(dc?.SelfLink, fo);
                //log.Info($"IQueryable created with hash {q?.GetHashCode().ToString()}");

                //Provider p = q?.Where(r => r.UnitedKingdomProviderReferenceNumber == PRN)
                //             .AsEnumerable()
                //             .FirstOrDefault();
                //log.Info($"ProviderStorage returning provider with name '{p?.ProviderName}'");
                //return p;

                return docClient.CreateDocumentQuery<Provider>(Collection.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                                .Where(p => p.UnitedKingdomProviderReferenceNumber == PRN)
                                .AsEnumerable()
                                .FirstOrDefault();

            } catch (Exception ex) {
                log.LogError("Exception thrown in GetByPRN", ex, "GetByPRN");
                throw ex;
            }
        }

        /// <summary>
        /// Gets all documents with partial matching Name from the collection and returns the data as Provider objects
        /// </summary>
        /// <param name="Name">Name fragment to search by</param>
        /// <param name="log">ILogger for logging info/errors</param>
        public IEnumerable<Provider> GetByName(string Name, ILogger log, out long count)
        {
            try {
                // Get matching provider by passed fragment of Name from the collection
                log.LogInformation($"Getting providers from collection matching Name {Name}");
                IQueryable<Provider> qry = docClient.CreateDocumentQuery<Provider>(Collection.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                                                    .Where(p => p.ProviderName.ToLower().Contains(Name.ToLower()));
                IEnumerable<Provider> matches = qry.AsEnumerable();
                count = matches.LongCount();
                return matches;

            } catch (Exception ex) {
                log.LogError("Exception thrown in GetByName", ex, "GetByName");
                throw ex;
            }
        }

        /// <summary>
        /// Updates a single venue document in the collection
        /// Currently only allow Status property to be changed
        /// </summary>
        /// <param name="provider">The Provider to update</param>
        /// <param name="log">ILogger for logging info/errors</param>
        public async Task<ResourceResponse<Document>> UpdateDocAsync(Provider provider, ILogger log)
        {
            try
            {
                // Get matching venue by Id from the collection
                log.LogInformation($"Getting provider from collection with Id {provider?.id}");
                Document updated = docClient.CreateDocumentQuery(Collection.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                                            .Where(u => u.Id == provider.id.ToString())
                                            .AsEnumerable()
                                            .FirstOrDefault();

                if (updated == null)
                    return null;

                string ps = updated.GetPropertyValue<string>("ProviderStatus");
                if (!string.IsNullOrWhiteSpace(ps) && !ps.StartsWith("PD"))
                    updated.SetPropertyValue("Status", (int)provider.Status);
                updated.SetPropertyValue("UpdatedBy", provider.UpdatedBy);
                updated.SetPropertyValue("DateUpdated", DateTime.Now);
                updated.SetPropertyValue("DateOnboarded", provider.DateOnboarded);
                return await docClient.UpsertDocumentAsync(Collection.SelfLink, updated);

            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}
