
using System;
using System.Collections.Generic;
using System.Text;
using Dfc.ProviderPortal.Providers;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using UKRLP.ProviderSynchronise;
using Newtonsoft.Json;


namespace UKRLP.Storage
{
    public class ProviderStorage
    {
        // TODO: Move to settings
        private const string db = "ukrlp";
        private const string collection = "ukrlp";

        // CosmosDB connection
        static private DocumentClient client = StorageFactory.DocumentClient; // new DocumentClient(new Uri(uriStorage), key);

        /// <summary>
        /// Public constructor
        /// </summary>
        public ProviderStorage() { }

        public async Task<bool> InsertDocs(IEnumerable<ProviderService.ProviderRecordStructure> providers, ILogger log)
        {
            // Insert documents into collection
            try {
                Task<ResourceResponse<Document>> task = null;
                ResourceResponse<Document> createddocs;

                // Insert each provider in turn as a document
                foreach (ProviderService.ProviderRecordStructure p in providers) {
                    task = client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(db, collection),
                                                      p);
                }

                // Wait for the last to be inserted
                if (task != null)
                    createddocs = await task;
            }
            catch (DocumentClientException ex) {
                Exception be = ex.GetBaseException();
                log.LogError(ex, $"Exception rasied at: {DateTime.Now}\n {be.Message}");
            } catch (Exception ex) {
                Exception be = ex.GetBaseException();
                log.LogError(ex, $"Exception rasied at: {DateTime.Now}\n {be.Message}");
            }
            finally {
            }
            return true;
        }



        //public ProviderStorage(ProviderService.ProviderRecordStructure[] providers, ILogger log)
        //{
        //    test(providers, log);
        //}

        //private async Task test(ProviderService.ProviderRecordStructure[] providers, ILogger log)
        //{

        //    //// Get CosmosDB details from config
        //    //var config = new Configuration();
        //    //config.AddJsonFile("local.settings.json");
        //    //var temp = config.Get("StorageURI");

        //    // CosmosDB connection
        //    //client = new DocumentClient(new Uri(uriStorage), key);

        //    // Get collections
        //    Task<FeedResponse<DocumentCollection>> task1 = client.ReadDocumentCollectionFeedAsync(UriFactory.CreateDatabaseUri(db));
        //    FeedResponse<DocumentCollection> cols = await task1;

        //    //// Create database if not present
        //    //Task<ResourceResponse<Database>> task2 = client.CreateDatabaseIfNotExistsAsync(new Database { Id = db });
        //    //ResourceResponse<Database> rr = await task2;

        //    // Get documents
        //    Task<ResourceResponse<DocumentCollection>> task3 = client.ReadDocumentCollectionAsync(
        //                                                                    UriFactory.CreateDocumentCollectionUri(db, collection));
        //    ResourceResponse<DocumentCollection> docs = await task3;

        //    // Insert documents into collection
        //    Task<ResourceResponse<Document>> task4 = null;
        //    ResourceResponse<Document> createddocs;
        //    foreach (ProviderService.ProviderRecordStructure p in providers) {
        //        task4 = client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(db, collection),
        //                                                                                  p); // providers[0]);
        //    }
        //    if (task4 != null)
        //        createddocs = await task4;

        //    // Get all documents
        //    Task<FeedResponse<dynamic>> task5 = client.ReadDocumentFeedAsync("dbs/w35JAA==/colls/w35JAJWUuAI=/", new FeedOptions { MaxItemCount = 1000 });
        //    FeedResponse<dynamic> alldocs = await task5;
        //}

        /// <summary>
        /// Inserts passed providers into CosmosDB collection
        /// </summary>
        /// <param name="providers">Provider data from service</param>
        /// <param name="log">ILogger to use for logging</param>

        //private async Task SaveProviderDocAsync(string DBName, string collection, Dfc.ProviderPortal.Providers.Provider provider)
        //{
        //    try {
        //        //log
        //        await client.ReadDocumentAsync(UriFactory.CreateDocumentUri(DBName, collection, provider.UnitedKingdomProviderReferenceNumber));
        //        //log("Found {0}", provider.UnitedKingdomProviderReferenceNumber);
        //    } catch (DocumentClientException de) {
        //        if (de.StatusCode == HttpStatusCode.NotFound) {
        //            //log
        //            await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DBName, collection), provider);
        //            //log("Wrote {0}", provider.UnitedKingdomProviderReferenceNumber);
        //        } else {
        //            throw;
        //        }
        //    }
        //}

        //private async Task CreateProvidersCollectionAsync()
        //{
        //    await client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(db),
        //                                                          new DocumentCollection { Id = db });
        //}
    }
}
