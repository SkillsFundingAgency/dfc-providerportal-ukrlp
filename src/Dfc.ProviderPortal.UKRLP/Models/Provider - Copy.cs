
using System;
using Dfc.ProviderPortal.UKRLP.Models;


namespace Dfc.ProviderPortal.Providers
{

    public class AzureSearchProviderModel
    {
        public Guid? id { get; set; }
        public int UnitedKingdomProviderReferenceNumber { get; set; }
        public string ProviderName { get; set; }
    }

}
