
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using UKRLP.ProviderSynchronise;
using UKRLP.Storage;


namespace Dfc.ProviderPortal.UKRLP
{
    public static class SchedulesProviderSync
    {
        [FunctionName("ScheduledProviderDownload")]
        public static void Run([TimerTrigger("0 0 0 */1 * *")]TimerInfo myTimer, ILogger log) {     // Every 24 hrs normally
        //public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log) {   // Every minute for debug

            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            ProviderSynchronise ps = new ProviderSynchronise();
            List<ProviderService.ProviderRecordStructure> output = ps.SynchroniseProviders(log);

            log.LogInformation($"Inserting {output.Count} providers to CosmosDB providers collection");
            Task<bool> task = new ProviderStorage().InsertDocs(output, log);
        }
    }
}
