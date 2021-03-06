﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.UKRLP.Models
{
    public interface IContactpersonaldetails
    {
        string[] PersonNameTitle { get; set; }
        string[] PersonGivenName { get; set; }
        string PersonFamilyName { get; set; }
        object PersonNameSuffix { get; set; }
        object PersonRequestedName { get; set; }
    }
}
