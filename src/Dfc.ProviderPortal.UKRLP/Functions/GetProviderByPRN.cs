
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
        private class PostData {
            public string PRN { get; set; }
        }

        [FunctionName("GetProviderByPRN")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
                                                          TraceWriter log)
        {
            // Get passed argument (from query if present, if from JSON posted in body if not)
            log.Info($"GetProviderByPRN starting");
            string PRN = req.RequestUri.ParseQueryString()["PRN"]?.ToString()
                            ?? (await req.Content.ReadAsAsync<PostData>())?.PRN;
            if (PRN == null)
                throw new FunctionException("Missing PRN argument", "GetProviderByPRN", null);

            // Return matching providers
            log.Info($"C# HTTP trigger function processed GetProviderByPRN request for PRN '{PRN}'");
            Providers.Provider p = new ProviderStorage().GetByPRN(PRN, log);
            log.Info($"GetProviderByPRN request ending");
            return req.CreateResponse<string>(HttpStatusCode.OK, JsonConvert.SerializeObject(p));
        }
    }
}
