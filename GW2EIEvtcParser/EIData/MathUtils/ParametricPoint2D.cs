using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GW2EIEvtcParser.EIData;

[JsonConverter(typeof(Converter))]
public class ParametricPoint2D(in Vector2 vector, long time)
{
    public readonly Vector2 XY = vector;
    public readonly long Time = time;

    public ParametricPoint2D(in Vector2 a, in Vector2 b, float ratio, long time) : this(Vector2.Lerp(a, b, ratio), time)
    {
    }

    public class Converter : JsonConverter<ParametricPoint2D>
    {
        public override ParametricPoint2D? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Failed to read {nameof(ParametricPoint2D)}");
            }

            var v = new Vector2();
            long t = 0;


            Span<char> buffer = stackalloc char[8];
            for(int i = 0; i < 4; i++)
            {
                var len = reader.CopyString(buffer);
                switch(buffer[..len])
                {
                    case "X": v.X = reader.GetSingle(); break;
                    case "Y": v.Y = reader.GetSingle(); break;
                    case "Time": t = reader.GetInt64(); break;
                }
            }

            //NOTE(Rennorb): We must not read the object end token of a container! See MS reader spec for more info.

            return new(in v, t);
        }

        public override void Write(Utf8JsonWriter writer, ParametricPoint2D value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("X", value.XY.X);
            writer.WriteNumber("Y", value.XY.Y);
            writer.WriteNumber("Time", value.Time);
            writer.WriteEndObject();
        }
    }
}
