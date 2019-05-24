using Dfc.ProviderPortal.UKRLP;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: WebJobsStartup(typeof(WebJobExtensionStatup), "Web Jobs Extension Startup")]

namespace Dfc.ProviderPortal.UKRLP
{
    public class WebJobExtensionStatup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .Build()
                .StartAsync();
        }
    }
}
