
using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ProviderService;
using Newtonsoft.Json;


namespace UKRLP.ProviderSynchronise
{
    public class ProviderSynchronise
    {
        public List<ProviderRecordStructure> SynchroniseProviders(ILogger log)
        {
            return SynchroniseProviders(DateTime.Now.AddDays(-1), log);
        }

        public List<ProviderRecordStructure> SynchroniseProviders(DateTime dtLastUpdate, ILogger log)
        {
            List<ProviderRecordStructure> results = new List<ProviderRecordStructure>();

            string[] statusesToFetch =
            {
                    "A", // Active
                    //"V", // Verified                      // Omitted, we suspect this may be a subset of Active providers
                    "PD1", // Deactivation in process
                    "PD2" // Deactivation complete
                };
            var request = Request(dtLastUpdate);
            foreach (String status in statusesToFetch)
            {
                log.LogInformation($"Downloading providers from UKRLP service with status {status}");

                request.SelectionCriteria.ProviderStatus = status;
                request.QueryId = GetNextQueryId();

                var pqp = new ProviderQueryParam {
                    ProviderQueryRequest = request
                };
                var retrieveall = new ProviderQueryPortTypeClient();

                Task<response> x = retrieveall.retrieveAllProvidersAsync(request);
                x.Wait();

                //return JsonConvert.SerializeObject(x.Result.ProviderQueryResponse.MatchingProviderRecords);
                //return x.Result.ProviderQueryResponse.MatchingProviderRecords;

                log.LogInformation($"UKRLP service returned {x.Result.ProviderQueryResponse.MatchingProviderRecords.LongLength} providers with status {status}");
                results.AddRange(x.Result.ProviderQueryResponse.MatchingProviderRecords);
            }
            //return null;
            return results;
        }

        private ProviderQueryStructure Request(DateTime dtLastUpdate)
        {
            SelectionCriteriaStructure scs = new SelectionCriteriaStructure
            {
                StakeholderId = "1",
                ProviderUpdatedSince = dtLastUpdate,
                ProviderUpdatedSinceSpecified = true,
                ApprovedProvidersOnly = YesNoType.No,
                ApprovedProvidersOnlySpecified = true,
                CriteriaCondition = QueryCriteriaConditionType.OR,
                CriteriaConditionSpecified = true
            };
            ProviderQueryStructure pqs = new ProviderQueryStructure
            {
                SelectionCriteria = scs
            };



            List<String> deletedItems = new List<String>();
            String[] activeStatuses =
            {
                    "A",
                    "V"
                };

            return pqs;
        }
        private static String GetNextQueryId()
        {
            Int32 id = 0;
            //try
            //{
            //    using (StreamReader sr = new StreamReader("NextQueryId.txt"))
            //    {
            //        String line = sr.ReadToEnd();
            //        Int32.TryParse(line, out id);
            //    }
            //}
            //catch
            //{ }

            id++;

            //File.WriteAllText("NextQueryId.txt", id.ToString());

            return id.ToString();
        }

    }
}
