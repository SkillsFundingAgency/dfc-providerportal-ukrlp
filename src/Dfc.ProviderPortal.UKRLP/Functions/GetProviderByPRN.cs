
using System;
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
    public static class GetProviderByPRN
    {
        private class PostData {
            public string PRN { get; set; }
        }

        [FunctionName("GetProviderByPRN")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req,
                                                          ILogger log)
        {
            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.InternalServerError);

            try {
                // Get passed argument (from query if present, if from JSON posted in body if not)
                log.LogInformation($"GetProviderByPRN starting");
                string PRN = req.RequestUri.ParseQueryString()["PRN"]?.ToString()
                                ?? (await req.Content.ReadAsAsync<PostData>())?.PRN;

                if (PRN == null)
                    response = req.CreateResponse(HttpStatusCode.BadRequest, ResponseHelper.ErrorMessage("Missing PRN argument"));
                else if (!int.TryParse(PRN, out int parsed))
                    response = req.CreateResponse(HttpStatusCode.BadRequest, ResponseHelper.ErrorMessage("Invalid PRN argument"));
                else {
                    // Get data
                    Providers.Provider p = new ProviderStorage().GetByPRN(PRN, log);
                    log.LogInformation($"GetProviderByPRN returning { p?.ProviderName }");

                    // Return results
                    response = req.CreateResponse(p == null ? HttpStatusCode.NoContent : HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json");
                }
            } catch (Exception ex) {
                throw ex;
            }
            return response;
        }
    }
}
