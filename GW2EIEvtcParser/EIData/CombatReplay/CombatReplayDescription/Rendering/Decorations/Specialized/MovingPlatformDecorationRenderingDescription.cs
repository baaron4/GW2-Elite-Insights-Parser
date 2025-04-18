using System.Text.Json;
using System.Text.Json.Serialization;
using static GW2EIEvtcParser.EIData.MovingPlatformDecoration;

namespace GW2EIEvtcParser.EIData;

using Position = (float x, float y, float z, float Angle, float Opacity, long Time);

public class MovingPlatformDecorationRenderingDescription : BackgroundDecorationRenderingDescription
{
    public class PositionConverter : JsonConverter<Position[]>
    {
        public override void Write(Utf8JsonWriter writer, Position[] value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var (x, y, z, angle, opacity, time) in value)
            {
                writer.WriteStartArray();
                writer.WriteNumberValue(x);
                writer.WriteNumberValue(y);
                writer.WriteNumberValue(z);
                writer.WriteNumberValue(angle);
                writer.WriteNumberValue(opacity);
                writer.WriteNumberValue(time);
                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }

        public override Position[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }

    [JsonConverter(typeof(PositionConverter))]
    public Position[] Positions { get; set; }


    internal MovingPlatformDecorationRenderingDescription(MovingPlatformDecorationRenderingData decoration, CombatReplayMap map, string metadataSignature) : base(decoration, metadataSignature)
    {
        Type = Types.MovingPlatform;
        Positions = decoration.Positions.OrderBy(x => x.time).Select(pos =>
        {
            (pos.x, pos.y) = map.GetMapCoordRounded(pos.x, pos.y);
            return pos;
        }).ToArray();
    }

}
