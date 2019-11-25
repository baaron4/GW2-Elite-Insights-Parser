using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using Newtonsoft.Json;

namespace GW2EIParser.EIData
{
    public class MovingPlatformDecorationSerializable : BackgroundDecorationSerializable
    {
        private class PositionConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var positions = ((double x, double y, double z, double angle, double opacity, int time)[])value;
                writer.WriteStartArray();
                foreach ((double x, double y, double z, double angle, double opacity, int time) position in positions)
                {
                    (double x, double y, double z, double angle, double opacity, int time) = position;
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
                return objectType == typeof((double x, double y, double z, double angle, double opacity, int time));
            }
        }


        public string Image { get;}
        public int Height { get; }
        public int Width { get; }

        [JsonConverter(typeof(PositionConverter))]
        public (double x, double y, double z, double angle, double opacity, int time)[] Positions { get; set; }


        public MovingPlatformDecorationSerializable(ParsedLog log, MovingPlatformDecoration decoration, CombatReplayMap map) : base(log, decoration, map)
        {
            Type = "MovingPlatform";
            Image = decoration.Image;
            Width = decoration.Width;
            Height = decoration.Height;
            Positions = decoration.Positions.OrderBy(x => x.time).Select(pos =>
            {
                (double mapX, double mapY) = map.GetMapCoord((float)pos.x, (float)pos.y);
                pos.x = mapX;
                pos.y = mapY;

                return pos;
            }).ToArray();
        }

    }

}
