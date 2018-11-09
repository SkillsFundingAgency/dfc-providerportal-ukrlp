
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
    public static class GetAllProviders
    {
        [FunctionName("GetAllProviders")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
                                                          ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed GetAllProviders request");
            Task<IEnumerable<Providers.Provider>> task = new ProviderStorage().GetAll(log);
            task.Wait();
            return req.CreateResponse<string>(HttpStatusCode.OK, JsonConvert.SerializeObject(task.Result));
        }
    }
}
