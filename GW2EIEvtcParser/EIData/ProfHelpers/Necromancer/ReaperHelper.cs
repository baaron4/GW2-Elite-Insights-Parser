using System.Collections.Generic;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ReaperHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(EnterReaperShroud, ReapersShroud).UsingBeforeWeaponSwap(true),
            new BuffLossCastFinder(ExitReaperShroud, ReapersShroud).UsingBeforeWeaponSwap(true),
            new BuffGainCastFinder(InfusingTerrorSkill, InfusingTerrorBuff),
            new DamageCastFinder(YouAreAllWeaklings, YouAreAllWeaklings).UsingDisableWithEffectData(),
            new EffectCastFinder(YouAreAllWeaklings, EffectGUIDs.ReaperYouAreAllWeaklings1)
                .UsingSrcSpecChecker(Spec.Reaper)
                .UsingSecondaryEffectChecker(EffectGUIDs.ReaperYouAreAllWeaklings2)
                .UsingSecondaryEffectChecker(EffectGUIDs.ReaperYouAreAllWeaklings3),
            new DamageCastFinder(Suffer, Suffer).UsingDisableWithEffectData(),
            new EffectCastFinder(Suffer, EffectGUIDs.ReaperSuffer).UsingSrcSpecChecker(Spec.Reaper),
            // new BuffGainCastFinder(Rise, DarkBond).UsingICD(500), // buff reapplied on every minion attack
            new MinionSpawnCastFinder(Rise, (int)MinionID.ShamblingHorror)
                .UsingChecker((evt, combatData, agentData, skillData) => evt.Src.GetFinalMaster().Spec == Spec.Reaper),
            new DamageCastFinder(ChillingNova, ChillingNova).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnFoeDamageModifier(Chilled, "Cold Shoulder", "15% on chilled target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Reaper, ByPresence, BuffImages.ColdShoulder, DamageModifierMode.PvE).WithBuilds(GW2Builds.March2019Balance),
            new BuffOnFoeDamageModifier(Chilled, "Cold Shoulder", "10% on chilled target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Reaper, ByPresence, BuffImages.ColdShoulder, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.March2019Balance),
            new BuffOnFoeDamageModifier(Chilled, "Cold Shoulder", "10% on chilled target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Reaper, ByPresence, BuffImages.ColdShoulder, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.March2019Balance),
            new DamageLogDamageModifier("Soul Eater", "10% to foes within 300 range", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Reaper, BuffImages.SoulEater, (x,log) =>
            {
                Point3D currentPosition = x.From.GetCurrentPosition(log, x.Time);
                Point3D currentTargetPosition = x.To.GetCurrentPosition(log, x.Time);
                if (currentPosition == null || currentTargetPosition == null)
                {
                    return false;
                }
                return currentPosition.DistanceToPoint(currentTargetPosition) <= 300.0;
            }, DamageModifierMode.All).UsingApproximate(true).WithBuilds(GW2Builds.July2019Balance)
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(ReapersShroud, "Reaper's Shroud", "-33%", DamageSource.NoPets, -33, DamageType.StrikeAndCondition, DamageType.All, Source.Reaper, ByPresence, BuffImages.ReapersShroud, DamageModifierMode.PvE),
            new BuffOnActorDamageModifier(ReapersShroud, "Reaper's Shroud", "-50%", DamageSource.NoPets, -50, DamageType.StrikeAndCondition, DamageType.All, Source.Reaper, ByPresence, BuffImages.ReapersShroud, DamageModifierMode.sPvPWvW),
            new BuffOnActorDamageModifier(InfusingTerrorBuff, "Infusing Terror", "-20%", DamageSource.NoPets, -20, DamageType.StrikeAndCondition, DamageType.All, Source.Reaper, ByPresence, BuffImages.InfusingTerror, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2023Balance),
            new BuffOnActorDamageModifier(InfusingTerrorBuff, "Infusing Terror", "-20%", DamageSource.NoPets, -20, DamageType.StrikeAndCondition, DamageType.All, Source.Reaper, ByPresence, BuffImages.InfusingTerror, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.May2023Balance),
            new BuffOnActorDamageModifier(InfusingTerrorBuff, "Infusing Terror", "-66%", DamageSource.NoPets, -66, DamageType.StrikeAndCondition, DamageType.All, Source.Reaper, ByPresence, BuffImages.InfusingTerror, DamageModifierMode.PvE).WithBuilds(GW2Builds.May2023Balance), 
            // Rise is unclear, I don't see any inc damage reducing fact for Dark Bond
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Reaper's Shroud", ReapersShroud, Source.Reaper, BuffClassification.Other, BuffImages.ReapersShroud),
            new Buff("Infusing Terror", InfusingTerrorBuff, Source.Reaper, BuffClassification.Other, BuffImages.InfusingTerror),
            new Buff("Dark Bond", DarkBond, Source.Reaper, BuffClassification.Other, BuffImages.Rise),
            new Buff("Reaper's Frost (Chilled to the Bone!)", ReapersFrostChilledToTheBone, Source.Reaper, BuffClassification.Other, BuffImages.ChilledToTheBone),
            new Buff("Reaper's Frost (Executioner's Scythe)", ReapersFrostExecutionersScythe, Source.Reaper, BuffClassification.Other, BuffImages.ChilledToTheBone),
        };

        private static readonly HashSet<long> _reaperShroudTransform = new HashSet<long>
        {
            EnterReaperShroud, ExitReaperShroud,
        };

        public static bool IsReaperShroudTransform(long id)
        {
            return _reaperShroudTransform.Contains(id);
        }

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.ShamblingHorror,
        };
        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }
    }
}
