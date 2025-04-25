using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class MirageHelper
{

    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new DamageCastFinder(Jaunt, Jaunt)
            .UsingDisableWithEffectData(),
        new EffectCastFinder(Jaunt, EffectGUIDs.MirageJaunt)
            .UsingSecondaryEffectChecker(EffectGUIDs.MirageJauntConflict1)
            .UsingSecondaryEffectChecker(EffectGUIDs.MirageJauntConflict2)
            .UsingSrcSpecChecker(Spec.Mirage),
        new BuffGainCastFinder(MirageCloakDodge, MirageCloak),
        // Illusionary Ambush not trackable due to conflicting effects with Jaunt and Axe of Symmetry
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Sharp Edges
        new BuffOnActorDamageModifier(Mod_SharpEdges, SharpEdges, "Sharp Edges", "15%", DamageSource.NoPets, 15, DamageType.Strike, DamageType.All, Source.Mirage, ByPresence, TraitImages.MirageMantle, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_SharpEdges, SharpEdges, "Sharp Edges", "25%", DamageSource.NoPets, 25, DamageType.Strike, DamageType.All, Source.Mirage, ByPresence, TraitImages.MirageMantle, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.February2025BalancePatch),
        new BuffOnActorDamageModifier(Mod_SharpEdges, SharpEdges, "Sharp Edges", "10%", DamageSource.NoPets, 10, DamageType.Strike, DamageType.All, Source.Mirage, ByPresence, TraitImages.MirageMantle, DamageModifierMode.sPvPWvW),
        // Nomad's Endurance
        new BuffOnActorDamageModifier(Mod_NomadsEndurance, Vigor, "Nomad's Endurance", "10%", DamageSource.NoPets, 10, DamageType.StrikeAndCondition, DamageType.All, Source.Mirage, ByPresence, TraitImages.NomadsEndurance, DamageModifierMode.All)
            .WithBuilds(GW2Builds.February2025BalancePatch),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Mirage Cloak", MirageCloak, Source.Mirage, BuffClassification.Other, SkillImages.MirageCloak),
        new Buff("False Oasis", FalseOasis, Source.Mirage, BuffClassification.Other, SkillImages.FalseOasis),
        // Spear
        new Buff("Sharp Edges", SharpEdges, Source.Mirage, BuffClassification.Other, TraitImages.MirageMantle),
    ];

    private static readonly HashSet<int> Minions = [];
    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }
}
