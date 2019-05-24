using Dfc.ProviderPortal.UKRLP;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;

[assembly: WebJobsStartup(typeof(WebJobExtensionStartup), "Web Jobs Extension Startup")]

namespace Dfc.ProviderPortal.UKRLP
{
    public class WebJobExtensionStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            // Will  eventaully do all the DI stuff here.
        }
    }
}