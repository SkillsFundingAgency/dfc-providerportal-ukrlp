
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
        static public DocumentClient DocumentClient = new DocumentClient(new Uri(JsonSettings.GetSetting("Storage:StorageURI")),
                                                                         JsonSettings.GetSetting("Storage:PrimaryKey")
                                                                        );
        // Find collection to query
        static public DocumentCollection DocumentCollection = GetDocumentCollectionAsync().Result;

        static public async Task<ResourceResponse<DocumentCollection>> GetDocumentCollectionAsync()
        {
            Task<ResourceResponse<DocumentCollection>> task = DocumentClient.ReadDocumentCollectionAsync(
                                                                    UriFactory.CreateDocumentCollectionUri(JsonSettings.GetSetting("Storage:Database"),
                                                                                                           JsonSettings.GetSetting("Storage:Collection")
                                                                                                         ));
            return task.Result;
        }
    }
}
