using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ChronomancerHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(ContinuumSplit, TimeAnchored), // Continuum Split
            new BuffLossCastFinder(ContinuumShift, TimeAnchored), // Continuum Shift
            new EffectCastFinder(SplitSecond, EffectGUIDs.ChronomancerSplitSecond),
            new EffectCastFinder(Rewinder, EffectGUIDs.ChronomancerRewinder),
            new EffectCastFinder(TimeSink, EffectGUIDs.ChronomancerTimeSink),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(Slow, "Danger Time", "30% crit damage on slowed target", DamageSource.NoPets, 30.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", 86181, GW2Builds.December2018Balance, DamageModifierMode.All, ((x, log) => x.HasCrit)),
            new BuffDamageModifierTarget(Slow, "Danger Time", "30% crit damage on slowed target", DamageSource.All, 30.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", GW2Builds.December2018Balance, GW2Builds.March2019Balance, DamageModifierMode.All, ((x, log) => x.HasCrit)),
            new BuffDamageModifierTarget(Slow, "Danger Time", "10% crit damage on slowed target", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", GW2Builds.March2019Balance, GW2Builds.May2021Balance, DamageModifierMode.All, ((x, log) => x.HasCrit)),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Time Echo",TimeEcho, Source.Chronomancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/8d/Deja_Vu.png"),
                new Buff("Time Anchored",TimeAnchored, Source.Chronomancer, BuffStackType.Queue, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/db/Continuum_Split.png"),
        };

        private static HashSet<long> NonCloneMinions = new HashSet<long>()
        {
            (int)MinionID.IllusionaryAvenger,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return NonCloneMinions.Contains(id);
        }

    }
}
