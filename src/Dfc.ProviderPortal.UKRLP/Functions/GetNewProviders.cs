
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
    public static class GetNewProviders
    {
        private class PostData {
            public string searchdate { get; set; }
        }

        [FunctionName("GetNewProviders")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
                                                          ILogger log)
        {
            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.InternalServerError);

            try {
                // Get passed argument (from query if present, if from JSON posted in body if not)
                log.LogInformation($"GetNewProviders starting");
                string searchdate = req.RequestUri.ParseQueryString()["searchdate"]?.ToString()
                                            ?? (await req.Content.ReadAsAsync<PostData>())?.searchdate;

                if (searchdate == null)
                    response = req.CreateResponse(HttpStatusCode.BadRequest, ResponseHelper.ErrorMessage("Missing searchdate argument"));
                else if (!DateTime.TryParse(searchdate, out DateTime parsed))
                    response = req.CreateResponse(HttpStatusCode.BadRequest, ResponseHelper.ErrorMessage("Invalid searchdate argument"));
                else {
                    // Find matching providers
                    log.LogInformation($"C# HTTP trigger function processed GetNewProviders request for '{searchdate}'");
                    IEnumerable<Providers.Provider> p = new ProviderStorage().GetNewProviders(parsed, log, out long count);

                    // Return results
                    log.LogInformation($"GetNewProviders returning {count} matching providers");
                    //return req.CreateResponse<string>(HttpStatusCode.OK, JsonConvert.SerializeObject(p));
                    response = req.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(p), Encoding.UTF8, "application/json");
                    return response;
                }
            } catch (Exception ex) {
                throw ex;
            }
            return response;
        }
    }
}
