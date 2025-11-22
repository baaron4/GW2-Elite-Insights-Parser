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
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

internal static class AntiquaryHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        // Artifacts
        new EffectCastFinder(UnstableSkrittBombSkill, EffectGUIDs.AntiquaryUnstableSkrittBomb1),
        new EffectCastFinder(ChakShield, EffectGUIDs.AntiquaryChakShield1)
            .UsingSecondaryEffectCheckerSameSrc(EffectGUIDs.AntiquaryChakShield3)
            .UsingSecondaryEffectCheckerSameSrc(EffectGUIDs.AntiquaryChakShield4),
        // Double Edge
        new BuffGainCastFinder(CanachCoinToss, CanachCoinTossHead1),
        new BuffGainCastFinder(CanachCoinToss, CanachCoinTossTail1),
        new BuffGainCastFinder(CanachCoinTossBackfired, CanachCoinTossBackfiredHead1),
        new BuffGainCastFinder(CanachCoinTossBackfired, CanachCoinTossBackfiredTail1),
        new BuffGainCastFinder(EmergencyJadeShieldSkill, EmergencyJadeShieldBuff),
        new BuffGainCastFinder(EmergencyJadeShieldBackfiredSkill, EmergencyJadeShieldBackfiredBuff),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = 
    [
        // Exhilarating Ephemera
        new BuffOnActorDamageModifier(Mod_ExhilaratingEphemera, ExhilaratingEphemera, "Exhilarating Ephemera", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Antiquary, ByPresence, TraitImages.ExhilaratingEphemera, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_ExhilaratingEphemera, ExhilaratingEphemera, "Exhilarating Ephemera", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Antiquary, ByPresence, TraitImages.ExhilaratingEphemera, DamageModifierMode.sPvPWvW),
        // Combat High
        new BuffOnActorDamageModifier(Mod_CombatHigh, CombatHigh, "Combat High", "30%", DamageSource.NoPets, 30.0, DamageType.StrikeAndCondition, DamageType.All, Source.Antiquary, ByPresence, TraitImages.CombatHigh, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_CombatHigh, CombatHigh, "Combat High", "20%", DamageSource.NoPets, 20.0, DamageType.StrikeAndCondition, DamageType.All, Source.Antiquary, ByPresence, TraitImages.CombatHigh, DamageModifierMode.sPvPWvW),
        // Summon Kriptis Turret
        new BuffOnFoeDamageModifier(Mod_SummonKryptisTurret, SummonKryptisTurretTargetBuff, "Kryptis Turret", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Antiquary, ByPresence, SkillImages.SummonKryptisTurret, DamageModifierMode.All)
            .UsingActorCheckerByPresence(SummonKryptisTurretPlayerBuff)
            .WithBuffOnFoeFromActor()
            .UsingEarlyExit((a, log) => log.CombatData.GetBuffApplyDataByIDBySrc(SummonKryptisTurretTargetBuff , a.AgentItem).Count == 0),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];

    internal static readonly IReadOnlyList<Buff> Buffs = 
    [
        // Traits
        new Buff("Scoundrel's Luck", ScoundrelsLuckBuff, Source.Antiquary, BuffClassification.Other, BuffImages.ScoundrelsLuck),
        new Buff("Exhilarating Ephemera", ExhilaratingEphemera, Source.Antiquary, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.ExhilaratingEphemera),
        new Buff("Prodigious Pincher", ProdigiousPincher, Source.Antiquary, BuffStackType.Stacking, 15, BuffClassification.Other, BuffImages.ProdigiousPincher),
        new Buff("Combat High", CombatHigh, Source.Antiquary, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.CombatHigh),
        // Backfire
        new Buff("Unstable Skritt Bomb", UnstableSkrittBombBuff, Source.Antiquary, BuffClassification.Other, SkillImages.UnstableSkrittBomb),
        // Artifacts
        new Buff("Forged Surfer Dash", ForgedSurferDashBuff, Source.Antiquary, BuffClassification.Other, SkillImages.ForgedSurferDash),
        new Buff("Zephyrite Sun Crystal", ZephyriteSunCrystalBuff, Source.Antiquary, BuffStackType.Stacking, 2, BuffClassification.Other, SkillImages.ZephyriteSunCrystal),
        new Buff("Holo-Dancer Decoy", HoloDancerDecoyBuff, Source.Antiquary, BuffStackType.StackingConditionalLoss, 2, BuffClassification.Other, SkillImages.HoloDancerDecoy),
        new Buff("Metal Legion Guitar", MetalLegionGuitarBuff, Source.Antiquary, BuffStackType.StackingConditionalLoss, 4, BuffClassification.Other, SkillImages.MetalLegionGuitar),
        new Buff("Mistburn Mortar", MistburnMortarBuff, Source.Antiquary, BuffStackType.StackingConditionalLoss, 7, BuffClassification.Other, SkillImages.MistburnMortar),
        new Buff("Exalted Hammer", ExaltedHammerBuff, Source.Antiquary, BuffStackType.StackingConditionalLoss, 2, BuffClassification.Other, SkillImages.ExaltedHammer),
        new Buff("Summon Kryptis Turret (Player)", SummonKryptisTurretPlayerBuff, Source.Antiquary, BuffClassification.Other, SkillImages.SummonKryptisTurret),
        new Buff("Summon Kryptis Turret (Target)", SummonKryptisTurretTargetBuff, Source.Antiquary, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Other, SkillImages.SummonKryptisTurret),
        new Buff("Kryptis Turret (1)", KryptisTurretBuff1, Source.Antiquary, BuffClassification.Hidden, BuffImages.Unknown),
        new Buff("Kryptis Turret (2)", KryptisTurretBuff2, Source.Antiquary, BuffClassification.Hidden, BuffImages.Unknown),
        // Double Edge
        new Buff("Canach-Coin Toss (Head 1)", CanachCoinTossHead1, Source.Antiquary, BuffClassification.Other, BuffImages.CanachCoinTossHead),
        new Buff("Canach-Coin Toss (Tail 1)", CanachCoinTossTail1, Source.Antiquary, BuffClassification.Other, BuffImages.CanachCoinTossTail),
        new Buff("Canach-Coin Toss (Head 2)", CanachCoinTossHead2, Source.Antiquary, BuffClassification.Other, BuffImages.CanachCoinTossHead),
        new Buff("Canach-Coin Toss (Tail 2)", CanachCoinTossTail2, Source.Antiquary, BuffClassification.Other, BuffImages.CanachCoinTossTail),
        new Buff("Canach-Coin Toss (Head 3)", CanachCoinTossHead3, Source.Antiquary, BuffClassification.Other, BuffImages.CanachCoinTossHead),
        new Buff("Canach-Coin Toss (Tail 3)", CanachCoinTossTail3, Source.Antiquary, BuffClassification.Other, BuffImages.CanachCoinTossTail),
        new Buff("Canach-Coin Toss (Backfired) (Head 1)", CanachCoinTossBackfiredHead1, Source.Antiquary, BuffClassification.Other, BuffImages.CanachCoinTossBackfiredHead),
        new Buff("Canach-Coin Toss (Backfired) (Tail 1)", CanachCoinTossBackfiredTail1, Source.Antiquary, BuffClassification.Other, BuffImages.CanachCoinTossBackfiredTail),
        new Buff("Canach-Coin Toss (Backfired) (Head 2)", CanachCoinTossBackfiredHead2, Source.Antiquary, BuffClassification.Other, BuffImages.CanachCoinTossBackfiredHead),
        new Buff("Canach-Coin Toss (Backfired) (Tail 2)", CanachCoinTossBackfiredTail2, Source.Antiquary, BuffClassification.Other, BuffImages.CanachCoinTossBackfiredTail),
        new Buff("Canach-Coin Toss (Backfired) (Head 3)", CanachCoinTossBackfiredHead3, Source.Antiquary, BuffClassification.Other, BuffImages.CanachCoinTossBackfiredHead),
        new Buff("Canach-Coin Toss (Backfired) (Tail 3)", CanachCoinTossBackfiredTail3, Source.Antiquary, BuffClassification.Other, BuffImages.CanachCoinTossBackfiredTail),
        new Buff("Emergency Jade Shield", EmergencyJadeShieldBuff, Source.Antiquary, BuffClassification.Other, SkillImages.EmergencyJadeShield),
        new Buff("Emergency Jade Shield (Backfired)", EmergencyJadeShieldBackfiredBuff, Source.Antiquary, BuffClassification.Other, SkillImages.EmergencyJadeShield),
        
    ];

    private static readonly HashSet<int> Minions = 
    [
        (int)MinionID.KryptisTurret,
        (int)MinionID.HoloDancer,
        (int)MinionID.SkrittThievesGuild,
    ];

    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

    public static void ProcessGadgets(IReadOnlyList<AgentItem> players, CombatData combatData, AgentData agentData)
    {
        var kryptisTurrets = combatData.GetBuffApplyData(KryptisTurretBuff1).ToList().Concat(combatData.GetBuffApplyData(KryptisTurretBuff2)).Select(x => x.To).Distinct();
        foreach (var kryptisTurret in kryptisTurrets)
        {
            if (kryptisTurret.Type == AgentItem.AgentType.Gadget)
            {
                kryptisTurret.OverrideType(AgentItem.AgentType.NPC, agentData);
                kryptisTurret.OverrideID(MinionID.KryptisTurret, agentData);
                kryptisTurret.OverrideName("Kryptis Turret");
            }
        }
        foreach (var maxHP in combatData.GetMaxHealthUpdateEventsByMaxHP(7470))
        {
            var gadget = maxHP.Src;
            if (gadget.Type == AgentItem.AgentType.Gadget)
            {
                var master = gadget.GetFinalMaster();
                if (master.IsPlayer && master.Spec == Spec.Antiquary)
                {
                    if (gadget.HitboxWidth == 118 && gadget.HitboxHeight == 0)
                    {
                        // The Holo-Dancer is correctly named
                        gadget.OverrideType(AgentItem.AgentType.NPC, agentData);
                        gadget.OverrideID(MinionID.HoloDancer, agentData);
                    }
                }
            }
        }
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Thief;

        // Skritt Scuffle
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.AntiquarySkrittScuffle, out var skrittScuffle))
        {
            var skill = new SkillModeDescriptor(player, Spec.Antiquary, SkrittScuffle);
            foreach (EffectEvent effect in skrittScuffle)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 15000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, EffectImages.EffectSkrittScuffle);
            }
        }

        // Chak Shield
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.AntiquaryChakShield1, out var chakShield))
        {
            var skill = new SkillModeDescriptor(player, Spec.Antiquary, ChakShield, SkillModeCategory.ProjectileManagement);
            foreach (EffectEvent effect in chakShield)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 4000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, EffectImages.EffectChakShield);
            }
        }
    }
}
