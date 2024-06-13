using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class IconDecoration : GenericIconDecoration
    {
        internal class ConstantIconDecoration : ConstantGenericIconDecoration
        {
            public float Opacity { get; }


            public ConstantIconDecoration(string icon, uint pixelSize, uint worldSize, float opacity) : base(icon, pixelSize, worldSize)
            {
                Opacity = opacity;
            }

            public override string GetID()
            {
                throw new NotImplementedException();
            }
        }
        internal class VariableIconDecoration : VariableGenericIconDecoration
        {
            public bool IsSquadMarker { get; private set; }
            public VariableIconDecoration((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
            {
            }
            public void UsingSquadMarker(bool isSquadMarker)
            {
                IsSquadMarker = isSquadMarker;
            }
        }
        private new ConstantIconDecoration ConstantDecoration => (ConstantIconDecoration)base.ConstantDecoration;
        private new VariableIconDecoration VariableDecoration => (VariableIconDecoration)base.VariableDecoration;

        public float Opacity => ConstantDecoration.Opacity;
        public bool IsSquadMarker => VariableDecoration.IsSquadMarker;

        protected IconDecoration()
        {

        }

        public IconDecoration(string icon, uint pixelSize, float opacity, (long start, long end) lifespan, GeographicalConnector connector) : base()
        {
            base.ConstantDecoration = new ConstantIconDecoration(icon, pixelSize, 0, opacity);
            base.VariableDecoration = new VariableIconDecoration(lifespan, connector);
        }

        public IconDecoration(string icon, uint pixelSize, uint worldSize, float opacity, (long start, long end) lifespan, GeographicalConnector connector) : base()
        {
            base.ConstantDecoration = new ConstantIconDecoration(icon, pixelSize, worldSize, opacity);
            base.VariableDecoration = new VariableIconDecoration(lifespan, connector);
        }

        public IconDecoration(string icon, uint pixelSize, float opacity, Segment lifespan, GeographicalConnector connector) : this(icon, pixelSize, opacity, (lifespan.Start, lifespan.End), connector)
        {
        }

        public IconDecoration(string icon, uint pixelSize, uint worldSize, float opacity, Segment lifespan, GeographicalConnector connector) : this(icon, pixelSize, worldSize, opacity, (lifespan.Start, lifespan.End), connector)
        {
        }

        public IconDecoration UsingSquadMarker(bool isSquadMarker)
        {
            VariableDecoration.UsingSquadMarker(isSquadMarker);
            return this;
        }
        //
        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new IconDecorationCombatReplayDescription(log, this, map, usedSkills, usedBuffs);
        }
    }
}
