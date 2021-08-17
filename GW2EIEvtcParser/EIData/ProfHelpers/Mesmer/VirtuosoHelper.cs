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
            new DamageLogApproximateDamageModifier("Mental Focus", "15% to foes within 600 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Virtuoso, "", (x,log) =>
            {
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) <= 600;
            }, ByPresence, ulong.MaxValue, ulong.MaxValue, DamageModifierMode.All)
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            // blade?
        };
    }
}
