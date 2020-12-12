using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using GW2EIBuilders.JsonModels;
using GW2EIEvtcParser;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GW2EIBuilders
{
    public class RawFormatBuilder
    {
        internal static readonly DefaultContractResolver DefaultJsonContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        public JsonLog JsonLog { get; }

        //

        public RawFormatBuilder(ParsedEvtcLog log, RawFormatSettings settings, Version parserVersion, string[] uploadLinks = null)
        {
            if (settings == null)
            {
                throw new InvalidDataException("Missing settings in RawFormatBuilder");
            }
            JsonLog = new JsonLog(log, settings, parserVersion, uploadLinks);
        }

        public void CreateJSON(StreamWriter sw, bool indent)
        {
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = DefaultJsonContractResolver
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
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = DefaultJsonContractResolver
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
