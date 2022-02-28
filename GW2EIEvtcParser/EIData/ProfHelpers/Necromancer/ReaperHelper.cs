using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class ReaperHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(30792, 29446, EIData.InstantCastFinder.DefaultICD), // Reaper shroud
            new BuffLossCastFinder(30961, 29446, EIData.InstantCastFinder.DefaultICD), // Reaper shroud
            new BuffGainCastFinder(29958, 30129, EIData.InstantCastFinder.DefaultICD), // Infusing Terror
            new DamageCastFinder(29414, 29414, EIData.InstantCastFinder.DefaultICD), // "You Are All Weaklings!"
            new DamageCastFinder(30670, 30670, EIData.InstantCastFinder.DefaultICD), // "Suffer!"
            new DamageCastFinder(30772, 30772, EIData.InstantCastFinder.DefaultICD), // "Rise!" --> better to check dark bond?
            new DamageCastFinder(29604, 29604, EIData.InstantCastFinder.DefaultICD), // Chilling Nova
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(722, "Cold Shoulder", "15% on chilled target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Reaper, ByPresence, "https://wiki.guildwars2.com/images/7/78/Cold_Shoulder.png", GW2Builds.March2019Balance, GW2Builds.EndOfLife, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(722, "Cold Shoulder", "10% on chilled target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Reaper, ByPresence, "https://wiki.guildwars2.com/images/7/78/Cold_Shoulder.png", GW2Builds.March2019Balance, GW2Builds.EndOfLife, DamageModifierMode.sPvPWvW),
            new BuffDamageModifierTarget(722, "Cold Shoulder", "10% on chilled target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Reaper, ByPresence, "https://wiki.guildwars2.com/images/7/78/Cold_Shoulder.png", 0, GW2Builds.March2019Balance, DamageModifierMode.PvE),
            new DamageLogApproximateDamageModifier("Soul Eater", "10% to foes within 300 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Reaper, "https://wiki.guildwars2.com/images/6/6c/Soul_Eater.png", (x,log) =>
            {
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) <= 300.0;
            }, ByPresence, GW2Builds.July2019Balance, GW2Builds.EndOfLife, DamageModifierMode.All)
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Reaper's Shroud", 29446, Source.Reaper, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/11/Reaper%27s_Shroud.png"),
                new Buff("Infusing Terror", 30129, Source.Reaper, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/db/Infusing_Terror.png"),
                new Buff("Dark Bond", 31247, Source.Reaper, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/64/%22Rise%21%22.png"),
        };

        private static HashSet<long> Minions = new HashSet<long>()
        {
            (int)MinionID.ShamblingHorror,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }
    }
}
