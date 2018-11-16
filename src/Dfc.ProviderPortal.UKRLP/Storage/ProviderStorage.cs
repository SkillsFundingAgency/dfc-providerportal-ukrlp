
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
        static private DocumentClient client = StorageFactory.DocumentClient;
        static private DocumentCollection Collection = StorageFactory.DocumentCollection;

        /// <summary>
        /// Public constructor
        /// </summary>
        public ProviderStorage() { }

        /// <summary>
        /// Inserts passed objects as documents into CosmosDB collection
        /// </summary>
        /// <param name="providers">Provider data from service</param>
        /// <param name="log">TraceWriter for logging info/errors</param>
        public async Task<bool> InsertDocs(IEnumerable<ProviderService.ProviderRecordStructure> providers, TraceWriter log)
        {
            // Insert documents into collection
            try {
                //Task<ResourceResponse<Document>> task = null;
                //Task[] tasks = new Task[providers.Count()];
                //int i = 0;

                // Insert each provider in turn as a document
                string database = SettingsHelper.Database;
                string collection = SettingsHelper.Collection;
                foreach (ProviderService.ProviderRecordStructure p in providers) {
                    Task<ResourceResponse<Document>> task = client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(database, collection),
                                                                                       p);
                    // TODO: Change to asynch operation
                    // If we make too many attempts too quickly we and use Task.WaitAll below then hundreds of "Request rate is large" exceptions thrown
                    await task;
                    //tasks[i++] = task;
                }

                // Wait for all tasks to complete
                //Task.WaitAll(tasks);
            }
            catch (DocumentClientException ex) {
                Exception be = ex.GetBaseException();
                log.Error($"Exception rasied at: {DateTime.Now}\n {be.Message}", ex);
                throw;
            } catch (Exception ex) {
                Exception be = ex.GetBaseException();
                log.Error($"Exception rasied at: {DateTime.Now}\n {be.Message}", ex);
                throw;
            }
            finally {
            }
            return true;
        }

        /// <summary>
        /// Gets all documents from the collection and returns the data as Provider objects
        /// </summary>
        /// <param name="log">TraceWriter for logging info/errors</param>
        public async Task<IEnumerable<Provider>> GetAll(TraceWriter log)
        {
            // Get all provider documents in the collection
            log.Info("Getting all providers from collection");
            Task<FeedResponse<dynamic>> task = client.ReadDocumentFeedAsync(Collection.SelfLink, new FeedOptions { MaxItemCount = -1 });
            FeedResponse<dynamic> response = await task;

            // Collections are schema-less and can therefore hold any data, even though we're only storing Provider docs
            // So we can cast the returned data by serializing to json and then deserialising into Provider objects
            log.Info($"Serializing data for {response.LongCount()} providers");
            string json = JsonConvert.SerializeObject(response);
            return JsonConvert.DeserializeObject<IEnumerable<Provider>>(json);
        }

        /// <summary>
        /// Gets all documents with matching PRN from the collection and returns the data as Provider objects
        /// </summary>
        /// <param name="PRN">UKPRN to search by</param>
        /// <param name="log">TraceWriter for logging info/errors</param>
        public Provider GetByPRN(string PRN, TraceWriter log)
        {
            try {
                // Get matching provider by PRN from the collection
                log.Info($"Getting providers from collection with PRN {PRN}");

                string uri = SettingsHelper.StorageURI;
                log.Info($"Using URI ending {uri.Substring(uri.Length - 15)}");

                string pk = SettingsHelper.PrimaryKey;
                //log.Info($"Using PK ending {pk.Substring(pk.Length - 6)}");

                string dbname = SettingsHelper.Database;
                log.Info($"Using database starting {dbname.Substring(0,3)}");

                string colname = SettingsHelper.Collection;
                log.Info($"Using collection starting {colname.Substring(0, 3)}");

                DocumentClient cli = new DocumentClient(new Uri(uri), pk);
                log.Info($"Using DocumentClient with hash {cli.GetHashCode().ToString()}");

                Task<ResourceResponse<DocumentCollection>> task = cli.ReadDocumentCollectionAsync(
                                                                        UriFactory.CreateDocumentCollectionUri(dbname, colname));
                task.Wait();
                DocumentCollection dc = task.Result;
                log.Info($"Using DocumentCollection with SelfLink {dc.SelfLink}");

                FeedOptions fo = new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 };
                log.Info($"Using FeedOptions with hash {fo.GetHashCode().ToString()}");

                IOrderedQueryable<Provider> q = cli.CreateDocumentQuery<Provider>(dc.SelfLink, fo);
                log.Info($"IQueryable created with hash {q.GetHashCode().ToString()}");

                Provider p = q.Where(r => r.UnitedKingdomProviderReferenceNumber == PRN)
                             .AsEnumerable()
                             .FirstOrDefault();
                log.Info($"ProviderStorage returning provider with name '{p.ProviderName}'");
                return p;

            } catch (Exception ex) {
                log.Error("Exception thrown in GetByPRN", ex, "GetByPRN");
                throw ex;
            }

            //return client.CreateDocumentQuery<Provider>(Collection.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
            //             .Where(p => p.UnitedKingdomProviderReferenceNumber == PRN)
            //             .AsEnumerable()
            //             .FirstOrDefault();
        }

        /// <summary>
        /// Gets all documents with partial matching Name from the collection and returns the data as Provider objects
        /// </summary>
        /// <param name="Name">Name fragment to search by</param>
        /// <param name="log">TraceWriter for logging info/errors</param>
        public IEnumerable<Provider> GetByName(string Name, TraceWriter log, out long count)
        {
            // Get matching provider by passed fragment of Name from the collection
            log.Info($"Getting providers from collection matching Name {Name}");
            IQueryable<Provider> qry = client.CreateDocumentQuery<Provider>(Collection.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                                             .Where(p => p.ProviderName.ToLower().Contains(Name.ToLower()));
            IEnumerable<Provider> matches = qry.AsEnumerable();
            count = matches.LongCount();
            return matches;
        }
    }
}
