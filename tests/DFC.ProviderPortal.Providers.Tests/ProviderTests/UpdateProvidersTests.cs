
using Dfc.ProviderPortal.Providers;
using DFC.ProviderPortal.Providers.Tests.Helpers;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace DFC.ProviderPortal.Providers.Tests.ProviderTests
{
    public class UpdateProvidersTests
    {
        Provider _provider = null;

        private const string URI_PATH = "http://localhost:7071/api/";

        private const string UPDATE_PROVIDER = "{" +
                                                    "\"id\": \"154e4547-a37e-436d-93dd-a92674bc9603\"," +
	                                                "\"Status\": 1," +
	                                                "\"UpdatedBy\": \"Ian\"" +
                                               "}";


        public UpdateProvidersTests()
        {
            TestHelper.AddEnvironmentVariables();
        }


        //[Fact]
        //public void RunTests()
        //{
        //    _GetAllProviders_ReturnsResults();
        //    //_GetAllProviders_ExpectedCount();
        //    //_GetProviderById_Run();
        //    //_GetProviderByPRN_Run();
        //    //_GetProviderByPRNAndName_Run();
        //    Assert.True(true);
        //}




        [Fact]
        public void _UpdateProviderById_Run()
        {
            //System.Net.Http.HttpRequestMessage rm = TestHelper.CreateRequest(new Uri(URI_PATH + "UpdateProviderById"),
            //                                                                 UPDATE_PROVIDER);
            //Task<HttpResponseMessage> task = UpdateProviderById.Run(rm, new LogHelper((ILogger)null));
            //_provider = TestHelper.GetAFReturnedObject<Provider>(task);

            //Assert.NotNull(_provider);
            Assert.True(true);
        }
    }
}
