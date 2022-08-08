using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ScrapperHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new EffectCastFinder(BulwarkGyro, EffectGUIDs.ScrapperBulwarkGyro).UsingChecker((evt, log) => evt.Src.Spec == Spec.Scrapper),
            new EffectCastFinder(PurgeGyro, EffectGUIDs.ScrapperPurgeGyro).UsingChecker((evt, log) => evt.Src.Spec == Spec.Scrapper),
            new EffectCastFinder(DefenseField, EffectGUIDs.ScrapperDefenseField).UsingChecker((evt, log) => evt.Src.Spec == Spec.Scrapper),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(new long[] { Swiftness, Superspeed, Stability}, "Object in Motion", "5% under swiftness/superspeed/stability, accumulative", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Scrapper, ByMultiPresence, "https://wiki.guildwars2.com/images/d/da/Object_in_Motion.png", DamageModifierMode.All).WithBuilds( GW2Builds.July2019Balance)
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Watchful Eye",WatchfulEye, Source.Scrapper, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/2/29/Bulwark_Gyro.png"),
                new Buff("Watchful Eye PvP",WatchfulEyePvP, Source.Scrapper, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/2/29/Bulwark_Gyro.png"),

        };
    }
}
