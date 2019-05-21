
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Dfc.ProviderPortal.Providers;


namespace Dfc.ProviderPortal.UKRLP.API.Controllers
{
    /// <summary>
    /// Controller containing one stub method per Azure function
    /// Allows Swashbuckle to generate the swagger doc for the Function App
    /// </summary>

    [Route("api")]
    [ApiController]
    public class UKRLPFunctionAppSwaggerGenController : ControllerBase
    {
        /// <summary />
        [HttpGet("GetAllProviders", Name = "GetAllProviders")]
        public ActionResult<IEnumerable<Provider>> GetAllProviders()
        {
            return new ActionResult<IEnumerable<Provider>>(new Provider[] {
                new Provider(new Providercontact[] { }, new Provideralias[] { }, new Verificationdetail[] { }) { id = Guid.NewGuid(), ProviderName = "GetAllProviders" },
                new Provider(new Providercontact[] { }, new Provideralias[] { }, new Verificationdetail[] { }) { id = Guid.NewGuid(), ProviderName = "GetAllProviders" }
            });
        }

        /// <summary />
        [HttpGet("GetProviderByPRN", Name = "GetProviderByPRN")]
        public ActionResult<Provider> GetProviderByPRN(int PRN)
        {
            return new ActionResult<Provider>(
                new Provider(new Providercontact[] { }, new Provideralias[] { }, new Verificationdetail[] { })
                            { id = Guid.NewGuid(), ProviderName = "GetProviderByPRN", ProviderStatus = PRN.ToString() }
            );
        }

        /// <summary />
        [HttpGet("GetProvidersByName", Name = "GetProvidersByName")]
        public ActionResult<Provider> GetProvidersByName(string Name)
        {
            return new ActionResult<Provider>(
                new Provider(new Providercontact[] { }, new Provideralias[] { }, new Verificationdetail[] { })
                            { id = Guid.NewGuid(), ProviderName = "GetProvidersByName", ProviderStatus = Name }
            );
        }

        /// <summary />
        [HttpPost("UpdateProviderById", Name = "UpdateProviderById")]
        public ActionResult<Document> UpdateProviderById(ProviderAdd provider)
        {
            return new ActionResult<Document>(
                new Document()
            );
        }
    }
}
