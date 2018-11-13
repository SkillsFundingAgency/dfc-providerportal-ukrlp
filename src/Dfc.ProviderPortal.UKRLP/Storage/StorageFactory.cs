
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
            Task<ResourceResponse<DocumentCollection>> task = DocumentClient.ReadDocumentCollectionAsync(
                                                                    UriFactory.CreateDocumentCollectionUri(SettingsHelper.Database,
                                                                                                           SettingsHelper.Collection
                                                                                                         ));
            return task.Result;
        }
    }
}
