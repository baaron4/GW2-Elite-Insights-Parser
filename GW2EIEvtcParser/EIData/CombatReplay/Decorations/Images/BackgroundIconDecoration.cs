using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BackgroundIconDecoration : GenericIconDecoration
    {
        public IReadOnlyList<ParametricPoint1D> Opacities { get; }
        public IReadOnlyList<ParametricPoint1D> Heights { get; }
        public BackgroundIconDecoration(string icon, uint pixelSize, uint worldSize, IReadOnlyList<ParametricPoint1D> opacities, IReadOnlyList<ParametricPoint1D> heights, (long start, long end) lifespan, GeographicalConnector connector) : base(icon, pixelSize, worldSize, lifespan, connector)
        {
            Opacities = opacities;
            Heights = heights;
        }

        public BackgroundIconDecoration(string icon, uint pixelSize, uint worldSize, IReadOnlyList<ParametricPoint1D> opacities, IReadOnlyList<ParametricPoint1D> heights, Segment lifespan, GeographicalConnector connector) : this(icon, pixelSize, worldSize, opacities, heights, (lifespan.Start, lifespan.End), connector)
        {
        }

        public override GenericAttachedDecoration UsingSkillMode(SkillModeDescriptor skill)
        {
            return this;
        }

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            return new BackgroundIconDecorationCombatReplayDescription(log, this, map);
        }
    }
}
