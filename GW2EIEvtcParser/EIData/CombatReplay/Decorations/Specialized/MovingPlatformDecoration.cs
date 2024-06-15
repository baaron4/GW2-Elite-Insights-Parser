using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class MovingPlatformDecoration : BackgroundDecoration
    {
        internal class ConstantMovingPlatformDecoration : ConstantBackgroundDecoration
        {
            public string Image { get; }
            public int Width { get; }
            public int Height { get; }
            public ConstantMovingPlatformDecoration(string image, int width, int height)
            {
                Image = image;
                Width = width;
                Height = height;
            }

            public override string GetSignature()
            {
                return "MP" + Height + Image.GetHashCode().ToString() + Width;
            }
        }
        internal class VariableMovingPlatformDecoration : VariableBackgroundDecoration
        {
            public List<(float x, float y, float z, float angle, float opacity, int time)> Positions { get; } =
                new List<(float x, float y, float z, float angle, float opacity, int time)>();
            public VariableMovingPlatformDecoration((long, long) lifespan) : base(lifespan)
            {
            }
        }
        private new ConstantMovingPlatformDecoration ConstantDecoration => (ConstantMovingPlatformDecoration)base.ConstantDecoration;
        private new VariableMovingPlatformDecoration VariableDecoration => (VariableMovingPlatformDecoration)base.VariableDecoration;
        //
        public string Image => ConstantDecoration.Image;
        public int Width => ConstantDecoration.Width;
        public int Height => ConstantDecoration.Height;

        public IReadOnlyList<(float x, float y, float z, float angle, float opacity, int time)> Positions => VariableDecoration.Positions;

        public MovingPlatformDecoration(string image, int width, int height, (long start, long end) lifespan) : base()
        {
            base.ConstantDecoration = new ConstantMovingPlatformDecoration(image, width, height);
            base.VariableDecoration = new VariableMovingPlatformDecoration(lifespan);
        }

        public void AddPosition(float x, float y, float z, double angle, double opacity, int time)
        {
            VariableDecoration.Positions.Add((x, y, z, (float)angle, (float)opacity, time));
        }

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new MovingPlatformDecorationCombatReplayDescription(this, map);
        }
    }
}
