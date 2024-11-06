using GW2EIEvtcParser;
using GW2EIEvtcParser.ParserHelpers;
using NUnit.Framework;
using System.Collections;

namespace GW2EIParser.tst.Generated;

[TestFixtureSource(typeof(ParsesSuccessfully), nameof(GenerateTests))]
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
                    
                    if(SupportedFileFormats.IsSupportedFormat(entry))
                    {
                        yield return innerEntry;
                    }
                }
            }
            else
            {
                if(SupportedFileFormats.IsSupportedFormat(entry))
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
        Assert.Null(failureReason, Path.GetFileName(path));
    }
}
