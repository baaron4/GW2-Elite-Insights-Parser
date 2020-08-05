using Newtonsoft.Json.Serialization;

namespace GW2EIControllers
{
    public static class ControllerHelper
    {
        public static readonly DefaultContractResolver ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

    }

}
