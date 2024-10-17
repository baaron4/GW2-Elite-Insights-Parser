
using System.Text.Json;

namespace GW2EIBuilders.JsonModels;
internal class SerializerSettings
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        IncludeFields = true,
        WriteIndented = false,
        //NOTE(Rennorb): htmlescapes by default
    };

    public static readonly JsonSerializerOptions Indentent = new(Default)
    {
        WriteIndented = true,
    };
}
