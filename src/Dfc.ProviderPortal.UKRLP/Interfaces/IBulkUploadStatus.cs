using System;

namespace Dfc.ProviderPortal.UKRLP.Models
{
    public interface IBulkUploadStatus
    {
        bool InProgress { get; set; }
        DateTime? StartedTimestamp { get; set; }
        int? TotalRowCount { get; set; }
    }
}
