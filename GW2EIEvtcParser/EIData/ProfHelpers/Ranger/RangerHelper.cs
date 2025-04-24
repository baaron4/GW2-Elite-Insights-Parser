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

internal static class RangerHelper
{

    private static readonly HashSet<int> SpiritIDs =
    [
        (int)MinionID.FrostSpirit,
        (int)MinionID.StoneSpirit,
        (int)MinionID.StormSpirit,
        (int)MinionID.SunSpirit,
        (int)MinionID.WaterSpirit,
        (int)MinionID.SpiritOfNatureRenewal,
    ];

    private static readonly HashSet<int> JuvenileFelinePetIDs =
    [
        (int)MinionID.JuvenileCheetah,
        (int)MinionID.JuvenileJaguar,
        (int)MinionID.JuvenileJungleStalker,
        (int)MinionID.JuvenileLynx,
        (int)MinionID.JuvenileSandLion,
        (int)MinionID.JuvenileSnowLeopard,
        (int)MinionID.JuvenileTiger,
        (int)MinionID.JuvenileWhiteTiger,
        (int)MinionID.JuvenileWarclaw,
    ];

    private static readonly HashSet<int> JuvenileBirdPetIDs =
    [
        (int)MinionID.JuvenileEagle,
        (int)MinionID.JuvenileHawk,
        (int)MinionID.JuvenileOwl,
        (int)MinionID.JuvenileRaven,
        (int)MinionID.JuvenileWhiteRaven,
    ];

    private static readonly HashSet<int> JuvenileDrakePetIDs =
    [
        (int)MinionID.JuvenileIceDrake,
        (int)MinionID.JuvenileMarshDrake,
        (int)MinionID.JuvenileReefDrake,
        (int)MinionID.JuvenileRiverDrake,
        (int)MinionID.JuvenileSalamanderDrake,
    ];

    private static readonly HashSet<int> JuvenileUrsinePetIDs =
    [
        (int)MinionID.JuvenileArctodus,
        (int)MinionID.JuvenileBlackBear,
        (int)MinionID.JuvenileBrownBear,
        (int)MinionID.JuvenileMurellow,
        (int)MinionID.JuvenilePolarBear,
    ];

    private static readonly HashSet<int> JuvenilePorcinePetIDs =
    [
        (int)MinionID.JuvenileBoar,
        (int)MinionID.JuvenilePig,
        (int)MinionID.JuvenileSiamoth,
        (int)MinionID.JuvenileWarthog,
    ];

    private static readonly HashSet<int> JuvenileMoaPetIDs =
    [
        (int)MinionID.JuvenileBlackMoa,
        (int)MinionID.JuvenileBlueMoa,
        (int)MinionID.JuvenilePinkMoa,
        (int)MinionID.JuvenileRedMoa,
        (int)MinionID.JuvenileWhiteMoa,
    ];
    private static readonly HashSet<int> JuvenileSpiderPetIDs =
    [
        (int)MinionID.JuvenileBlackWidowSpider,
        (int)MinionID.JuvenileCaveSpider,
        (int)MinionID.JuvenileForestSpider,
        (int)MinionID.JuvenileJungleSpider,
    ];

    private static readonly HashSet<int> JuvenileDevourerPetIDs =
    [
        (int)MinionID.JuvenileCarrionDevourer,
        (int)MinionID.JuvenileLashtailDevourer,
        (int)MinionID.JuvenileWhiptailDevourer,
    ]; 
    
    private static readonly HashSet<int> JuvenileCaninePetIDs =
    [
        (int)MinionID.JuvenileAlpineWolf,
        (int)MinionID.JuvenileFernHound,
        (int)MinionID.JuvenileHyena,
        (int)MinionID.JuvenileKrytanDrakehound,
        (int)MinionID.JuvenileWolf,
    ];

    private static readonly HashSet<int> JuvenileJellyfishPetIDs =
    [
        (int)MinionID.JuvenileBlueJellyfish,
        (int)MinionID.JuvenileRainbowJellyfish,
        (int)MinionID.JuvenileRedJellyfish,
    ];

    private static readonly HashSet<int> JuvenileWyvernPetIDs =
    [
        (int)MinionID.JuvenileEletricWywern,
        (int)MinionID.JuvenileFireWywern,
    ];

