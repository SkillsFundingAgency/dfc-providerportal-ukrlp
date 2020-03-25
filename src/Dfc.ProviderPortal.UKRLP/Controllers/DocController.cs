using Dfc.ProviderPortal.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProviderService;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;

namespace Dfc.ProviderPortal.UKRLP.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    [ApiController]
    public class DocController : ControllerBase
    {
        [Route("UpdateProviderById")]
        [HttpPost]
        [ProducesResponseType(typeof(Provider), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProviderById(Provider provider)
        {
            return Ok();
        }

        [Route("UpdateProviderDetails")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProviderDetails(Provider provider)
        {
            return Ok();
        }

        [Route("GetAllProviders")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Provider>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllProviders()
        {
            return Ok();
        }

        [Route("GetProviderByPRN")]
        [HttpGet]
        [ProducesResponseType(typeof(Provider), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProviderByPRN(string PRN)
        {
            return Ok();
        }

        [Route("GetProvidersByName")]
        [HttpGet]
        [ProducesResponseType(typeof(Provider), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProvidersByName(string Name)
        {
            return Ok();
        }

        [Route("SetProviderDefaultDateUpdated")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Provider>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SetProviderDefaultDateUpdated()
        {
            return Ok();
        }

        [Route("SyncProviders")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProviderRecordStructure>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SyncProviders()
        {
            return Ok();
        }

        [Route("GetLiveProvidersForAzureSearch")]
        [HttpGet]
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<AzureSearchProviderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetLiveProvidersForAzureSearch()
        {
            return Ok();
        }
    }
}
