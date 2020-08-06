using System.Text;
using Newtonsoft.Json.Serialization;

namespace GW2EIBuilders
{
    public abstract class BuilderHelper
    {
        internal static readonly DefaultContractResolver ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        public static readonly UTF8Encoding NoBOMEncodingUTF8 = new UTF8Encoding(false);
    }
}
