using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using static GW2EIEvtcParser.EIData.MovingPlatformDecoration;

namespace GW2EIEvtcParser.EIData
{
    using Position = (float x, float y, float z, float angle, float opacity, int time);

    public class MovingPlatformDecorationRenderingDescription : BackgroundDecorationRenderingDescription
    {
        private class PositionConverter : JsonConverter<Position[]>
        {
            public override void Write(Utf8JsonWriter writer, Position[] positions, JsonSerializerOptions serializer)
            {
                writer.WriteStartArray();
                foreach ((float x, float y, float z, float angle, float opacity, int time) in positions)
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

            public override Position[] Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions serializer)
            {
                throw new NotSupportedException();
            }

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(Position);
            }
        }

        [JsonConverter(typeof(PositionConverter))]
        public Position[] Positions { get; set; }


        internal MovingPlatformDecorationRenderingDescription(MovingPlatformDecorationRenderingData decoration, CombatReplayMap map, string metadataSignature) : base(decoration, metadataSignature)
        {
            Type = "MovingPlatform";
            Positions = decoration.Positions.OrderBy(x => x.time).Select(pos =>
            {
                (pos.x, pos.y) = map.GetMapCoord(pos.x, pos.y);
                return pos;
            }).ToArray();
        }

    }

}
