using System.Collections.Generic;
using GW2EIEvtcParser.EIData.Buffs;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class DaredevilHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(Bound, BoundingDodger), 
            new BuffGainCastFinder(ImpalingLotus, LotusTraining),
            new BuffGainCastFinder(Dash, UnhinderedCombatant),
            //new DamageCastFinder(30520, 30520), // Debilitating Arc
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(LotusTraining, "Lotus Training", "10% cDam (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Daredevil, ByPresence, BuffImages.LotusTraining, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
            new BuffDamageModifier(LotusTraining, "Lotus Training", "10% cDam (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Daredevil, ByPresence, BuffImages.LotusTraining, DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance, GW2Builds.June2021Balance),
            new BuffDamageModifier(LotusTraining, "Lotus Training", "15% cDam (4s) after dodging", DamageSource.NoPets, 15.0, DamageType.Condition, DamageType.All, Source.Daredevil, ByPresence, BuffImages.LotusTraining, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance, GW2Builds.June2021Balance),
            new BuffDamageModifier(LotusTraining, "Lotus Training", "15% cDam (4s) after dodging", DamageSource.NoPets, 15.0, DamageType.Condition, DamageType.All, Source.Daredevil, ByPresence, BuffImages.LotusTraining, DamageModifierMode.All).WithBuilds(GW2Builds.June2021Balance),
            new BuffDamageModifier(BoundingDodger, "Bounding Dodger", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, BuffImages.BoundingDodger, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
            new BuffDamageModifier(BoundingDodger, "Bounding Dodger", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, BuffImages.BoundingDodger, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
            new BuffDamageModifier(BoundingDodger, "Bounding Dodger", "15% (4s) after dodging", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, BuffImages.BoundingDodger, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance, GW2Builds.August2022Balance),
            new BuffDamageModifier(BoundingDodger, "Bounding Dodger", "15% (4s) after dodging", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, BuffImages.BoundingDodger, DamageModifierMode.All).WithBuilds(GW2Builds.August2022Balance),
            new BuffDamageModifierTarget(Weakness, "Weakening Strikes", "7% if weakness on target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, BuffImages.WeakeningStrikes, DamageModifierMode.All).WithBuilds(GW2Builds.April2019Balance, GW2Builds.August2022Balance),
            new BuffDamageModifierTarget(Weakness, "Weakening Strikes", "7% if weakness on target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, BuffImages.WeakeningStrikes, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.August2022Balance),
            new BuffDamageModifierTarget(Weakness, "Weakening Strikes", "10% if weakness on target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, BuffImages.WeakeningStrikes, DamageModifierMode.PvE).WithBuilds(GW2Builds.August2022Balance),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Palm Strike", PalmStrike, Source.Daredevil, BuffClassification.Other, BuffImages.PalmStrike),
            new Buff("Pulmonary Impact", PulmonaryImpactBuff, Source.Daredevil, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.PalmStrike),
            new Buff("Lotus Training", LotusTraining, Source.Daredevil, BuffClassification.Other, BuffImages.LotusTraining),
            new Buff("Unhindered Combatant", UnhinderedCombatant, Source.Daredevil, BuffClassification.Other, BuffImages.UnhinderedCombatant),
            new Buff("Bounding Dodger", BoundingDodger, Source.Daredevil, BuffClassification.Other, BuffImages.BoundingDodger),
            new Buff("Weakening Strikes", WeakeningStrikes, Source.Daredevil, BuffClassification.Other, BuffImages.WeakeningStrikes).WithBuilds(GW2Builds.April2019Balance, GW2Builds.EndOfLife),
        };

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.Daredevil1,
            (int)MinionID.Daredevil2,
            (int)MinionID.Daredevil3,
            (int)MinionID.Daredevil4,
            (int)MinionID.Daredevil5,
            (int)MinionID.Daredevil6,
            (int)MinionID.Daredevil7,
            (int)MinionID.Daredevil8,
            (int)MinionID.Daredevil9,
            (int)MinionID.Daredevil10,
        };
        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }

    }
}
