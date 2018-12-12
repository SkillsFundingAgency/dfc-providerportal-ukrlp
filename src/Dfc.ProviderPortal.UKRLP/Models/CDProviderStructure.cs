
using Dfc.ProviderPortal.UKRLP.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace Dfc.ProviderPortal.Providers
{
    /// <summary>
    /// ProviderRecordStructure plus CD fields found in Provider class
    /// Needed to allow extra fields to be added before document is initially written
    /// </summary>
    public class CDProviderStructure : ProviderService.ProviderRecordStructure //, IProvider
    {
        public Status Status { get; set; }
        public DateTime DateDownloaded { get; set; }
        public DateTime DateOnboarded { get; set; }
        public DateTime DateUpdated { get; set; }
        public string UpdatedBy { get; set; }
    }
}
