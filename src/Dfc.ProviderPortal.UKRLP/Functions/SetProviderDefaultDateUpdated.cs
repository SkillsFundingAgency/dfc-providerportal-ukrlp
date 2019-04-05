
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UKRLP.ProviderSynchronise;
using UKRLP.Storage;


namespace Dfc.ProviderPortal.Providers
{
    public static class SetProviderDefaultDateUpdated
    {
        [FunctionName("SetProviderDefaultDateUpdated")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
                                                     ILogger log)
        {
            log.LogInformation("SetProviderDefaultDateUpdated HTTP trigger function processed a request.");
            var output = new ProviderStorage().SetDefaultDateUpdatedAsync(log); //, out long rowcount);
            output.Wait();
            return new JsonResult(output.Result);
        }
    }
}
