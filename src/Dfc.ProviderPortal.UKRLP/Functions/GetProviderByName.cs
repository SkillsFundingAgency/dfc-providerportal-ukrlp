
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
    public static class GetProviderByName
    {
        [FunctionName("GetProviderByName")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
                                                          ILogger log)
        {
            // Check argument
            if (req.RequestUri.ParseQueryString()["Name"] == null)
                throw new FunctionException("Missing Name argument", "GetProviderByName", null);

            // Get argument
            log.LogInformation($"C# HTTP trigger function processed GetProviderByName request for '{{Name}}'");
            string Name = req.RequestUri.ParseQueryString()["Name"].ToString();

            // Return matching providers
            IEnumerable<Providers.Provider> p = new ProviderStorage().GetByName(Name, log, out long count);
            log.LogInformation($"GetProviderByName returning {count} matching providers");
            return req.CreateResponse<string>(HttpStatusCode.OK, JsonConvert.SerializeObject(p));
        }
    }
}
