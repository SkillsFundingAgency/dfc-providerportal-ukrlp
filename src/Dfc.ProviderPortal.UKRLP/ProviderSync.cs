
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
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
            //string output = ps.SynchroniseProviders();
            ProviderService.ProviderRecordStructure[] output = ps.SynchroniseProviders();
            //JsonConvert.DeserializeObject<Provider>(output);

            Task<bool> task = new ProviderStorage().InsertDocs(output, log);
            task.Wait();

            //var listProviders = JsonConvert.DeserializeObject<IEnumerable<Provider>>(output);
            //return req.CreateResponse<string>(HttpStatusCode.OK, output);
            return req.CreateResponse<string>(HttpStatusCode.OK, JsonConvert.SerializeObject(output));
        }
    }
}
