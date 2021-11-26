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
            }, ByPresence, 118697, ulong.MaxValue, DamageModifierMode.PvE),
            //new BuffDamageModifier(62529, "Deadly Blades", "3% per stack", DamageSource.NoPets, 5.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Virtuoso, ByPresence, "https://wiki.guildwars2.com/images/1/15/Deadly_Blades.png", 118697, ulong.MaxValue, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            // blade?
             //new Buff("Deadly Blades", 62529, Source.Virtuoso, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/15/Deadly_Blades.png", 118697, ulong.MaxValue),
        };
    }
}