    private static readonly HashSet<int> JuvenilePetIDs = new HashSet<int>() { 
        (int)MinionID.JuvenileArmorFish,
        (int)MinionID.JuvenileBristleback,
        (int)MinionID.JuvenileFangedIboga,
        (int)MinionID.JuvenileJacaranda,
        (int)MinionID.JuvenilePhoenix,
        (int)MinionID.JuvenileRockGazelle,
        (int)MinionID.JuvenileShark,
        (int)MinionID.JuvenileSiegeTurtle,
        (int)MinionID.JuvenileSmokescale,
        (int)MinionID.JuvenileWallow,
        (int)MinionID.JuvenileAetherHunter,
        (int)MinionID.JuvenileSkyChakStriker,
        (int)MinionID.JuvenileSpinegazer,
        (int)MinionID.JuvenileJanthiriBee,
    }
    .Union(JuvenileFelinePetIDs)
    .Union(JuvenileBirdPetIDs)
    .Union(JuvenileDrakePetIDs)
    .Union(JuvenileUrsinePetIDs)
    .Union(JuvenilePorcinePetIDs)
    .Union(JuvenileMoaPetIDs)
    .Union(JuvenileSpiderPetIDs)
    .Union(JuvenileDevourerPetIDs)
    .Union(JuvenileCaninePetIDs)
    .Union(JuvenileJellyfishPetIDs)
    .Union(JuvenileWyvernPetIDs)
    .ToHashSet();

    internal static bool IsJuvenileFelinePet(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return JuvenileFelinePetIDs.Contains(agentItem.ID);
    }

    internal static bool IsJuvenileBirdPet(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return JuvenileBirdPetIDs.Contains(agentItem.ID);
    }

    internal static bool IsJuvenileDrakePet(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return JuvenileDrakePetIDs.Contains(agentItem.ID);
    }
    internal static bool IsJuvenileUrsinePet(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return JuvenileUrsinePetIDs.Contains(agentItem.ID);
    }

    internal static bool IsJuvenilePorcinePet(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return JuvenilePorcinePetIDs.Contains(agentItem.ID);
    }

    internal static bool IsJuvenileMoaPet(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return JuvenileMoaPetIDs.Contains(agentItem.ID);
    }
    internal static bool IsJuvenileSpiderPet(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return JuvenileSpiderPetIDs.Contains(agentItem.ID);
    }
    internal static bool IsJuvenileDevourerPet(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return JuvenileDevourerPetIDs.Contains(agentItem.ID);
    }
    internal static bool IsJuvenileCaninePet(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return JuvenileCaninePetIDs.Contains(agentItem.ID);
    }

    internal static bool IsJuvenileJellyfishPet(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return JuvenileJellyfishPetIDs.Contains(agentItem.ID);
    }

    internal static bool IsJuvenileWyvernPet(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return JuvenileWyvernPetIDs.Contains(agentItem.ID);
    }

    private static bool IsJuvenilePetID(int id)
    {
        return JuvenilePetIDs.Contains(id);
    }

    internal static bool IsJuvenilePet(AgentItem agentItem)
    {
        if (agentItem.Type == AgentItem.AgentType.Gadget)
        {
            return false;
        }
        return IsJuvenilePetID(agentItem.ID);
    }

