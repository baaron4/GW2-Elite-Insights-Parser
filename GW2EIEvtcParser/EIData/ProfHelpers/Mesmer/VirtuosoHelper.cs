using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class VirtuosoHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new DamageLogApproximateDamageModifier("Mental Focus", "10% to foes within 600 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Virtuoso, "https://wiki.guildwars2.com/images/d/da/Mental_Focus.png", (x,log) =>
            {
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) <= 600;
            }, ByPresence, GW2Builds.EODBeta1, GW2Builds.EndOfLife, DamageModifierMode.PvE),
            new BuffDamageModifier(63409, "Deadly Blades", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Virtuoso, ByPresence, "https://wiki.guildwars2.com/images/1/15/Deadly_Blades.png", GW2Builds.EODBeta4, GW2Builds.EndOfLife, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Deadly Blades", 63409, Source.Virtuoso, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/15/Deadly_Blades.png"),
        };

        private static HashSet<long> Minions = new HashSet<long>();
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }
    }
}
