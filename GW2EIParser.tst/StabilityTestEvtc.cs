using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GW2EIEvtcParser;
using GW2EIEvtcParser.Exceptions;
using GW2EIParser.Exceptions;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace GW2EIParser.tst
{
    [TestFixture]
    public class StabilityTestEvtc
    {
        private bool Loop(BlockingCollection<string> failed, BlockingCollection<string> messages, string file)
        {
            try
            {
                ParsedEvtcLog log = TestHelper.ParseLog(file, TestHelper.APIController);
                TestHelper.JsonString(log);
                TestHelper.HtmlString(log);
                TestHelper.CsvString(log);
            }
            catch (EncompassException canc)
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
                if (!(ex is TooShortException || ex is SkipException || ex is IncompleteLogException))
                {
                    failed.Add(file);
                    messages.Add(ex.Message);
                    return false;
                }
                return true;
            }
            finally
            {
                GC.Collect();
            }
            return true;
        }

        private void GenerateCrashData(BlockingCollection<string> failed, BlockingCollection<string> messages, string type, bool copy)
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../GW2EIParser.tst/EvtcLogs/Crashes/";

            Directory.CreateDirectory(testLocation + "/Logs");

            string logName = testLocation + "log_" + type + ".json";
            if (File.Exists(logName))
            {
                File.Delete(logName);
            }

            var failedList = failed.ToList();
            var messagesList = messages.ToList();
            var dict = new Dictionary<string, string>();
            for (int i = 0; i < failedList.Count; i++)
            {
                string evtcName = failedList[i].Split('\\').Last();
                if (copy)
                {
                    File.Copy(failedList[i], testLocation + "Logs/" + evtcName, true);
                }
                dict[evtcName] = messagesList[i];
            }

            using (var fs = new FileStream(logName, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs, TestHelper.NoBOMEncodingUTF8))
                {
                    var serializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ContractResolver = TestHelper.DefaultJsonContractResolver
                    };
                    var writer = new JsonTextWriter(sw)
                    {
                        Formatting = Formatting.Indented
                    };
                    serializer.Serialize(writer, dict);
                }
            }
        }

        [Test]
        public void TestEvtc()
        {

        string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../GW2EIParser.tst/EvtcLogs/StabilityTest";
            if (!Directory.Exists(testLocation))
            {
                Directory.CreateDirectory(testLocation);
            }
            Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");

            var failed = new BlockingCollection<string>();
            var messages = new BlockingCollection<string>();
            var toCheck = Directory.EnumerateFiles(testLocation, "*.evtc", SearchOption.AllDirectories).ToList();
            Parallel.ForEach(toCheck, file => Loop(failed, messages, file));

            GenerateCrashData(failed, messages, "evtc", true);

            Assert.IsTrue(failed.Count == 0, "Check Crashes folder");
        }

        [Test]
        public void TestEvtcZip()
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../GW2EIParser.tst/EvtcLogs/StabilityTest";
            if (!Directory.Exists(testLocation))
            {
                Directory.CreateDirectory(testLocation);
            }
            Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");
            var failed = new BlockingCollection<string>();
            var messages = new BlockingCollection<string>();
            var toCheck = Directory.EnumerateFiles(testLocation, "*.evtc.zip", SearchOption.AllDirectories).ToList();
            Parallel.ForEach(toCheck, file => Loop(failed, messages, file));

            GenerateCrashData(failed, messages, "evtczip", true);

            Assert.IsTrue(failed.Count == 0, "Check Crashes folder");
        }

        [Test]
        public void TestZevtc()
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../GW2EIParser.tst/EvtcLogs/StabilityTest";
            if (!Directory.Exists(testLocation))
            {
                Directory.CreateDirectory(testLocation);
            }
            Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");

            var failed = new BlockingCollection<string>();
            var messages = new BlockingCollection<string>();
            var toCheck = Directory.EnumerateFiles(testLocation, "*.zevtc", SearchOption.AllDirectories).ToList();
            Parallel.ForEach(toCheck, file => Loop(failed, messages, file));

            GenerateCrashData(failed, messages, "zevtc", true);

            Assert.IsTrue(failed.Count == 0, "Check Crashes folder");
        }

        [Test]
        public void TestCrashed()
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../GW2EIParser.tst/EvtcLogs/Crashes/Logs";
            if (!Directory.Exists(testLocation))
            {
                Directory.CreateDirectory(testLocation);
            }
            var failed = new BlockingCollection<string>();
            int failedCount = 0;
            var messages = new BlockingCollection<string>();
            var toCheck = Directory.EnumerateFiles(testLocation, "*.zevtc", SearchOption.AllDirectories).ToList();
            foreach (string file in toCheck)
            {
                if (Loop(failed, messages, file))
                {
                    File.Delete(file);
                }
            }
            GenerateCrashData(failed, messages, "zevtc_remaining", false);
            failedCount += failed.Count;

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

            Assert.IsTrue(failedCount == 0, "Check Crashes folder");
        }
    }
}
