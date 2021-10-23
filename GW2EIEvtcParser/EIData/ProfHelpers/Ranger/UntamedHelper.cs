using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class UntamedHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            // Unnatural Traversal ?
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Confirm if strike or strike and condition
            new BuffDamageModifier(-1, "Ferocious Symbiosis", "3%", DamageSource.NoPets, 3.0, DamageType.Strike, DamageType.All, Source.Untamed, ByStack, "https://wiki.guildwars2.com/images/d/d7/Premeditation.png", DamageModifierMode.All),
            new BuffDamageModifier(-1, "Vow of the Untamed", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Untamed, ByPresence, "https://wiki.guildwars2.com/images/d/d7/Premeditation.png", DamageModifierMode.All),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Ferocious Symbiosis",-1, Source.Untamed, ArcDPSEnums.BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            new Buff("Unleashed (Player)",-1, Source.Untamed, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            new Buff("Unleashed (Pet)",-1, Source.Untamed, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            new Buff("Perilous Gift",-1, Source.Untamed, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
            new Buff("Forest's Fortification",-1, Source.Untamed, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
        };

    }
}
