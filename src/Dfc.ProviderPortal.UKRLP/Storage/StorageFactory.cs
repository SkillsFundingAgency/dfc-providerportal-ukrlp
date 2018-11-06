
using System;
using Microsoft.Azure.Documents.Client;


namespace UKRLP.Storage
{
    public static class StorageFactory
    {
        // TODO: Move to settings
        private const string uriStorage = "https://dfc-dev-providerportal-cdb.documents.azure.com/"; //;AccountKey=%3C%3Ckey%3E%3E;";
        private const string key = "qXsndvyi2W1bmM8NL2pyYC3Q6w8rLFhrlYmYwK4NzXIUi5jbvN7JlUkAGWa8ZeT0un9JMzxz94MdVS48c5CNIg==";

        // Cosmos connection
        static public DocumentClient DocumentClient = new DocumentClient(new Uri(uriStorage), key);
    }
}
