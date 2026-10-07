using System.Text;
using System.Text.Json;
using GW2EIBuilders;
using GW2EIEvtcParser;
using GW2EIGW2API;
using GW2EIJSON;

namespace GW2EIParser.tst;

internal static class TestHelper
{
    internal static readonly UTF8Encoding NoBOMEncodingUTF8 = new(false);
    private static readonly Version Version = new(1, 0);
    public static readonly EvtcParserSettings ParserSettings = new(2200, 150)
    {
        DetailedWvWParse = true,
    };
    private static readonly HTMLSettings htmlSettings = new();
    private static readonly RawFormatSettings rawSettings = new();
    private static readonly CSVSettings csvSettings = new(",");
    private static readonly HTMLAssets htmlAssets = new();

    internal static readonly string SkillAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SkillList.json";
    internal static readonly string MapAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/MapList.json";
    internal static readonly string SpecAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SpecList.json";
    internal static readonly string TraitAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/TraitList.json";

    internal static readonly GW2APIController APIController = new(SkillAPICacheLocation, SpecAPICacheLocation, TraitAPICacheLocation, MapAPICacheLocation);

    internal class TestOperationController : ParserController
    {
        public TestOperationController()
        {

        }

        public override void UpdateProgressWithCancellationCheck(string status)
        {
        }
    }

    public static ParsedEvtcLog? ParseLog(string location, GW2APIController apiController)
    {
        var parser = new EvtcParser(ParserSettings, apiController);

        var fInfo = new FileInfo(location);
        ParsedEvtcLog? parsedLog = parser.ParseLog(new TestOperationController(), fInfo, out var failureReason, true);
        failureReason?.Throw();
        return parsedLog;
    }

    public static void JsonString(ParsedEvtcLog log)
    {
        var ms = new MemoryStream();
        var builder = new RawFormatBuilder(log, rawSettings, Version, new UploadResults());

        builder.CreateJSON(ms, false);
    }

    public static void CsvString(ParsedEvtcLog log)
    {
        var ms = new MemoryStream();
        var sw = new StreamWriter(ms);
        var builder = new CSVBuilder(log, csvSettings, Version, new UploadResults());

        builder.CreateCSV(sw);
        sw.Close();
    }

    public static void HtmlString(ParsedEvtcLog log)
    {
        var ms = new MemoryStream();
        var sw = new StreamWriter(ms, NoBOMEncodingUTF8);
        var builder = new HTMLBuilder(log, htmlSettings, htmlAssets, Version, new UploadResults());

        builder.CreateHTML(sw, null);
        sw.Close();
    }

    public static JsonLog JsonLog(ParsedEvtcLog log)
    {
        var builder = new RawFormatBuilder(log, rawSettings, Version, new UploadResults());
        return builder.GetJson();
    }

    ///////////////////////////////////////
    /// Thanks gemini
    public static List<string> Compare(string json1, string json2)
    {
        using var doc1 = JsonDocument.Parse(json1);
        using var doc2 = JsonDocument.Parse(json2);

        var differences = new List<string>();
        CompareElements(doc1.RootElement, doc2.RootElement, "$", differences);
        return differences;
    }

    private static void CompareElements(JsonElement e1, JsonElement e2, string path, List<string> diffs)
    {
        if (e1.ValueKind != e2.ValueKind)
        {
            diffs.Add($"Mismatch at {path}: Kind '{e1.ValueKind}' != '{e2.ValueKind}'");
            return;
        }

        switch (e1.ValueKind)
        {
            case JsonValueKind.Object:
                CompareObjects(e1, e2, path, diffs);
                break;

            case JsonValueKind.Array:
                CompareArrays(e1, e2, path, diffs);
                break;

            case JsonValueKind.String:
                if (e1.GetString() != e2.GetString())
                {
                    diffs.Add($"Value mismatch at {path}: '{e1.GetString()}' != '{e2.GetString()}'");
                }

                break;

            case JsonValueKind.Number:
                if (e1.GetRawText() != e2.GetRawText())
                {
                    diffs.Add($"Value mismatch at {path}: {e1.GetRawText()} != {e2.GetRawText()}");
                }

                break;

            case JsonValueKind.True:
            case JsonValueKind.False:
                if (e1.GetBoolean() != e2.GetBoolean())
                {
                    diffs.Add($"Value mismatch at {path}: {e1.GetBoolean()} != {e2.GetBoolean()}");
                }

                break;

            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                break;
        }
    }

    private static void CompareObjects(JsonElement obj1, JsonElement obj2, string path, List<string> diffs)
    {
        var props2 = new Dictionary<string, JsonElement>();
        foreach (var prop in obj2.EnumerateObject())
        {
            props2[prop.Name] = prop.Value;
        }

        var visitedProps = new HashSet<string>();

        foreach (var prop1 in obj1.EnumerateObject())
        {
            visitedProps.Add(prop1.Name);
            string currentPath = $"{path}.{prop1.Name}";

            if (props2.TryGetValue(prop1.Name, out var value2))
            {
                CompareElements(prop1.Value, value2, currentPath, diffs);
            }
            else
            {
                diffs.Add($"Missing key at {path}: Property '{prop1.Name}' found in first object but missing in second.");
            }
        }

        foreach (var prop2 in obj2.EnumerateObject())
        {
            if (!visitedProps.Contains(prop2.Name))
            {
                diffs.Add($"Extra key at {path}: Property '{prop2.Name}' found in second object but missing in first.");
            }
        }
    }

    private static void CompareArrays(JsonElement arr1, JsonElement arr2, string path, List<string> diffs)
    {
        int len1 = arr1.GetArrayLength();
        int len2 = arr2.GetArrayLength();

        if (len1 != len2)
        {
            diffs.Add($"Array length mismatch at {path}: First has {len1} items, second has {len2} items.");
        }

        int minLen = Math.Min(len1, len2);
        for (int i = 0; i < minLen; i++)
        {
            CompareElements(arr1[i], arr2[i], $"{path}[{i}]", diffs);
        }
    }

}
