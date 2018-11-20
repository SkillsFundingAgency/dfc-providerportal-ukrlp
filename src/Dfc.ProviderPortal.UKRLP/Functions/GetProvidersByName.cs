
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
    public static class GetProvidersByName
    {
        private class PostData {
            public string Name { get; set; }
        }

        [FunctionName("GetProvidersByName")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
                                                          TraceWriter log)
        {
            // Get passed argument (from query if present, if from JSON posted in body if not)
            string name = req.RequestUri.ParseQueryString()["Name"]?.ToString()
                            ?? (await req.Content.ReadAsAsync<PostData>())?.Name;
            if (name == null)
                throw new FunctionException("Missing name argument", "GetProviderByName", null);

            // Find matching providers
            log.Info($"C# HTTP trigger function processed GetProviderByName request for '{name}'");
            IEnumerable<Providers.Provider> p = new ProviderStorage().GetByName(name, log, out long count);

            // Return results
            log.Info($"GetProviderByName returning {count} matching providers");
            //return req.CreateResponse<string>(HttpStatusCode.OK, JsonConvert.SerializeObject(p));
            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json");
            return response;
        }
    }
}
