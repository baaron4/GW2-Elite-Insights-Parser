using System.Collections.Generic;
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
            new BuffGainCastFinder(BoundingDodgerSkill, BoundingDodgerEffect), // Bounding Dodger
            new BuffGainCastFinder(LotusTrainingSkill, LotusTrainingEffect), // Lotus Training
            //new DamageCastFinder(30520, 30520), // Debilitating Arc
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(LotusTrainingEffect, "Lotus Training", "10% cDam (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/e/ea/Lotus_Training.png", DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
            new BuffDamageModifier(LotusTrainingEffect, "Lotus Training", "10% cDam (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/e/ea/Lotus_Training.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance, GW2Builds.June2021Balance),
            new BuffDamageModifier(LotusTrainingEffect, "Lotus Training", "15% cDam (4s) after dodging", DamageSource.NoPets, 15.0, DamageType.Condition, DamageType.All, Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/e/ea/Lotus_Training.png", DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance, GW2Builds.June2021Balance),
            new BuffDamageModifier(LotusTrainingEffect, "Lotus Training", "15% cDam (4s) after dodging", DamageSource.NoPets, 15.0, DamageType.Condition, DamageType.All, Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/e/ea/Lotus_Training.png", DamageModifierMode.All).WithBuilds(GW2Builds.June2021Balance),
            new BuffDamageModifier(BoundingDodgerEffect, "Bounding Dodger", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png", DamageModifierMode.PvE),
            new BuffDamageModifier(BoundingDodgerEffect, "Bounding Dodger", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png", DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
            new BuffDamageModifier(BoundingDodgerEffect, "Bounding Dodger", "15% (4s) after dodging", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png", DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance),
            new BuffDamageModifierTarget(Weakness, "Weakening Strikes", "7% if weakness on target", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/7/7c/Weakening_Strikes.png", DamageModifierMode.All).WithBuilds(GW2Builds.April2019Balance),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Palm Strike",PalmStrike, Source.Daredevil, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
                new Buff("Pulmonary Impact",PulmonaryImpact, Source.Daredevil, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
                new Buff("Lotus Training", LotusTrainingEffect, Source.Daredevil, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/ea/Lotus_Training.png"),
                new Buff("Unhindered Combatant", UnhinderedCombatant, Source.Daredevil, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a1/Unhindered_Combatant.png"),
                new Buff("Bounding Dodger", BoundingDodgerEffect, Source.Daredevil, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png"),
                new Buff("Weakening Strikes", WeakeningStrikes, Source.Daredevil, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7c/Weakening_Strikes.png", GW2Builds.April2019Balance, GW2Builds.EndOfLife),
        };

        private static HashSet<long> Minions = new HashSet<long>()
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
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }

    }
}
