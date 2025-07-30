using GW2EIEvtcParser.EncounterLogic;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class ScourgeHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(TrailOfAnguish, TrailOfAnguishBuff),
        new DamageCastFinder(NefariousFavorSkill, NefariousFavorShadeHit),
        new DamageCastFinder(GarishPillarSkill, GarishPillarHit),
        new BuffGainCastFinder(DesertShroud, DesertShroudBuff)
            .UsingDurationChecker(6000),
        new BuffGainCastFinder(SandstormShroudSkill, DesertShroudBuff)
            .UsingDurationChecker(3500),
        new EXTBarrierCastFinder(SandCascadeSkill, SandCascadeBarrier),
        new BuffGainCastFinder(SadisticSearing, SadisticSearing)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new BuffLossCastFinder(SadisticSearingActivation, SadisticSearing)
            .UsingChecker((blcf, combatData, agentData, skillData) =>
            {
                long sadisticSearingDuration = 10000 - blcf.RemovedDuration;
                if (combatData.GetDamageData(ManifestSandShadeShadeHit).Any(x => x.CreditedFrom.Is(blcf.To) && x.Time >= blcf.Time - sadisticSearingDuration && x.Time <= blcf.Time))
                {
                    return true;
                }
                return false;
            })
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = [];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = 
    [
        // Blood as Sand
        new BuffOnActorDamageModifier(Mod_BloodAsSand, [SandShadeBuff, SandSavantSandShadeBuff], "Blood As Sand", "-15% while shade is active", DamageSource.Incoming, -15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Necromancer, ByPresence, TraitImages.BloodAsSand, DamageModifierMode.All)
            .UsingApproximate()
            .WithBuilds(GW2Builds.July2023BalanceAndSilentSurfCM),
    ];

    public static List<BuffEvent> AddShadeBuffsFromEffects(IReadOnlyList<EffectEvent> shadeEffects, FightData fightData, SkillData skillData, GW2BuildEvent gw2Build, EvtcVersionEvent evtcVersion)
    {
        var res = new List<BuffEvent>();
        uint buffInstance = 0;
        foreach (EffectEvent shade in shadeEffects)
        {
            if (shade is EffectEventCBTS45)
            {
                // unreliable for early ends
                continue;
            }
            SkillItem skill = shade.GUIDEvent.ContentGUID == EffectGUIDs.ScourgeShadeSandSavant ? skillData.Get(SandSavantSandShadeBuff) : skillData.Get(SandShadeBuff);
            int expectedDuration;
            if (fightData.Logic.SkillMode == FightLogic.SkillModeEnum.WvW || fightData.Logic.SkillMode == FightLogic.SkillModeEnum.sPvP)
            {
                expectedDuration = gw2Build.Build >= GW2Builds.October2019Balance ? 15000 : 10000;
            }
            else
            {
                expectedDuration = gw2Build.Build >= GW2Builds.July2023BalanceAndSilentSurfCM ? 8000 : 20000;
            }
            int duration = shade.HasDynamicEndTime ? Math.Min((int)(shade.DynamicEndTime - shade.Time), expectedDuration) : expectedDuration;
            res.Add(new BuffApplyEvent(shade.Src, shade.Src, shade.Time, duration, skill, IFF.Friend, buffInstance, true));
            res.Add(new BuffRemoveSingleEvent(shade.Src, shade.Src, shade.Time + duration, Math.Max(expectedDuration - duration, 0), skill, IFF.Friend, buffInstance++));
        }
        return res;
    }

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Sadistic Searing", SadisticSearing, Source.Scourge, BuffClassification.Other, TraitImages.SadisticSearing),
        new Buff("Path Uses", PathUses, Source.Scourge, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.SandSwell),
        new Buff("Trail of Anguish", TrailOfAnguishBuff, Source.Scourge, BuffClassification.Other, SkillImages.TrailofAnguish),
        new Buff("Desert / Sandstorm Shroud", DesertShroudBuff, Source.Scourge, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.DesertSandstormShroud),
        new Buff("Sand Shade", SandShadeBuff, Source.Scourge, BuffStackType.Stacking, 3, BuffClassification.Other, SkillImages.ManifestSandShade),
        new Buff("Sand Shade (Sand Savant)", SandSavantSandShadeBuff, Source.Scourge, BuffClassification.Other, SkillImages.ManifestSandShade),
    ];

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Necromancer;

        // Sand Swell portal locations
        if (log.CombatData.TryGetGroupedEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.ScourgeSandSwellPortal, out var sandswellPortals))
        {
            var skill = new SkillModeDescriptor(player, Spec.Scourge, SandSwell, SkillModeCategory.Portal);
            foreach (var group in sandswellPortals)
            {
                AttachedDecoration? first = null;
                foreach (EffectEvent effect in group)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 8000, player.AgentItem, PathUses);
                    var connector = new PositionConnector(effect.Position);
                    replay.Decorations.Add(new CircleDecoration(90, lifespan, color, 0.5, connector).UsingSkillMode(skill));
                    AttachedDecoration icon = new IconDecoration(EffectImages.PortalSandswell, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.7f, lifespan, connector).UsingSkillMode(skill);
                    if (first == null)
                    {
                        first = icon;
                    }
                    else
                    {
                        replay.Decorations.Add(first.LineTo(icon, color, 0.5).UsingSkillMode(skill));
                    }
                    replay.Decorations.Add(icon);
                }
            }

        }
        // Shade
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, [EffectGUIDs.ScourgeShade, EffectGUIDs.ScourgeShadeSandSavant], out var scourgeShades))
        {
            var skill = new SkillModeDescriptor(player, Spec.Scourge, ManifestSandShadeSkill);
            foreach (EffectEvent effect in scourgeShades)
            {
                uint radius = (uint)(effect.GUIDEvent.ContentGUID == EffectGUIDs.ScourgeShadeSandSavant ? 300 : 180);
                long duration;
                if (log.FightData.Logic.SkillMode == FightLogic.SkillModeEnum.WvW || log.FightData.Logic.SkillMode == FightLogic.SkillModeEnum.sPvP)
                {
                    duration = log.LogData.GW2Build >= GW2Builds.October2019Balance ? 15000 : 10000;
                }
                else
                {
                    duration = log.LogData.GW2Build >= GW2Builds.July2023BalanceAndSilentSurfCM ? 8000 : 20000;
                }
                (long, long) lifespan = effect.ComputeLifespan(log, duration);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, radius, EffectImages.EffectShade);
            }
        }
    }
}
