using BenchmarkDotNet.Attributes;
using GW2EIEvtcParser;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;

namespace GW2EIParserBenchmark;

[MemoryDiagnoser]
public class EvctParserBenchmark
{
    [ParamsSource(nameof(Files))]
    public string _filePath { get; set; } = null!;

    public static IEnumerable<string> Files()
    {
        string testFilesPath = Path.Combine(AppContext.BaseDirectory, "TestFiles");
        if (Directory.Exists(testFilesPath))
        {
            IEnumerable<string> files = Directory.EnumerateFiles(testFilesPath, "*", SearchOption.TopDirectoryOnly);
            return files.Where(SupportedFileFormats.IsSupportedFormat);
        }

        throw new InvalidDataException($"No files are present in {testFilesPath}");
    }

    public EvtcParser? Parser;
    private readonly ParserController parserController = new TestOperationController();

    [GlobalSetup]
    public void Setup()
    {
        EvtcParserSettings parserSettings = new(0, 0);
        GW2APIController apiController = new("./Content/SkillList.json", "./Content/SpecList.json", "./Content/TraitList.json", "./Content/MapList.json");
        Parser = new EvtcParser(parserSettings, apiController);
    }

    [Benchmark]
    public ParsedEvtcLog? ParseLog()
    {
        FileInfo fileInfo = new(_filePath);
        ParsedEvtcLog? test = Parser?.ParseLog(parserController, fileInfo, out _);
        return test;
    }
}
