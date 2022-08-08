using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ReaperHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(EnterReaperShroud, ReapersShroud), // Reaper shroud
            new BuffLossCastFinder(ExitReaperShroud, ReapersShroud), // Reaper shroud
            new BuffGainCastFinder(InfusingTerrorSkill, InfusingTerrorEffect), // Infusing Terror
            new DamageCastFinder(YouAreAllWeaklings, YouAreAllWeaklings), // "You Are All Weaklings!"
            new DamageCastFinder(Suffer, Suffer), // "Suffer!"
            new BuffGainCastFinder(Rise, DarkBond).UsingICD(500), // "Rise!"
            new DamageCastFinder(ChillingNova, ChillingNova), // Chilling Nova
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(Chilled, "Cold Shoulder", "15% on chilled target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Reaper, ByPresence, "https://wiki.guildwars2.com/images/7/78/Cold_Shoulder.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.March2019Balance),
            new BuffDamageModifierTarget(Chilled, "Cold Shoulder", "10% on chilled target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Reaper, ByPresence, "https://wiki.guildwars2.com/images/7/78/Cold_Shoulder.png", DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.March2019Balance),
            new BuffDamageModifierTarget(Chilled, "Cold Shoulder", "10% on chilled target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Reaper, ByPresence, "https://wiki.guildwars2.com/images/7/78/Cold_Shoulder.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.March2019Balance),
            new DamageLogDamageModifier("Soul Eater", "10% to foes within 300 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Reaper, "https://wiki.guildwars2.com/images/6/6c/Soul_Eater.png", (x,log) =>
            {
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) <= 300.0;
            }, ByPresence, DamageModifierMode.All).UsingApproximate(true).WithBuilds(GW2Builds.July2019Balance)
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Reaper's Shroud", ReapersShroud, Source.Reaper, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/11/Reaper%27s_Shroud.png"),
                new Buff("Infusing Terror", InfusingTerrorEffect, Source.Reaper, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/db/Infusing_Terror.png"),
                new Buff("Dark Bond", DarkBond, Source.Reaper, BuffClassification.Other, "https://wiki.guildwars2.com/images/6/64/%22Rise%21%22.png"),
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
