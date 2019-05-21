using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LuckParser.Parser;
using System.IO;
using LuckParser.Exceptions;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LuckParser.tst
{
    [TestClass]
    public class StabilityTestEvtc
    {
        private void Loop(List<string> failed, List<string> messages,  string file)
        {
            try
            {
                ParsedLog log = TestHelper.ParseLog(file);
                TestHelper.JsonString(log);
                TestHelper.HtmlString(log);
            }
            catch (CancellationException canc)
            {
                if (canc.InnerException == null || !(canc.InnerException is TooShortException || canc.InnerException is SkipException))
                {
                    failed.Add(file);
                    messages.Add(canc.Message);
                }
            }
            catch (Exception ex)
            {
                if (!(ex is TooShortException || ex is SkipException))
                {
                    failed.Add(file);
                    messages.Add(ex.Message);
                }
            }
        }

        private void GenerateCrashData(List<string> failed, List<string> messages, string type)
        {
            if (failed.Count == 0)
            {
                return;
            }
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../EvtcLogs/Crashes/";

            string logName = testLocation + "log_" + type + ".json";

            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < failed.Count; i++)
            {
                dict[failed[i].Split('\\').Last()] = messages[i];
            }

            using (FileStream fs = new FileStream(logName, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8))
                {
                    DefaultContractResolver contractResolver = new DefaultContractResolver
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
                        Formatting = Newtonsoft.Json.Formatting.Indented
                    };
                    serializer.Serialize(writer, dict);
                }
            }
        }

        [TestMethod]
        public void TestEvtc()
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../EvtcLogs/StabilityTest";

            Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");

            List<string> failed = new List<string>();
            List<string> messages = new List<string>();
            List<string> toCheck = Directory.EnumerateFiles(testLocation, "*.evtc", SearchOption.AllDirectories).ToList();
            foreach (string file in toCheck)
            {
                Loop(failed, messages, file);
            }

            GenerateCrashData(failed, messages, "evtc");

            Assert.IsTrue(failed.Count == 0, failed.ToString());
        }

        [TestMethod]
        public void TestEvtcZip()
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../EvtcLogs/StabilityTest";

            Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");
            List<string> failed = new List<string>();
            List<string> messages = new List<string>();
            List<string> toCheck = Directory.EnumerateFiles(testLocation, "*.evtc.zip", SearchOption.AllDirectories).ToList();
            foreach (string file in toCheck)
            {
                Loop(failed, messages, file);
            }

            GenerateCrashData(failed, messages, "evtczip");

            Assert.IsTrue(failed.Count == 0, failed.ToString());
        }

        [TestMethod]
        public void TestZevtc()
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../EvtcLogs/StabilityTest";

            Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");

            List<string> failed = new List<string>();
            List<string> messages = new List<string>();
            List<string> toCheck = Directory.EnumerateFiles(testLocation, "*.zevtc", SearchOption.AllDirectories).ToList();
            foreach (string file in toCheck)
            {
                Loop(failed, messages, file);
            }

            GenerateCrashData(failed, messages, "zevtc");

            Assert.IsTrue(failed.Count == 0, failed.ToString());
        }
    }
}
