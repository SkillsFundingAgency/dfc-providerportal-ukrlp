
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
        /// CosmosDB connection
        /// </summary>
        static private DocumentClient client = StorageFactory.DocumentClient;

        /// <summary>
        /// Public constructor
        /// </summary>
        public ProviderStorage() { }

        /// <summary>
        /// Inserts passed objects as documents into CosmosDB collection
        /// </summary>
        /// <param name="providers">Provider data from service</param>
        /// <param name="log">ILogger for logging info/errors</param>
        /// <returns></returns>
        public async Task<bool> InsertDocs(IEnumerable<ProviderService.ProviderRecordStructure> providers, ILogger log)
        {
            // Insert documents into collection
            try {
                Task<ResourceResponse<Document>> task = null;
                ResourceResponse<Document> createddocs;

                // Insert each provider in turn as a document
                string database = JsonSettings.GetSetting("Storage:Database");
                string collection = JsonSettings.GetSetting("Storage:Collection");
                foreach (ProviderService.ProviderRecordStructure p in providers) {
                    task = client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(database, collection),
                                                      p);
                }

                // Wait for the last to be inserted
                if (task != null)
                    createddocs = await task;
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
    }
}
