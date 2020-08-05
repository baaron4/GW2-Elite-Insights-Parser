using System;
using Newtonsoft.Json.Serialization;

namespace GW2EIControllers
{
    public static class ControllerHelper
    {
        public static readonly DefaultContractResolver ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

        private static string _parserName;
        private static Version _parserVersion;

        public static void SetControllerInformation(string parserName, Version parserVersion)
        {
            _parserName = parserName;
            _parserVersion = parserVersion;
            GW2APIController.InitAPICache();
        }

        public static (string parserName, Version parserVersion) GetControllerInformation()
        {
            if (_parserName == null || _parserVersion == null)
            {
                throw new InvalidOperationException("Controller information not set, please call ControllerHelper.SetControllerInformation");
            }
            return (_parserName, _parserVersion);
        }

    }

}