    internal static bool IsKnownMinionID(int id)
    {
        return IsJuvenilePetID(id) || SpiritIDs.Contains(id);
    }

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        //new DamageCastFinder(12573,12573), // Hunter's Shot
        //new DamageCastFinder(12507,12507), // Crippling Shot
        new BuffGainCastFinder(SicEmSkill, SicEmBuff)
            .WithMinions(true),
        new BuffGainCastFinder(SicEmSkill, SicEmPvPBuff)
            .WithMinions(true),
        new BuffGainCastFinder(SignetOfStone, SignetOfStoneActive)
            .UsingChecker((evt, combatData, agentData, skillData) => Math.Abs(evt.AppliedDuration - 6000) < ServerDelayConstant), // Signet of Stone
        new BuffGainCastFinder(LesserSignetOfStone, SignetOfStoneActive)
            .UsingChecker((evt, combatData, agentData, skillData) => Math.Abs(evt.AppliedDuration - 5000) < ServerDelayConstant)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait), // Lesser Signet of Stone
        new BuffGainCastFinder(SharpeningStonesSkill, SharpeningStonesBuff),
        new BuffGainCastFinder(QuickDraw, QuickDraw)
            .UsingAfterWeaponSwap(true)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new BuffGainCastFinder(AttackOfOpportunity, AttackOfOpportunity)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new EXTHealingCastFinder(WindborneNotes, WindborneNotes)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new EXTHealingCastFinder(InvigoratingBond, InvigoratingBond)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new EXTHealingCastFinder(EvasivePurity, EvasivePurity)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new EXTBarrierCastFinder(ProtectMe, ProtectMe),
        new BuffGiveCastFinder(GuardSkill, GuardBuff)
            .UsingChecker(((evt, combatData, agentData, skillData) => Math.Abs(evt.AppliedDuration - 6000) < ServerDelayConstant)),
        new BuffGiveCastFinder(LesserGuardSkill, GuardBuff)
            .UsingChecker(((evt, combatData, agentData, skillData) => Math.Abs(evt.AppliedDuration - 4000) < ServerDelayConstant))
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new BuffGiveCastFinder(SearchAndRescueSkill, SearchAndRescueBuff)
            .UsingICD(1100).UsingNotAccurate(true),
        new EffectCastFinder(LightningReflexes, EffectGUIDs.RangerLightningReflexes)
            .UsingSrcBaseSpecChecker(Spec.Ranger),
        new EffectCastFinderByDst(QuickeningZephyr, EffectGUIDs.RangerQuickeningZephyr)
            .UsingDstBaseSpecChecker(Spec.Ranger),
        new EffectCastFinderByDst(SignetOfRenewalSkill, EffectGUIDs.RangerSignetOfRenewal)
            .UsingDstBaseSpecChecker(Spec.Ranger),
        new EffectCastFinderByDst(SignetOfTheHuntSkill, EffectGUIDs.RangerSignetOfTheHunt)
            .UsingDstBaseSpecChecker(Spec.Ranger),
        new MinionSpawnCastFinder(RangerPetSpawned, JuvenilePetIDs)
            .UsingNotAccurate(true),
    ];

    private static bool SicEmFromDst(DamageEvent x, ParsedEvtcLog log)
    {
        AgentItem src = x.From;
        var effectApply = log.CombatData.GetBuffDataByIDByDst(SicEmBuff, src).LastOrDefault(y => y is BuffApplyEvent && y.Time <= x.Time);
        if (effectApply != null)
        {
            return x.To == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
        }
        return false;
    }

    private static bool TargetBelow600Range(DamageEvent x, ParsedEvtcLog log)
    {
        return x.Skill.IsWeaponSkill && TargetWithinRangeChecker(x, log, 600);
    }
    private static bool TargetAbove600Range(DamageEvent x, ParsedEvtcLog log)
    {
        return x.Skill.IsWeaponSkill && !TargetWithinRangeChecker(x, log, 600);
    }

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Skills
        // - Sic 'Em
        new BuffOnActorDamageModifier(Mod_SicEm, SicEmBuff, "Sic 'Em!", "40%", DamageSource.NoPets, 40.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, SkillImages.SicEm, DamageModifierMode.PvE)
            .UsingChecker(SicEmFromDst)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_SicEm, SicEmBuff, "Sic 'Em!", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, SkillImages.SicEm, DamageModifierMode.sPvPWvW)
            .UsingChecker(SicEmFromDst)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_SicEm, SicEmBuff, "Sic 'Em!", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, SkillImages.SicEm, DamageModifierMode.All)
            .UsingChecker(SicEmFromDst)
            .WithBuilds(GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_SicEmPet, SicEmBuff, "Sic 'Em!", "40% Pet", DamageSource.PetsOnly, 40.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, SkillImages.SicEm, DamageModifierMode.All),
        // - Frost Spirit
        new BuffOnActorDamageModifier(Mod_FrostSpirit, FrostSpiritBuff, "Frost Spirit", "5%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, SkillImages.FrostSpirit, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),

        // Marksmanship
        // - Farsighted (<= 600)
        new DamageLogDamageModifier(Mod_FarsightedClose, "Farsighted (<= 600)", "5% with weapon skills below 600 range", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.SteadyFocus, TargetBelow600Range, DamageModifierMode.All)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.July2018Balance, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new DamageLogDamageModifier(Mod_FarsightedClose, "Farsighted (<= 600)", "5% with weapon skills below 600 range", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.SteadyFocus, TargetBelow600Range, DamageModifierMode.sPvPWvW)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM, GW2Builds.February2025BalancePatch),
        new DamageLogDamageModifier(Mod_FarsightedClose, "Farsighted (<= 600)", "10% with weapon skills below 600 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.SteadyFocus, TargetBelow600Range, DamageModifierMode.PvE)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM, GW2Builds.February2025BalancePatch),
        new DamageLogDamageModifier(Mod_FarsightedClose, "Farsighted (<= 600)", "5% with weapon skills below 600 range", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.SteadyFocus, TargetBelow600Range, DamageModifierMode.sPvP)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        new DamageLogDamageModifier(Mod_FarsightedClose, "Farsighted (<= 600)", "10% with weapon skills below 600 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.SteadyFocus, TargetBelow600Range, DamageModifierMode.PvEWvW)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        // - Farsighted (> 600)
        new DamageLogDamageModifier(Mod_FarsightedFar, "Farsighted (> 600)", "10% with weapon skills above 600 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.SteadyFocus, TargetAbove600Range, DamageModifierMode.All)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.July2018Balance, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new DamageLogDamageModifier(Mod_FarsightedFar, "Farsighted (> 600)", "15% with weapon skills above 600 range", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.SteadyFocus, TargetAbove600Range, DamageModifierMode.PvE)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM, GW2Builds.February2025BalancePatch),
        new DamageLogDamageModifier(Mod_FarsightedFar, "Farsighted (> 600)", "10% with weapon skills above 600 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.SteadyFocus, TargetAbove600Range, DamageModifierMode.sPvPWvW)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM, GW2Builds.February2025BalancePatch),
        new DamageLogDamageModifier(Mod_FarsightedFar, "Farsighted (> 600)", "10% with weapon skills above 600 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.SteadyFocus, TargetAbove600Range, DamageModifierMode.sPvP)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        new DamageLogDamageModifier(Mod_FarsightedFar, "Farsighted (> 600)", "15% with weapon skills above 600 range", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.SteadyFocus, TargetAbove600Range, DamageModifierMode.PvEWvW)
            .UsingApproximate(true)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        // - Predator's Onslaught
        new BuffOnFoeDamageModifier(Mod_PredatorsOnslaught, [Stun, Taunt, Daze, Crippled, Fear, Immobile, Chilled], "Predator's Onslaught", "15% to disabled or movement-impaired foes", DamageSource.All, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, TraitImages.PredatorsOnslaught, DamageModifierMode.All)
            .UsingApproximate(true),
        new BuffOnFoeDamageModifier(Mod_Wolfsong, Vulnerability, "Wolfsong", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, TraitImages.Wolfsong, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.April2025BalancePatch),
        new BuffOnFoeDamageModifier(Mod_Wolfsong, Vulnerability, "Wolfsong", "5%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, TraitImages.Wolfsong, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.April2025BalancePatch),

        // Skirmishing
        // - Hunter's Tactics
        new DamageLogDamageModifier(Mod_HuntersTactics, "Hunter's Tactics", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.HuntersTactics, (x, log) => x.IsFlanking , DamageModifierMode.All)
            .WithBuilds(GW2Builds.February2020Balance, GW2Builds.June2022Balance),
        new DamageLogDamageModifier(Mod_HuntersTactics, "Hunter's Tactics", "10% while flanking", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.HuntersTactics, (x, log) => x.IsFlanking , DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.June2022Balance),
        new DamageLogDamageModifier(Mod_HuntersTactics, "Hunter's Tactics", "15% while flanking", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.HuntersTactics, (x, log) => x.IsFlanking , DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.June2022Balance, GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        new DamageLogDamageModifier(Mod_HuntersTactics, "Hunter's Tactics", "15% while flanking or against defiant", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.HuntersTactics, (x, log) => x.IsFlanking || x.To.GetCurrentBreakbarState(log, x.Time) != BreakbarState.None , DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        // - Light on your Feet
        new BuffOnActorDamageModifier(Mod_LightOnYourFeet, LightOnYourFeet, "Light on your Feet", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, TraitImages.LightOnYourFeet, DamageModifierMode.All),

        // Nature Magic
        // - Bountiful Hunter
        new BuffOnActorDamageModifier(Mod_BountifulHunterWithPet, NumberOfBoons, "Bountiful Hunter", "1% per boon (player and pet)", DamageSource.All, 1.0, DamageType.Strike, DamageType.All, Source.Ranger, ByStack, TraitImages.BountifulHunter, DamageModifierMode.All),
        
        // Wilderness Survival
        // - Survival Instincts
        new DamageLogDamageModifier(Mod_SurvivalInstincts, "Survival Instincts (Outgoing)","10% if hp >=50%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.SurvivalInstincts, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 50.0, DamageModifierMode.All)
            .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary)
            .UsingApproximate(true),

        // Beastmastery
        // - Beastly Warden
        new DamageLogDamageModifier(Mod_BeastlyWardenPetOnly, "Beastly Warden (Pets)", "20% for Ursine and Porcine pets", DamageSource.PetsOnly, 20.0, DamageType.All, DamageType.All, Source.Ranger, TraitImages.BeastlyWarden, (x, log) => IsJuvenileUrsinePet(x.From) || IsJuvenilePorcinePet(x.From) , DamageModifierMode.All)
            .UsingEarlyExit((a, log) => {
                return a.GetMinions(log).Any(x => IsJuvenileUrsinePet(x.Value.ReferenceAgentItem) || IsJuvenilePorcinePet(x.Value.ReferenceAgentItem));
            })
            .WithBuilds(GW2Builds.April2025BalancePatch),
        
        // Mace
        new BuffOnActorDamageModifier(Mod_ForceOfNature, ForceOfNature, "Force of Nature", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, SkillImages.ForceOfNature, DamageModifierMode.All)
            .WithBuilds(GW2Builds.February2024NewWeapons, GW2Builds.March2024BalanceAndCerusLegendary),
        new BuffOnActorDamageModifier(Mod_ForceOfNature, ForceOfNature, "Force of Nature", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, SkillImages.ForceOfNature, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary, GW2Builds.May2024LonelyTowerFractalRelease),
        new BuffOnActorDamageModifier(Mod_ForceOfNature, ForceOfNature, "Force of Nature", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, SkillImages.ForceOfNature, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary, GW2Builds.May2024LonelyTowerFractalRelease),
        new BuffOnActorDamageModifier(Mod_ForceOfNature, ForceOfNature, "Force of Nature", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, SkillImages.ForceOfNature, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2024LonelyTowerFractalRelease),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        // Wilderness Survival
        // - Oakheart Salve
        new BuffOnActorDamageModifier(Mod_OakheartSalve, Regeneration, "Oakheart Salve", "-5% under regeneration", DamageSource.Incoming, -5.0, DamageType.Strike, DamageType.All, Source.Ranger, ByPresence, TraitImages.OakheartSalve, DamageModifierMode.All),
        // - Survival Instincts
        new DamageLogDamageModifier(Mod_SurvivalInstincts, "Survival Instincts (Incoming)","10% if hp <= 50%", DamageSource.Incoming, 10.0, DamageType.Strike, DamageType.All, Source.Ranger, TraitImages.SurvivalInstincts, (x, log) => x.AgainstUnderFifty, DamageModifierMode.All)
            .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Counterattack", Counterattack, Source.Ranger, BuffClassification.Other, SkillImages.Counterattack),
        // Signets
        new Buff("Signet of Renewal", SignetOfRenewalBuff, Source.Ranger, BuffClassification.Other, SkillImages.SignetOfRenewal),
        new Buff("Signet of Stone", SignetOfStoneBuff, Source.Ranger, BuffClassification.Other, SkillImages.SignetOfStone),
        new Buff("Signet of Stone (Pet)", SignetOfStonePetBuff, Source.Ranger, BuffClassification.Other, SkillImages.SignetOfStone), // not present even on soulbeast?
        new Buff("Signet of Stone (Active)", SignetOfStoneActive, Source.Ranger, BuffClassification.Other, SkillImages.SignetOfStone),
        new Buff("Signet of the Wild", SignetOfTheWild, Source.Ranger, BuffClassification.Other, SkillImages.SignetOfTheWild),
        new Buff("Signet of the Wild (Pet)", SignetOfTheWildPet, Source.Ranger, BuffClassification.Other, SkillImages.SignetOfTheWild),
        new Buff("Signet of the Hunt", SignetOfTheHuntBuff, Source.Ranger, BuffClassification.Other, SkillImages.SignetOfTheHunt),
        new Buff("Signet of the Hunt (Pet)", SignetOfTheHuntPetBuff, Source.Ranger, BuffClassification.Other, SkillImages.SignetOfTheHunt),
        // Spirits
        // new Boon("Water Spirit (old)", 50386, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, SkillImages.WaterSpirit),
        new Buff("Frost Spirit", FrostSpiritOld, Source.Ranger, BuffClassification.Offensive, SkillImages.FrostSpirit)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2018Balance),
        new Buff("Sun Spirit", SunSpiritOld, Source.Ranger, BuffClassification.Offensive, SkillImages.SunSpirit)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2018Balance),
        new Buff("Stone Spirit", StoneSpiritOld, Source.Ranger, BuffClassification.Defensive, SkillImages.StoneSpirit)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2018Balance),
        //new Boon("Storm Spirit (old)", 50381, BoonSource.Ranger, BoonType.Duration, 1, BoonEnum.DefensiveBuffTable, SkillImages.StormSpirit),
        // Spirits reworked
        new Buff("Water Spirit", WaterSpiritBuff, Source.Ranger, BuffClassification.Defensive, SkillImages.WaterSpirit)
            .WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),
        new Buff("Frost Spirit", FrostSpiritBuff, Source.Ranger, BuffClassification.Offensive, SkillImages.FrostSpirit)
            .WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),
        new Buff("Sun Spirit", SunSpiritBuff, Source.Ranger, BuffClassification.Offensive, SkillImages.SunSpirit)
            .WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),
        new Buff("Stone Spirit", StoneSpiritBuff, Source.Ranger, BuffClassification.Defensive, SkillImages.StoneSpirit)
            .WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),
        new Buff("Storm Spirit", StormSpiritBuff, Source.Ranger, BuffClassification.Support, SkillImages.StormSpirit)
            .WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),
        // Skills
        new Buff("Call of the Wild", CallOfTheWild, Source.Ranger, BuffClassification.Other, SkillImages.CallOfTheWild)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
        new Buff("Call of the Wild", CallOfTheWild, Source.Ranger, BuffStackType.Stacking, 3, BuffClassification.Other, SkillImages.CallOfTheWild)
            .WithBuilds(GW2Builds.July2019Balance, GW2Builds.February2020Balance),
        new Buff("Strength of the Pack!", StrengthOfThePack, Source.Ranger, BuffClassification.Other, SkillImages.StrengthOfThePack),
        new Buff("Sic 'Em!", SicEmBuff, Source.Ranger, BuffClassification.Other, SkillImages.SicEm),
        new Buff("Sic 'Em! (PvP)", SicEmPvPBuff, Source.Ranger, BuffClassification.Other, SkillImages.SicEm),
        new Buff("Sharpening Stones", SharpeningStonesBuff, Source.Ranger, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.SharpeningStone),
        new Buff("Sharpen Spines", SharpenSpinesBuff, Source.Ranger, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.SharpenSpines),
        new Buff("Guard!", GuardBuff, Source.Ranger, BuffClassification.Defensive, SkillImages.Guard),
        new Buff("Search and Rescue!", SearchAndRescueBuff, Source.Ranger, BuffClassification.Support, SkillImages.SearchAndRescue),
        new Buff("Ancestral Grace", AncestralGraceBuff, Source.Ranger, BuffClassification.Other, SkillImages.AncestralGrace)
            .WithBuilds(GW2Builds.June2023BalanceAndSOTOBetaAndSilentSurfNM),
        // Traits
        new Buff("Attack of Opportunity", AttackOfOpportunity, Source.Ranger, BuffClassification.Other, TraitImages.MomentOfClarity),
        new Buff("Clarion Bond", ClarionBond, Source.Ranger, BuffClassification.Other, TraitImages.ClarionBond),
        new Buff("Spotter", Spotter, Source.Ranger, BuffClassification.Offensive, TraitImages.Spotter)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2022Balance),
        new Buff("Opening Strike", OpeningStrike, Source.Ranger, BuffClassification.Other, TraitImages.OpeningStrike),
        new Buff("Quick Draw", QuickDraw, Source.Ranger, BuffClassification.Other, TraitImages.QuickDraw),
        new Buff("Light on your Feet", LightOnYourFeet, Source.Ranger, BuffStackType.Queue, 25, BuffClassification.Other, TraitImages.LightOnYourFeet),
        new Buff("Poison Master", PoisonMasterBuff, Source.Ranger, BuffClassification.Other, TraitImages.PoisonMaster),
        // Mace
        new Buff("Force of Nature", ForceOfNature, Source.Ranger, BuffClassification.Other, SkillImages.ForceOfNature)
            .WithBuilds(GW2Builds.February2024NewWeapons),
        new Buff("Nature's Strength", NaturesStrength, Source.Ranger, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, SkillImages.NaturesStrength)
            .WithBuilds(GW2Builds.February2024NewWeapons),
        new Buff("Tapped Out", TappedOut, Source.Ranger, BuffClassification.Other, SkillImages.TappedOut)
            .WithBuilds(GW2Builds.February2024NewWeapons),
        new Buff("Pet Heal (Oaken Cudgel)", OakenCudgelPetHeal, Source.Ranger, BuffClassification.Other, SkillImages.OakenCudgel)
            .WithBuilds(GW2Builds.February2024NewWeapons),
        // Spear
        new Buff("Hunter's Prowess", HuntersProwess, Source.Ranger, BuffStackType.Queue, 9, BuffClassification.Other, SkillImages.HuntersProwess),
    ];

    public static void ProcessGadgets(IReadOnlyList<AgentItem> players, CombatData combatData)
    {
        var playerAgents = new HashSet<AgentItem>(players);
        // entangle works fine already
        HashSet<AgentItem> jacarandaEmbraces = GetOffensiveGadgetAgents(combatData, JacarandasEmbraceMinion, playerAgents);
        HashSet<AgentItem> blackHoles = GetOffensiveGadgetAgents(combatData, BlackHoleMinion, playerAgents);
        var rangers = players.Where(x => x.BaseSpec == Spec.Ranger);
        var rangersCount = rangers.Count();
        // if only one ranger, could only be that one
        if (rangersCount == 1)
        {
            AgentItem ranger = rangers.First();
            SetGadgetMaster(jacarandaEmbraces, ranger);
            SetGadgetMaster(blackHoles, ranger);
        }
        else if (rangersCount > 1)
        {
            AttachMasterToGadgetByCastData(combatData, jacarandaEmbraces, new List<long> { JacarandasEmbraceSkill }, 1000);
            AttachMasterToGadgetByCastData(combatData, blackHoles, new List<long> { BlackHoleSkill }, 1000);
        }
    }

    internal static void ComputeProfessionCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        Color color = Colors.Ranger;

        // Siege Turtle Hunker Down
        if (log.CombatData.TryGetEffectEventsByMasterWithGUID(player.AgentItem, EffectGUIDs.RangerHunkerDown, out var hunkerDowns))
        {
            var skill = new SkillModeDescriptor(player, Spec.Ranger, HunkerDownPetTurtle, SkillModeCategory.ProjectileManagement);
            foreach (EffectEvent effect in hunkerDowns)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectHunkerDown);
            }
        }
        // Barrage
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RangerBarrage1, out var barrages))
        {
            var skill = new SkillModeDescriptor(player, Spec.Ranger, Barrage, SkillModeCategory.ShowOnSelect);
            foreach (EffectEvent effect in barrages)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 600); // ~600ms interval
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, EffectImages.EffectBarrage);
            }
        }
        // Bonfire
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RangerBonfire, out var bonfires))
        {
            var skill = new SkillModeDescriptor(player, Spec.Ranger, Bonfire, SkillModeCategory.ShowOnSelect);
            foreach (EffectEvent effect in bonfires)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 8000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectBonfire);
            }
        }
        // Healing Spring - Inactive
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RangerHealingSpringInactive2, out var healingSpringsInactive))
        {
            var skill = new SkillModeDescriptor(player, Spec.Ranger, HealingSpring, SkillModeCategory.ShowOnSelect);
            foreach (EffectEvent effect in healingSpringsInactive)
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, 300000);
                var connector = new PositionConnector(effect.Position);
                // Halved the saturation of the circle and icon to distinguish between inactive and active.
                replay.Decorations.Add(new CircleDecoration(180, lifespan, color, 0.25, connector).UsingFilled(false).UsingSkillMode(skill));
                replay.Decorations.Add(new IconDecoration(EffectImages.EffectHealingSpring, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.25f, lifespan, connector).UsingSkillMode(skill));
            }
        }
        // Healing Spring - Active
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RangerHealingSpringActive, out var healingSpringsActive))
        {
            var skill = new SkillModeDescriptor(player, Spec.Ranger, HealingSpring, SkillModeCategory.Cleanse | SkillModeCategory.Heal);
            foreach (EffectEvent effect in healingSpringsActive)
            {
                long duration = log.LogData.GW2Build < GW2Builds.March2024BalanceAndCerusLegendary ? 10000 : 5000;
                (long, long) lifespan = effect.ComputeLifespan(log, duration);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectHealingSpring);
            }
        }
        // Frost Trap
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RangerFrostTrap, out var frostTraps))
        {
            var skill = new SkillModeDescriptor(player, Spec.Ranger, FrostTrap, SkillModeCategory.ShowOnSelect);
            foreach (EffectEvent effect in frostTraps)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectFrostTrap);
            }
        }
        // Flame Trap
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RangerFlameTrap, out var flameTraps))
        {
            var skill = new SkillModeDescriptor(player, Spec.Ranger, FlameTrap, SkillModeCategory.ShowOnSelect);
            foreach (EffectEvent effect in flameTraps)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectFlameTrap);
            }
        }
        // Viper's Nest
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RangerVipersNest, out var vipersNests))
        {
            var skill = new SkillModeDescriptor(player, Spec.Ranger, VipersNest, SkillModeCategory.ShowOnSelect);
            foreach (EffectEvent effect in vipersNests)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 3000);
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectVipersNest);
            }
        }
        // Spike Trap
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.RangerSpikeTrap, out var spikeTraps))
        {
            var skill = new SkillModeDescriptor(player, Spec.Ranger, SpikeTrap, SkillModeCategory.ShowOnSelect | SkillModeCategory.CC);
            foreach (EffectEvent effect in spikeTraps)
            {
                (long, long) lifespan = effect.ComputeLifespan(log, 2000); // roughly time displayed ingame
                AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 240, EffectImages.EffectSpikeTrap);
            }
        }
        // Sublime Conversion
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.DruidSublimeConversion2, out var sublimeConversions2))
        {
            var skill = new SkillModeDescriptor(player, Spec.Ranger, SublimeConversion, SkillModeCategory.ProjectileManagement);
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.DruidSublimeConversion1, out var sublimeConversions1))
            {
                foreach (EffectEvent effect in sublimeConversions1)
                {
                    if (sublimeConversions2.Any(x => Math.Abs(x.Time - effect.Time) < ServerDelayConstant))
                    {
                        (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                        var connector = new PositionConnector(effect.Position);
                        var rotationConnector = new AngleConnector(effect.Rotation.Z);
                        replay.Decorations.Add(new RectangleDecoration(400, 60, lifespan, color, 0.5, connector).UsingFilled(false).UsingRotationConnector(rotationConnector).UsingSkillMode(skill));
                        replay.Decorations.Add(new IconDecoration(EffectImages.EffectSublimeConversion, CombatReplaySkillDefaultSizeInPixel, CombatReplaySkillDefaultSizeInWorld, 0.5f, lifespan, connector).UsingSkillMode(skill));
                    }
                }
            }
        }
    }
}
