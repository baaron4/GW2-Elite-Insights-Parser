using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class DeadeyeHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(NumberOfBoonsID, "Premeditation", "1% per boon",DamageSource.NoPets, 1.0, DamageType.Power, DamageType.All, Source.Deadeye, ByStack, "https://wiki.guildwars2.com/images/d/d7/Premeditation.png", DamageModifierMode.All),
            new BuffDamageModifier(46333, "Iron Sight", "10% to marked target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, Source.Deadeye, ByPresence, "https://wiki.guildwars2.com/images/d/dd/Iron_Sight.png", DamageModifierMode.All, (x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(46333).Where(y => y is BuffApplyEvent && y.To == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By;
                }
                return false;
            }),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Kneeling",42869, Source.Deadeye, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/56/Kneel.png"),
                new Buff("Deadeye's Gaze", 46333, Source.Deadeye, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/78/Deadeye%27s_Mark.png"),
        };

    }
}
