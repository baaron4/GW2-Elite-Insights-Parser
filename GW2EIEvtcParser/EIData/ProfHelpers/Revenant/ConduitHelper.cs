using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class ConduitHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(LegendaryEntityStanceSkill, LegendaryEntityStanceBuff),
        new BuffGainCastFinder(CosmicWisdomSkill, CosmicWisdomBuff)
            .UsingChecker((evt, combatData, agentData, skillData) => evt.AppliedDuration == 7000)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        new DamageCastFinder(Mistfire, Mistfire) // TODO: check if there is an effect
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Unconditional),
        // The scythe effect appears twice around the player when using skills 6-9 in Razah with Form of the Dervish buff active.
        new EffectCastFinder(FormOfTheDervishDamage, EffectGUIDs.ConduitFormOfTheDervishScythe)                 
            .UsingIsAroundDstChecker()
            // In this we check any ground effects to be present. If there are, we use the other cast finder.
            .UsingNoSecondaryEffectSameSrcInvertedTypeChecker(EffectGUIDs.ConduitFormOfTheDervishScythe)
            .UsingICD(10),
        // The scythe effect appears only once around the player and once on the ground when using the elite skill.
        new EffectCastFinder(FormOfTheDervishDamageElite, EffectGUIDs.ConduitFormOfTheDervishScythe)
            .UsingNotIsAroundDstChecker()
            .UsingICD(10),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = [   
        // - Targeted Destruction Numinous Gift
        new BuffOnFoeDamageModifier(Mod_TargetedDestruction_NuminousGift, Vulnerability, "Targeted Destruction (Numinous Gift)", "0.5% per stack vuln + 5% base", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Conduit, new GainComputerByStackPlusConstant(5.0), TraitImages.TargetedDestruction, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2025VoEBeta),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = 
    [
        // Shielding Hands
        new BuffOnActorDamageModifier(Mod_ShieldingHands, ShieldingHandsBuff, "Shielding Hands", "-75%", DamageSource.Incoming, -75, DamageType.StrikeAndCondition, DamageType.All, Source.Conduit, ByPresence, SkillImages.ShieldingHands, DamageModifierMode.All),
        // Retribution Numinous Gift
        new BuffOnActorDamageModifier(Mod_DeterminedResolution_NuminousGift, Resolution, "Determined Resolution (Numinous Gift)", "-12% under resolution", DamageSource.Incoming, -12.0, DamageType.Strike, DamageType.All, Source.Conduit, ByPresence, TraitImages.DeterminedResolution, DamageModifierMode.sPvP)
            .WithBuilds(GW2Builds.August2025VoEBeta),
        new BuffOnActorDamageModifier(Mod_DeterminedResolution_NuminousGift, Resolution, "Determined Resolution (Numinous Gift)", "-15% under resolution", DamageSource.Incoming, -15.0, DamageType.Strike, DamageType.All, Source.Conduit, ByPresence, TraitImages.DeterminedResolution, DamageModifierMode.PvEWvW)
            .WithBuilds(GW2Builds.August2025VoEBeta),
    ];

    // TODO: check if new buffs with the rework
    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Legendary Entity Stance", LegendaryEntityStanceBuff, Source.Conduit, BuffClassification.Other, SkillImages.LegendaryEntityStance),
        new Buff("Cosmic Wisdom", CosmicWisdomBuff, Source.Conduit, BuffClassification.Other, BuffImages.CosmicWisdom)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        // Razah
        new Buff("Form of the Dervish (Razah Active)", FormOfTheDervishRazahActiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheDervish),
        new Buff("Form of the Dervish (Razah Passive)", FormOfTheDervishRazahPassiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheDervish),
        // Shiro
        new Buff("Form of the Assassin (Shiro Active)", FormOfTheAssassinShiroActiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheAssassin),
        new Buff("Form of the Assassin (Shiro Passive)", FormOfTheAssassinShiroPassiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheAssassin),
        // Mallxy
        new Buff("Form of the Mesmer (Mallyx Active)", FormOfTheMesmerMallyxActiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheMesmer),
        new Buff("Form of the Mesmer (Mallyx Passive)", FormOfTheMesmerMallyxPassiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheMesmer),
        // Ventari
        new Buff("Form of the Monk (Ventari Active)", FormOfTheMonkVentariActiveBuff_Beta, Source.Conduit, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.FormOfTheMonk)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new Buff("Form of the Monk (Ventari Passive)", FormOfTheMonkVentariPassiveBuff, Source.Conduit, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.FormOfTheMonk)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new Buff("Form of the Monk (Ventari Active)", FormOfTheMonkVentariActiveBuff, Source.Conduit, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.FormOfTheMonk)
            .WithBuilds(GW2Builds.OctoberVoERelease),// Why is this an intensity buff Anet?
        new Buff("Form of the Monk (Ventari Passive)", FormOfTheMonkVentariPassiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheMonk)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        // Jalis
        new Buff("Form of the Warrior (Jalis Active)", FormOfTheWarriorJalisActiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheWarrior),
        new Buff("Form of the Warrior (Jalis Passive)", FormOfTheWarriorJalisPassiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheWarrior),
        // Skills
        new Buff("Shielding Hands", ShieldingHandsBuff, Source.Conduit, BuffClassification.Other, SkillImages.ShieldingHands),
        // Traits
        new Buff("Lingering Determination", LingeringDetermination, Source.Conduit, BuffClassification.Other, TraitImages.LingeringDetermination),
    ];

    internal static void RedirectGladiatorsDefenseCastEvents(CombatData combatData, SkillData skillData, Dictionary<long, List<AnimatedCastEvent>> animatedCastDataByID)
    {
        var casts = combatData.GetAnimatedCastData(GladiatorsDefenseAnimation);
        if (casts.Count == 0)
        {
            return;
        }
        var skill = skillData.Get(GladiatorsDefense);
        foreach (var cast in casts)
        {
            cast.OverrideSkill(skill);
        }
        animatedCastDataByID.Remove(GladiatorsDefenseAnimation);
        animatedCastDataByID.Add(GladiatorsDefense, casts.ToList());
    }

    public static bool IsLegendSwap(long id)
    {
        return LegendaryEntityStanceSkill == id;
    }
}
