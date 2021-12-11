using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class MechanistHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Need to check mech specy id for those
            new BuffDamageModifier(63243, "Force Signet", "10%, including Mech", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Mechanist, ByPresence, "https://wiki.guildwars2.com/images/b/b3/Force_Signet.png", GW2Builds.EODBeta4, GW2Builds.EndOfLife, DamageModifierMode.All, (x,log) =>
            {
                return x.From == x.CreditedFrom || x.From.ID == (int)MinionID.JadeMech;
            }),
            new BuffDamageModifier(63322, "Superconducting Signet", "10%, including Mech", DamageSource.All, 10.0, DamageType.Condition, DamageType.All, Source.Catalyst, ByPresence, "https://wiki.guildwars2.com/images/5/51/Superconducting_Signet.png", GW2Builds.EODBeta4, GW2Builds.EndOfLife, DamageModifierMode.All, (x,log) =>
            {
                return x.From == x.CreditedFrom || x.From.ID == (int)MinionID.JadeMech;
            }),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Rectifier Signet",63305, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c4/Rectifier_Signet.png"),
            new Buff("Barrier Signet",63064, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b8/Barrier_Signet.png"),
            new Buff("Force Signet",63243, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b3/Force_Signet.png"),
            new Buff("Shift Signet",63068, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d1/Shift_Signet.png"),
            new Buff("Superconducting Signet",63322, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/51/Superconducting_Signet.png"),
            new Buff("Overclock Signet",63059, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c7/Overclock_Signet.png"),
            //
            //new Buff("Rectifier Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c4/Rectifier_Signet.png"),
            new Buff("Barrier Signet (J-Drive)",63228, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b8/Barrier_Signet.png"),
            //new Buff("Force Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b3/Force_Signet.png"),
            //new Buff("Shift Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d1/Shift_Signet.png"),
            //new Buff("Superconducting Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/51/Superconducting_Signet.png"),
            new Buff("Overclock Signet (J-Drive)",63378, Source.Mechanist, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c7/Overclock_Signet.png"),
        };

        private static HashSet<long> Minions = new HashSet<long>()
        {
            (int)MinionID.JadeMech,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }

    }
}
