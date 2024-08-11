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
            public override GenericDecorationMetadataDescription GetCombatReplayMetadataDescription()
            {
                return new MovingPlatformDecorationMetadataDescription(this);
            }
        }
        internal class MovingPlatformDecorationRenderingData : BackgroundDecorationRenderingData
        {
            public List<(float x, float y, float z, float angle, float opacity, int time)> Positions { get; } =
                new List<(float x, float y, float z, float angle, float opacity, int time)>();
            public MovingPlatformDecorationRenderingData((long, long) lifespan) : base(lifespan)
            {
            }

            public override GenericDecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
            {
                return new MovingPlatformDecorationRenderingDescription(this, map, metadataSignature);
            }
        }
        private new MovingPlatformDecorationMetadata DecorationMetadata => (MovingPlatformDecorationMetadata)base.DecorationMetadata;
        private new MovingPlatformDecorationRenderingData DecorationRenderingData => (MovingPlatformDecorationRenderingData)base.DecorationRenderingData;
        //
        public string Image => DecorationMetadata.Image;
        public int Width => DecorationMetadata.Width;
        public int Height => DecorationMetadata.Height;

        public IReadOnlyList<(float x, float y, float z, float angle, float opacity, int time)> Positions => DecorationRenderingData.Positions;
        internal MovingPlatformDecoration(MovingPlatformDecorationMetadata metadata, MovingPlatformDecorationRenderingData renderingData) : base(metadata, renderingData)
        {
        }
        public MovingPlatformDecoration(string image, int width, int height, (long start, long end) lifespan) : base(new MovingPlatformDecorationMetadata(image, width, height), new MovingPlatformDecorationRenderingData(lifespan))
        {
        }

        public void AddPosition(float x, float y, float z, double angle, double opacity, int time)
        {
            DecorationRenderingData.Positions.Add((x, y, z, (float)angle, (float)opacity, time));
        }
    }
}
