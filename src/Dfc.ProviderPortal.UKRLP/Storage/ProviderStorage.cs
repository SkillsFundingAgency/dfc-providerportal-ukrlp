
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
                    await DeleteAndRecreateCollection(log); //TruncateCollection(log);

                //Task<ResourceResponse<Document>> task = null;
                //Task[] tasks = new Task[providers.Count()];
                int i = 0;

                // Insert each provider in turn as a document
                string database = SettingsHelper.Database;
                string collection = SettingsHelper.Collection;
                log.LogInformation($"Inserting {providers.Count()} provider documents");
                foreach (ProviderService.ProviderRecordStructure p in providers)
                {
                    Guid id = Guid.NewGuid();
                    DateTime? dateUpdated = null;
                    DateTime? dateOnboarded = null;
                    string whoUpdated = null;
                    Status status = Status.Registered; // default value
                    List<Task> tasks = new List<Task>();
                    i++;
                    log.LogInformation($"Processing provider {i} ({p.ProviderName})");

                    // Check for existing documents (unless we already know we've just deleted them all)
                    if (!EmptyCollectionFirst)
                    {
                        // TODO: Change to use faster Upsert (which currently errors as doesn't like using UKPRN as PartitionKey)
                        // Any docs with this PRN already in the database? Then delete them before re-adding the provider.
                        IEnumerable<Document> existing = docClient.CreateDocumentQuery<Document>(Collection.SelfLink,
                                                                            new SqlQuerySpec("SELECT * FROM ukrlp p WHERE p.UnitedKingdomProviderReferenceNumber = @UKPRN",
                                                                                             new SqlParameterCollection(new[] {
                                                                                                    new SqlParameter { Name = "@UKPRN", Value = p.UnitedKingdomProviderReferenceNumber }
                                                                                             })),
                                                                            new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                                                                  //.Where(s => s.GetPropertyValue<string>("UnitedKingdomProviderReferenceNumber") == p.UnitedKingdomProviderReferenceNumber)  // bang!
                                                                  .AsEnumerable();

                        // Get CD field values from any existing docs then delete them
                        if (existing.Any()) {
                            foreach (Document doc in existing) {
                                id = doc.GetPropertyValue<Guid>("id");
                                dateUpdated = doc.GetPropertyValue<DateTime?>("DateUpdated");
                                dateOnboarded = doc.GetPropertyValue<DateTime?>("DateOnboarded");
                                whoUpdated = doc.GetPropertyValue<string>("UpdatedBy");
                                status = (Status)doc.GetPropertyValue<int>("Status");
                                await docClient.DeleteDocumentAsync(doc.SelfLink);
                            }
                        }
                    }

                    // Add provider doc to collection
                    //Task<ResourceResponse<Document>> task = client.UpsertDocumentAsync(Collection.SelfLink,
                    //                                                                   p,
                    //                                                                   new RequestOptions { PartitionKey = new PartitionKey(p.UnitedKingdomProviderReferenceNumber) });

                    // Set CD field values, including Status, appropriately for deactivated/deactivating providers
                    //Provider provider = new Provider(null, null, null)
                    //{
                    //    id = Guid.NewGuid(),
                    //    DateDownloaded = DateTime.Now,
                    //    //ExpiryDate = p.ExpiryDate,
                    //    //ExpiryDateSpecified = p.ExpiryDateSpecified,
                    //    //ProviderAliases = p.ProviderAliases,
                    //    //ProviderAssociations = p.ProviderAssociations,
                    //    ProviderContact = null, //new Providercontact(new Contactaddress(), new Contactpersonaldetails()), //p.ProviderContact,
                    //    ProviderName = p.ProviderName,
                    //    ProviderStatus = p.ProviderStatus,
                    //    //ProviderVerificationDate = p.ProviderVerificationDate,
                    //    //ProviderVerificationDateSpecified = p.ProviderVerificationDateSpecified,
                    //    UnitedKingdomProviderReferenceNumber = p.UnitedKingdomProviderReferenceNumber //,
                    //    //VerificationDetails = p.VerificationDetails
                    //};

                    // Insert document
                    ResourceResponse<Document> response = await docClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(database, collection),
                                                                                              p);

                    //// TODO: Change to asynch operation
                    //// If we make too many attempts too quickly we and use Task.WaitAll below then hundreds of "Request rate is large" exceptions thrown
                    //task.Wait();
                    ////tasks[i++] = task;


                    //IEnumerable<Document> docs = docClient.CreateDocumentQuery<Document>(Collection.SelfLink,
                    //                                   new SqlQuerySpec("SELECT * FROM ukrlp p WHERE p.UnitedKingdomProviderReferenceNumber = @UKPRN",
                    //                                                    new SqlParameterCollection(new[] {
                    //                                                                                    new SqlParameter { Name = "@UKPRN", Value = p.UnitedKingdomProviderReferenceNumber }
                    //                                                    })),
                    //                new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                    //               //.Where(s => s.GetPropertyValue<string>("UnitedKingdomProviderReferenceNumber") == p.UnitedKingdomProviderReferenceNumber)  // bang!
                    //               .AsEnumerable();

                    // Change provider properties and save
                    //if (docs.Any()) {
                    Document d = response.Resource; // docs.First();
                        d.Id = id.ToString();
                        d.SetPropertyValue("DateUpdated", dateUpdated ?? new DateTime(2000, 1, 1));
                        d.SetPropertyValue("DateDownloaded", DateTime.Now);
                        if (dateOnboarded.HasValue)
                            d.SetPropertyValue("DateOnboarded", dateOnboarded);
                        if (!string.IsNullOrWhiteSpace(whoUpdated))
                            d.SetPropertyValue("UpdatedBy", whoUpdated);
                        if (p.ProviderStatus == "PD1" || p.ProviderStatus == "PD2")
                            d.SetPropertyValue("Status", Status.Unregistered);
                        else
                            d.SetPropertyValue("Status", status); // Status.Registered;
                        tasks.Add(docClient.ReplaceDocumentAsync(d.SelfLink, d));
                    //}

                    // Wait for all tasks to complete
                    Task.WaitAll(tasks.ToArray());
                }

            } catch (DocumentClientException ex) {
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


        /// <summary>
        /// Delete and recreate Cosmos DB providers collection (faster than deleting all documents)
        /// </summary>
        /// <param name="log">ILogger for logging info/errors</param>
        private async static Task DeleteAndRecreateCollection(ILogger log) //DocumentClient client, string collectionId)
        {
            try {
                // Use own DocumentClient, otherwise static helper class throws exception if collection doesn't exist
                using (DocumentClient client = new DocumentClient(new Uri(SettingsHelper.StorageURI),
                                                                          SettingsHelper.PrimaryKey
                                                                 ))
                {
                    // Delete collection if it exists
                    string collectionId = SettingsHelper.Collection;
                    Uri uriDB = UriFactory.CreateDatabaseUri(SettingsHelper.Database);
                    IEnumerable<DocumentCollection> collections = await client.ReadDocumentCollectionFeedAsync(uriDB);
                    if (collections.Select(c => c.Id).Contains(collectionId))
                    {
                        log.LogInformation($"Deleting collection {collectionId} from database {SettingsHelper.Database}");
                        await client.DeleteDocumentCollectionAsync(collections.First(c => c.Id == collectionId).SelfLink);
                        log.LogInformation("Deletion successful");
                    }

                    // Then recreate it
                    log.LogInformation($"Recreating collection {collectionId} in database {SettingsHelper.Database}");
                    await client.CreateDocumentCollectionAsync(uriDB, new DocumentCollection { Id = collectionId });
                    log.LogInformation("Creation successful");
                }

            } catch (Exception ex) {
                throw ex;
            }
        }



        /// <summary>
        /// Delete all providers from Cosmos DB collection
        /// </summary>
        /// <param name="log">ILogger for logging info/errors</param>
        private bool TruncateCollection(ILogger log)
        {
            try {
                log.LogInformation("Deleting all docs from providers collection");
                IEnumerable<Document> docs = docClient.CreateDocumentQuery<Document>(Collection.SelfLink,
                                                                                     new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                                                      .AsEnumerable<Document>();

                // Keep the count, saves constantly recounting collection
                int docCount = docs.Count();
                log.LogInformation($"Deleting {docCount} documents");

                //foreach (Document d in docs)
                //    await docClient.DeleteDocumentAsync(d.SelfLink);

                int batchsize = 40;
                for (int i = 0; i < docCount; i += batchsize)
                {
                    log.LogInformation($"Deleting next {batchsize} documents, batch {(int)(i / batchsize + 1)} of {(int)(docCount / batchsize + 0.5)}");
                    var tasks = docs.Skip(i)
                                    .Take(batchsize)
                                    .Select(async d => await docClient.DeleteDocumentAsync(d.SelfLink))
                                    .ToArray();
                    Task.WaitAll(tasks);
                }

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
                //string json = JsonConvert.SerializeObject(docs);
                //return JsonConvert.DeserializeObject<IEnumerable<Provider>>(json);
                return ((IEnumerable<dynamic>)docs).Select(d => (Provider)d);

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
                // Get matching provider by PRN from the collection //
                log.LogInformation($"Getting providers from collection with PRN {PRN}");
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
        /// Gets all documents for live providers from the collection and returns the data as Provider objects
        /// </summary>
        /// <param name="log">ILogger for logging info/errors</param>
        public IEnumerable<AzureSearchProviderModel> GetLiveProvidersForAzureSearch(ILogger log, out long count)
        {
            try {
                // Get live providers from the collection
                log.LogInformation($"Getting live providers from collection");
                IQueryable<Provider> qry = docClient.CreateDocumentQuery<Provider>(Collection.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                                                    .Where(p => p.Status == Status.Onboarded);
                IEnumerable<AzureSearchProviderModel> matches = qry.AsEnumerable()
                                                                   .Select(p => new AzureSearchProviderModel()
                                                                   {
                                                                       id = p.id,
                                                                       UnitedKingdomProviderReferenceNumber = int.Parse(p.UnitedKingdomProviderReferenceNumber),
                                                                       ProviderName = p.ProviderName,
                                                                       Status=p.Status,
                                                                       ProviderStatus=p.ProviderStatus,
                                                                       CourseDirectoryName=p.CourseDirectoryName,
                                                                       TradingName=p.TradingName
                                                                   });
                count = matches.LongCount();
                return matches;

            } catch (Exception ex) {
                log.LogError("Exception thrown in GetLiveProvidersForAzureSearch", ex, "GetLiveProvidersForAzureSearch");
                throw ex;
            }
        }

        /// <summary>
        /// Sets default DateUpdated value for all documents without one
        /// </summary>
        /// <param name="log">ILogger for logging info/errors</param>
        public async Task<IEnumerable<Provider>> SetDefaultDateUpdatedAsync(ILogger log) //, out long count)
        {
            try {
                // Get providers from the collection
                log.LogInformation($"Getting providers from collection");
                //IQueryable<Provider> qry = docClient.CreateDocumentQuery<Provider>(Collection.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                //                                    .Where(p => p.DateUpdated == null);
                Task<IEnumerable<Provider>> task = GetAll(log);
                task.Wait();
                IEnumerable<Provider> providers =  task.Result
                                                       .Where(p => p.DateUpdated == null || p.DateUpdated == DateTime.MinValue);

                Uri uri = UriFactory.CreateDocumentCollectionUri(SettingsHelper.Database, SettingsHelper.Collection);
                log.LogInformation($"Setting default DateUpdated for {providers.LongCount()} provider documents");
                foreach (Provider p in providers) { //qry.AsEnumerable()) {
                    if (p.DateUpdated < new DateTime(2000, 1, 1)) {
                        p.DateUpdated = new DateTime(2000, 1, 1);
                        //docClient.UpsertDocumentAsync(uri, p);
                        await UpdateDocAsync(p, log, false);
                    }
                }

                //count = providers.LongCount();
                return providers; // qry.AsEnumerable();

            } catch (Exception ex) {
                log.LogError("Exception thrown in SetDefaultDateUpdated", ex, "SetDefaultDateUpdated");
                throw ex;
            }
        }

        /// <summary>
        /// Updates a single venue document in the collection
        /// Currently only allow Status property to be changed
        /// </summary>
        /// <param name="provider">The Provider to update</param>
        /// <param name="log">ILogger for logging info/errors</param>
        public async Task<ResourceResponse<Document>> UpdateDocAsync(Provider provider, ILogger log, bool UpdateDateUpdated = true)
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
                if (UpdateDateUpdated) {
                    updated.SetPropertyValue("DateUpdated", DateTime.Now);
                    updated.SetPropertyValue("DateOnboarded", provider.DateOnboarded);
                } else
                    updated.SetPropertyValue("DateUpdated", provider.DateUpdated);
                return await docClient.UpsertDocumentAsync(Collection.SelfLink, updated);

            } catch (Exception ex) {
                throw ex;
            }
        }
        public async Task<ResourceResponse<Document>> UpdateFullDocAsync(Provider provider, ILogger log, bool UpdateDateUpdated = true)
        {
            try
            {
                // Get matching venue by Id from the collection
                log.LogInformation($"Getting provider from collection with Id {provider?.id}");
                Document exists = docClient.CreateDocumentQuery(Collection.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                                            .Where(u => u.Id == provider.id.ToString())
                                            .AsEnumerable()
                                            .FirstOrDefault();
                if (exists == null)
                    return null;

                return await docClient.UpsertDocumentAsync(Collection.SelfLink, provider);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
