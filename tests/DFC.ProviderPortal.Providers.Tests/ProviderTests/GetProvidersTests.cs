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

        private const string VENUE_BY_ID = "{ \"id\": \"e3f1acbc-9eb2-4c38-81ec-fb2feb270035\" }";
        private const string VENUE_BY_PRN = "{ \"PRN\": 123456789 }";
        private const string VENUE_BY_PRN_AND_NAME = "{" +
                                                     "  \"PRN\": 123456789," +
                                                     "  \"Name\": \"My fab Provider\" }";

        public GetProvidersTests()
        {
            TestHelper.AddEnvironmentVariables();
        }


        [Fact]
        public void RunTests()
        {
            _GetAllProviders_ReturnsResults();
            //_GetAllVenues_ExpectedCount();
            //_GetVenueById_Run();
            //_GetVenuesByPRN_Run();
            //_GetVenuesByPRNAndName_Run();
            Assert.True(true);
        }




        [Fact]
        public void _GetAllProviders_ReturnsResults()
        {
            System.Net.Http.HttpRequestMessage rm = TestHelper.CreateRequest(new Uri(URI_PATH + "GetAllProviders"), "");
            Task<HttpResponseMessage> task = GetAllProviders.Run(rm, new LogHelper((ILogger)null));

            _providers = TestHelper.GetAFReturnedObjects<Provider>(task);
            Assert.True(_providers.Any());
            //Assert.All<Venue>(venues, v => Guid.TryParse(v.id.ToString(), out Guid g));
        }

        //[Fact]
        //public void _GetVenuesByPRN_Run()
        //{
        //    System.Net.Http.HttpRequestMessage rm = TestHelper.CreateRequest(new Uri(URI_PATH + "GetVenueById"),
        //                                                                     VENUE_BY_PRN);
        //    Task<HttpResponseMessage> task = GetVenuesByPRN.Run(rm, new LogHelper((ILogger)null));
        //    _providers = TestHelper.GetAFReturnedObjects<Provider>(task);

        //    Assert.True(_providers.Any());
        //}

        //[Fact]
        //public void _GetVenuesByPRNAndName_Run()
        //{
        //    System.Net.Http.HttpRequestMessage rm = TestHelper.CreateRequest(new Uri(URI_PATH + "GetVenueById"),
        //                                                                     VENUE_BY_PRN_AND_NAME);
        //    Task<HttpResponseMessage> task = GetVenuesByPRNAndName.Run(rm, new LogHelper((ILogger)null));
        //    _providers = TestHelper.GetAFReturnedObjects<Provider>(task);

        //    Assert.True(_providers.Any());
        //}

        //[Fact]
        //public void _GetAllVenues_ExpectedCount()
        //{
        //    Assert.True(_providers.LongCount() == EXPECTED_COUNT);
        //}

        //[Fact]
        //public void _GetVenueById_Run()
        //{
        //    System.Net.Http.HttpRequestMessage rm = TestHelper.CreateRequest(new Uri(URI_PATH + "GetVenueById"),
        //                                                                     VENUE_BY_ID);
        //    Task<HttpResponseMessage> task = GetVenueById.Run(rm, new LogHelper((ILogger)null));
        //    Provider provider = TestHelper.GetAFReturnedObject<Provider>(task);

        //    Assert.True(provider != null);
        //}
    }
}
