using System.Diagnostics;
using System.Reflection;
using GW2EIEvtcParser;
using GW2EIEvtcParser.Exceptions;
using GW2EIParserCommons.Exceptions;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Internal;
using static GW2EIParser.tst.StabilityTestEvtc;

[assembly: CLSCompliant(false)]
namespace GW2EIParser.tst;


[TestFixture]
public class StabilityTestEvtc
{
    internal sealed class EVTCTestItem
    {
        public readonly string File;
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
            Stopwatch stopWatch = new();
            stopWatch.Start();
            TestContext.Progress.WriteLine($"Started {evtcTestItem.File}");
            ParsedEvtcLog? log = TestHelper.ParseLog(evtcTestItem.File, TestHelper.APIController);
            if (log != null)
            {
                TestHelper.JsonString(log);
                TestHelper.HtmlString(log);
                TestHelper.CsvString(log);
                stopWatch.Stop();
                var elapsed = stopWatch.ElapsedMilliseconds;
                // Catch absurdly high times
                if (elapsed > (log.LogData.IsInstance || log.LogData.Logic.ParseMode == GW2EIEvtcParser.LogLogic.LogLogic.ParseModeEnum.WvW ? 300000 : 30000))
                {
                    throw new TimeoutException("Too much time spent");
                }
                TestContext.Progress.WriteLine($"Finished {evtcTestItem.File}");
            } 
            else
            {
                stopWatch.Stop();
            }
        }
        catch (ProgramException canc)
        {
            var finalException = ParserHelper.GetFinalException(canc);
            if (finalException is not EIException)
            {
                evtcTestItem.SetFailure(finalException.Message + "\n" + finalException.StackTrace);
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            var finalException = ParserHelper.GetFinalException(ex);
            if (finalException is not EIException)
            {
                evtcTestItem.SetFailure(finalException.Message + "\n" + finalException.StackTrace);
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
            splitLogFiles.Add([]);
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
    public void TestEvtc([Values(0, 1, 2, 3)] int startIndex)
    {

        string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../GW2EIParser.tst/EvtcLogs/StabilityTest";
        if (!Directory.Exists(testLocation))
        {
            Directory.CreateDirectory(testLocation);
        }
        Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");
        var toCheck = Directory.EnumerateFiles(testLocation, "*.evtc", SearchOption.AllDirectories).Select(x => new EVTCTestItem(x)).ToList();
        toCheck = toCheck.OrderBy(x =>
        {
            var fInfo = new FileInfo(x.File);
            return fInfo.Exists ? fInfo.Length : 0;
        }).Take(new Range(500 * startIndex, 500 * startIndex + 499)).ToList();
        TestContext.Progress.WriteLine($"Testing {toCheck.Count} items");
        TestContext.Progress.WriteLine($"Total Size {toCheck.Sum(x =>
        {
            var fInfo = new FileInfo(x.File);
            return fInfo.Exists ? fInfo.Length : 0;
        }) / 1e6} mb");
        toCheck.ForEach(evtcTestItem => Loop(evtcTestItem)); ;
        //Parallel.ForEach(GetSplitList(toCheck), evtcTestItems => evtcTestItems.ForEach(evtcTestItem => Loop(evtcTestItem)));

        GenerateCrashData(toCheck, "evtc" + startIndex, true);

        Assert.IsTrue(!toCheck.Any(x => x.Failed), "Check Crashes folder");
    }

    [Test]
    public void TestEvtcZip([Values(0, 1, 2, 3)] int startIndex)
    {
        string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../GW2EIParser.tst/EvtcLogs/StabilityTest";
        if (!Directory.Exists(testLocation))
        {
            Directory.CreateDirectory(testLocation);
        }
        Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");
        var toCheck = Directory.EnumerateFiles(testLocation, "*.evtc.zip", SearchOption.AllDirectories).Select(x => new EVTCTestItem(x)).ToList();
        toCheck = toCheck.OrderBy(x =>
        {
            var fInfo = new FileInfo(x.File);
            return fInfo.Exists ? fInfo.Length : 0;
        }).Take(new Range(500 * startIndex, 500 * startIndex + 499)).ToList();
        TestContext.Progress.WriteLine($"Testing {toCheck.Count} items");
        TestContext.Progress.WriteLine($"Total Size {toCheck.Sum(x =>
        {
            var fInfo = new FileInfo(x.File);
            return fInfo.Exists ? fInfo.Length : 0;
        }) / 1e6} mb");
        toCheck.ForEach(evtcTestItem => Loop(evtcTestItem)); ;
        //Parallel.ForEach(GetSplitList(toCheck), evtcTestItems => evtcTestItems.ForEach(evtcTestItem => Loop(evtcTestItem)));

        GenerateCrashData(toCheck, "evtczip" + startIndex, true);

        Assert.IsTrue(!toCheck.Any(x => x.Failed), "Check Crashes folder");
    }

    [Test]
    public void TestZevtc([Values(0, 1, 2, 3, 4, 5, 6, 7 , 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 26, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40)] int startIndex)
    {
        string testLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/../../GW2EIParser.tst/EvtcLogs/StabilityTest";
        if (!Directory.Exists(testLocation))
        {
            Directory.CreateDirectory(testLocation);
        }
        Assert.IsTrue(Directory.Exists(testLocation), "Test Directory missing");

        var toCheck = Directory.EnumerateFiles(testLocation, "*.zevtc", SearchOption.AllDirectories).Select(x => new EVTCTestItem(x)).ToList();
        toCheck = toCheck.OrderBy(x =>
        {
            var fInfo = new FileInfo(x.File);
            return fInfo.Exists ? fInfo.Length : 0;
        }).Take(new Range(500 * startIndex, 500 * startIndex + 499)).ToList();
        TestContext.Progress.WriteLine($"Testing {toCheck.Count} items");
        TestContext.Progress.WriteLine($"Total Size {toCheck.Sum(x =>
        {
            var fInfo = new FileInfo(x.File);
            return fInfo.Exists ? fInfo.Length : 0;
        })/ 1e6} mb");
        toCheck.ForEach(evtcTestItem => Loop(evtcTestItem));
        //Parallel.ForEach(GetSplitList(toCheck), evtcTestItems => evtcTestItems.ForEach(evtcTestItem => Loop(evtcTestItem)));

        GenerateCrashData(toCheck, "zevtc" + startIndex, true);

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
