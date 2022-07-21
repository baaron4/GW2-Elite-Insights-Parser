using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

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
            new BuffDamageModifier(HarmoniousConduit, "Harmonious Conduit", "10% (4s) after overload", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Tempest, ByPresence, "https://wiki.guildwars2.com/images/b/b3/Harmonious_Conduit.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife ,GW2Builds.October2019Balance),
            new BuffDamageModifier(TranscendentTempest, "Transcendent Tempest", "7% (7s) after overload", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Tempest, ByPresence, "https://wiki.guildwars2.com/images/a/ac/Transcendent_Tempest_%28effect%29.png", DamageModifierMode.All).WithBuilds(GW2Builds.October2019Balance),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Rebound",Rebound, Source.Tempest, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/0/03/%22Rebound%21%22.png"),
                new Buff("Harmonious Conduit",HarmoniousConduit, Source.Tempest, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b3/Harmonious_Conduit.png", 0, GW2Builds.October2019Balance),
                new Buff("Transcendent Tempest",TranscendentTempest, Source.Tempest, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/ac/Transcendent_Tempest_%28effect%29.png", GW2Builds.October2019Balance, GW2Builds.EndOfLife),
                new Buff("Static Charge",StaticCharge, Source.Tempest, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/4/4b/Overload_Air.png"),
                new Buff("Heat Sync",HeatSync, Source.Tempest, BuffClassification.Support, "https://wiki.guildwars2.com/images/d/d9/Heat_Sync.png"),
        };

    }
}
