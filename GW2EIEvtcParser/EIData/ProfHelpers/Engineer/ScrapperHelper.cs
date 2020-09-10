using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;

namespace GW2EIEvtcParser.EIData
{
    internal static class ScrapperHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(new long[] { 719, 5974, 1122}, "Object in Motion", "5% under swiftness/superspeed/stability, accumulative", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, ParserHelper.Source.Scrapper, ByMultiPresence, "https://wiki.guildwars2.com/images/d/da/Object_in_Motion.png", 97950, ulong.MaxValue, DamageModifierMode.All)
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Watchful Eye",31229, ParserHelper.Source.Scrapper, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/29/Bulwark_Gyro.png"),
                new Buff("Watchful Eye PvP",46910, ParserHelper.Source.Scrapper, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/29/Bulwark_Gyro.png"),

        };
    }
}
