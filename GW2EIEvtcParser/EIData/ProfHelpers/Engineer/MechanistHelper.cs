using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class MechanistHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            /*new Buff("Rectifier Signet",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            new Buff("Barrier Signet",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            new Buff("Force Signet",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            new Buff("Shift Signet",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            new Buff("Superconducting Signet",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            new Buff("Overclock Signet",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            //
            new Buff("Rectifier Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            new Buff("Barrier Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            new Buff("Force Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            new Buff("Shift Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            new Buff("Superconducting Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            new Buff("Overclock Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),*/
        };

    }
}
