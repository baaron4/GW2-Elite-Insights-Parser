using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class UntamedHelper
{

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(UnleashPet, PetUnleashed),
        new BuffGainCastFinder(UnleashRanger, Unleashed),
        new BuffGainCastFinder(RestorativeStrikes, RestorativeStrikes)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new EffectCastFinderByDst(MutateConditions, EffectGUIDs.UntamedMutateConditions)
            .UsingDstSpecChecker(Spec.Untamed),
        new EffectCastFinderByDst(UnnaturalTraversal, EffectGUIDs.UntamedUnnaturalTraversal)
            .UsingDstSpecChecker(Spec.Untamed),

        // Pet
        new EffectCastFinder(VenomousOutburst, EffectGUIDs.UntamedVenomousOutburst)
            .WithMinions(true),
        new EffectCastFinder(RendingVines, EffectGUIDs.UntamedRendingVines)
            .WithMinions(true),
        new EffectCastFinder(EnvelopingHaze, EffectGUIDs.UntamedEnvelopingHaze)
            .WithMinions(true),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_FerociousSymbiosis, FerociousSymbiosis, "Ferocious Symbiosis", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, TraitImages.FerociousSymbiosis, DamageModifierMode.All)
            .WithBuilds(GW2Builds.EODBeta1, GW2Builds.November2022Balance),
        new BuffOnActorDamageModifier(Mod_FerociousSymbiosis, FerociousSymbiosis, "Ferocious Symbiosis", "4% per stack", DamageSource.NoPets, 4.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, TraitImages.FerociousSymbiosis, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.November2022Balance, GW2Builds.SOTOReleaseAndBalance),
        new BuffOnActorDamageModifier(Mod_FerociousSymbiosis, FerociousSymbiosis, "Ferocious Symbiosis", "5% per stack", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, TraitImages.FerociousSymbiosis, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.SOTOReleaseAndBalance),
        new BuffOnActorDamageModifier(Mod_FerociousSymbiosis, FerociousSymbiosis, "Ferocious Symbiosis", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, TraitImages.FerociousSymbiosis, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.November2022Balance, GW2Builds.May2023BalanceHotFix),
        new BuffOnActorDamageModifier(Mod_FerociousSymbiosis, FerociousSymbiosis, "Ferocious Symbiosis", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, TraitImages.FerociousSymbiosis, DamageModifierMode.sPvP)
            .WithBuilds(GW2Builds.May2023BalanceHotFix),
        new BuffOnActorDamageModifier(Mod_FerociousSymbiosis, FerociousSymbiosis, "Ferocious Symbiosis", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, TraitImages.FerociousSymbiosis, DamageModifierMode.WvW)
            .WithBuilds(GW2Builds.May2023BalanceHotFix),
        //
        new BuffOnActorDamageModifier(Mod_VowOfTheUntamed, Unleashed, "Vow of the Untamed", "15% when unleashed", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, TraitImages.VowOfTheUntamed, DamageModifierMode.All)
            .WithBuilds(GW2Builds.EODBeta1, GW2Builds.March2022Balance),
        new BuffOnActorDamageModifier(Mod_VowOfTheUntamed, Unleashed, "Vow of the Untamed", "25% when unleashed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, TraitImages.VowOfTheUntamed, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.March2022Balance),
        new BuffOnActorDamageModifier(Mod_VowOfTheUntamed, Unleashed, "Vow of the Untamed", "15% when unleashed", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, TraitImages.VowOfTheUntamed, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.March2022Balance, GW2Builds.May2023BalanceHotFix),
        new BuffOnActorDamageModifier(Mod_VowOfTheUntamed, Unleashed, "Vow of the Untamed", "10% when unleashed", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, TraitImages.VowOfTheUntamed, DamageModifierMode.sPvP)
            .WithBuilds(GW2Builds.May2023BalanceHotFix),
        new BuffOnActorDamageModifier(Mod_VowOfTheUntamed, Unleashed, "Vow of the Untamed", "15% when unleashed", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, TraitImages.VowOfTheUntamed, DamageModifierMode.WvW)
            .WithBuilds(GW2Builds.May2023BalanceHotFix, GW2Builds.February2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_VowOfTheUntamed, Unleashed, "Vow of the Untamed", "20% when unleashed", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, TraitImages.VowOfTheUntamed, DamageModifierMode.WvW)
            .WithBuilds(GW2Builds.February2025BalancePatch),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        new CounterOnActorDamageModifier(Mod_PerilousGift, PerilousGift, "Perilous Gift", "No damage from incoming attacks or conditions", DamageSource.Incoming, DamageType.StrikeAndCondition, DamageType.StrikeAndCondition, Source.Untamed, SkillImages.PerilousGift, DamageModifierMode.All)
            .WithBuilds(GW2Builds.EODBeta4),
        new BuffOnActorDamageModifier(Mod_ForestsFortification, ForestsFortification, "Forest's Fortification", "-50%", DamageSource.Incoming, -50.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, SkillImages.ForestsFortification, DamageModifierMode.All)
            .WithBuilds(GW2Builds.EODBeta1, GW2Builds.April2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_ForestsFortification, ForestsFortification, "Forest's Fortification", "-33%", DamageSource.Incoming, -33.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, SkillImages.ForestsFortification, DamageModifierMode.sPvP)
            .WithBuilds(GW2Builds.April2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_ForestsFortification, ForestsFortification, "Forest's Fortification", "-50%", DamageSource.Incoming, -50.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, SkillImages.ForestsFortification, DamageModifierMode.PvEWvW)
            .WithBuilds(GW2Builds.April2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_VowOfTheUntamed, Unleashed, "Vow of the Untamed", "-10% when not unleashed", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Untamed, ByAbsence, TraitImages.VowOfTheUntamed, DamageModifierMode.All)
            .WithBuilds(GW2Builds.EODBeta1, GW2Builds.March2022Balance),
        new BuffOnActorDamageModifier(Mod_VowOfTheUntamed, Unleashed, "Vow of the Untamed", "-25% when not unleashed", DamageSource.Incoming, -25.0, DamageType.Strike, DamageType.All, Source.Untamed, ByAbsence, TraitImages.VowOfTheUntamed, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.March2022Balance),
        new BuffOnActorDamageModifier(Mod_VowOfTheUntamed, Unleashed, "Vow of the Untamed", "-10% when not unleashed", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Untamed, ByAbsence, TraitImages.VowOfTheUntamed, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.March2022Balance, GW2Builds.February2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_VowOfTheUntamed, Unleashed, "Vow of the Untamed", "-10% when not unleashed", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Untamed, ByAbsence, TraitImages.VowOfTheUntamed, DamageModifierMode.sPvP)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_VowOfTheUntamed, Unleashed, "Vow of the Untamed", "-15% when not unleashed", DamageSource.Incoming, -15.0, DamageType.Strike, DamageType.All, Source.Untamed, ByAbsence, TraitImages.VowOfTheUntamed, DamageModifierMode.WvW)
            .WithBuilds(GW2Builds.February2025BalancePatch),
    ];


    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Ferocious Symbiosis", FerociousSymbiosis, Source.Untamed, BuffStackType.Stacking, 5, BuffClassification.Other, TraitImages.FerociousSymbiosis),
        new Buff("Unleashed", Unleashed, Source.Untamed, BuffClassification.Other, SkillImages.UnleashRanger),
        new Buff("Pet Unleashed", PetUnleashed, Source.Untamed, BuffClassification.Other, SkillImages.UnleashPet),
        new Buff("Perilous Gift", PerilousGift, Source.Untamed, BuffClassification.Other, SkillImages.PerilousGift),
        new Buff("Forest's Fortification", ForestsFortification, Source.Untamed, BuffClassification.Other, SkillImages.ForestsFortification),
        new Buff("Unleashed Power", UnleashedPowerBuff, Source.Untamed, BuffClassification.Other, TraitImages.UnleashedPower),
        new Buff("Restorative Strikes", RestorativeStrikes, Source.Untamed, BuffClassification.Other, TraitImages.RestorativeStrikes),
    ];

}
