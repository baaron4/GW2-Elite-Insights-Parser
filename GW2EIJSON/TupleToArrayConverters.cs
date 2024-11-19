using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GW2EIJSON;

public class Tuple2ToArrayConverter<T1, T2> : JsonConverter<ValueTuple<T1, T2>>
{
    public static readonly Tuple2ToArrayConverter<T1, T2> Instance = new();

    public override (T1, T2) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if(reader.Read() == false || reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException($"Encountered wrong token while trying to parse tuple:\nexpected: {JsonTokenType.StartArray}, actual: {reader.TokenType}");
        }

        //TODO(Rennorb) @correctness: not 100% sure if we can just disregard the nullable attribute here
        return (JsonSerializer.Deserialize<T1>(ref reader, options)!, JsonSerializer.Deserialize<T2>(ref reader, options)!);
        
        //NOTE(Rennorb): We must not read the array end token of a container! See MS reader spec for more info.
    }

    public override void Write(Utf8JsonWriter writer, (T1, T2) value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        JsonSerializer.Serialize(writer, value.Item1, options);
        JsonSerializer.Serialize(writer, value.Item2, options);
        writer.WriteEndArray();
    }
}

public class Tuple2ToArrayConverterFactory : JsonConverterFactory
{
    public static readonly Tuple2ToArrayConverterFactory Instance = new();

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(ValueTuple<,>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var tupleTypes = typeToConvert.GenericTypeArguments;
        var converterType = typeof(Tuple2ToArrayConverter<,>).MakeGenericType(tupleTypes);
        return (JsonConverter?)converterType.GetField(nameof(Tuple2ToArrayConverter<int,int>.Instance), BindingFlags.Public | BindingFlags.Static)!.GetValue(null);
    }
}
