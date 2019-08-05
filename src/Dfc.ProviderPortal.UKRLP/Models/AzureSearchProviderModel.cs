
using System;
using Dfc.ProviderPortal.UKRLP.Models;


namespace Dfc.ProviderPortal.Providers
{

    public class AzureSearchProviderModel
    {
        public Guid? id { get; set; }
        public int UnitedKingdomProviderReferenceNumber { get; set; }
        public string ProviderName { get; set; }
        public Status Status { get; set; }
        public string ProviderStatus { get; set; }
        public string CourseDirectoryName { get; set; }
        public string TradingName { get; set; }
        public string ProviderAlias { get; set; }
        
    }

}
