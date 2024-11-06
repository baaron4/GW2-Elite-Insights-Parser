using GW2EIEvtcParser;
using NUnit.Framework;

namespace GW2EIParser.tst;

[TestFixture]
public class ParsingErrors
{
    [TestCase("20231017-143845.zevtc", "Fight is longer than 24h")]
    [TestCase("20241009-215946.zevtc", "Queue logic based must have a > 1 capacity")]
    [TestCase("20231214-203704.zevtc", "Fight is too short: 1043 < 2200")]
    [TestCase("20240908-224412.zevtc", "Missing Build Event")]
    [TestCase("20220719-200758.zevtc", "Unable to read beyond the end of the stream")]
    [TestCase("20230210-223914.zevtc", "No Targets found")]
    [TestCase("20231002-202344.zevtc", "No players found")]
    [TestCase("20230403-211830.zevtc", "No valid players")]
    [TestCase("20230130-215041.zevtc", "Enervators not found")]
    [TestCase("20241028-100044.zevtc", "Missing GUID event for effect 12954")]
    public void ReportsError(string logFile, string expectedInErrorMessage)
    {
        var parser = new EvtcParser(TestHelper.ParserSettings, TestHelper.APIController);

        var file = new FileInfo("TestInput/Broken/" + logFile);
        if(!file.Exists)
        {
            Assert.Inconclusive($"Required file ({file}) does not exist.");
            return;
        }

        _ = parser.ParseLog(new TestHelper.TestOperationController(), file, out var failureReason, false);
        Assert.NotNull(failureReason, "Expected an error to occur");
        Assert.True(failureReason.Reason.Contains(expectedInErrorMessage), $"Expected\n'{failureReason.Reason}'\n\n to contain\n'{expectedInErrorMessage}'");
    }
}
