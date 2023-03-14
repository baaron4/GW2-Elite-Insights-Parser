using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using GW2EIEvtcParser;
using GW2EIEvtcParser.Exceptions;
using GW2EIParser.Exceptions;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;

[assembly: System.CLSCompliant(false)]
namespace GW2EIParser.tst
{

    [TestFixture]
    public class StabilityTestEvtc
    {
        internal class EVTCTestItem
        {
            public string File { get; }
            public bool Failed { get; private set; }
            public string FailedMessage { get; private set; }
            public EVTCTestItem(string file)
            {
                File = file;
            }
            public void SetFailure(string message)
            {
                Failed = true;
                FailedMessage = message;
            }
        }
        private static bool Loop(EVTCTestItem evtcTestItem)
        {
            try
            {
                ParsedEvtcLog log = TestHelper.ParseLog(evtcTestItem.File, TestHelper.APIController);
                TestHelper.JsonString(log);
                TestHelper.HtmlString(log);
                TestHelper.CsvString(log);
            }
            catch (ProgramException canc)
            {
                if (canc.InnerException == null || !(canc.InnerException is EIException))
                {
                    evtcTestItem.SetFailure(canc.Message);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                if (!(ex is EIException))
                {
                    evtcTestItem.SetFailure(ex.Message);
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

        private static void GenerateCrashData(List<EVTCTestItem> evtcTestItems, string type, bool copy)
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../GW2EIParser.tst/EvtcLogs/Crashes/";

            Directory.CreateDirectory(testLocation + "/Logs");

            string logName = testLocation + "log_" + type + ".json";
            if (File.Exists(logName))
            {
                File.Delete(logName);
            }

            var dict = new Dictionary<string, string>();
            foreach (EVTCTestItem evtcTestItem in evtcTestItems)
            {
                if (evtcTestItem.Failed)
                {
                    string evtcName = evtcTestItem.File.Split('\\').Last();
                    if (copy)
                    {
                        File.Copy(evtcTestItem.File, testLocation + "Logs/" + evtcName, true);
                    }
                    dict[evtcName] = evtcTestItem.FailedMessage;
                }
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
                    using (var writer = new JsonTextWriter(sw)
                    {
                        Formatting = Formatting.Indented
                    })
                    {
                        serializer.Serialize(writer, dict);
                    }
                }
            }
        }


        private static List<List<EVTCTestItem>> GetSplitList(IReadOnlyList<EVTCTestItem> logFiles)
        {
            int splitCount = Math.Max(Environment.ProcessorCount / 2, 1);
            var splitLogFiles = new List<List<EVTCTestItem>>();
            var sizeSortedLogFiles = new List<EVTCTestItem>(logFiles);
            for (int i = 0; i < splitCount; i++)
            {
                splitLogFiles.Add(new List<EVTCTestItem>());
            }
            sizeSortedLogFiles.Sort((x, y) =>
            {
                var fInfoX = new FileInfo(x.File);
                long xValue = fInfoX.Exists ? fInfoX.Length : 0;
                var fInfoY = new FileInfo(y.File);
                long yValue = fInfoY.Exists ? fInfoY.Length : 0;
                return xValue.CompareTo(yValue);
            });

            int index = 0;
            foreach (EVTCTestItem file in sizeSortedLogFiles)
            {
                splitLogFiles[index].Add(file);
                index = (index + 1) % splitCount;
            }
            return splitLogFiles;
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
            var toCheck = Directory.EnumerateFiles(testLocation, "*.evtc", SearchOption.AllDirectories).Select(x => new EVTCTestItem(x)).ToList();
            Parallel.ForEach(GetSplitList(toCheck), evtcTestItems => evtcTestItems.ForEach(evtcTestItem => Loop(evtcTestItem)));

            GenerateCrashData(toCheck, "evtc", true);

            Assert.IsTrue(!toCheck.Any(x => x.Failed), "Check Crashes folder");
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
            var toCheck = Directory.EnumerateFiles(testLocation, "*.evtc.zip", SearchOption.AllDirectories).Select(x => new EVTCTestItem(x)).ToList();
            Parallel.ForEach(GetSplitList(toCheck), evtcTestItems => evtcTestItems.ForEach(evtcTestItem => Loop(evtcTestItem)));

            GenerateCrashData(toCheck, "evtczip", true);

            Assert.IsTrue(!toCheck.Any(x => x.Failed), "Check Crashes folder");
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

            var toCheck = Directory.EnumerateFiles(testLocation, "*.zevtc", SearchOption.AllDirectories).Select(x => new EVTCTestItem(x)).ToList();
            Parallel.ForEach(GetSplitList(toCheck), evtcTestItems => evtcTestItems.ForEach(evtcTestItem => Loop(evtcTestItem)));

            GenerateCrashData(toCheck, "zevtc", true);

            Assert.IsTrue(!toCheck.Any(x => x.Failed), "Check Crashes folder");
        }

        [Test]
        public void TestCrashed()
        {
            string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../GW2EIParser.tst/EvtcLogs/Crashes/Logs";
            if (!Directory.Exists(testLocation))
            {
                Directory.CreateDirectory(testLocation);
            }
            int failedCount = 0;
            var toCheck = Directory.EnumerateFiles(testLocation, "*.zevtc", SearchOption.AllDirectories).Select(x => new EVTCTestItem(x)).ToList();
            foreach (EVTCTestItem evtcTestItem in toCheck)
            {
                if (Loop(evtcTestItem))
                {
                    File.Delete(evtcTestItem.File);
                }
            }
            GenerateCrashData(toCheck, "zevtc_remaining", false);
            failedCount += toCheck.Count(x => x.Failed);

            toCheck = Directory.EnumerateFiles(testLocation, "*.evtc", SearchOption.AllDirectories).Select(x => new EVTCTestItem(x)).ToList();
            foreach (EVTCTestItem evtcTestItem in toCheck)
            {
                if (Loop(evtcTestItem))
                {
                    File.Delete(evtcTestItem.File);
                }
            }
            GenerateCrashData(toCheck, "evtc_remaining", false);
            failedCount += toCheck.Count(x => x.Failed);

            toCheck = Directory.EnumerateFiles(testLocation, "*.evtc.zip", SearchOption.AllDirectories).Select(x => new EVTCTestItem(x)).ToList();
            foreach (EVTCTestItem evtcTestItem in toCheck)
            {
                if (Loop(evtcTestItem))
                {
                    File.Delete(evtcTestItem.File);
                }
            }
            GenerateCrashData(toCheck, "evtczip_remaining", false);
            failedCount += toCheck.Count(x => x.Failed);

            Assert.IsTrue(failedCount == 0, "Check Crashes folder");
        }
    }
}
