
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using UKRLP.Storage;


namespace Dfc.ProviderPortal.Providers
{
    public static class UpdateProviderById
    {
        [FunctionName("UpdateProviderById")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
                                                          ILogger log)
        {
            Provider provider = null;
            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.InternalServerError);

            try {
                // Get passed argument from JSON posted in body if not)
                log.LogInformation($"UpdateProviderById starting");
                dynamic parms = await (dynamic)req.Content.ReadAsAsync<object>();
                string Id = req.RequestUri.ParseQueryString()["id"]?.ToString() ?? parms?.id;
                string status = req.RequestUri.ParseQueryString()["status"]?.ToString() ?? parms?.Status;
                string updatedby = req.RequestUri.ParseQueryString()["updatedby"]?.ToString() ?? parms?.UpdatedBy;

                if (Id == null)
                    response = req.CreateResponse(HttpStatusCode.BadRequest, ResponseHelper.ErrorMessage("Missing id argument"));
                else if (!Guid.TryParse(Id, out Guid parsedId))
                    response = req.CreateResponse(HttpStatusCode.BadRequest, ResponseHelper.ErrorMessage("Invalid id argument"));
                else if (string.IsNullOrEmpty(status))
                    response = req.CreateResponse(HttpStatusCode.BadRequest, ResponseHelper.ErrorMessage("Missing status argument"));
                else if (!int.TryParse(status, out int parsedStatus))
                    response = req.CreateResponse(HttpStatusCode.BadRequest, ResponseHelper.ErrorMessage("Invalid status argument"));
                else if (parsedStatus != (int)Status.Registered && parsedStatus != (int)Status.Onboarded && parsedStatus != (int)Status.Unregistered)
                    response = req.CreateResponse(HttpStatusCode.BadRequest, ResponseHelper.ErrorMessage("Invalid status argument"));
                else if (string.IsNullOrEmpty(updatedby))
                    response = req.CreateResponse(HttpStatusCode.BadRequest, ResponseHelper.ErrorMessage("Missing UpdatedBy argument"));
                else
                {
                    // Insert data as new document in collection
                    provider = new Provider(null, null, null) { id = parsedId, Status = (Status)parsedStatus, UpdatedBy = updatedby };
                    Document result = await new ProviderStorage().UpdateDocAsync(provider, log);
                    if (result == null)
                        response = req.CreateResponse(HttpStatusCode.BadRequest,
                                                      ResponseHelper.ErrorMessage($"Cannot update document with id {provider?.id}"));
                    else
                        response = req.CreateResponse(HttpStatusCode.OK);

                    // Return results
                    provider = (dynamic)result;
                    response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                }
            } catch (Exception ex) {
                throw ex;
            }
            return response;
        }
    }
}
