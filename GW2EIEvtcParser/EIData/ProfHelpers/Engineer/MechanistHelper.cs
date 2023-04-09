using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class MechanistHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new EffectCastFinder(ShiftSignetSkill, EffectGUIDs.MechanistShiftSignet).UsingChecker((evt, combatData, agentData, skillData) => evt.Src.Spec == Spec.Mechanist),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Need to check mech specy id for those
            new BuffDamageModifier(ForceSignet, "Force Signet", "10%, including Mech", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Mechanist, ByPresence, BuffImages.ForceSignet, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta4).UsingChecker((x,log) =>
            {
                return x.From == x.CreditedFrom || x.From.IsSpecies(MinionID.JadeMech);
            }),
            new BuffDamageModifier(SuperconductingSignet, "Superconducting Signet", "10%, including Mech", DamageSource.All, 10.0, DamageType.Condition, DamageType.All, Source.Mechanist, ByPresence, BuffImages.SuperconductingSignet, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta4).UsingChecker((x,log) =>
            {
                return x.From == x.CreditedFrom || x.From.IsSpecies(MinionID.JadeMech);
            }),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Rectifier Signet", RectifierSignet, Source.Mechanist, BuffClassification.Other, BuffImages.RectifierSignet),
            new Buff("Barrier Signet", BarrierSignet, Source.Mechanist, BuffClassification.Other, BuffImages.BarrierSignet),
            new Buff("Force Signet", ForceSignet, Source.Mechanist, BuffClassification.Other, BuffImages.ForceSignet),
            new Buff("Shift Signet", ShiftSignetEffect, Source.Mechanist, BuffClassification.Other, BuffImages.ShiftSignet),
            new Buff("Superconducting Signet", SuperconductingSignet, Source.Mechanist, BuffClassification.Other, BuffImages.SuperconductingSignet),
            new Buff("Overclock Signet", OverclockSignet, Source.Mechanist, BuffClassification.Other, BuffImages.OverclockSignet),
            new Buff("Mechanical Genius", MechanicalGenius, Source.Mechanist, BuffClassification.Other, BuffImages.MechanicalGenius),
            //
            //new Buff("Rectifier Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, BuffImages.RectifierSignet),
            new Buff("Barrier Signet (J-Drive)", BarrierSignetJDrive, Source.Mechanist, BuffClassification.Other, BuffImages.BarrierSignet),
            //new Buff("Force Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, BuffImages.ForceSignet),
            //new Buff("Shift Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, BuffImages.ShiftSignet),
            //new Buff("Superconducting Signet (J-Drive)",-1, Source.Mechanist, BuffNature.GraphOnlyBuff, BuffImages.SuperconductingSignet),
            new Buff("Overclock Signet (J-Drive)", OverclockSignetJDrive, Source.Mechanist, BuffClassification.Other, BuffImages.OverclockSignet),
        };

        private static HashSet<long> Minions = new HashSet<long>()
        {
            (int)MinionID.JadeMech,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }

    }
}
