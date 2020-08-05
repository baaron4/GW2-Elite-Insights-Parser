using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GW2EIEvtcParser;
using GW2EIBuilders;
using GW2EIBuilders.JsonModels;
using GW2EIUtils;
using Newtonsoft.Json.Linq;

namespace GW2EIParser.tst
{
    public class TestHelper
    {
        private static readonly EvtcParserSettings parserSettings = new EvtcParserSettings(false, true, true, true, true);
        private static readonly HTMLSettings htmlSettings = new HTMLSettings("", new Version(), false, false);
        private static readonly RawFormatSettings rawSettings = new RawFormatSettings("", new Version(), true);
        private static readonly CSVSettings csvSettings = new CSVSettings("", new Version(), ",");

        private class TestOperationController : OperationTracer
        {
            public TestOperationController() : base()
            {

            }

            public override void UpdateProgressWithCancellationCheck(string status)
            {
            }
        }

        public static ParsedEvtcLog ParseLog(string location)
        {
            var parser = new EvtcParser(parserSettings);

            var fInfo = new FileInfo(location);

            return parser.ParseLog(new TestOperationController(), fInfo);
        }

        public static string JsonString(ParsedEvtcLog log)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms, GeneralHelper.NoBOMEncodingUTF8);
            var builder = new RawFormatBuilder(log, null);

            builder.CreateJSON(sw, false);
            sw.Close();

            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static string CsvString(ParsedEvtcLog log)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            var builder = new CSVBuilder(log, csvSettings);

            builder.CreateCSV(sw);
            sw.Close();

            return sw.ToString();
        }

        public static string HtmlString(ParsedEvtcLog log)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms, GeneralHelper.NoBOMEncodingUTF8);
            var builder = new HTMLBuilder(log, htmlSettings);

            builder.CreateHTML(sw, null);
            sw.Close();

            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static JsonLog JsonLog(ParsedEvtcLog log)
        {
            var builder = new RawFormatBuilder(log, rawSettings, null);
            return builder.JsonLog;
        }

        ///////////////////////////////////////
        ///

        //https://stackoverflow.com/questions/24876082/find-and-return-json-differences-using-newtonsoft-in-c

        /// <summary>
        /// Deep compare two NewtonSoft JObjects. If they don't match, returns text diffs
        /// </summary>
        /// <param name="source">The expected results</param>
        /// <param name="target">The actual results</param>
        /// <returns>Text string</returns>

        public static StringBuilder CompareObjects(JObject source, JObject target)
        {
            var returnString = new StringBuilder();
            foreach (KeyValuePair<string, JToken> sourcePair in source)
            {
                if (sourcePair.Value.Type == JTokenType.Object)
                {
                    if (target.GetValue(sourcePair.Key) == null)
                    {
                        returnString.Append("Key " + sourcePair.Key
                                            + " not found" + Environment.NewLine);
                    }
                    else if (target.GetValue(sourcePair.Key).Type != JTokenType.Object)
                    {
                        returnString.Append("Key " + sourcePair.Key
                                            + " is not an object in target" + Environment.NewLine);
                    }
                    else
                    {
                        returnString.Append(CompareObjects(sourcePair.Value.ToObject<JObject>(),
                            target.GetValue(sourcePair.Key).ToObject<JObject>()));
                    }
                }
                else if (sourcePair.Value.Type == JTokenType.Array)
                {
                    if (target.GetValue(sourcePair.Key) == null)
                    {
                        returnString.Append("Key " + sourcePair.Key
                                            + " not found" + Environment.NewLine);
                    }
                    else
                    {
                        returnString.Append(CompareArrays(sourcePair.Value.ToObject<JArray>(),
                            target.GetValue(sourcePair.Key).ToObject<JArray>(), sourcePair.Key));
                    }
                }
                else
                {
                    JToken expected = sourcePair.Value;
                    JToken actual = target.SelectToken("['" + sourcePair.Key + "']");
                    if (actual == null)
                    {
                        returnString.Append("Key " + sourcePair.Key
                                            + " not found" + Environment.NewLine);
                    }
                    else
                    {
                        if (!JToken.DeepEquals(expected, actual))
                        {
                            returnString.Append("Key " + sourcePair.Key + ": "
                                                + sourcePair.Value + " !=  "
                                                + target.Property(sourcePair.Key).Value
                                                + Environment.NewLine);
                        }
                    }
                }
            }
            return returnString;
        }

        /// <summary>
        /// Deep compare two NewtonSoft JArrays. If they don't match, returns text diffs
        /// </summary>
        /// <param name="source">The expected results</param>
        /// <param name="target">The actual results</param>
        /// <param name="arrayName">The name of the array to use in the text diff</param>
        /// <returns>Text string</returns>
        public static StringBuilder CompareArrays(JArray source, JArray target, string arrayName = "")
        {
            var returnString = new StringBuilder();
            for (int index = 0; index < source.Count; index++)
            {

                JToken expected = source[index];
                if (expected.Type == JTokenType.Object)
                {
                    JToken actual = (index >= target.Count) ? new JObject() : target[index];
                    returnString.Append(CompareObjects(expected.ToObject<JObject>(),
                        actual.ToObject<JObject>()));
                }
                else
                {

                    JToken actual = (index >= target.Count) ? "" : target[index];
                    if (!JToken.DeepEquals(expected, actual))
                    {
                        if (string.IsNullOrEmpty(arrayName))
                        {
                            returnString.Append("Index " + index + ": " + expected
                                                + " != " + actual + Environment.NewLine);
                        }
                        else
                        {
                            returnString.Append("Key " + arrayName
                                                + "[" + index + "]: " + expected
                                                + " != " + actual + Environment.NewLine);
                        }
                    }
                }
            }
            return returnString;
        }

    }
}
