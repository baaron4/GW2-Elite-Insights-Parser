using System.Text;
using Newtonsoft.Json.Serialization;

namespace GW2EIGW2API
{
    public static class APIHelper
    {

        public static readonly DefaultContractResolver DefaultJsonContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

        public static readonly UTF8Encoding NoBOMEncodingUTF8 = new UTF8Encoding(false);
    }
}
