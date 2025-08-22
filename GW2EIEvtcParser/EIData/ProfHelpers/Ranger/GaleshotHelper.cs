using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class GaleshotHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder = 
    [
        new EffectCastFinder(SummonCycloneBow, EffectGUIDs.GaleshotSummonCycloneBow)
            .UsingDstSpecChecker(Spec.Galeshot)
            .UsingBeforeWeaponSwap(),
        new EffectCastFinderByDst(DismissCycloneBow, EffectGUIDs.GaleshotDismissCycloneBow)
            .UsingDstSpecChecker(Spec.Galeshot)
            .UsingBeforeWeaponSwap(),
        // TODO Add Wind Shear if it's instant cast, verify https://wiki.guildwars2.com/wiki/Wind_Shear
        // TODO Add Wuthering Wind proc? https://wiki.guildwars2.com/wiki/Wuthering_Wind
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = 
    [
        // Wind Force
        new BuffOnActorDamageModifier(Mod_WindForce, WindForce, "Wind Force", "3% stacking", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Galeshot, ByMultipliyingStack, BuffImages.WindForce, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_WindForce, WindForce, "Wind Force", "2% stacking", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Galeshot, ByMultipliyingStack, BuffImages.WindForce, DamageModifierMode.sPvPWvW),
        
        // Gale Force
        new BuffOnActorDamageModifier(Mod_GaleForce, GaleForce, "Gale Force", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Galeshot, ByPresence, TraitImages.GaleForce, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_GaleForce, GaleForce, "Gale Force", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Galeshot, ByPresence, TraitImages.GaleForce, DamageModifierMode.sPvPWvW),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Wind Force", WindForce, Source.Galeshot, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.WindForce),
        new Buff("Gale Force", GaleForce, Source.Galeshot, BuffClassification.Other, TraitImages.GaleForce), // TODO Verify stacking type
        new Buff("Wuthering Wind", WutheringWindBuff, Source.Galeshot, BuffClassification.Other, TraitImages.WutheringWind),
    ];

    private static readonly HashSet<long> _cycloneBows =
    [
        SummonCycloneBow, DismissCycloneBow
    ];

    public static bool IsCycloneBowTransformation(long id)
    {
        return _cycloneBows.Contains(id);
    }
}
