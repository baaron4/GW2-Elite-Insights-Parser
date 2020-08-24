using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;

namespace GW2EIEvtcParser.EIData
{
    internal class RenegadeHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(41858, 44272, EIData.InstantCastFinder.DefaultICD), // Legendary Renegade Stance
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(42883, "Kalla's Fervor", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Condition, DamageType.All, ParserHelper.Source.Renegade, ByStack, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png", DamageModifierMode.All),
            new BuffDamageModifier(45614, "Improved Kalla's Fervor", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Condition, DamageType.All, ParserHelper.Source.Renegade, ByStack, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png", DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        { 
                new Buff("Legendary Renegade Stance",44272, ParserHelper.Source.Renegade, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/19/Legendary_Renegade_Stance.png"),
                new Buff("Breakrazor's Bastion",44682, ParserHelper.Source.Renegade, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/a/a7/Breakrazor%27s_Bastion.png"),
                new Buff("Razorclaw's Rage",41016, ParserHelper.Source.Renegade, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/7/73/Razorclaw%27s_Rage.png"),
                new Buff("Soulcleave's Summit",45026, ParserHelper.Source.Renegade, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/7/78/Soulcleave%27s_Summit.png"),          
                new Buff("Kalla's Fervor",42883, ParserHelper.Source.Renegade, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png"),
                new Buff("Improved Kalla's Fervor",45614, ParserHelper.Source.Renegade, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/9e/Kalla%27s_Fervor.png"),
        };
    }
}
