using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    internal static class MirageHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(45449, 45449, EIData.InstantCastFinder.DefaultICD), // Jaunt
            new BuffGainCastFinder(SkillItem.MirageCloakDodgeId, 40408, EIData.InstantCastFinder.DefaultICD), // Mirage Cloak
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Mirage Cloak",40408, ParserHelper.Source.Mirage, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a5/Mirage_Cloak_%28effect%29.png"),
        };
    }
}
