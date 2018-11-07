
using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;


namespace Dfc.ProviderPortal.UKRLP
{
    /// <summary>
    /// Read settings from local json file (no settings available here as they are through DI in a .NET Core MVC app)
    /// </summary>
    public static class JsonSettings
    {
        /// <summary>
        /// Built config root from settings file
        /// </summary>
        static public IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("local.settings.json")
                                                                            .Build();
        /// <summary>
        /// Gets a setting by key
        /// </summary>
        /// <param name="key">Key path, eg "Section:KeyName"</param>
        public static string GetSetting(string key) {
            return config.GetValue<string>(key);
        }
    }
}
