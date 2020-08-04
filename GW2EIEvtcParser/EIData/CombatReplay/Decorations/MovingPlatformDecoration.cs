using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public class MovingPlatformDecoration : BackgroundDecoration
    {
        public string Image { get; }
        public int Width { get; }
        public int Height { get; }

        public List<(double x, double y, double z, double angle, double opacity, int time)> Positions { get; } =
            new List<(double x, double y, double z, double angle, double opacity, int time)>();

        public MovingPlatformDecoration(string image, int width, int height, (int start, int end) lifespan) : base(lifespan)
        {
            Image = image;
            Width = width;
            Height = height;
        }

        public void AddPosition(double x, double y, double z, double angle, double opacity, int time)
        {
            Positions.Add((x, y, z, angle, opacity, time));
        }

        public override GenericDecorationSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new MovingPlatformDecorationSerializable(this, map);
        }
    }
}
