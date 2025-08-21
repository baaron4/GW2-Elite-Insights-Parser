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
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = [];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];

    internal static readonly IReadOnlyList<Buff> Buffs = [];

    private static readonly HashSet<long> _cycloneBows =
    [
        SummonCycloneBow, DismissCycloneBow
    ];

    public static bool IsCycloneBowTransformation(long id)
    {
        return _cycloneBows.Contains(id);
    }
}
