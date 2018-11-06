
using Newtonsoft.Json;
using ProviderService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace UKRLP.ProviderSynchronise
{
    public class ProviderSynchronise
    {
        //public string SynchroniseProviders()
        public ProviderRecordStructure[] SynchroniseProviders()
        {
            return SynchroniseProviders(DateTime.Now.AddDays(-1));
        }
        //public string SynchroniseProviders(DateTime dtLastUpdate)
        public ProviderRecordStructure[] SynchroniseProviders(DateTime dtLastUpdate)
        {
            string[] statusesToFetch =
            {
                    "A", // Active
                    "V", // Verified
                    "PD1", // Deactivation in process
                    "PD2" // Deactivation complete
                };
            var request = Request(dtLastUpdate);
            foreach (String status in statusesToFetch)
            {
                request.SelectionCriteria.ProviderStatus = status;
                request.QueryId = GetNextQueryId();

                var pqp = new ProviderQueryParam
                {
                    ProviderQueryRequest = request
                };
                var retrieveall = new ProviderQueryPortTypeClient();

                Task<response> x = retrieveall.retrieveAllProvidersAsync(request);
                x.Wait();

                //return JsonConvert.SerializeObject(x.Result.ProviderQueryResponse.MatchingProviderRecords);
                return x.Result.ProviderQueryResponse.MatchingProviderRecords;
            }
            //return string.Empty;
            return null;
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
