using System.Collections.Generic;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ScourgeHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(TrailOfAnguish, TrailOfAnguishBuff),
            // Trail of Anguish? Unique effect?
            // new EffectCastFinder(TrailOfAnguish, EffectGUIDs.ScourgeTrailOfAnguish).UsingSrcSpecChecker(Spec.Scourge).UsingICD(6100),
            new DamageCastFinder(NefariousFavorSkill, NefariousFavorShadeHit),
            new DamageCastFinder(GarishPillarSkill, GarishPillarHit),
            new BuffGainCastFinder(DesertShroud, DesertShroudBuff).UsingDurationChecker(6000),
            new BuffGainCastFinder(SandstormShroudSkill, DesertShroudBuff).UsingDurationChecker(3500),
            // new EXTBarrierCastFinder(DesertShroud, DesertShroud),
            new EXTBarrierCastFinder(SandCascadeSkill, SandCascadeBarrier),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Sadistic Searing", SadisticSearing, Source.Scourge, BuffClassification.Other, BuffImages.SadisticSearing),
            new Buff("Path Uses", PathUses, Source.Scourge, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.SandSwell),
        };
    }
}
