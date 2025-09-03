using System.Xml;
using GW2EIBuilders.JsonModels;
using GW2EIEvtcParser;
using GW2EIJSON;
using System.Text.Json;
using System.Xml.Serialization;

namespace GW2EIBuilders;

public class RawFormatBuilder
{
    private JsonLog _jsonLog;

    public RawFormatBuilder(ParsedEvtcLog log, RawFormatSettings settings, Version parserVersion, UploadResults uploadResults)
    {
        if (settings == null)
        {
            throw new InvalidDataException("Missing settings in RawFormatBuilder");
        }
        _jsonLog = JsonLogBuilder.BuildJsonLog(log, settings, parserVersion, uploadResults.ToArray());
    }

    /// <summary>
    /// Returns a copy of JsonLog object that will be used by the builder.
    /// </summary>
    public JsonLog GetJson()
    {
        //TODO(Rennorb) @perf: What in the javascript is this...
        return JsonSerializer.Deserialize<JsonLog>(JsonSerializer.Serialize(_jsonLog, SerializerSettings.Default)!, SerializerSettings.Default)!;
    }

    /// <summary>
    /// Writes the original JsonLog of the RawFormat builder
    /// </summary>
    public void CreateJSON(Stream stream, bool indent)
    {
        CreateJSON(_jsonLog, stream, indent);
    }

    /// <summary>
    /// Writes the given JsonLog to the stream as json formatted text.
    /// </summary>
    public static void CreateJSON(JsonLog jsonLog, Stream stream, bool indent)
    {
        JsonSerializer.Serialize(stream, jsonLog, indent ? SerializerSettings.Indentent : SerializerSettings.Default);
    }

}
