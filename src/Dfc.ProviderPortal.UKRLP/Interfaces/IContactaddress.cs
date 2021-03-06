﻿using Dfc.ProviderPortal.Providers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dfc.ProviderPortal.UKRLP.Models
{
    public interface IContactaddress
    {
        SAON SAON { get; set; }
        PAON PAON { get; set; }
        string StreetDescription { get; set; }
        object UniqueStreetReferenceNumber { get; set; }
        object Locality { get; set; }
        string[] Items { get; set; }
        int[] ItemsElementName { get; set; }
        object PostTown { get; set; }
        string PostCode { get; set; }
        object UniquePropertyReferenceNumber { get; set; }
    }
}
