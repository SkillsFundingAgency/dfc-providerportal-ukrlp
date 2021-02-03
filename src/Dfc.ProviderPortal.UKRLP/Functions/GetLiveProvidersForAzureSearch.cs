
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UKRLP.Storage;


namespace Dfc.ProviderPortal.Providers
{
    public static class GetLiveProvidersForAzureSearch
    {
        //private class PostData {
        //    public string PRN { get; set; }
        //}

        [FunctionName("GetLiveProvidersForAzureSearch")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req,
                                                          ILogger log)
        {
            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.InternalServerError);

            try {
                // Get data (only required fields to speed retrieval)
                log.LogInformation($"GetLiveProvidersForAzureSearch starting");
                IEnumerable<AzureSearchProviderModel> results = new ProviderStorage().GetLiveProvidersForAzureSearch(log, out long count);

                // Return results
                log.LogInformation($"GetLiveProvidersForAzureSearch returning {count} providers");
                response = req.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(results), Encoding.UTF8, "application/json");
                return response;

            } catch (Exception ex) {
                throw ex;
            }
            return response;
        }
    }
}
