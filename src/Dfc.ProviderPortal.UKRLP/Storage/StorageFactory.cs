
using System;
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
    }
}
