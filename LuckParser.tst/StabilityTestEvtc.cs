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
        private bool Loop(List<string> failed, List<string> messages,  string file)
        {
            try
            {
                ParsedLog log = TestHelper.ParseLog(file);
                TestHelper.JsonString(log);
                TestHelper.HtmlString(log);
                TestHelper.CsvString(log);
            }
            catch (CancellationException canc)
            {
                if (canc.InnerException == null || !(canc.InnerException is TooShortException || canc.InnerException is SkipException))
                {
                    failed.Add(file);
                    messages.Add(canc.Message);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (!(ex is TooShortException || ex is SkipException))
                {
                    failed.Add(file);
                    messages.Add(ex.Message);
                    return false;
                }
                return true;
            }
            return true;
        }

        private void GenerateCrashData(List<string> failed, List<string> messages, string type, bool copy)
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../EvtcLogs/Crashes/";

            Directory.CreateDirectory(testLocation + "/Logs");

            string logName = testLocation + "log_" + type + ".json";
            if (File.Exists(logName))
            {
                File.Delete(logName);
            }

            Dictionary<string, string> dict = new Dictionary<string, string>();
            for (int i = 0; i < failed.Count; i++)
            {
                string evtcName = failed[i].Split('\\').Last();
                if (copy)
                {
                    File.Copy(failed[i], testLocation + "Logs/" + evtcName, true);
                }
                dict[evtcName] = messages[i];
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

            GenerateCrashData(failed, messages, "evtc", true);

            Assert.IsTrue(failed.Count == 0, "Check Crashes folder");
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

            GenerateCrashData(failed, messages, "evtczip", true);

            Assert.IsTrue(failed.Count == 0, "Check Crashes folder");
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

            GenerateCrashData(failed, messages, "zevtc", true);

            Assert.IsTrue(failed.Count == 0, "Check Crashes folder");
        }

        [TestMethod]
        public void TestCrashed()
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "../../../EvtcLogs/Crashes/Logs";

            List<string> failed = new List<string>();
            int failedCount = 0;
            List<string> messages = new List<string>();
            List<string> toCheck = Directory.EnumerateFiles(testLocation, "*.zevtc", SearchOption.AllDirectories).ToList();
            foreach (string file in toCheck)
            {
                if (Loop(failed, messages, file))
                {
                    File.Delete(file);
                }
            }
            GenerateCrashData(failed, messages, "zevtc_remaining", false);
            failedCount += failed.Count;
            failed.Clear();
            messages.Clear();

            toCheck = Directory.EnumerateFiles(testLocation, "*.evtc", SearchOption.AllDirectories).ToList();
            foreach (string file in toCheck)
            {
                if (Loop(failed, messages, file))
                {
                    File.Delete(file);
                }
            }
            GenerateCrashData(failed, messages, "evtc_remaining", false);
            failedCount += failed.Count;
            failed.Clear();
            messages.Clear();

            toCheck = Directory.EnumerateFiles(testLocation, "*.evtc.zip", SearchOption.AllDirectories).ToList();
            foreach (string file in toCheck)
            {
                if (Loop(failed, messages, file))
                {
                    File.Delete(file);
                }
            }
            GenerateCrashData(failed, messages, "evtczip_remaining", false);
            failedCount += failed.Count;
            failed.Clear();
            messages.Clear();

            Assert.IsTrue(failedCount == 0, "Check Crashes folder");
        }
    }
}
