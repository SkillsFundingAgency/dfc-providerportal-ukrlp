using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using Dfc.ProviderPortal.Providers;
using UKRLP.Storage;
using Microsoft.Azure.Documents;
using System.Text;

namespace Dfc.ProviderPortal.UKRLP.Functions
{
    public static class UpdateProviderDetails
    {
        [FunctionName("UpdateProviderDetails")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req,
                                                          ILogger log)
        {
            Provider provider = await req.Content.ReadAsAsync<Provider>();
            HttpResponseMessage response = req.CreateResponse(HttpStatusCode.InternalServerError);

            try
            {
                // Get passed argument from JSON posted in body if not)
                log.LogInformation($"UpdateProviderDetails starting");

                if (provider.id == null || provider.id == Guid.Empty)
                    response = req.CreateResponse(HttpStatusCode.BadRequest, ResponseHelper.ErrorMessage("Missing or empty id argument"));
                else
                {

                    Document result = await new ProviderStorage().UpdateFullDocAsync(provider, log);
                    if (result == null)
                        response = req.CreateResponse(HttpStatusCode.BadRequest,
                                                      ResponseHelper.ErrorMessage($"Cannot update document with id {provider?.id}"));
                    else
                        response = req.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }
    }
}
