using System.Collections.Generic;
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
            new BuffGainCastFinder(EnterDeathShroud, DeathShroud), // Death shroud
            new BuffLossCastFinder(ExitDeathShroud, DeathShroud), // Death shroud
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
            new BuffDamageModifierTarget(NumberOfBoons, "Spiteful Talisman", "10% on boonless target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByAbsence, "https://wiki.guildwars2.com/images/9/96/Spiteful_Talisman.png", DamageModifierMode.All),
            new BuffDamageModifier(Downed, "Death's Embrace", "25% on while downed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/5/5d/Death%27s_Embrace.png", DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
            new BuffDamageModifier(Downed, "Death's Embrace", "25% on while downed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/5/5d/Death%27s_Embrace.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance),
            new BuffDamageModifier(Downed, "Death's Embrace", "5% on while downed", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/5/5d/Death%27s_Embrace.png", DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance),
            new BuffDamageModifierTarget(Fear, "Dread", "20% on feared target", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2018Balance),
            new BuffDamageModifierTarget(Fear, "Dread", "33% on feared target", DamageSource.NoPets, 33.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png", DamageModifierMode.All).WithBuilds(GW2Builds.August2018Balance, GW2Builds.February2020Balance),
            new BuffDamageModifierTarget(Fear, "Dread", "33% on feared target", DamageSource.NoPets, 33.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance, 104844),
            new BuffDamageModifierTarget(Fear, "Dread", "15% on feared target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png", DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance, 104844),
            new DamageLogDamageModifier("Close to Death", "20% below 50% HP", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Necromancer,"https://wiki.guildwars2.com/images/b/b2/Close_to_Death.png", (x, log) => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
            // Soul Reaping
            new BuffDamageModifier(SoulBarbs, "Soul Barbs", "10% after entering or exiting shroud", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Soul_Barbs.png", DamageModifierMode.All).WithBuilds(GW2Builds.December2018Balance, GW2Builds.May2021Balance),
            new BuffDamageModifier(SoulBarbs, "Soul Barbs", "10% after entering or exiting shroud", DamageSource.NoPets, 10.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Soul_Barbs.png", DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance),
            new BuffDamageModifier(new long[] {DeathShroud, ReapersShroud, HarbingerShroud}, "Death Perception", "15% crit damage while in shroud", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Soul_Barbs.png", DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.June2022Balance), // no tracked for Scourge
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {     
                //forms
                new Buff("Lich Form",LichForm, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/ab/Lich_Form.png"),
                new Buff("Death Shroud", DeathShroud, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/f5/Death_Shroud.png"),
                //signets
                new Buff("Signet of Vampirism",SignetOfVampirism, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Buff("Vampiric Mark",VampiricMark, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Buff("Signet of Vampirism (Shroud)",SignetOfVampirismShroud, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Buff("Plague Signet",PlagueSignet, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c5/Plague_Signet.png"),
                new Buff("Plague Sending",PlagueSending, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Plague_Sending.png"),
                new Buff("Plague Signet (Shroud)",PlagueSignetShroud, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c5/Plague_Signet.png"),
                new Buff("Signet of Spite",SignetOfSpite, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/df/Signet_of_Spite.png"),
                new Buff("Signet of Spite (Shroud)",SignetOfSpiteShroud, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/df/Signet_of_Spite.png"),
                new Buff("Signet of the Locust",SignetOfTheLocust, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a3/Signet_of_the_Locust.png"),
                new Buff("Signet of the Locust (Shroud)",SignetOfTheLocustShroud, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a3/Signet_of_the_Locust.png"),
                new Buff("Signet of Undeath",SignetOfUndeath, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/9c/Signet_of_Undeath.png"),
                new Buff("Signet of Undeath (Shroud)",SignetOfUndeathShroud, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/9c/Signet_of_Undeath.png"),
                //skills
                new Buff("Spectral Walk",SpectralWalkEffectOld, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/33/Spectral_Walk.png", 0, GW2Builds.December2018Balance),
                new Buff("Spectral Walk",SpectralWalkEffect, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/33/Spectral_Walk.png", GW2Builds.December2018Balance, GW2Builds.EndOfLife),
                new Buff("Spectral Armor",SpectralArmorEffect, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d1/Spectral_Armor.png"),
                new Buff("Locust Swarm", LocustSwarm, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/77/Locust_Swarm.png"),
                //new Boon("Sand Cascade", 43759, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1e/Sand_Cascade.png"),
                //traits
                new Buff("Corrupter's Defense",CorruptersDefense, Source.Necromancer, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/11/Corrupter%27s_Fervor.png", 0, GW2Builds.October2019Balance),
                new Buff("Death's Carapace",DeathsCarapace, Source.Necromancer, BuffStackType.Stacking, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/58/Death%27s_Carapace.png", GW2Builds.October2019Balance, GW2Builds.EndOfLife),
                new Buff("Flesh of the Master",FleshOfTheMaster, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e9/Flesh_of_the_Master.png", 0, GW2Builds.October2019Balance),
                new Buff("Vampiric Aura", VampiricAura, Source.Necromancer, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/d/da/Vampiric_Presence.png"),
                new Buff("Vampiric Strikes", VampiricStrikes, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/da/Vampiric_Presence.png"),
                new Buff("Last Rites",LastRites, Source.Necromancer, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/1/1a/Last_Rites_%28effect%29.png"),
                new Buff("Soul Barbs",SoulBarbs, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/bd/Soul_Barbs.png"),
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
