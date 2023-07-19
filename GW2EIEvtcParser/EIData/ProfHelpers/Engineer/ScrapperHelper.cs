using System.Collections.Generic;
using GW2EIEvtcParser.EIData.Buffs;
using static GW2EIEvtcParser.ArcDPSEnums;
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
            new EffectCastFinder(BulwarkGyro, EffectGUIDs.ScrapperBulwarkGyro).UsingSrcSpecChecker(Spec.Scrapper),
            new EffectCastFinder(PurgeGyro, EffectGUIDs.ScrapperPurgeGyro).UsingSrcSpecChecker(Spec.Scrapper),
            new EffectCastFinder(DefenseField, EffectGUIDs.ScrapperDefenseField).UsingSrcSpecChecker(Spec.Scrapper),
            new EffectCastFinder(BypassCoating, EffectGUIDs.ScrapperBypassCoating).UsingSrcSpecChecker(Spec.Scrapper),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(new long[] { Swiftness, Superspeed, Stability }, "Object in Motion", "5% under swiftness/superspeed/stability, accumulative", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Scrapper, ByMultiPresence, BuffImages.ObjectInMotion, DamageModifierMode.All).WithBuilds(GW2Builds.July2019Balance)
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Watchful Eye", WatchfulEye, Source.Scrapper, BuffClassification.Defensive, BuffImages.BulwarkGyro),
            new Buff("Watchful Eye PvP", WatchfulEyePvP, Source.Scrapper, BuffClassification.Defensive, BuffImages.BulwarkGyro),
        };

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.BlastGyro,
            (int)MinionID.BulwarkGyro,
            (int)MinionID.FunctionGyro,
            (int)MinionID.MedicGyro,
            (int)MinionID.ShredderGyro,
            (int)MinionID.SneakGyro,
            (int)MinionID.PurgeGyro,
        };

        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }
    }
}
