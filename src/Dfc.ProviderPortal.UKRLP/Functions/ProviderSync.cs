
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
    public static class ProviderSync
    {
        [FunctionName("SyncProviders")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
                                                          ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            ProviderSynchronise ps = new ProviderSynchronise();
            ProviderService.ProviderRecordStructure[] output = ps.SynchroniseProviders();

            Task<bool> task = new ProviderStorage().InsertDocs(output, log);
            task.Wait();
            return req.CreateResponse<string>(HttpStatusCode.OK, JsonConvert.SerializeObject(output));
        }
    }
}
