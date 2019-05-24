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
        public IActionResult UpdateProviderById(Provider provider, [Required]string code)
        {
            return Ok();
        }

        [Route("UpdateProviderDetails")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateProviderDetails(Provider provider, [Required]string code)
        {
            return Ok();
        }

        [Route("GetAllProviders")]
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<Provider>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetAllProviders([Required]string code)
        {
            return Ok();
        }

        [Route("GetProviderByPRN")]
        [HttpGet]
        [ProducesResponseType(typeof(Provider), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProviderByPRN(string PRN, [Required]string code)
        {
            return Ok();
        }

        [Route("GetProvidersByName")]
        [HttpGet]
        [ProducesResponseType(typeof(Provider), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProvidersByName(string Name, [Required]string code)
        {
            return Ok();
        }

        [Route("SetProviderDefaultDateUpdated")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Provider>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProvidersByName([Required]string code)
        {
            return Ok();
        }

        //[Route("DownloadProvidersAsJson")]
        //[HttpGet]
        //[ProducesResponseType(typeof(Stream), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public IActionResult DownloadProvidersAsJson([Required]string code)
        //{
        //    return Ok();
        //}

        [Route("SyncProviders")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProviderRecordStructure>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult SyncProviders([Required]string code)
        {
            return Ok();
        }

        [Route("GetLiveProvidersForAzureSearch")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AzureSearchProviderModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetLiveProvidersForAzureSearch([Required]string code)
        {
            return Ok();
        }
    }
}
