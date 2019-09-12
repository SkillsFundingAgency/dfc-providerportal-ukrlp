using System;

namespace Dfc.ProviderPortal.UKRLP.Models
{
    public class BulkUploadStatus
    {
        public bool InProgress { get; set; }
        public DateTime? StartedTimestamp { get; set; }
        public int? TotalRowCount { get; set; }
        public bool PublishInProgress { get; set; }
    }
}
