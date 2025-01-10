using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.DamageModifierIDs;

namespace GW2EIEvtcParser.EIData;

internal static class MirageHelper
{

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new DamageCastFinder(Jaunt, Jaunt),
        // new EffectCastFinderByDst(Jaunt, EffectGUIDs.MirageJaunt).UsingDstSpecChecker(Spec.Mirage),
        new BuffGainCastFinder(MirageCloakDodge, MirageCloak), // Mirage Cloak
        //new EffectCastFinderByDst(IllusionaryAmbush, EffectGUIDs.MirageIllusionaryAmbush).UsingChecker((evt, log) => evt.Dst.Spec == Spec.Mirage),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_SharpEdges, SharpEdges, "Sharp Edges", "15%", DamageSource.NoPets, 15, DamageType.Strike, DamageType.All, Source.Mirage, ByPresence, TraitImages.MirageMantle, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_SharpEdges, SharpEdges, "Sharp Edges", "10%", DamageSource.NoPets, 10, DamageType.Strike, DamageType.All, Source.Mirage, ByPresence, TraitImages.MirageMantle, DamageModifierMode.sPvPWvW),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Mirage Cloak", MirageCloak, Source.Mirage, BuffClassification.Other, SkillImages.MirageCloak),
        new Buff("False Oasis", FalseOasis, Source.Mirage, BuffClassification.Other, SkillImages.FalseOasis),
        // Spear
        new Buff("Sharp Edges", SharpEdges, Source.Mirage, BuffClassification.Other, TraitImages.MirageMantle),
    ];

    private static HashSet<int> Minions = [];
    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }
}
