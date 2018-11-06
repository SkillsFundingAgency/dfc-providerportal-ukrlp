
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
        public static void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            ProviderSynchronise ps = new ProviderSynchronise();
            //string output = ps.SynchroniseProviders();
            ProviderService.ProviderRecordStructure[] output = ps.SynchroniseProviders();

            //ProviderStorage store = new ProviderStorage(output, log);
            //store.InsertDocs(output, log);
            Task<bool> task = new ProviderStorage().InsertDocs(output, log);
            task.Wait();
        }
    }
}
