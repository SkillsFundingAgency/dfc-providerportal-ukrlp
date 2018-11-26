
using System;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Dfc.ProviderPortal.UKRLP;


namespace UKRLP.Storage
{
    /// <summary>
    /// Factory class for Storage objects
    /// </summary>
    public static class StorageFactory
    {
        /// <summary>
        /// CosmosDB connection created using settings
        /// </summary>
        static public DocumentClient DocumentClient = new DocumentClient(new Uri(SettingsHelper.StorageURI),
                                                                         SettingsHelper.PrimaryKey
                                                                        );
        // Find collection to query
        static public DocumentCollection DocumentCollection = GetDocumentCollectionAsync().Result;

        static public async Task<ResourceResponse<DocumentCollection>> GetDocumentCollectionAsync()
        {
            try {
                //// Check whether database exists and create if not
                //Database db = await DocumentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = "CourseDirectory" });

                //// Same for collection
                //var task = DocumentClient.CreateDocumentCollectionIfNotExistsAsync(db.SelfLink,
                //                                                                   new DocumentCollection { Id = "Providers" });
                //return task.Result;

                Task<ResourceResponse<DocumentCollection>> task = DocumentClient.ReadDocumentCollectionAsync(
                                                                        UriFactory.CreateDocumentCollectionUri(SettingsHelper.Database,
                                                                                                               SettingsHelper.Collection
                                                                                                             ));
                return task.Result;

            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}
