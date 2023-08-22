using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class UntamedHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(UnleashPet, PetUnleashed),
            new BuffGainCastFinder(UnleashRanger, Unleashed),
            new EffectCastFinderByDst(MutateConditions, EffectGUIDs.UntamedMutateConditions).UsingDstSpecChecker(Spec.Untamed),
            new EffectCastFinderByDst(UnnaturalTraversal, EffectGUIDs.UntamedUnnaturalTraversal).UsingDstSpecChecker(Spec.Untamed),

            // Pet
            new EffectCastFinderFromMinion(VenomousOutburst, EffectGUIDs.UntamedVenomousOutburst),
            new EffectCastFinderFromMinion(RendingVines, EffectGUIDs.UntamedRendingVines),
            new EffectCastFinderFromMinion(EnvelopingHaze, EffectGUIDs.UntamedEnvelopingHaze),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(FerociousSymbiosis, "Ferocious Symbiosis", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, BuffImages.FerociousSymbiosis, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta1, GW2Builds.November2022Balance),
            new BuffDamageModifier(FerociousSymbiosis, "Ferocious Symbiosis", "4% per stack", DamageSource.NoPets, 4.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, BuffImages.FerociousSymbiosis, DamageModifierMode.PvE).WithBuilds(GW2Builds.November2022Balance, GW2Builds.SOTOReleaseAndBalance),
            new BuffDamageModifier(FerociousSymbiosis, "Ferocious Symbiosis", "5% per stack", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, BuffImages.FerociousSymbiosis, DamageModifierMode.PvE).WithBuilds(GW2Builds.SOTOReleaseAndBalance),
            new BuffDamageModifier(FerociousSymbiosis, "Ferocious Symbiosis", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, BuffImages.FerociousSymbiosis, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.November2022Balance, GW2Builds.May2023BalanceHotFix),
            new BuffDamageModifier(FerociousSymbiosis, "Ferocious Symbiosis", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, BuffImages.FerociousSymbiosis, DamageModifierMode.sPvP).WithBuilds(GW2Builds.May2023BalanceHotFix),
            new BuffDamageModifier(FerociousSymbiosis, "Ferocious Symbiosis", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, BuffImages.FerociousSymbiosis, DamageModifierMode.WvW).WithBuilds(GW2Builds.May2023BalanceHotFix),
            new BuffDamageModifier(Unleashed, "Vow of the Untamed", "15% when unleashed", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, BuffImages.VowOfTheUntamed, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta1, GW2Builds.March2022Balance),
            new BuffDamageModifier(Unleashed, "Vow of the Untamed", "25% when unleashed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, BuffImages.VowOfTheUntamed, DamageModifierMode.PvE).WithBuilds(GW2Builds.March2022Balance),
            new BuffDamageModifier(Unleashed, "Vow of the Untamed", "15% when unleashed", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, BuffImages.VowOfTheUntamed, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.March2022Balance, GW2Builds.May2023BalanceHotFix),
            new BuffDamageModifier(Unleashed, "Vow of the Untamed", "10% when unleashed", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, BuffImages.VowOfTheUntamed, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.May2023BalanceHotFix),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Ferocious Symbiosis", FerociousSymbiosis, Source.Untamed, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.FerociousSymbiosis),
            new Buff("Unleashed", Unleashed, Source.Untamed, BuffClassification.Other, BuffImages.UnleashRanger),
            new Buff("Pet Unleashed", PetUnleashed, Source.Untamed, BuffClassification.Other, BuffImages.UnleashPet),
            new Buff("Perilous Gift", PerilousGift, Source.Untamed, BuffClassification.Other, BuffImages.PerilousGift),
            new Buff("Forest's Fortification", ForestsFortification, Source.Untamed, BuffClassification.Other, BuffImages.ForestsFortification),
        };

    }
}
