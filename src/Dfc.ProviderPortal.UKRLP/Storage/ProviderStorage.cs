
using System;
using System.Collections.Generic;
using System.Text;
using Dfc.ProviderPortal.Providers;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
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
        /// <param name="log">ILogger for logging info/errors</param>
        public async Task<bool> InsertDocs(IEnumerable<ProviderService.ProviderRecordStructure> providers, ILogger log)
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
                log.LogError(ex, $"Exception rasied at: {DateTime.Now}\n {be.Message}");
                throw;
            } catch (Exception ex) {
                Exception be = ex.GetBaseException();
                log.LogError(ex, $"Exception rasied at: {DateTime.Now}\n {be.Message}");
                throw;
            }
            finally {
            }
            return true;
        }

        /// <summary>
        /// Gets all documents from the collection and returns the data as Provider objects
        /// </summary>
        /// <param name="log">ILogger for logging info/errors</param>
        public async Task<IEnumerable<Provider>> GetAll(ILogger log)
        {
            // Get all provider documents in the collection
            log.LogInformation("Getting all providers from collection");
            Task<FeedResponse<dynamic>> task = client.ReadDocumentFeedAsync(Collection.SelfLink, new FeedOptions { MaxItemCount = -1 });
            FeedResponse<dynamic> response = await task;

            // Collections are schema-less and can therefore hold any data, even though we're only storing Provider docs
            // So we can cast the returned data by serializing to json and then deserialising into Provider objects
            log.LogInformation($"Serializing data for {response.LongCount()} providers");
            string json = JsonConvert.SerializeObject(response);
            return JsonConvert.DeserializeObject<IEnumerable<Provider>>(json);
        }

        /// <summary>
        /// Gets all documents with matching PRN from the collection and returns the data as Provider objects
        /// </summary>
        /// <param name="PRN">UKPRN to search by</param>
        /// <param name="log">ILogger for logging info/errors</param>
        public Provider GetByPRN(string PRN, ILogger log)
        {
            // Get matching provider by PRN from the collection
            log.LogInformation($"Getting providers from collection with PRN {PRN}");
            return client.CreateDocumentQuery<Provider>(Collection.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                         .Where(p => p.UnitedKingdomProviderReferenceNumber == PRN)
                         .AsEnumerable()
                         .FirstOrDefault();
        }

        /// <summary>
        /// Gets all documents with partial matching Name from the collection and returns the data as Provider objects
        /// </summary>
        /// <param name="Name">Name fragment to search by</param>
        /// <param name="log">ILogger for logging info/errors</param>
        public IEnumerable<Provider> GetByName(string Name, ILogger log, out long count)
        {
            // Get matching provider by passed fragment of Name from the collection
            log.LogInformation($"Getting providers from collection matching Name {Name}");
            IQueryable<Provider> qry = client.CreateDocumentQuery<Provider>(Collection.SelfLink, new FeedOptions { EnableCrossPartitionQuery = true, MaxItemCount = -1 })
                                             .Where(p => p.ProviderName.ToLower().Contains(Name.ToLower()));
            IEnumerable<Provider> matches = qry.AsEnumerable();
            count = matches.LongCount();
            return matches;
        }
    }
}
