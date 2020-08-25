using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    internal static class ScrapperHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Watchful Eye",31229, ParserHelper.Source.Scrapper, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/29/Bulwark_Gyro.png"),
                new Buff("Watchful Eye PvP",46910, ParserHelper.Source.Scrapper, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/29/Bulwark_Gyro.png"),

        };
    }
}
