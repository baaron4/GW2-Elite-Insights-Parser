using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
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
            new BuffGainCastFinder(SadisticSearing, SadisticSearing).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new BuffLossCastFinder(SadisticSearingActivation, SadisticSearing).UsingChecker((blcf, combatData, agentData, skillData) => 
            {
                long sadisticSearingDuration = 10000 - blcf.RemovedDuration;
                if (combatData.GetDamageData(ManifestSandShadeShadeHit).Any(x => x.CreditedFrom == blcf.To && x.Time >= blcf.Time - sadisticSearingDuration && x.Time <= blcf.Time)) 
                {
                    return true;
                }
                return false;
            }).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Sadistic Searing", SadisticSearing, Source.Scourge, BuffClassification.Other, BuffImages.SadisticSearing),
            new Buff("Path Uses", PathUses, Source.Scourge, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.SandSwell),
        };

         internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            const string lineColor = "rgba(30, 193, 110, 0.5)";

            foreach (List<EffectEvent> group in log.CombatData.GetGroupedEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ScourgeSandSwellPortal))
            {
                IconDecoration first = null;
                for (int i = 0; i < group.Count; i++)
                {
                    EffectEvent effect = group[i];
                    (int, int) lifespan = ProfHelper.ComputeEffectLifespan(log, effect, 8000, player.AgentItem, PathUses);
                    var decoration = new IconDecoration(ParserIcons.PortalSandswell, 128, 0.7f, player, lifespan, new PositionConnector(effect.Position));
                    replay.Decorations.Add(decoration);
                    if (i == 0)
                    {
                        first = decoration;
                    }
                    else
                    {
                        replay.Decorations.Add(first.LineTo(decoration, 0, lineColor));
                    }
                }
            }
        }
    }
}
