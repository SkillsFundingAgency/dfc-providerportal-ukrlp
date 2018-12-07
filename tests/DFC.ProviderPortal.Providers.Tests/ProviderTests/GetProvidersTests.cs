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
    public class GetProvidersTests
    {
        private IEnumerable<Provider> _providers = null;

        private const string URI_PATH = "http://localhost:7071/api/";
        private const long EXPECTED_COUNT = 15979;

        private const string PROVIDER_BY_ID = "{ \"id\": \"e3f1acbc-9eb2-4c38-81ec-fb2feb270035\" }";
        private const string PROVIDER_BY_PRN = "{ \"PRN\": 12345678 }";
        private const string PROVIDER_BY_PRN_AND_NAME = "{" +
                                                     "  \"PRN\": 12345678," +
                                                     "  \"Name\": \"My Provider\" }";

        public GetProvidersTests()
        {
            TestHelper.AddEnvironmentVariables();
        }


        [Fact]
        public void RunTests()
        {
            _GetAllProviders_ReturnsResults();
            //_GetAllProviders_ExpectedCount();
            //_GetProviderById_Run();
            //_GetProviderByPRN_Run();
            //_GetProviderByPRNAndName_Run();
            Assert.True(true);
        }




        [Fact]
        public void _GetAllProviders_ReturnsResults()
        {
            System.Net.Http.HttpRequestMessage rm = TestHelper.CreateRequest(new Uri(URI_PATH + "GetAllProviders"), "");
            Task<HttpResponseMessage> task = GetAllProviders.Run(rm, new LogHelper((ILogger)null));

            _providers = TestHelper.GetAFReturnedObjects<Provider>(task);
            Assert.True(_providers.Any());
        }

        //[Fact]
        //public void _GetProviderByPRN_Run()
        //{
        //    System.Net.Http.HttpRequestMessage rm = TestHelper.CreateRequest(new Uri(URI_PATH + "GetProviderByPRN"),
        //                                                                     PROVIDER_BY_PRN);
        //    Task<HttpResponseMessage> task = GetProviderByPRN.Run(rm, new LogHelper((ILogger)null));
        //    _providers = TestHelper.GetAFReturnedObjects<Provider>(task);

        //    Assert.True(_providers.Any());
        //}

        //[Fact]
        //public void _GetProviderByPRNAndName_Run()
        //{
        //    System.Net.Http.HttpRequestMessage rm = TestHelper.CreateRequest(new Uri(URI_PATH + "GetProviderByPRNAndName"),
        //                                                                     PROVIDER_BY_PRN_AND_NAME);
        //    Task<HttpResponseMessage> task = GetProviderByPRNAndName.Run(rm, new LogHelper((ILogger)null));
        //    _providers = TestHelper.GetAFReturnedObjects<Provider>(task);

        //    Assert.True(_providers.Any());
        //}

        //[Fact]
        //public void _GetAllProviders_ExpectedCount()
        //{
        //    Assert.True(_providers.LongCount() == EXPECTED_COUNT);
        //}

        //[Fact]
        //public void _GetProviderById_Run()
        //{
        //    System.Net.Http.HttpRequestMessage rm = TestHelper.CreateRequest(new Uri(URI_PATH + "GetProviderById"),
        //                                                                     PROVIDER_BY_ID);
        //    Task<HttpResponseMessage> task = GetProviderById.Run(rm, new LogHelper((ILogger)null));
        //    Provider provider = TestHelper.GetAFReturnedObject<Provider>(task);

        //    Assert.True(provider != null);
        //}
    }
}
