
using System;
using System.Threading.Tasks;
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
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            ProviderSynchronise ps = new ProviderSynchronise();
            ProviderService.ProviderRecordStructure[] output = ps.SynchroniseProviders();

            Task<bool> task = new ProviderStorage().InsertDocs(output, log);
            task.Wait();
        }
    }
}
