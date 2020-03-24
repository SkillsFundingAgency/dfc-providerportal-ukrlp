using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UKRLP.Storage;

namespace Dfc.ProviderPortal.Providers
{
    public static class GetActiveProviders
    {
        [FunctionName("GetActiveProviders")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req,
            ILogger log)
        {
            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.InternalServerError);

            try
            {
                log.LogInformation($"GetActiveProviders starting");
                var results = await new ProviderStorage().GetAllActive(log);

                // Return results
                response = req.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(results), Encoding.UTF8, "application/json");
                return response;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}