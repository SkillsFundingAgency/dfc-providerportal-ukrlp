using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using UKRLP.ProviderSynchronise;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Dfc.ProviderPortal.Providers
{
    public static class ProviderSync
    {
        
        [FunctionName("SyncProviders")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage  req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

           
            ProviderSynchronise ps = new ProviderSynchronise();

            string output = ps.SynchroniseProviders();

            //JsonConvert.DeserializeObject<Provider>(output);

            var listProviders = JsonConvert.DeserializeObject<IEnumerable<Provider>>(output);

            return req.CreateResponse<string>(HttpStatusCode.OK, output);
        }


    }
}
