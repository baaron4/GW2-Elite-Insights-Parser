using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GW2EIJSON;

public class Tuple2ToExceptionConverter<T1, T2> : JsonConverter<ValueTuple<T1, T2>>
{
    public static readonly Tuple2ToExceptionConverter<T1, T2> Instance = new();

    public override (T1, T2) Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new InvalidOperationException("Tuples are not supported");
    }

    public override void Write(Utf8JsonWriter writer, (T1, T2) value, JsonSerializerOptions options)
    {
        throw new InvalidOperationException("Tuples are not supported");
    }
}

public class Tuple2ToExceptionConverterFactory : JsonConverterFactory
{
    public static readonly Tuple2ToExceptionConverterFactory Instance = new();

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(ValueTuple<,>);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var tupleTypes = typeToConvert.GenericTypeArguments;
        var converterType = typeof(Tuple2ToExceptionConverter<,>).MakeGenericType(tupleTypes);
        return (JsonConverter?)converterType.GetField(nameof(Tuple2ToExceptionConverter<int,int>.Instance), BindingFlags.Public | BindingFlags.Static)!.GetValue(null);
    }
}
