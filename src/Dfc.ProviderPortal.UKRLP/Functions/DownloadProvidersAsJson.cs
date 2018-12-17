
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UKRLP.ProviderSynchronise;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace Dfc.ProviderPortal.Providers
{
    public static class DownloadProvidersAsJson
    {
        [FunctionName("DownloadProvidersAsJson")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req,
                                                          ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            ProviderSynchronise ps = new ProviderSynchronise();
            List<ProviderService.ProviderRecordStructure> output = ps.SynchroniseProviders(DateTime.MinValue, log);

            //var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            //System.IO.File.WriteAllText("C:\out.json", serializer.Serialize(output));

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            //string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");
            //using (StreamWriter sw = new StreamWriter(path))
            //using (JsonWriter writer = new JsonTextWriter(bw))
            MemoryStream ms = new MemoryStream();
            using (StreamWriter sw = new StreamWriter(ms))
            using (JsonWriter writer = new JsonTextWriter(sw))
                serializer.Serialize(writer, output);

            //return req.CreateResponse<string>(HttpStatusCode.OK, JsonConvert.SerializeObject(output));
            return new FileContentResult(ms.ToArray(), "file/text") { FileDownloadName = "download.json" };
        }
    }
}
