
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;


namespace Dfc.ProviderPortal.UKRLP
{
    /// <summary>
    /// Read settings from environment variables (no settings available here as they are through DI in a .NET Core MVC app)
    /// These can be input in Debug tab of project's properties and are held in launchSettings.json
    /// They should match environment variables set up by DevOps in settings for Azure Functions
    /// </summary>
    public static class SettingsHelper
    {
        /// <summary>
        /// Built config root from settings file
        /// </summary>
        static public IConfigurationRoot config = new ConfigurationBuilder().AddEnvironmentVariables()
                                                                            .Build();

        /// <summary>
        /// Properties wrapping up app setttings
        /// </summary>
        static public string StorageURI = config.GetValue<string>("APPSETTING_StorageURI");
        static public string PrimaryKey = config.GetValue<string>("APPSETTING_PrimaryKey");
        static public string Database = config.GetValue<string>("APPSETTING_Database");
        static public string Collection = config.GetValue<string>("APPSETTING_Collection");

        ///// <summary>
        ///// Gets a setting by key
        ///// </summary>
        ///// <param name="key">Key path, eg "Section:KeyName"</param>
        //public static string GetSetting(string key) {
        //    return config.GetValue<string>(key);
        //}
    }
}
