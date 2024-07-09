using System;
using System.Linq;
using Newtonsoft.Json;

namespace GW2EIEvtcParser.EIData
{
    public class MovingPlatformDecorationCombatReplayDescription : BackgroundDecorationCombatReplayDescription
    {
        private class PositionConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var positions = ((float x, float y, float z, float angle, float opacity, int time)[])value;
                writer.WriteStartArray();
                foreach ((float x, float y, float z, float angle, float opacity, int time) in positions)
                {
                    writer.WriteStartArray();
                    writer.WriteValue(x);
                    writer.WriteValue(y);
                    writer.WriteValue(z);
                    writer.WriteValue(angle);
                    writer.WriteValue(opacity);
                    writer.WriteValue(time);
                    writer.WriteEndArray();
                }

                writer.WriteEndArray();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                throw new NotSupportedException();
            }

            public override bool CanRead => false;

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof((float x, float y, float z, float angle, float opacity, int time));
            }
        }


        public string Image { get; }
        public int Height { get; }
        public int Width { get; }

        [JsonConverter(typeof(PositionConverter))]
        public (float x, float y, float z, float angle, float opacity, int time)[] Positions { get; set; }


        internal MovingPlatformDecorationCombatReplayDescription(MovingPlatformDecoration decoration, CombatReplayMap map) : base(decoration)
        {
            Type = "MovingPlatform";
            Image = decoration.Image;
            Width = decoration.Width;
            Height = decoration.Height;
            Positions = decoration.Positions.OrderBy(x => x.time).Select(pos =>
            {
                (float mapX, float mapY) = map.GetMapCoord((float)pos.x, (float)pos.y);
                pos.x = mapX;
                pos.y = mapY;

                return pos;
            }).ToArray();
        }

    }

}
