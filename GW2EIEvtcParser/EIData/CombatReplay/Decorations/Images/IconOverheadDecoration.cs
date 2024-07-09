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
            internal override GenericDecoration GetDecorationFromVariable(GenericDecorationRenderingData renderingData)
            {
                if (renderingData is IconOverheadDecorationRenderingData  expectedRenderingData)
                {
                    return new IconOverheadDecoration(this,  expectedRenderingData);
                }
                throw new InvalidOperationException("Expected VariableIconOverheadDecoration");
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
        }
        internal IconOverheadDecoration(IconOverheadDecorationMetadata metadata, IconOverheadDecorationRenderingData renderingData) : base(metadata, renderingData)
        {
        }
        public IconOverheadDecoration(string icon, uint pixelSize, float opacity, (long start, long end) lifespan, AgentConnector connector) : base(new IconDecorationMetadata(icon, pixelSize, Math.Min(connector.Agent.HitboxWidth / 2, 250), opacity), new IconDecorationRenderingData(lifespan, connector))
        {
        }

        public IconOverheadDecoration(string icon, uint pixelSize, float opacity, Segment lifespan, AgentConnector connector) : this(icon, pixelSize, opacity, (lifespan.Start, lifespan.End), connector)
        {
        }
        //

        public override GenericDecorationRenderableDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new IconOverheadDecorationRenderableDescription(log, this, map, usedSkills, usedBuffs);
        }
    }
}
