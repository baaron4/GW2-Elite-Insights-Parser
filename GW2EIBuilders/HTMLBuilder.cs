using System.IO.Compression;
using System.Text;
using GW2EIBuilders.HtmlModels;
using GW2EIEvtcParser;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tracing;

[assembly: CLSCompliant(false)]
namespace GW2EIBuilders;

// compile-time generated serialization logic
[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata | JsonSourceGenerationMode.Serialization,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    IncludeFields = true, WriteIndented = false, NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
    Converters = [
        typeof(GW2EIJSON.Tuple2ToExceptionConverterFactory),
    ]
)]
[JsonSerializable(typeof(LogDataDto))]
partial class LogDataDtoSerializerContext : JsonSerializerContext {  }


public class HTMLBuilder
{
    private static readonly UTF8Encoding NoBOMEncodingUTF8 = new(false);

    private readonly string _eiJS;
    private readonly string _eiCRJS;
    private readonly string _eiHealingExtJS;

    private readonly string _scriptVersion;
    private readonly int _scriptVersionRev;

    private readonly ParsedEvtcLog _log;
    private readonly Version _parserVersion;
    private readonly bool _cr;
    private readonly bool _light;
    private readonly bool _externalScripts;
    private readonly string? _externalScriptsPath;
    private readonly string? _externalScriptsCdn;
    private readonly bool _compressJson;

    private readonly UploadResults _uploadLink;

