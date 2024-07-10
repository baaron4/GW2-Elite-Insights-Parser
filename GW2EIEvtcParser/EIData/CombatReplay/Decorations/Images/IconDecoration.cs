using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class IconDecoration : GenericIconDecoration
    {
        internal class IconDecorationMetadata : GenericIconDecorationMetadata
        {
            public float Opacity { get; }


            public IconDecorationMetadata(string icon, uint pixelSize, uint worldSize, float opacity) : base(icon, pixelSize, worldSize)
            {
                Opacity = (float)Math.Round(opacity, 2);
            }

            public override string GetSignature()
            {
                return "I" + PixelSize + Image.GetHashCode().ToString() + WorldSize + Opacity.ToString();
            }
            public override GenericDecorationMetadataDescription GetCombatReplayMetadataDescription()
            {
                return new IconDecorationMetadataDescription(this);
            }
        }
        internal class IconDecorationRenderingData : GenericIconDecorationRenderingData
        {
            public bool IsSquadMarker { get; private set; }
            public IconDecorationRenderingData((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
            {
            }
            public void UsingSquadMarker(bool isSquadMarker)
            {
                IsSquadMarker = isSquadMarker;
            }
            public override GenericDecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
            {
                return new IconDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
            }
        }
        private new IconDecorationMetadata DecorationMetadata => (IconDecorationMetadata)base.DecorationMetadata;
        private new IconDecorationRenderingData DecorationRenderingData => (IconDecorationRenderingData)base.DecorationRenderingData;

        public float Opacity => DecorationMetadata.Opacity;
        public bool IsSquadMarker => DecorationRenderingData.IsSquadMarker;

        internal IconDecoration(IconDecorationMetadata metadata, IconDecorationRenderingData renderingData) : base(metadata, renderingData)
        {
        }

        public IconDecoration(string icon, uint pixelSize, float opacity, (long start, long end) lifespan, GeographicalConnector connector) : this(icon, pixelSize, 0, opacity, lifespan, connector)
        {
        }

        public IconDecoration(string icon, uint pixelSize, uint worldSize, float opacity, (long start, long end) lifespan, GeographicalConnector connector) : base(new IconDecorationMetadata(icon, pixelSize, worldSize, opacity), new IconDecorationRenderingData(lifespan, connector))
        {
        }

        public IconDecoration(string icon, uint pixelSize, float opacity, Segment lifespan, GeographicalConnector connector) : this(icon, pixelSize, opacity, (lifespan.Start, lifespan.End), connector)
        {
        }

        public IconDecoration(string icon, uint pixelSize, uint worldSize, float opacity, Segment lifespan, GeographicalConnector connector) : this(icon, pixelSize, worldSize, opacity, (lifespan.Start, lifespan.End), connector)
        {
        }

        public IconDecoration UsingSquadMarker(bool isSquadMarker)
        {
            DecorationRenderingData.UsingSquadMarker(isSquadMarker);
            return this;
        }
        //
    }
}
