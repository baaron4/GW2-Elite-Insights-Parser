using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class SpecterHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            //new BuffGainCastFinder(-1, -1, EIData.InstantCastFinder.DefaultICD), // Shadow Shroud Enter
            //new BuffLossCastFinder(-1, -1, EIData.InstantCastFinder.DefaultICD), // Shadow Shroud Exit
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            //new Buff("Shadow Shroud",-1, Source.Specter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            //new Buff("Shrouded Ally",-1, Source.Specter, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            //new Buff("Rot Wallow Venom",-1, Source.Specter, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
        };

    }
}
