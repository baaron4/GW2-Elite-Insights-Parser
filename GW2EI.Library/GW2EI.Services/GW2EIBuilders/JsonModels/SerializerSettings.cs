
using System.Text.Json;
using System.Text.Json.Serialization;
using GW2EIJSON;

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
        TypeInfoResolver = JsonLogSerializerContext.Default,
        //NOTE(Rennorb): htmlescapes by default
    };

    public static readonly JsonSerializerOptions Indentent = new(Default)
    {
        WriteIndented = true,
    };
}
