using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class DeadeyeHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new EffectCastFinderByDst(Mercy, EffectGUIDs.DeadeyeMercy).UsingChecker((evt, combatData, agentData, skillData) => evt.Dst.Spec == Spec.Deadeye), // Needs more testing to check for collisions
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(NumberOfBoons, "Premeditation", "1% per boon",DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByStack, BuffImages.Premeditation, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
            new BuffOnActorDamageModifier(NumberOfBoons, "Premeditation", "1% per boon",DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByStack, BuffImages.Premeditation, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.August2022Balance, GW2Builds.July2023BalanceAndSilentSurfCM),
            new BuffOnActorDamageModifier(NumberOfBoons, "Premeditation", "1.5% per boon",DamageSource.NoPets, 1.5, DamageType.Strike, DamageType.All, Source.Deadeye, ByStack, BuffImages.Premeditation, DamageModifierMode.PvE).WithBuilds(GW2Builds.August2022Balance, GW2Builds.July2023BalanceAndSilentSurfCM),
            new BuffOnActorDamageModifier(NumberOfBoons, "Premeditation", "1% per boon",DamageSource.NoPets, 1.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByStack, BuffImages.Premeditation, DamageModifierMode.All).WithBuilds( GW2Builds.July2023BalanceAndSilentSurfCM),
            //
            new BuffOnActorDamageModifier(DeadeyesGaze, "Iron Sight", "10% to marked target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, BuffImages.IronSight, DamageModifierMode.All).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(DeadeyesGaze).Where(y => y is BuffApplyEvent && y.To == src).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
                }
                return false;
            }).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
            new BuffOnActorDamageModifier(DeadeyesGaze, "Iron Sight", "10% to marked target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, BuffImages.IronSight, DamageModifierMode.sPvPWvW).UsingChecker((x, log) => {
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(DeadeyesGaze).Where(y => y is BuffApplyEvent && y.To == x.From).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
                }
                return false;
            }).WithBuilds(GW2Builds.August2022Balance, GW2Builds.July2023BalanceAndSilentSurfCM),
            new BuffOnActorDamageModifier(DeadeyesGaze, "Iron Sight", "15% to marked target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, BuffImages.IronSight, DamageModifierMode.PvE).UsingChecker((x, log) => {
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(DeadeyesGaze).Where(y => y is BuffApplyEvent && y.To == x.From).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
                }
                return false;
            }).WithBuilds(GW2Builds.August2022Balance, GW2Builds.July2023BalanceAndSilentSurfCM),

            new BuffOnActorDamageModifier(DeadeyesGaze, "Iron Sight", "10% to marked target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, BuffImages.IronSight, DamageModifierMode.All).UsingChecker((x, log) => {
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(DeadeyesGaze).Where(y => y is BuffApplyEvent && y.To == x.From).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.To == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
                }
                return false;
            }).WithBuilds(GW2Builds.July2023BalanceAndSilentSurfCM),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(DeadeyesGaze, "Iron Sight", "-10% from marked target", DamageSource.NoPets, -10.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, BuffImages.IronSight, DamageModifierMode.All).UsingChecker((x, log) => {
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(DeadeyesGaze).Where(y => y is BuffApplyEvent && y.To == x.To).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.From == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
                }
                return false;
            }).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
            new BuffOnActorDamageModifier(DeadeyesGaze, "Iron Sight", "-10% from marked target", DamageSource.NoPets, -10.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, BuffImages.IronSight, DamageModifierMode.sPvPWvW).UsingChecker((x, log) => {
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(DeadeyesGaze).Where(y => y is BuffApplyEvent && y.To == x.To).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.From == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
                }
                return false;
            }).WithBuilds(GW2Builds.August2022Balance),
            new BuffOnActorDamageModifier(DeadeyesGaze, "Iron Sight", "-15% from marked target", DamageSource.NoPets, -15.0, DamageType.Strike, DamageType.All, Source.Deadeye, ByPresence, BuffImages.IronSight, DamageModifierMode.PvE).UsingChecker((x, log) => {
                AbstractBuffEvent effectApply = log.CombatData.GetBuffData(DeadeyesGaze).Where(y => y is BuffApplyEvent && y.To == x.To).LastOrDefault(y => y.Time <= x.Time);
                if (effectApply != null)
                {
                    return x.From == effectApply.By.GetMainAgentWhenAttackTarget(log, x.Time);
                }
                return false;
            }).WithBuilds(GW2Builds.August2022Balance),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Kneeling", Kneeling, Source.Deadeye, BuffClassification.Other, BuffImages.Kneel).WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOBetaAndSilentSurfNM),
            new Buff("Deadeye's Gaze", DeadeyesGaze, Source.Deadeye, BuffClassification.Other, BuffImages.DeadeyesMark),
        };

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.Deadeye1,
            (int)MinionID.Deadeye2,
            (int)MinionID.Deadeye3,
            (int)MinionID.Deadeye4,
            (int)MinionID.Deadeye5,
            (int)MinionID.Deadeye6,
            (int)MinionID.Deadeye7,
            (int)MinionID.Deadeye8,
            (int)MinionID.Deadeye9,
            (int)MinionID.Deadeye10,
        };
        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }

    }
}
