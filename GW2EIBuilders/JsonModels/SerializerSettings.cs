
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GW2EIBuilders.JsonModels;
internal class SerializerSettings
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
        //NOTE(Rennorb): htmlescapes by default
        Converters = {
            //TODO(Rennorb) @perf: Replace with explicit attributes so it doesn't need to be invoked dynamically.
            GW2EIJSON.Tuple2ToArrayConverterFactory.Instance,
        }
    };

    public static readonly JsonSerializerOptions Indentent = new(Default)
    {
        WriteIndented = true,
    };
}
