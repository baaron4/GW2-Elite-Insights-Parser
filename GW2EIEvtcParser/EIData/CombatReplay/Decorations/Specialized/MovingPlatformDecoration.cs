using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class MovingPlatformDecoration : BackgroundDecoration
    {
        internal class MovingPlatformDecorationMetadata : BackgroundDecorationMetadata
        {
            public string Image { get; }
            public int Width { get; }
            public int Height { get; }
            public MovingPlatformDecorationMetadata(string image, int width, int height)
            {
                Image = image;
                Width = width;
                Height = height;
            }

            public override string GetSignature()
            {
                return "MP" + Height + Image.GetHashCode().ToString() + Width;
            }
            internal override GenericDecoration GetDecorationFromVariable(VariableGenericDecoration variable)
            {
                if (variable is VariableMovingPlatformDecoration expectedVariable)
                {
                    return new MovingPlatformDecoration(this, expectedVariable);
                }
                throw new InvalidOperationException("Expected VariableMovingPlatformDecoration");
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
        private new MovingPlatformDecorationMetadata DecorationMetadata => (MovingPlatformDecorationMetadata)base.DecorationMetadata;
        private new VariableMovingPlatformDecoration VariableDecoration => (VariableMovingPlatformDecoration)base.VariableDecoration;
        //
        public string Image => DecorationMetadata.Image;
        public int Width => DecorationMetadata.Width;
        public int Height => DecorationMetadata.Height;

        public IReadOnlyList<(float x, float y, float z, float angle, float opacity, int time)> Positions => VariableDecoration.Positions;
        internal MovingPlatformDecoration(MovingPlatformDecorationMetadata metadata, VariableMovingPlatformDecoration variable) : base(metadata, variable)
        {
        }
        public MovingPlatformDecoration(string image, int width, int height, (long start, long end) lifespan) : base(new MovingPlatformDecorationMetadata(image, width, height), new VariableMovingPlatformDecoration(lifespan))
        {
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
