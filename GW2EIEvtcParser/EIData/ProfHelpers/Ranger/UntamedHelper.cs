using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
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
            new EffectCastFinderByDst(MutateConditions, EffectGUIDs.UntamedMutateConditions).UsingChecker((evt, log) => evt.Dst.Spec == Spec.Untamed),
            new EffectCastFinderByDst(UnnaturalTraversal, EffectGUIDs.UntamedUnnaturalTraversal).UsingChecker((evt, log) => evt.Dst.Spec == Spec.Untamed)
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(FerociousSymbiosis, "Ferocious Symbiosis", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, "https://wiki.guildwars2.com/images/7/73/Ferocious_Symbiosis.png", DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta1),
            new BuffDamageModifier(Unleashed, "Vow of the Untamed", "15% when unleashed", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Vow_of_the_Untamed.png", DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta1, GW2Builds.March2022Balance),
            new BuffDamageModifier(Unleashed, "Vow of the Untamed", "25% when unleashed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Vow_of_the_Untamed.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.March2022Balance),
            new BuffDamageModifier(Unleashed, "Vow of the Untamed", "15% when unleashed", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Vow_of_the_Untamed.png", DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.March2022Balance),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Ferocious Symbiosis",FerociousSymbiosis, Source.Untamed, ArcDPSEnums.BuffStackType.Stacking, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/73/Ferocious_Symbiosis.png"),
            new Buff("Unleashed",Unleashed, Source.Untamed, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/91/Unleash_Ranger.png"),
            new Buff("Pet Unleashed",PetUnleashed, Source.Untamed, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/43/Unleash_Pet.png"),
            new Buff("Perilous Gift",PerilousGift, Source.Untamed, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d6/Perilous_Gift.png"),
            new Buff("Forest's Fortification",ForestsFortification, Source.Untamed, BuffClassification.Other, "https://wiki.guildwars2.com/images/4/43/Forest%27s_Fortification.png"),
        };

    }
}
