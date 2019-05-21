using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.UKRLP.Models
{
    public interface IVerificationdetail
    {
        string VerificationAuthority { get; set; }
        string VerificationID { get; set; }
    }
}
