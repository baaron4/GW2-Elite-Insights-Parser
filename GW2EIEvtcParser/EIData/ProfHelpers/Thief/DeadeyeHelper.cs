using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class DeadeyeHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(NumberOfBoons, "Premeditation", "1% per boon",DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByStack, "https://wiki.guildwars2.com/images/d/d7/Premeditation.png", DamageModifierMode.All),
            new BuffDamageModifier(DeadeyesGaze, "Iron Sight", "10% to marked target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, "https://wiki.guildwars2.com/images/d/dd/Iron_Sight.png", DamageModifierMode.All).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(DeadeyesGaze).Where(y => y is BuffApplyEvent && y.To == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By;
                }
                return false;
            }),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Kneeling",Kneeling, Source.Deadeye, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/56/Kneel.png"),
                new Buff("Deadeye's Gaze", DeadeyesGaze, Source.Deadeye, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/78/Deadeye%27s_Mark.png"),
        };

        private static HashSet<long> Minions = new HashSet<long>()
        {
            (int)MinionID.Deadeye1,
            (int)MinionID.Deadeye2,
            (int)MinionID.Deadeye3,
            (int)MinionID.Deadeye4,
            (int)MinionID.Deadeye5,
            (int)MinionID.Deadeye6,
            (int)MinionID.Deadeye7,
            (int)MinionID.Deadeye8,
            (int)MinionID.Deadeye9,
            (int)MinionID.Deadeye10,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }

    }
}
