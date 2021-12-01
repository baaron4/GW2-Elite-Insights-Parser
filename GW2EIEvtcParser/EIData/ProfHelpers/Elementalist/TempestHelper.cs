using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class TempestHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            //new DamageCastFinder(30662, 30662, 10000), // "Feel the Burn!" - shockwave, fire aura indiscernable from the focus skill
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(31353, "Harmonious Conduit", "10% (4s) after overload", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Tempest, ByPresence, "https://wiki.guildwars2.com/images/b/b3/Harmonious_Conduit.png", 0 , GW2Builds.October2019Balance, DamageModifierMode.PvE),
            new BuffDamageModifier(31353, "Transcendent Tempest", "7% (7s) after overload", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Tempest, ByPresence, "https://wiki.guildwars2.com/images/a/ac/Transcendent_Tempest_%28effect%29.png", GW2Builds.October2019Balance , GW2Builds.EndOfLife, DamageModifierMode.All),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Rebound",31337, Source.Tempest, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/0/03/%22Rebound%21%22.png"),
                new Buff("Harmonious Conduit",31353, Source.Tempest, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b3/Harmonious_Conduit.png", 0, GW2Builds.October2019Balance),
                new Buff("Transcendent Tempest",31353, Source.Tempest, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/ac/Transcendent_Tempest_%28effect%29.png", GW2Builds.October2019Balance, GW2Builds.EndOfLife),
                new Buff("Static Charge",31487, Source.Tempest, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/4/4b/Overload_Air.png"),
                new Buff("Heat Sync",30462, Source.Tempest, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/d/d9/Heat_Sync.png"),
        };

    }
}
