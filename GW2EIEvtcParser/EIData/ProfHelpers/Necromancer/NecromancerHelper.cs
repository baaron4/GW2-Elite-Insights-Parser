using System.Collections.Generic;
using GW2EIEvtcParser.EIData.Buffs;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class NecromancerHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(EnterDeathShroud, DeathShroud).UsingBeforeWeaponSwap(true), // Death shroud
            new BuffLossCastFinder(ExitDeathShroud, DeathShroud).UsingBeforeWeaponSwap(true), // Death shroud
            new DamageCastFinder(SpitefulSpirit, SpitefulSpirit), // Spiteful Spirit
            new DamageCastFinder(LesserEnfeeble, LesserEnfeeble), // Lesser Enfeeble
            new DamageCastFinder(LesserSpinalShivers, LesserSpinalShivers), // Lesser Spinal Shivers
            new BuffGainCastFinder(SpectralArmorSkill, SpectralArmorEffect).WithBuilds(GW2Builds.December2018Balance), // Spectral Armor
            new BuffGainCastFinder(SpectralWalkSkill, SpectralWalkEffectOld).WithBuilds(GW2Builds.StartOfLife, GW2Builds.December2018Balance), // Spectral Walk
            new BuffGainCastFinder(SpectralWalkSkill, SpectralWalkEffect).WithBuilds(GW2Builds.December2018Balance), // Spectral Walk
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Spite
            new BuffDamageModifierTarget(NumberOfBoons, "Spiteful Talisman", "10% on boonless target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByAbsence, BuffImages.SpitefulTalisman, DamageModifierMode.All),
            new BuffDamageModifier(Downed, "Death's Embrace", "25% on while downed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.DeathsEmbrace, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
            new BuffDamageModifier(Downed, "Death's Embrace", "25% on while downed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.DeathsEmbrace, DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance),
            new BuffDamageModifier(Downed, "Death's Embrace", "5% on while downed", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.DeathsEmbrace, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance),
            new BuffDamageModifierTarget(Fear, "Dread", "20% on feared target", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.UnholyFervor, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2018Balance),
            new BuffDamageModifierTarget(Fear, "Dread", "33% on feared target", DamageSource.NoPets, 33.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.UnholyFervor, DamageModifierMode.All).WithBuilds(GW2Builds.August2018Balance, GW2Builds.February2020Balance),
            new BuffDamageModifierTarget(Fear, "Dread", "33% on feared target", DamageSource.NoPets, 33.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.UnholyFervor, DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance, GW2Builds.July2020Balance),
            new BuffDamageModifierTarget(Fear, "Dread", "15% on feared target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.UnholyFervor, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance, GW2Builds.July2020Balance),
            new DamageLogDamageModifier("Close to Death", "20% below 50% HP", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Necromancer, BuffImages.CloseToDeath, (x, log) => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
            // Soul Reaping
            new BuffDamageModifier(SoulBarbs, "Soul Barbs", "10% after entering or exiting shroud", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.SoulBarbs, DamageModifierMode.All).WithBuilds(GW2Builds.December2018Balance, GW2Builds.May2021Balance),
            new BuffDamageModifier(SoulBarbs, "Soul Barbs", "10% after entering or exiting shroud", DamageSource.NoPets, 10.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Necromancer, ByPresence, BuffImages.SoulBarbs, DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance),
            new BuffDamageModifier(new long[] {DeathShroud, ReapersShroud, HarbingerShroud}, "Death Perception", "15% crit damage while in shroud", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, BuffImages.SoulBarbs, DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.June2022Balance), // no tracked for Scourge
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {     
            // Forms
            new Buff("Lich Form", LichForm, Source.Necromancer, BuffClassification.Other, BuffImages.LichForm),
            new Buff("Death Shroud", DeathShroud, Source.Necromancer, BuffClassification.Other, BuffImages.DeathShroud),
            // Signets
            new Buff("Signet of Vampirism", SignetOfVampirism, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfVampirism),
            new Buff("Vampiric Mark", VampiricMark, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.SignetOfVampirism),
            new Buff("Signet of Vampirism (Shroud)", SignetOfVampirismShroud, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfVampirism),
            new Buff("Plague Signet", PlagueSignet, Source.Necromancer, BuffClassification.Other, BuffImages.PlagueSignet),
            new Buff("Plague Sending", PlagueSending, Source.Necromancer, BuffClassification.Other, BuffImages.PlagueSending),
            new Buff("Plague Signet (Shroud)", PlagueSignetShroud, Source.Necromancer, BuffClassification.Other, BuffImages.PlagueSignet),
            new Buff("Signet of Spite", SignetOfSpite, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfSpite),
            new Buff("Signet of Spite (Shroud)", SignetOfSpiteShroud, Source.Necromancer, BuffClassification.Other,BuffImages.SignetOfSpite),
            new Buff("Signet of the Locust", SignetOfTheLocust, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfTheLocust),
            new Buff("Signet of the Locust (Shroud)", SignetOfTheLocustShroud, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfTheLocust),
            new Buff("Signet of Undeath", SignetOfUndeath, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfUndeath),
            new Buff("Signet of Undeath (Shroud)", SignetOfUndeathShroud, Source.Necromancer, BuffClassification.Other, BuffImages.SignetOfUndeath),
            // Skills
            new Buff("Spectral Walk", SpectralWalkEffectOld, Source.Necromancer, BuffClassification.Other, BuffImages.SpectralWalk, GW2Builds.StartOfLife, GW2Builds.December2018Balance),
            new Buff("Spectral Walk", SpectralWalkEffect, Source.Necromancer, BuffClassification.Other, BuffImages.SpectralWalk, GW2Builds.December2018Balance, GW2Builds.EndOfLife),
            new Buff("Spectral Armor", SpectralArmorEffect, Source.Necromancer, BuffClassification.Other, BuffImages.SpectralArmor),
            new Buff("Locust Swarm", LocustSwarm, Source.Necromancer, BuffClassification.Other, BuffImages.LocustSwarm),
            //new Boon("Sand Cascade", 43759, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, BuffImages.SandCascade),
            // Traits
            new Buff("Corrupter's Defense", CorruptersDefense, Source.Necromancer, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.CorruptersFervor, GW2Builds.StartOfLife, GW2Builds.October2019Balance),
            new Buff("Death's Carapace", DeathsCarapace, Source.Necromancer, BuffStackType.Stacking, 30, BuffClassification.Other, BuffImages.DeathsCarapace, GW2Builds.October2019Balance, GW2Builds.EndOfLife),
            new Buff("Flesh of the Master", FleshOfTheMaster, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.FleshOfTheMaster, GW2Builds.StartOfLife, GW2Builds.October2019Balance),
            new Buff("Vampiric Aura", VampiricAura, Source.Necromancer, BuffClassification.Defensive, BuffImages.VampiricPresence),
            new Buff("Vampiric Strikes", VampiricStrikes, Source.Necromancer, BuffClassification.Other, BuffImages.VampiricPresence),
            new Buff("Last Rites", LastRites, Source.Necromancer, BuffClassification.Defensive, BuffImages.LastRites),
            new Buff("Soul Barbs", SoulBarbs, Source.Necromancer, BuffClassification.Other, BuffImages.SoulBarbs),
        };

        private static readonly HashSet<long> _shroudTransform = new HashSet<long>
        {
            EnterDeathShroud, ExitDeathShroud,
            EnterReaperShroud, ExitReaperShroud, 
            EnterHarbingerShroud, ExitHarbingerShroud
        };

        public static bool IsShroudTransform(long id)
        {
            return _shroudTransform.Contains(id);
        }

        private static HashSet<long> Minions = new HashSet<long>()
        {
            (int)MinionID.BloodFiend,
            (int)MinionID.FleshGolem,
            (int)MinionID.ShadowFiend,
            (int)MinionID.FleshWurm,
            (int)MinionID.BoneFiend,
            (int)MinionID.BoneMinion,
            (int)MinionID.UnstableHorror,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }
    }
}
