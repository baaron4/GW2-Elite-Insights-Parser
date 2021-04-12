using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using GW2EIBuilders.JsonModels;
using GW2EIEvtcParser;
using GW2EIJSON;
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
        private JsonLog _jsonLog { get; }

        //

        public RawFormatBuilder(ParsedEvtcLog log, RawFormatSettings settings, Version parserVersion, UploadResults uploadResults)
        {
            if (settings == null)
            {
                throw new InvalidDataException("Missing settings in RawFormatBuilder");
            }
            _jsonLog = JsonLogBuilder.BuildJsonLog(log, settings, parserVersion, uploadResults.ToArray());
        }

        /// <summary>
        /// Returns a copy of JsonLog object that will be used by the builder.
        /// </summary>
        /// <returns></returns>
        public JsonLog GetJson()
        {
            var sw = new StringWriter();
            var serializer = new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            var writer = new JsonTextWriter(sw)
            {
                Formatting =  Newtonsoft.Json.Formatting.None
            };
            serializer.Serialize(writer, _jsonLog);
            writer.Close();
            JsonLog log = JsonConvert.DeserializeObject<JsonLog>(sw.ToString(), new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            return log;
        }

        /// <summary>
        /// Creates a json file based on the original JsonLog of the RawFormat builder
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="indent"></param>
        public void CreateJSON(StreamWriter sw, bool indent)
        {
            CreateJSON(_jsonLog, sw, indent);
        }

        /// <summary>
        /// Creates a json file based on the given JsonLog
        /// </summary>
        /// <param name="jsonLog"></param>
        /// <param name="sw"></param>
        /// <param name="indent"></param>
        public static void CreateJSON(JsonLog jsonLog, StreamWriter sw, bool indent)
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
            serializer.Serialize(writer, jsonLog);
            writer.Close();
        }

        /// <summary>
        /// Creates an xml file based on the original JsonLog of the RawFormat builder
        /// </summary>
        /// <param name="sw"></param>
        /// <param name="indent"></param>
        public void CreateXML(StreamWriter sw, bool indent)
        {
            CreateXML(_jsonLog, sw, indent);
        }

        /// <summary>
        /// Creates an xml file based on the given JsonLog
        /// </summary>
        /// <param name="jsonLog"></param>
        /// <param name="sw"></param>
        /// <param name="indent"></param>
        public static void CreateXML(JsonLog jsonLog, StreamWriter sw, bool indent)
        {
            var settings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = DefaultJsonContractResolver
            };
            var root = new Dictionary<string, JsonLog>()
            {
                {"log", jsonLog }
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
