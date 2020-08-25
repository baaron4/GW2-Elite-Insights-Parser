using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    internal static class SpellbreakerHelper
    {
        /////////////////////
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(43745, 40616, EIData.InstantCastFinder.DefaultICD), // Sight beyond Sight
            new DamageCastFinder(45534, 45534, EIData.InstantCastFinder.DefaultICD), // Loss Aversion

        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {

        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Sight beyond Sight",40616, ParserHelper.Source.Spellbreaker, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/d/d7/Sight_beyond_Sight.png"),
                new Buff("Full Counter",43949, ParserHelper.Source.Spellbreaker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/fb/Full_Counter.png"),
                new Buff("Attacker's Insight",41963, ParserHelper.Source.Spellbreaker, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/89/Attacker%27s_Insight.png"),
        };


    }
}
