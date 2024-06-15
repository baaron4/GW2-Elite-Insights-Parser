using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BackgroundIconDecoration : GenericIconDecoration
    {
        internal class ConstantBackgroundIconDecoration : ConstantGenericIconDecoration
        {

            public ConstantBackgroundIconDecoration(string icon, uint pixelSize, uint worldSize) : base(icon, pixelSize, worldSize)
            {
            }

            public override string GetSignature()
            {
                return "BI" + PixelSize + Image.GetHashCode().ToString() + WorldSize;
            }
        }
        internal class VariableBackgroundIconDecoration : VariableGenericIconDecoration
        {
            public IReadOnlyList<ParametricPoint1D> Opacities { get; }
            public IReadOnlyList<ParametricPoint1D> Heights { get; }
            public VariableBackgroundIconDecoration((long, long) lifespan, IReadOnlyList<ParametricPoint1D> opacities, IReadOnlyList<ParametricPoint1D> heights, GeographicalConnector connector) : base(lifespan, connector)
            {
                Opacities = opacities;
                Heights = heights;
            }
            public override void UsingSkillMode(SkillModeDescriptor skill)
            {
            }
        }
        private new VariableBackgroundIconDecoration VariableDecoration => (VariableBackgroundIconDecoration)base.VariableDecoration;

        public IReadOnlyList<ParametricPoint1D> Opacities => VariableDecoration.Opacities;
        public IReadOnlyList<ParametricPoint1D> Heights => VariableDecoration.Heights;
        public BackgroundIconDecoration(string icon, uint pixelSize, uint worldSize, IReadOnlyList<ParametricPoint1D> opacities, IReadOnlyList<ParametricPoint1D> heights, (long start, long end) lifespan, GeographicalConnector connector) : base()
        {
            ConstantDecoration = new ConstantBackgroundIconDecoration(icon, pixelSize, worldSize);
            base.VariableDecoration = new VariableBackgroundIconDecoration(lifespan, opacities, heights, connector);
        }

        public BackgroundIconDecoration(string icon, uint pixelSize, uint worldSize, IReadOnlyList<ParametricPoint1D> opacities, IReadOnlyList<ParametricPoint1D> heights, Segment lifespan, GeographicalConnector connector) : this(icon, pixelSize, worldSize, opacities, heights, (lifespan.Start, lifespan.End), connector)
        {
        }

        public override GenericDecorationCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log , Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs)
        {
            return new BackgroundIconDecorationCombatReplayDescription(log, this, map, usedSkills, usedBuffs);
        }
    }
}
