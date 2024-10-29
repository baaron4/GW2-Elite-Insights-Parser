using GW2EIEvtcParser;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GW2EIParser.tst;

[TestFixtureSource(typeof(ParsesSuccessfully), nameof(ParsesSuccessfully.GenerateTests))]
public class ParsesSuccessfully
{
    public static IEnumerable GenerateTests => EnumerateRecursively("TestInput/ShouldParse");

    static IEnumerable<string> EnumerateRecursively(string path)
    {
        foreach(var entry in Directory.EnumerateFileSystemEntries(path))
        {
            if(File.GetAttributes(entry).HasFlag(FileAttributes.Directory))
            {
                foreach(var innerEntry in EnumerateRecursively(entry))
                {
                    if(entry.EndsWith("evtc", System.StringComparison.InvariantCulture))
                    {
                        yield return innerEntry;
                    }
                }
            }
            else
            {
                if(entry.EndsWith("evtc", System.StringComparison.InvariantCulture))
                {
                    yield return entry;
                }
            }
        }
    }


    string path;
    public ParsesSuccessfully(string path) => this.path = path;

    [Test]
    public void ParsesWithoutError()
    {
        var parser = new EvtcParser(TestHelper.ParserSettings, TestHelper.APIController);

        _ = parser.ParseLog(new TestHelper.TestOperationController(), new FileInfo(this.path), out var failureReason, false);
        Assert.Null(failureReason, $"Expected no error to occur, but was\n\n{failureReason?.Reason}");
    }
}
