using System.Collections.Generic;
using System.IO;
using System.Xml;
using GW2EIEvtcParser;
using GW2EIParser.Builders.JsonModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GW2EIParser.Builders
{
    public class RawFormatBuilder
    {

        public JsonLog JsonLog { get; }

        //

        public RawFormatBuilder(ParsedEvtcLog log, RawFormatSettings settings, string[] uploadLinks = null)
        {
            if (settings == null)
            {
                throw new InvalidDataException("Missing settings in RawFormatBuilder");
            }
            JsonLog = new JsonLog(log, settings, uploadLinks);
        }

        public void CreateJSON(StreamWriter sw, bool indent)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = contractResolver
            };
            var writer = new JsonTextWriter(sw)
            {
                Formatting = indent ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None
            };
            serializer.Serialize(writer, JsonLog);
            writer.Close();
        }

        public void CreateXML(StreamWriter sw, bool indent)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = contractResolver
            };
            var root = new Dictionary<string, JsonLog>()
            {
                {"log", JsonLog }
            };
            string json = JsonConvert.SerializeObject(root, settings);

            XmlDocument xml = JsonConvert.DeserializeXmlNode(json);
            var xmlTextWriter = new XmlTextWriter(sw)
            {
                Formatting = indent ? System.Xml.Formatting.Indented : System.Xml.Formatting.None
            };

            xml.WriteTo(xmlTextWriter);
            xmlTextWriter.Close();
        }

    }
}
