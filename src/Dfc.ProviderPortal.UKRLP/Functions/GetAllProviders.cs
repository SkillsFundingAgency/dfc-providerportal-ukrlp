
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.Text;
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
                                                          TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed GetAllProviders request");
            Task<IEnumerable<Providers.Provider>> task = new ProviderStorage().GetAll(log);
            task.Wait();

            // Return results
            log.Info($"GetAllProviders returning results");
            //return req.CreateResponse<string>(HttpStatusCode.OK, JsonConvert.SerializeObject(task.Result));
            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(task.Result), Encoding.UTF8, "application/json");
            return response;
        }
    }
}
