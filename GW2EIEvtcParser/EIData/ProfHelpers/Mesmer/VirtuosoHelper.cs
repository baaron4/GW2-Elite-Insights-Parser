using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class VirtuosoHelper
{

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new EffectCastFinder(BladesongDistortion, EffectGUIDs.MesmerDistortionOrMindWrack)
            .UsingChecker((evt, combatData, agentData, skillData) => {
                if(evt.Src.Spec != Spec.Virtuoso) {
                    return false;
                }
                if (!combatData.GetBuffDataByIDByDst(DistortionBuff, evt.Src).Any(buffEvt => buffEvt is BuffApplyEvent && Math.Abs(buffEvt.Time - evt.Time) < ServerDelayConstant))
                {
                    return false;
                }
                if (combatData.GetAnimatedCastData(BladeRenewal).Any(castEvt => castEvt.Caster.Is(evt.Src) && evt.Time <= castEvt.EndTime && evt.Time >= castEvt.Time)) {
                    return false;
                }
                return true;
            })
            .WithBuilds(GW2Builds.October2022Balance),
        new EffectCastFinder(BladeturnRequiem, EffectGUIDs.VirtuosoBladeturnRequiem)
            .UsingSrcSpecChecker(Spec.Virtuoso)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new EffectCastFinder(ThousandCuts, EffectGUIDs.VirtuosoThousandCuts)
            .UsingSrcSpecChecker(Spec.Virtuoso),
    ];


    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Mental Focus
        new DamageLogDamageModifier(Mod_MentalFocus, "Mental Focus", "10% to foes within 600 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Virtuoso, TraitImages.MentalFocus, (x, log) => TargetWithinRangeChecker(x, log, 600), DamageModifierMode.PvE)
            .UsingApproximate()
            .WithBuilds(GW2Builds.EODBeta4),
        // Deadly Blades
        new BuffOnActorDamageModifier(Mod_DeadlyBlades, DeadlyBlades, "Deadly Blades", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Virtuoso, ByPresence, TraitImages.DeadlyBlades, DamageModifierMode.All)
            .WithBuilds(GW2Builds.EODBeta4),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Deadly Blades", DeadlyBlades, Source.Virtuoso, BuffClassification.Other, TraitImages.DeadlyBlades),
        new Buff("Bladeturn", Bladeturn, Source.Virtuoso, BuffClassification.Other, SkillImages.BladeturnRequiem),
        new Buff("Virtuoso Blade", VirtuosoBlades, Source.Virtuoso, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, BuffImages.PowerAttribute),
        new Buff("Psychic Riposte", PsychicRiposteBuff, Source.Virtuoso, BuffClassification.Other, TraitImages.PsychicRiposte),
    ];

    public static List<BuffEvent> TransformVirtuosoBladeStorage(IReadOnlyList<BuffEvent> buffs, AgentItem a, SkillData skillData, EvtcVersionEvent evtcVersion)
    {
        var res = new List<BuffEvent>();
        var bladeIDs = new HashSet<long>
        {
            VirtuosoBlade1,
            VirtuosoBlade2,
            VirtuosoBlade3,
            VirtuosoBlade4,
            VirtuosoBlade5,
        };
        var blades = buffs.Where(x => bladeIDs.Contains(x.BuffID));
        SkillItem skill = skillData.Get(VirtuosoBlades);
        var lastAddedBuffInstance = new Dictionary<long, BuffApplyEvent>();
        foreach (BuffEvent blade in blades)
        {
            if (blade is BuffApplyEvent bae)
            {
                res.Add(new BuffApplyEvent(bae.By, a, bae.Time, bae.AppliedDuration, skill, bae.IFF, bae.BuffInstance, true));
                lastAddedBuffInstance[blade.BuffID] = bae;
            }
            else if (blade is BuffRemoveAllEvent brae)
            {
                uint removedInstance = 0;
                long elapsedTime = 0;
                if (lastAddedBuffInstance.TryGetValue(blade.BuffID, out var apply))
                {
                    removedInstance = apply.BuffInstance;
                    elapsedTime = brae.Time - apply.Time;
                }
                int removedDuration = brae.RemovedDuration;
                if (evtcVersion.Build >= ArcDPSBuilds.RemovedDurationForInfiniteDurationStacksChanged)
                {
                    removedDuration -= (int)elapsedTime;
                }
                res.Add(new BuffRemoveSingleEvent(brae.By, a, brae.Time, removedDuration, skill, brae.IFF, removedInstance));
            }
            else if (blade is BuffRemoveSingleEvent brse)
            {
                res.Add(new BuffRemoveSingleEvent(brse.By, a, brse.Time, brse.RemovedDuration, skill, brse.IFF, brse.BuffInstance));
            }
        }
        return res;
    }

    private static readonly HashSet<int> Minions = [];
    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Mesmer;

        // Rain of Swords
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.VirtuosoRainOfSwords, out var rainOfSwords))
        {
            var skill = new SkillModeDescriptor(player, Spec.Virtuoso, RainOfSwords);
            foreach (EffectEvent effect in rainOfSwords)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 6000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 280, EffectImages.EffectRainOfSwords);
            }
        }
        // Thousand Cuts
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.VirtuosoThousandCuts, out var thousandCuts))
        {
            var skill = new SkillModeDescriptor(player, Spec.Virtuoso, ThousandCuts);
            foreach (EffectEvent effect in thousandCuts)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                var connector = (PositionConnector)new PositionConnector(effect.Position).WithOffset(new(0f, 600.0f, 0), true);
                var rotationConnector = new AngleConnector(effect.Rotation.Z);
                // 30 units width is a guess
                replay.Decorations.Add(new RectangleDecoration(30, 1200, lifespan, color, 0.5, connector)
                    .UsingRotationConnector(rotationConnector)
                    .UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectThousandCuts, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector)
                    .UsingRotationConnector(rotationConnector)
                    .UsingSkillMode(skill));
            }
        }
    }
}
