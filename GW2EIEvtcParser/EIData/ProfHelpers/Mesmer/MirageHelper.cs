using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class MirageHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(Jaunt, Jaunt), // Jaunt
            new BuffGainCastFinder(MirageCloakDodge, MirageCloak), // Mirage Cloak
            new EffectCastFinder(SandThroughGlass, EffectGUIDs.MirageSandThroughGlass).UsingChecker((evt, log) => evt.Src.Spec == Spec.Mirage),
            //new EffectCastFinderByDst(IllusionaryAmbush, EffectGUIDs.MirageIllusionaryAmbush).UsingChecker((evt, log) => evt.Dst.Spec == Spec.Mirage),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Mirage Cloak",MirageCloak, ParserHelper.Source.Mirage, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a5/Mirage_Cloak_%28effect%29.png"),
                new Buff("False Oasis",FalseOasis, ParserHelper.Source.Mirage, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/32/False_Oasis.png"),
        };

        private static HashSet<long> Minions = new HashSet<long>();
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }
    }
}
