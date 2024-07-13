using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class IconOverheadDecoration : IconDecoration
    {
        internal class IconOverheadDecorationMetadata : IconDecorationMetadata
        {


            public IconOverheadDecorationMetadata(string icon, uint pixelSize, uint worldSize, float opacity) : base(icon, pixelSize, worldSize, opacity)
            {
            }

            public override string GetSignature()
            {
                return "IO" + PixelSize + Image.GetHashCode().ToString() + WorldSize + Opacity.ToString();
            }
            public override GenericDecorationMetadataDescription GetCombatReplayMetadataDescription()
            {
                return new IconOverheadDecorationMetadataDescription(this);
            }
        }
        internal class IconOverheadDecorationRenderingData : IconDecorationRenderingData
        {
            public IconOverheadDecorationRenderingData((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
            {
            }
            public override void UsingSkillMode(SkillModeDescriptor skill)
            {
            }

            public override GenericDecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
            {
                return new IconOverheadDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
            }
        }
        internal IconOverheadDecoration(IconOverheadDecorationMetadata metadata, IconOverheadDecorationRenderingData renderingData) : base(metadata, renderingData)
        {
        }
        public IconOverheadDecoration(string icon, uint pixelSize, float opacity, (long start, long end) lifespan, AgentConnector connector) : base(new IconOverheadDecorationMetadata(icon, pixelSize, Math.Min(connector.Agent.HitboxWidth / 2, 250), opacity), new IconOverheadDecorationRenderingData(lifespan, connector))
        {
        }

        public IconOverheadDecoration(string icon, uint pixelSize, float opacity, Segment lifespan, AgentConnector connector) : this(icon, pixelSize, opacity, (lifespan.Start, lifespan.End), connector)
        {
        }
        //
    }
}
