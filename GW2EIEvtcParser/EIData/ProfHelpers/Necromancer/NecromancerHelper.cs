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
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

internal static class NecromancerHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(EnterDeathShroud, DeathShroud)
            .UsingBeforeWeaponSwap(true),
        new BuffLossCastFinder(ExitDeathShroud, DeathShroud)
            .UsingBeforeWeaponSwap(true),
        new DamageCastFinder(LesserEnfeeble, LesserEnfeeble)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new DamageCastFinder(LesserSpinalShivers, LesserSpinalShivers)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),

        // distinguish unholy burst & spiteful spirit using hit, unholy burst will only ever trigger if a target is hit
        new DamageCastFinder(UnholyBurst, UnholyBurst),
        new DamageCastFinder(SpitefulSpirit, SpitefulSpirit)
            .UsingDisableWithEffectData()
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new EffectCastFinder(SpitefulSpirit, EffectGUIDs.NecromancerUnholyBurst)
            .UsingSrcBaseSpecChecker(Spec.Necromancer)
            .UsingChecker((evt, combatData, skillData, agentData) => !CombatData.FindRelatedEvents(combatData.GetBuffData(DesertShroudBuff).OfType<BuffRemoveAllEvent>(), evt.Time, 50).Any()) // collides with sandstorm shroud
            .UsingChecker((evt, combatData, skillData, agentData) => !combatData.HasRelatedHit(UnholyBurst, evt.Src, evt.Time))
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),

        new BuffGainCastFinder(SpectralArmorSkill, SpectralArmorBuff)
            .WithBuilds(GW2Builds.December2018Balance),
        new BuffGainCastFinder(SpectralWalkSkill, SpectralWalkOldBuff)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.December2018Balance),
        new BuffGainCastFinder(SpectralWalkSkill, SpectralWalkBuff)
            .WithBuilds(GW2Builds.December2018Balance),
        new BuffLossCastFinder(SpectralRecallSkill, SpectralWalkTeleportBuff)
            .UsingChecker((evt, combatData, skillData, agentData) => !CombatData.FindRelatedEvents(combatData.GetBuffData(SpectralWalkBuff).OfType<BuffRemoveAllEvent>(), evt.Time + 120).Any())
            .WithBuilds(GW2Builds.December2018Balance),
        new EffectCastFinderByDst(PlagueSignetSkill, EffectGUIDs.NecromancerPlagueSignet)
            .UsingDstBaseSpecChecker(Spec.Necromancer),
        new EXTHealingCastFinder(SpitefulRenewal, SpitefulRenewal)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        // Minions
        new MinionCommandCastFinder(RigorMortisSkill, (int) MinionID.BoneFiend),
        new MinionCommandCastFinder(HauntSkill, (int) MinionID.ShadowFiend),
        new MinionCommandCastFinder(NecroticTraversal, (int) MinionID.FleshWurm),
        // Spear
        new EffectCastFinder(DistressSkill, EffectGUIDs.NecromancerSpearDistress)
            .UsingChecker((effectEvent, combatData, agentData, skillData) => CombatData.FindRelatedEvents(combatData.GetBuffRemoveAllData(DistressBuff).OfType<BuffRemoveAllEvent>(), effectEvent.Time).Any()),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Spite
        // - Spiteful Talisman
        new BuffOnFoeDamageModifier(Mod_SpitefulTalisman, NumberOfBoons, "Spiteful Talisman", "10% on boonless target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByAbsence, TraitImages.SpitefulTalisman, DamageModifierMode.All),
        // - Death's Embrace
        new BuffOnActorDamageModifier(Mod_DeathsEmbrace, Downed, "Death's Embrace", "25% on while downed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, TraitImages.DeathsEmbrace, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
        new BuffOnActorDamageModifier(Mod_DeathsEmbrace, Downed, "Death's Embrace", "25% on while downed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, TraitImages.DeathsEmbrace, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.February2020Balance),
        new BuffOnActorDamageModifier(Mod_DeathsEmbrace, Downed, "Death's Embrace", "5% on while downed", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, TraitImages.DeathsEmbrace, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.February2020Balance),
        // - Dread
        new BuffOnFoeDamageModifier(Mod_Dread, Fear, "Dread", "20% on feared target", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, TraitImages.UnholyFervor, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2018Balance),
        new BuffOnFoeDamageModifier(Mod_Dread, Fear, "Dread", "33% on feared target", DamageSource.NoPets, 33.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, TraitImages.UnholyFervor, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2018Balance, GW2Builds.February2020Balance),
        new BuffOnFoeDamageModifier(Mod_Dread, Fear, "Dread", "33% on feared target", DamageSource.NoPets, 33.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, TraitImages.UnholyFervor, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.July2020Balance),
        new BuffOnFoeDamageModifier(Mod_Dread, Fear, "Dread", "15% on feared target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, TraitImages.UnholyFervor, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.July2020Balance),
        // - Close to Death
        new DamageLogDamageModifier(Mod_CloseToDeath, "Close to Death", "20% below 50% HP", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Necromancer, TraitImages.CloseToDeath, (x, log) => x.AgainstUnderFifty, DamageModifierMode.All),
        
        // Soul Reaping
        // - Soul Barbs
        new BuffOnActorDamageModifier(Mod_SoulBarbs, SoulBarbs, "Soul Barbs", "10% after entering or exiting shroud", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, TraitImages.SoulBarbs, DamageModifierMode.All)
            .WithBuilds(GW2Builds.December2018Balance, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_SoulBarbs, SoulBarbs, "Soul Barbs", "10% after entering or exiting shroud", DamageSource.NoPets, 10.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Necromancer, ByPresence, TraitImages.SoulBarbs, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2021Balance),
        // - Death Perception
        new BuffOnActorDamageModifier(Mod_DeathPerception, [DeathShroud, ReapersShroud, DesertShroudBuff, HarbingerShroud], "Death Perception", "15% crit damage while in shroud", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, TraitImages.DeathPerception, DamageModifierMode.All)
            .UsingChecker((x, log) => x.HasCrit)
            .WithBuilds(GW2Builds.June2022Balance),

        // Death Magic
        // - Necromantic Corruption
        new DamageLogDamageModifier(Mod_NecromanticCorruption, "Necromantic Corruption", "25% strike damage for minions", DamageSource.PetsOnly, 25.0, DamageType.Strike, DamageType.All, Source.Necromancer, TraitImages.NecromanticCorruption, (x, log) => IsAnyUndeadMinion(x.From), DamageModifierMode.All)
            .UsingEarlyExit((a, log) => a.GetMinions(log).Any(x => IsAnyUndeadMinion(x.Value.ReferenceAgentItem))),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        // Death Shroud
        new BuffOnActorDamageModifier(Mod_DeathShroud, DeathShroud, "Death Shroud", "-33%", DamageSource.Incoming, -33, DamageType.StrikeAndCondition, DamageType.All, Source.Necromancer, ByPresence, SkillImages.DeathShroud, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_DeathShroud, DeathShroud, "Death Shroud", "-50%", DamageSource.Incoming, -50, DamageType.StrikeAndCondition, DamageType.All, Source.Necromancer, ByPresence, SkillImages.DeathShroud, DamageModifierMode.sPvPWvW),
        
        // Death Magic
        // - Dark Defiance
        new BuffOnActorDamageModifier(Mod_DarkDefiance, Protection, "Dark Defiance", "-20%", DamageSource.Incoming, -20, DamageType.Condition, DamageType.All, Source.Necromancer, ByPresence, TraitImages.DarkDefiance, DamageModifierMode.All),
        // - Beyond the Veil
        new BuffOnActorDamageModifier(Mod_BeyondTheVeil, DeathsCarapace, "Beyond the Veil", "-10%", DamageSource.Incoming, -10, DamageType.Condition, DamageType.All, Source.Necromancer, ByPresence, TraitImages.BeyondTheVeil, DamageModifierMode.PvE)
            .UsingChecker((dl, log) => dl.To.GetBuffStatus(log, DeathsCarapace, dl.Time).Value >= 10)
            .WithBuilds(GW2Builds.October2019Balance),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        // Forms
        new Buff("Lich Form", LichForm, Source.Necromancer, BuffClassification.Other, SkillImages.LichForm),
        new Buff("Death Shroud", DeathShroud, Source.Necromancer, BuffClassification.Other, SkillImages.DeathShroud),
        // Signets
        new Buff("Signet of Vampirism", SignetOfVampirism, Source.Necromancer, BuffClassification.Other, SkillImages.SignetOfVampirism),
        new Buff("Vampiric Mark", VampiricMark, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.SignetOfVampirism),
        new Buff("Signet of Vampirism (Shroud)", SignetOfVampirismShroud, Source.Necromancer, BuffClassification.Other, SkillImages.SignetOfVampirism),
        new Buff("Plague Signet", PlagueSignetBuff, Source.Necromancer, BuffClassification.Other, SkillImages.PlagueSignet),
        new Buff("Plague Signet (Shroud)", PlagueSignetShroud, Source.Necromancer, BuffClassification.Other, SkillImages.PlagueSignet),
        new Buff("Signet of Spite", SignetOfSpite, Source.Necromancer, BuffClassification.Other, SkillImages.SignetOfSpite),
        new Buff("Signet of Spite (Shroud)", SignetOfSpiteShroud, Source.Necromancer, BuffClassification.Other,SkillImages.SignetOfSpite),
        new Buff("Signet of the Locust", SignetOfTheLocust, Source.Necromancer, BuffClassification.Other, SkillImages.SignetOfTheLocust),
        new Buff("Signet of the Locust (Shroud)", SignetOfTheLocustShroud, Source.Necromancer, BuffClassification.Other, SkillImages.SignetOfTheLocust),
        new Buff("Signet of Undeath", SignetOfUndeathBuff, Source.Necromancer, BuffClassification.Other, SkillImages.SignetOfUndeath),
        new Buff("Signet of Undeath (Shroud)", SignetOfUndeathShroud, Source.Necromancer, BuffClassification.Other, SkillImages.SignetOfUndeath),
        // Skills
        new Buff("Spectral Walk", SpectralWalkOldBuff, Source.Necromancer, BuffClassification.Other, SkillImages.NecroticTraversal)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2018Balance),
        new Buff("Spectral Walk", SpectralWalkOldBuff, Source.Necromancer, BuffClassification.Other, SkillImages.SpectralWalk)
            .WithBuilds(GW2Builds.July2018Balance, GW2Builds.December2018Balance),
        new Buff("Spectral Walk", SpectralWalkBuff, Source.Necromancer, BuffClassification.Other, SkillImages.SpectralWalk)
            .WithBuilds(GW2Builds.December2018Balance),
        new Buff("Spectral Walk (Teleport)", SpectralWalkTeleportBuff, Source.Necromancer, BuffClassification.Other, SkillImages.SpectralWalk)
            .WithBuilds(GW2Builds.December2018Balance),
        new Buff("Spectral Armor", SpectralArmorBuff, Source.Necromancer, BuffClassification.Other, SkillImages.SpectralArmor),
        new Buff("Locust Swarm", LocustSwarm, Source.Necromancer, BuffClassification.Other, SkillImages.LocustSwarm),
        new Buff("Grim Specter", GrimSpecterBuff, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.GrimSpecter),
        new Buff("Grim Specter (Target)", GrimSpecterTargetBuff, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.GrimSpecter),
        // Traits
        new Buff("Plague Sending", PlagueSending, Source.Necromancer, BuffClassification.Other, TraitImages.PlagueSending),
        new Buff("Corrupter's Defense", CorruptersDefense, Source.Necromancer, BuffStackType.Stacking, 10, BuffClassification.Other, TraitImages.CorruptersFervor)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2019Balance),
        new Buff("Death's Carapace", DeathsCarapace, Source.Necromancer, BuffStackType.Stacking, 30, BuffClassification.Other, TraitImages.DeathsCarapace)
            .WithBuilds(GW2Builds.October2019Balance),
        new Buff("Flesh of the Master", FleshOfTheMaster, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, TraitImages.FleshOfTheMaster)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2019Balance),
        new Buff("Vampiric Aura", VampiricAura, Source.Necromancer, BuffClassification.Defensive, TraitImages.VampiricPresence),
        new Buff("Vampiric Strikes", VampiricStrikes, Source.Necromancer, BuffClassification.Other, TraitImages.VampiricPresence),
        new Buff("Last Rites", LastRites, Source.Necromancer, BuffClassification.Defensive, TraitImages.LastRites),
        new Buff("Soul Barbs", SoulBarbs, Source.Necromancer, BuffClassification.Other, TraitImages.SoulBarbs),
        new Buff("Taste For Blood", TasteForBlood, Source.Necromancer, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Support, TraitImages.OverflowingThirst),
        // Spear
        new Buff("Extirpation", Extirpation, Source.Necromancer, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, SkillImages.Extirpate),
        new Buff("Soul Shards", SoulShards, Source.Necromancer, BuffStackType.StackingConditionalLoss, 6, BuffClassification.Other, BuffImages.SoulShards),
        new Buff("Distress", DistressBuff, Source.Necromancer, BuffClassification.Other, SkillImages.Distress),
        new Buff("Dark Stalker", DarkStalker, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.MonsterSkill),
    ];

    private static readonly HashSet<long> _shroudTransform =
    [
        EnterDeathShroud, ExitDeathShroud,
    ];

    public static bool IsDeathShroudTransform(long id)
    {
        return _shroudTransform.Contains(id);
    }

    public static bool IsShroudTransform(long id)
    {
        return IsDeathShroudTransform(id)
            || ReaperHelper.IsReaperShroudTransform(id)
            || HarbingerHelper.IsHarbingerShroudTransform(id);
    }

    private static readonly HashSet<int> Minions =
    [
        (int)MinionID.BloodFiend,
        (int)MinionID.FleshGolem,
        (int)MinionID.ShadowFiend,
        (int)MinionID.FleshWurm,
        (int)MinionID.BoneFiend,
        (int)MinionID.BoneMinion,
        (int)MinionID.UnstableHorror,
    ];
    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

    /// <summary>
    /// Checks if a minion is a Necromancer, Reaper or Rune/Relic of the Lich minion.
    /// </summary>
    internal static bool IsAnyUndeadMinion(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return IsKnownMinionID(agentItem.ID) || ReaperHelper.IsKnownMinionID(agentItem.ID) || agentItem.IsSpecies(MinionID.JaggedHorror);
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Necromancer;

        // Well of Blood
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerWellOfBlood, out var wellOfBloods))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, WellOfBlood, SkillModeCategory.Heal);
            foreach (EffectEvent effect in wellOfBloods)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfBlood);
            }
        }
        // Well of Suffering
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerWellOfSuffering, out var wellOfSufferings))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, WellOfSuffering);
            foreach (EffectEvent effect in wellOfSufferings)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 6000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfSuffering);
            }
        }
        // Well of Darkness
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerWellOfDarkness, out var wellOfDarknesses))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, WellOfDarkness);
            foreach (EffectEvent effect in wellOfDarknesses)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfDarkness);
            }
        }
        // Well of Corruption
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerWellOfCorruption, out var wellOfCorruptions))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, WellOfCorruption);
            foreach (EffectEvent effect in wellOfCorruptions)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectWellOfCorruption);
            }
        }
        // Corrosive Poison Cloud
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerCorrosivePoisonCloud, out var poisonClouds))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, CorrosivePoisonCloud, SkillModeCategory.ProjectileManagement);
            foreach (EffectEvent effect in poisonClouds)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 8000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectCorrosivePoisonCloud);
            }
        }
        // Plaguelands
        if (log.CombatData.TryGetEffectEventsBySrcWithGUIDs(player.AgentItem, [
                        EffectGUIDs.NecromancerPlaguelandsPulse1,
                        EffectGUIDs.NecromancerPlaguelandsPulse2,EffectGUIDs.NecromancerPlaguelandsPulse3
                        ], out var plaguelandPulses))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, Plaguelands);
            foreach (EffectEvent effect in plaguelandPulses)
            {
                int start = (int)effect.Time - 500;
                int end = (int)effect.Time;
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, (start, end), color, 0.5, connector).UsingGrowingEnd(end).UsingSkillMode(skill));
            }
        }
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerPlaguelands, out var plaguelands))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, Plaguelands);
            foreach (EffectEvent effect in plaguelands)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 10000);
                var connector = new PositionConnector(effect.Position);
                replay.Decorations.Add(new CircleDecoration(240, lifespan, color, 0.5, connector).UsingFilled(false).UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectPlaguelands, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
            }
        }

        // Mark of Blood or Chillblains (Staff 2/3)
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerMarkOfBloodOrChillblains, out var markOfBloodOrChillblains))
        {
            var markCasts = player.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.SkillId == MarkOfBlood || x.SkillId == Chillblains || x.Skill.IsDodge(log.SkillData));
            foreach (EffectEvent effect in markOfBloodOrChillblains)
            {
                SkillModeDescriptor skill;
                string icon;
                bool fromDodge = false;
                var markCastsOnEffect = markCasts.Where(x => effect.Time - ServerDelayConstant > x.Time && x.EndTime > effect.Time + ServerDelayConstant);
                if (markCastsOnEffect.Count() == 1)
                {
                    skill = new SkillModeDescriptor(player, Spec.Necromancer, markCastsOnEffect.First().SkillId);
                    if (skill.SkillID != MarkOfBlood && skill.SkillID != Chillblains)
                    {
                        fromDodge = true;
                        skill = new SkillModeDescriptor(player, Spec.Necromancer, MarkOfBlood);
                    }
                    icon = skill.SkillID == Chillblains ? EffectImages.EffectChillblains : EffectImages.EffectMarkOfBlood;
                }
                else
                {
                    skill = new SkillModeDescriptor(player, Spec.Necromancer, MarkOfBloodOrChillblainsTrigger);
                    icon = EffectImages.EffectMarkOfBloodOrChillblains;
                }
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, fromDodge ? 6000 : 30000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, icon);
            }
        }
        // Mark of Blood Activated (Staff 2)
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerMarkOfBloodActivated1, out var marksOfBloodActivated))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, MarkOfBlood);
            foreach (EffectEvent effect in marksOfBloodActivated)
            {
                (long, long) lifespan = ((int)effect.Time, (int)effect.Time + 500);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 300, EffectImages.EffectMarkOfBlood);
            }
        }
        // Chillblains Activated (Staff 3)
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerChillblainsActivated, out var chillblainsActivated))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, Chillblains);
            foreach (EffectEvent effect in chillblainsActivated)
            {
                (long, long) lifespan = ((int)effect.Time, (int)effect.Time + 500);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 300, EffectImages.EffectChillblains);
            }
        }

        // Putrid Mark (Staff 4)
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerPutridMark, out var putridMarks))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, PutridMark);
            foreach (EffectEvent effect in putridMarks)
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, 30000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectPutridMark);
            }
        }
        // Putrid Mark (Staff 4) Activated
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerPutridMarkActivated1, out var putridMarksActivated))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, PutridMark);
            foreach (EffectEvent effect in putridMarksActivated)
            {
                (long, long) lifespan = ((int)effect.Time, (int)effect.Time + 500);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 300, EffectImages.EffectPutridMark);
            }
        }

        // Reaper's Mark (Staff 5)
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerReapersMark, out var reapersMarks))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, ReapersMark);
            foreach (EffectEvent effect in reapersMarks)
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, 30000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectReapersMark);
            }
        }
        // Reaper's Mark (Staff 5) Activated
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerReapersMarkActivated, out var reapersMarksActivated))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, ReapersMark, SkillModeCategory.CC);
            foreach (EffectEvent effect in reapersMarksActivated)
            {
                (long, long) lifespan = ((int)effect.Time, (int)effect.Time + 500);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 300, EffectImages.EffectReapersMark);
            }
        }

        // Signet of Undeath
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerSignetOfUndeathGroundMark, out var signetOfUndeath))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, SignetOfUndeathSkill, SkillModeCategory.Heal);
            foreach (EffectEvent effect in signetOfUndeath)
            {
                (long, long) lifespan = ((int)effect.Time, (int)effect.Time + 500);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, EffectImages.EffectSignetOfUndeath);
            }
        }

        // Spectral Ring
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.NecromancerSpectralRing, out var spectralRings))
        {
            var skill = new SkillModeDescriptor(player, Spec.Necromancer, SpectralRing, SkillModeCategory.CC);
            foreach (EffectEvent effect in spectralRings)
            {
                long duration = log.FightData.Logic.SkillMode == EncounterLogic.FightLogic.SkillModeEnum.WvW ? 5000 : 8000;
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, duration);
                AddDoughnutSkillDecoration(replay, effect, color, skill, lifespan, 180, 200, EffectImages.EffectSpectralRing);
            }
        }
    }
}
