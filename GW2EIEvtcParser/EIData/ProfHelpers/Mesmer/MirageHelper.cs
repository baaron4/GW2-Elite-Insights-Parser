using System.Collections.Generic;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class MirageHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(Jaunt, Jaunt),
            // new EffectCastFinderByDst(Jaunt, EffectGUIDs.MirageJaunt).UsingDstSpecChecker(Spec.Mirage),
            new BuffGainCastFinder(MirageCloakDodge, MirageCloak), // Mirage Cloak
            //new EffectCastFinderByDst(IllusionaryAmbush, EffectGUIDs.MirageIllusionaryAmbush).UsingChecker((evt, log) => evt.Dst.Spec == Spec.Mirage),
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(SharpEdges, "Sharp Edges", "15%", DamageSource.NoPets, 15, DamageType.Strike, DamageType.All, Source.Mirage, ByPresence, BuffImages.MonsterSkill, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(SharpEdges, "Sharp Edges", "10%", DamageSource.NoPets, 10, DamageType.Strike, DamageType.All, Source.Mirage, ByPresence, BuffImages.MonsterSkill, DamageModifierMode.sPvPWvW),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Mirage Cloak", MirageCloak, Source.Mirage, BuffClassification.Other, BuffImages.MirageCloak),
            new Buff("False Oasis", FalseOasis, Source.Mirage, BuffClassification.Other, BuffImages.FalseOasis),
            // Spear
            new Buff("Sharp Edges", SharpEdges, Source.Mirage, BuffClassification.Other, BuffImages.MonsterSkill),
        };

        private static HashSet<int> Minions = new HashSet<int>();
        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }
    }
}
