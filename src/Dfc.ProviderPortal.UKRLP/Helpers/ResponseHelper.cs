
using Newtonsoft.Json;


namespace Dfc.ProviderPortal.Providers
{
    public static class ResponseHelper
    {
        static public string ErrorMessage(string msg)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { message = msg });
        }
    }
}
