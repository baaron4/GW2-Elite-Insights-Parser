using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class MechanistHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new EffectCastFinder(ShiftSignetSkill, EffectGUIDs.MechanistShiftSignet1)
                .UsingSrcSpecChecker(Spec.Mechanist),
            new MinionCastCastFinder(OverclockSignetSkill, JadeBusterCannon)
                .UsingDisableWithEffectData()
                .UsingNotAccurate(true),
            new EffectCastFinderByDst(OverclockSignetSkill, EffectGUIDs.MechanistOverclockSignet)
                .UsingDstSpecChecker(Spec.Mechanist),

            // Mech
            new DamageCastFinder(CrashDown, CrashDown)
                .UsingDisableWithEffectData(),
            new MinionSpawnCastFinder(CrashDown, (int)MinionID.JadeMech)
                .UsingChecker((spawn, combatData, agentData, skillData) =>
                {
                    // TODO: what if shift signet ports the minion away from effect before arc polls the position? so far unable to produce
                    Point3D pos = combatData.GetMovementData(spawn.Src).OfType<PositionEvent>().FirstOrDefault(evt => evt.Time + ServerDelayConstant >= spawn.Time)?.GetPoint3D();
                    if (pos != null && combatData.TryGetEffectEventsByGUID(EffectGUIDs.MechanistCrashDownImpact, out IReadOnlyList<EffectEvent> effects))
                    {
                        return CombatData.FindRelatedEvents(effects, spawn.Time + 800).Any(effect => pos.Distance2DToPoint(effect.Position) < 10.0f);
                    }
                    return false;
                }) // intersect first position after spawn with delayed effect
                .UsingNotAccurate(true),
            new MinionCastCastFinder(RoilingSmash, RoilingSmash),
            new MinionCastCastFinder(ExplosiveKnuckle, ExplosiveKnuckle),
            new MinionCastCastFinder(SparkRevolver, SparkRevolver),
            new BuffGainCastFinder(DischargeArray, DischargeArrayBuff).WithMinions(true),
            new EffectCastFinderByDst(CrisisZone, EffectGUIDs.MechanistCrisisZone)
                .WithMinions(true)
                .UsingSecondaryEffectChecker(EffectGUIDs.MechanistMechEyeGlow)
                .UsingChecker((effect, combatData, agentData, skillData) => effect.Dst.IsSpecies(MinionID.JadeMech)),
            new MinionCastCastFinder(CoreReactorShot, CoreReactorShot),
            new MinionCastCastFinder(JadeMortar, JadeMortar),
            new MinionCastCastFinder(BarrierBurst, BarrierBurst),
            new MinionCastCastFinder(SkyCircus, SkyCircus),
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            // Need to check mech specy id for those
            new BuffOnActorDamageModifier(ForceSignet, "Force Signet", "10%, including Mech", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Mechanist, ByPresence, BuffImages.ForceSignet, DamageModifierMode.All)
                .WithBuilds(GW2Builds.EODBeta4)
                .UsingChecker((x,log) =>
                {
                    return x.From == x.CreditedFrom || x.From.IsSpecies(MinionID.JadeMech);
                }),
            new BuffOnActorDamageModifier(SuperconductingSignet, "Superconducting Signet", "10%, including Mech", DamageSource.All, 10.0, DamageType.Condition, DamageType.All, Source.Mechanist, ByPresence, BuffImages.SuperconductingSignet, DamageModifierMode.All)
                .WithBuilds(GW2Builds.EODBeta4)
                .UsingChecker((x,log) =>
                {
                    return x.From == x.CreditedFrom || x.From.IsSpecies(MinionID.JadeMech);
                }),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(BarrierSignet, "Barrier Signet", "-10%", DamageSource.NoPets, -10, DamageType.StrikeAndCondition, DamageType.All, Source.Mechanist, ByPresence, BuffImages.BarrierSignet, DamageModifierMode.All),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Rectifier Signet", RectifierSignet, Source.Mechanist, BuffClassification.Other, BuffImages.RectifierSignet),
            new Buff("Barrier Signet", BarrierSignet, Source.Mechanist, BuffClassification.Other, BuffImages.BarrierSignet),
            new Buff("Force Signet", ForceSignet, Source.Mechanist, BuffClassification.Other, BuffImages.ForceSignet),
            new Buff("Shift Signet", ShiftSignetBuff, Source.Mechanist, BuffClassification.Other, BuffImages.ShiftSignet),
            new Buff("Superconducting Signet", SuperconductingSignet, Source.Mechanist, BuffClassification.Other, BuffImages.SuperconductingSignet),
            new Buff("Overclock Signet", OverclockSignetBuff, Source.Mechanist, BuffClassification.Other, BuffImages.OverclockSignet),
            new Buff("Mechanical Genius", MechanicalGenius, Source.Mechanist, BuffClassification.Other, BuffImages.MechanicalGenius),
            new Buff("Exigency Protocols", ExigencyProtocol, Source.PetSpecific, BuffClassification.Other, BuffImages.ExigencyProtocol),
            //
            //new Buff("Rectifier Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, BuffImages.RectifierSignet),
            new Buff("Barrier Signet (J-Drive)", BarrierSignetJDrive, Source.Mechanist, BuffClassification.Other, BuffImages.BarrierSignet),
            //new Buff("Force Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, BuffImages.ForceSignet),
            //new Buff("Shift Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, BuffImages.ShiftSignet),
            //new Buff("Superconducting Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, BuffImages.SuperconductingSignet),
            new Buff("Overclock Signet (J-Drive)", OverclockSignetJDrive, Source.Mechanist, BuffClassification.Other, BuffImages.OverclockSignet),
        };

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.JadeMech,
        };
        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }

    }
}