    // https://point2blog.wordpress.com/2012/12/26/compressdecompress-a-string-in-c/
    private static string CompressAndBase64(string s)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(s);
        using (var msi = new MemoryStream(bytes))
        {
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }
                return Convert.ToBase64String(mso.ToArray());
            }
        }
    }

    public HTMLBuilder(ParsedEvtcLog log, HTMLSettings settings, HTMLAssets assets, Version parserVersion, UploadResults uploadResults)
    {
        if (settings == null)
        {
            throw new InvalidDataException("Missing settings in HTMLBuilder");
        }
        _eiJS = assets.EIJavascriptCode;
        _eiCRJS = assets.EICRJavascriptCode;
        _eiHealingExtJS = assets.EIHealingExtJavascriptCode;
        _parserVersion = parserVersion;
        _scriptVersion = parserVersion.Major + "." + parserVersion.Minor;
#if !DEBUG
        _scriptVersion += "." + parserVersion.Build;
#else
        _scriptVersion += "-debug";
#endif
        _scriptVersionRev = parserVersion.Revision;
        _log = log;

        _uploadLink = uploadResults;

        _cr = _log.CanCombatReplay;
        _light = settings.HTMLLightTheme;
        _externalScripts = settings.ExternalHTMLScripts;
        _externalScriptsPath = settings.ExternalHtmlScriptsPath;
        _externalScriptsCdn = settings.ExternalHtmlScriptsCdn;
        _compressJson = settings.CompressJson;
    }

    private (string?, string?) BuildAssetPaths(string? path)
    {
        string? cdn = null;
        string? external = null;
        if (_externalScripts && !string.IsNullOrWhiteSpace(path))
        {
            if (!string.IsNullOrWhiteSpace(_externalScriptsCdn))
            {
                cdn = _externalScriptsCdn.EndsWith('/') ? _externalScriptsCdn[..^1] : _externalScriptsCdn;
            }
            external = path;
            // Setting: External Scripts Path
            // overwrite jsPath (create directory) if files should be placed on different location
            // settings.externalHtmlScriptsPath is set by the user
            if (!string.IsNullOrWhiteSpace(_externalScriptsPath))
            {
                bool validPath = false;

                if (!Directory.Exists(_externalScriptsPath))
                {
                    try
                    {
                        Directory.CreateDirectory(_externalScriptsPath);
                        validPath = true;
                    }
                    catch
                    {
                        // something went wrong on creating the external folder (invalid chars?)      
                        // this will skip the saving in this path and continue with jsscript files in the root path for the report
                        _log.UpdateProgressWithCancellationCheck("HTML Warning: can't create external script folder");
                    }
                }
                else
                {
                    try
                    {
                        // Verify write access
                        // https://stackoverflow.com/a/6371533
                        using (FileStream fs = File.Create(
                               Path.Combine(
                                   _externalScriptsPath,
                                   "EI-" + Path.GetRandomFileName()
                               ),
                               1,
                               FileOptions.DeleteOnClose)
                           )
                        { }
                        validPath = true;
                    }
                    catch
                    {
                        _log.UpdateProgressWithCancellationCheck("HTML Warning: can't write in external script folder");
                        // couldn't write to directory
                    }
                }

                // if the creation of the folder did not fail or the folder already exists use it to include within the report
                if (validPath)
                {
                    external = _externalScriptsPath;
                }
            }
        }
        return (external, cdn);
    }

    /// <summary>
    /// Create the damage taken distribution table for a given player
    /// </summary>
    /// <param name="p"></param>
    /// <param name="phaseIndex"></param>

    public void CreateHTML(StreamWriter sw, string? path)
    {
        using var _t = new AutoTrace("Create HTML (write to stream)");

        StringBuilder html = new(Properties.Resources.tmplMain);
        var (externalPath, cdnPath) = BuildAssetPaths(path);
        _log.UpdateProgressWithCancellationCheck("HTML: replacing global variables");
        html.Replace("${bootstrapTheme}", !_light ? "slate" : "yeti");
        
        // Compression stuff
        html.Replace("<!--${CompressionRequire}-->", _compressJson ? "<script src=\"https://cdnjs.cloudflare.com/ajax/libs/pako/1.0.10/pako.min.js\"></script>" : "");
        html.Replace("<!--${CompressionUtils}-->", _compressJson ? Properties.Resources.compressionUtils : "");

        _log.UpdateProgressWithCancellationCheck("HTML: building CSS");
        html.Replace("<!--${Css}-->", BuildCss(externalPath, cdnPath));
        _log.UpdateProgressWithCancellationCheck("HTML: building JS");
        html.Replace("<!--${Js}-->", BuildEIJs(externalPath, cdnPath));
        _log.UpdateProgressWithCancellationCheck("HTML: building Combat Replay JS");
        html.Replace("<!--${CombatReplayJS}-->", BuildCombatReplayJS(externalPath, cdnPath));
        html.Replace("<!--${HealingExtensionJS}-->", BuildHealingExtensionJS(externalPath, cdnPath));

        var logData = LogDataDto.BuildLogData(_log, _cr, _light, _parserVersion, _uploadLink);
        var crData = logData.CrData;
        var graphData = logData.GraphData;
        var healingExtData = logData.HealingStatsExtension;
        var barrierExtData = logData.BarrierStatsExtension;
        logData.CrData = null;
        logData.GraphData = null;
        logData.HealingStatsExtension = null;
        logData.BarrierStatsExtension = null;
        _t.Log("built log data");
        //NOTE(Rennoeb): json last, because its large
        string json = JsonSerializer.Serialize(logData, LogDataDtoSerializerContext.Default.LogDataDto);
        string jsonGraph = graphData != null ? JsonSerializer.Serialize(graphData, LogDataDtoSerializerContext.Default.ChartDataDto) : "null";
        string jsonCR = crData != null ? JsonSerializer.Serialize(crData, LogDataDtoSerializerContext.Default.CombatReplayDto) : "null";
        _t.Log("Serialized JSON");
        string jsonHeal = healingExtData != null ? JsonSerializer.Serialize(healingExtData, LogDataDtoSerializerContext.Default.HealingStatsExtension) : "null";
        _t.Log("Serialized JSON");
        string jsonBarrier = barrierExtData != null ? JsonSerializer.Serialize(barrierExtData, LogDataDtoSerializerContext.Default.BarrierStatsExtension) : "null";
        _t.Log("Serialized JSON");

        html.Replace("'${logDataJson}'", _compressJson ? ("'" + CompressAndBase64(json) + "'") : json);
        html.Replace("'${CRDataJson}'", _compressJson ? ("'" + CompressAndBase64(jsonCR) + "'") : jsonCR);
        html.Replace("'${graphDataJson}'", _compressJson ? ("'" + CompressAndBase64(jsonGraph) + "'") : jsonGraph);
        html.Replace("'${healingDataJson}'", _compressJson ? ("'" + CompressAndBase64(jsonHeal) + "'") : jsonHeal);
        html.Replace("'${barrierDataJson}'", _compressJson ? ("'" + CompressAndBase64(jsonBarrier) + "'") : jsonBarrier);
        _t.Log("appended JSON");

        sw.Write(html);
        return;
    }

    private static string CreateAssetFile(string? externalPath, string? cdnPath, string fileName, string content)
    {
        bool externalNull = string.IsNullOrEmpty(externalPath);
        bool cdnNull = string.IsNullOrEmpty(cdnPath);
        if (externalNull && cdnNull)
        {
            throw new InvalidDataException("Either externalPath or cdnPath must be non null");
        }
        string filePath = "";
        // generate file if external is present
        if (!externalNull)
        {
            filePath = Path.Combine(externalPath!, fileName);

            // always create file in DEBUG
#if !DEBUG
            // if the file already exists, skip creation
            if (!File.Exists(filePath))
            {
#endif
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (var scriptWriter = new StreamWriter(fs, NoBOMEncodingUTF8))
                {
                    scriptWriter.Write(content);
                }
            }
            catch (IOException)
            {
            }
#if !DEBUG
            }
#endif
        }
        // Priority to cdn
        if (!cdnNull)
        {
            return cdnPath + "/" + fileName;
        }
        return "file://" + filePath;
    }

    private string BuildCombatReplayJS(string? externalPath, string? cdnPath)
    {
        if (!_cr)
        {
            return "";
        }
        string scriptContent = _eiCRJS;
        bool externalNull = string.IsNullOrEmpty(externalPath);
        bool cdnNull = string.IsNullOrEmpty(cdnPath);
        if (!externalNull || !cdnNull)
        {
            string fileName = "EliteInsights-CR-" + _scriptVersion + ".js";
            string path = CreateAssetFile(externalPath, cdnPath, fileName, scriptContent);
            return "<script src=\"" + path + "?version=" + _scriptVersionRev + "\"></script>\n";
        }
        else
        {
            return "<script>\r\n" + scriptContent + "\r\n</script>";
        }
    }

    private string BuildHealingExtensionJS(string? externalPath, string? cdnPath)
    {
        if (!_log.CombatData.HasEXTHealing)
        {
            return "";
        }
        string scriptContent = _eiHealingExtJS;
        bool externalNull = string.IsNullOrEmpty(externalPath);
        bool cdnNull = string.IsNullOrEmpty(cdnPath);
        if (!externalNull || !cdnNull)
        {
            string fileName = "EliteInsights-HealingExt-" + _scriptVersion + ".js";
            string path = CreateAssetFile(externalPath, cdnPath, fileName, scriptContent);
            return "<script src=\"" + path + "?version=" + _scriptVersionRev + "\"></script>\n";
        }
        else
        {
            return "<script>\r\n" + scriptContent + "\r\n</script>";
        }
    }

    private string BuildCss(string? externalPath, string? cdnPath)
    {
        string scriptContent = Properties.Resources.css;
        bool externalNull = string.IsNullOrEmpty(externalPath);
        bool cdnNull = string.IsNullOrEmpty(cdnPath);
        if (!externalNull || !cdnNull)
        {
            string fileName = "EliteInsights-" + _scriptVersion + ".css";
            string path = CreateAssetFile(externalPath, cdnPath, fileName, scriptContent);
            return "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + path + "?version=" + _scriptVersionRev + "\">";
        }
        else
        {
            return "<style type=\"text/css\">\r\n" + scriptContent + "\r\n</style>";
        }
    }

    private string BuildEIJs(string? externalPath, string? cdnPath)
    {
        string scriptContent = _eiJS;
        bool externalNull = string.IsNullOrEmpty(externalPath);
        bool cdnNull = string.IsNullOrEmpty(cdnPath);
        if (!externalNull || !cdnNull)
        {
            string fileName = "EliteInsights-" + _scriptVersion + ".js";
            string path = CreateAssetFile(externalPath, cdnPath, fileName, scriptContent);
            return "<script src=\"" + path + "?version=" + _scriptVersionRev + "\"></script>";
        }
        else
        {
            return "<script>\r\n" + scriptContent + "\r\n</script>";
        }
    }
}
