using GW2EIEvtcParser;
using NUnit.Framework;

namespace GW2EIParser.tst;
[TestFixture]
sealed class Regression
{
    [Test]
    public static void BuffUptime()
    {
        var file = new FileInfo("TestInput/ShouldParse/Regression/20241014-141601.zevtc");
        if(!file.Exists)
        {
            Assert.Inconclusive($"Required file ({file}) does not exist.");
            return;
        }

        var parser = new EvtcParser(TestHelper.ParserSettings, TestHelper.APIController);
        var log = parser.ParseLog(new TestHelper.TestOperationController(), file, out var failureReason, false);
        Assert.NotNull(log);
        Assert.Null(failureReason);
        var data = GW2EIBuilders.JsonModels.JsonLogBuilder.BuildJsonLog(log!, new(true), new Version(), new GW2EIBuilders.UploadResults(""));
        var player = data.Players![0];
        var mightData = player.BuffUptimes!.First(d => d.Id == 740).BuffData;
        var generationSelf = mightData![0].Generated![player.Name!];
        Assert.AreEqual(2.383, generationSelf, 0);
    }
}
