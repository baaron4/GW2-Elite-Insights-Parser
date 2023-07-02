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
            new BuffGainCastFinder(TrailOfAnguish, TrailOfAnguishEffect),
            // Trail of Anguish? Unique effect?
            // new EffectCastFinder(TrailOfAnguish, EffectGUIDs.ScourgeTrailOfAnguish).UsingSrcSpecChecker(Spec.Scourge).UsingICD(6100),
            new DamageCastFinder(NefariousFavorSkill, NefariousFavorShadeHit),
            new DamageCastFinder(GarishPillarSkill, GarishPillarHit),
            new BuffGainCastFinder(DesertShroud, DesertShroudEffect).UsingDurationChecker(6000),
            new BuffGainCastFinder(SandstormShroudSkill, DesertShroudEffect).UsingDurationChecker(3500),
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

         internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            const string lineColor = "rgba(30, 193, 110, 0.5)";

            foreach (List<EffectEvent> group in log.CombatData.GetGroupedEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ScourgeSandSwellPortal))
            {
                IconDecoration first = null;
                for (int i = 0; i < group.Count; i++)
                {
                    EffectEvent effect = group[i];
                    int start = (int)effect.Time;
                    var remove = log.CombatData.GetBuffData(PathUses).OfType<BuffRemoveAllEvent>().FirstOrDefault(x => x.Time >= start);
                    int end = (int?)remove?.Time ?? start + 8000;
                    var decoration = new IconDecoration(ParserIcons.PortalSandswell, 128, 0.7f, effect.Src, (start, end), new PositionConnector(effect.Position));
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
