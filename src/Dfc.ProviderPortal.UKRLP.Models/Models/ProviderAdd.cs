
using System;


namespace Dfc.ProviderPortal.Providers
{
    public class ProviderAdd // : ValueObject<ProviderAdd>, IProviderAdd
    {
        public Guid id { get; set; }
        public int Status { get; set; }
        public string UpdatedBy { get; set; }

        //public ProviderAdd(
        //    Guid _id,
        //int status,
        //string updatedBy)
        //{
        //    Throw.IfNull(_id, nameof(_id));
        //    Throw.IfNull(status, nameof(status));
        //    Throw.IfNullOrWhiteSpace(updatedBy, nameof(updatedBy));

        //    id = _id;
        //    Status = status;
        //    UpdatedBy = updatedBy;
        //}

        //protected override IEnumerable<object> GetEqualityComponents()
        //{
        //    yield return id;
        //    yield return Status;
        //    yield return UpdatedBy;
        //}
    }
}
