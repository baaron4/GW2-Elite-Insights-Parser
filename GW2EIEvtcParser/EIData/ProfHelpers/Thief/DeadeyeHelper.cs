using System.Collections.Generic;
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
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Kneeling",42869, Source.Deadeye, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/56/Kneel.png"),
                new Buff("Deadeye's Gaze", 46333, Source.Deadeye, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/78/Deadeye%27s_Mark.png"),
        };

    }
}
