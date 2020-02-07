using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GW2EIParser.Builders;
using GW2EIParser.Builders.JsonModels;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using Newtonsoft.Json.Linq;

namespace GW2EIParser.tst
{
    public class TestHelper
    {

        private class TestParserSettings : ParserSettings
        {
            public TestParserSettings()
            {
                SkipFailedTries = false;
                ParseCombatReplay = true;
                ParsePhases = true;
                AnonymousPlayer = true;
                ComputeDamageModifiers = true;
                RawTimelineArrays = true;
                MultiTasks = true;
            }
        }

        public static ParsedLog ParseLog(string location)
        {
            var parser = new ParsingController(new TestParserSettings());

            var row = new GridRow(location as string, "Ready to parse")
            {
                BgWorker = new System.ComponentModel.BackgroundWorker()
                {
                    WorkerReportsProgress = true
                }
            };


            var fInfo = new FileInfo(row.Location);
            if (!fInfo.Exists)
            {
                throw new FileNotFoundException("Error Encountered: File does not exist", fInfo.FullName);
            }
            if (!ProgramHelper.IsSupportedFormat(fInfo.Name))
            {
                throw new InvalidDataException("Error Encountered: Not EVTC");
            }

            return parser.ParseLog(row, fInfo.FullName);
        }

        public static string JsonString(ParsedLog log)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms, GeneralHelper.NoBOMEncodingUTF8);
            var builder = new RawFormatBuilder(log, null);

            builder.CreateJSON(sw, false);
            sw.Close();

            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static string CsvString(ParsedLog log)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            var builder = new CSVBuilder(sw, ",", log, null);

            builder.CreateCSV();
            sw.Close();

            return sw.ToString();
        }

        public static string HtmlString(ParsedLog log)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms, GeneralHelper.NoBOMEncodingUTF8);
            var builder = new HTMLBuilder(log, null, false, false);

            builder.CreateHTML(sw, null);
            sw.Close();

            return Encoding.UTF8.GetString(ms.ToArray());
        }

        public static JsonLog JsonLog(ParsedLog log)
        {
            var builder = new RawFormatBuilder(log, null);
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
