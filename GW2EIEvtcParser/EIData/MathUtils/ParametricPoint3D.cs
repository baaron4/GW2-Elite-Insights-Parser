using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GW2EIEvtcParser.EIData;

// Serialized as part of several decoration descriptors. Take care when changing shape.
[JsonConverter(typeof(Converter))]
public readonly struct ParametricPoint3D(in Vector3 value, long time)
{
    public readonly Vector3 XYZ = value;
    public readonly long Time = time;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ParametricPoint3D(float x, float y, float z, long time) : this(new(x,y,z), time)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly ParametricPoint3D WithChangedTime(long newTime) => new(XYZ, newTime);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsNaNOrInfinity()
    {
        return Double.IsNaN(XYZ.X) || Double.IsNaN(XYZ.Y) || Double.IsNaN(XYZ.Z) ||
               Double.IsInfinity(XYZ.X) || Double.IsInfinity(XYZ.Y) || Double.IsInfinity(XYZ.Z);
    }

    public class Converter : JsonConverter<ParametricPoint3D>
    {
        public override ParametricPoint3D Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"Failed to read {nameof(ParametricPoint3D)}");
            }

            var v = new Vector3();
            long t = 0;


            Span<char> buffer = stackalloc char[8];
            for(int i = 0; i < 4; i++)
            {
                var len = reader.CopyString(buffer);
                switch(buffer[..len])
                {
                    case "X": v.X = reader.GetSingle(); break;
                    case "Y": v.Y = reader.GetSingle(); break;
                    case "Z": v.Z = reader.GetSingle(); break;
                    case "Time": t = reader.GetInt64(); break;
                }
            }

            //NOTE(Rennorb): We must not read the object end token of a container! See MS reader spec for more info.

            return new(v, t);
        }

        public override void Write(Utf8JsonWriter writer, ParametricPoint3D value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("X", value.XYZ.X);
            writer.WriteNumber("Y", value.XYZ.Y);
            writer.WriteNumber("Z", value.XYZ.Z);
            writer.WriteNumber("Time", value.Time);
            writer.WriteEndObject();
        }
    }
}
