
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using UKRLP.ProviderSynchronise;
using Newtonsoft.Json;
using UKRLP.Storage;


namespace Dfc.ProviderPortal.Providers
{
    public static class GetProviderByPRN
    {
        [FunctionName("GetProviderByPRN")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
                                                          ILogger log)
        {
            // Check argument
            if (req.RequestUri.ParseQueryString()["PRN"] == null)
                throw new FunctionException("Missing PRN argument", "GetProviderByPRN", null);

            // Get argument
            string PRN = req.RequestUri.ParseQueryString()["PRN"].ToString();
            log.LogInformation($"C# HTTP trigger function processed GetProviderByPRN request for PRN {{PRN}}");

            // Return matching providers
            Providers.Provider p = new ProviderStorage().GetByPRN(PRN, log);
            return req.CreateResponse<string>(HttpStatusCode.OK, JsonConvert.SerializeObject(p));
        }
    }
}
