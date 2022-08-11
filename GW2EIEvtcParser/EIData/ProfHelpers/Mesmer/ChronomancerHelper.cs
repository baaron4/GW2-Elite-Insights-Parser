using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
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
            new EffectCastFinder(SplitSecond, EffectGUIDs.ChronomancerSplitSecond).UsingChecker((evt, combatData) => {
                if (evt.Src.Spec != Spec.Chronomancer)
                {
                    return false;
                }
                // Clones also trigger this effect but it is sourced to the master, we need to check that the effect happened at the same time as a shatter effect event
                EffectGUIDEvent shatterGUIDEvent = combatData.GetEffectGUIDEvent(EffectGUIDs.ChronomancerShatter);
                if (shatterGUIDEvent == null)
                {
                    return false;
                }
                var shatterEvents = combatData.GetEffectEvents(shatterGUIDEvent.ContentID);
                if  (shatterEvents.Any(x => x.Src == evt.Src && Math.Abs(x.Time - evt.Time) < ParserHelper.ServerDelayConstant && x.Position.Distance2DToPoint(evt.Position) < 0.1)) {
                    return true;
                }
                return false;
            }),
            new EffectCastFinder(Rewinder, EffectGUIDs.ChronomancerRewinder).UsingChecker((evt, combatData) => evt.Src.Spec == Spec.Chronomancer),
            new EffectCastFinder(TimeSink, EffectGUIDs.ChronomancerTimeSink).UsingChecker((evt, combatData) => evt.Src.Spec == Spec.Chronomancer),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(Slow, "Danger Time", "30% crit damage on slowed target", DamageSource.NoPets, 30.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(86181, GW2Builds.December2018Balance),
            new BuffDamageModifierTarget(Slow, "Danger Time", "30% crit damage on slowed target", DamageSource.All, 30.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.December2018Balance, GW2Builds.March2019Balance),
            new BuffDamageModifierTarget(Slow, "Danger Time", "10% crit damage on slowed target", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.March2019Balance, GW2Builds.May2021Balance),
            new BuffDamageModifier(Alacrity, "Improved Alacrity", "10% crit under alacrity", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Chronomancer, ByPresence, "https://wiki.guildwars2.com/images/e/e9/Improved_Alacrity.png", DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.August2022BalanceHotFix),
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
