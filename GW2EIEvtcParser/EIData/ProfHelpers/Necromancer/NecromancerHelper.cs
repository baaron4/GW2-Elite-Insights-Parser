using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class NecromancerHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(10574, 790, EIData.InstantCastFinder.DefaultICD), // Death shroud
            new BuffLossCastFinder(10585, 790, EIData.InstantCastFinder.DefaultICD), // Death shroud
            new DamageCastFinder(29560, 29560, EIData.InstantCastFinder.DefaultICD), // Spiteful Spirit
            new DamageCastFinder(13907, 13907, EIData.InstantCastFinder.DefaultICD), // Lesser Enfeeble
            new DamageCastFinder(13906, 13906, EIData.InstantCastFinder.DefaultICD), // Lesser Spinal Shivers
            new BuffGainCastFinder(10583, 10582, EIData.InstantCastFinder.DefaultICD, GW2Builds.December2018Balance, GW2Builds.EndOfLife), // Spectral Armor
            new BuffGainCastFinder(10685, 15083, EIData.InstantCastFinder.DefaultICD, 0, GW2Builds.December2018Balance), // Spectral Walk
            new BuffGainCastFinder(10685, 53476, EIData.InstantCastFinder.DefaultICD, GW2Builds.December2018Balance, GW2Builds.EndOfLife), // Spectral Walk
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Spite
            new BuffDamageModifierTarget(NumberOfBoonsID, "Spiteful Talisman", "10% on boonless target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByAbsence, "https://wiki.guildwars2.com/images/9/96/Spiteful_Talisman.png", DamageModifierMode.All),
            new BuffDamageModifier(770, "Death's Embrace", "25% on while downed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/5/5d/Death%27s_Embrace.png",0, GW2Builds.February2020Balance, DamageModifierMode.All),
            new BuffDamageModifier(770, "Death's Embrace", "25% on while downed", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/5/5d/Death%27s_Embrace.png",GW2Builds.February2020Balance, GW2Builds.EndOfLife, DamageModifierMode.PvE),
            new BuffDamageModifier(770, "Death's Embrace", "5% on while downed", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/5/5d/Death%27s_Embrace.png",GW2Builds.February2020Balance, GW2Builds.EndOfLife, DamageModifierMode.sPvPWvW),
            new BuffDamageModifierTarget(791, "Dread", "20% on feared target", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png", 0, GW2Builds.August2018Balance, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(791, "Dread", "33% on feared target", DamageSource.NoPets, 33.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png",GW2Builds.August2018Balance, GW2Builds.February2020Balance, DamageModifierMode.All),
            new BuffDamageModifierTarget(791, "Dread", "33% on feared target", DamageSource.NoPets, 33.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png",GW2Builds.February2020Balance, 104844, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(791, "Dread", "15% on feared target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/e/e2/Unholy_Fervor.png",GW2Builds.February2020Balance, 104844, DamageModifierMode.sPvPWvW),
            new DamageLogDamageModifier("Close to Death", "20% below 50% HP", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Necromancer,"https://wiki.guildwars2.com/images/b/b2/Close_to_Death.png", (x, log) => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
            // Soul Reaping
            new BuffDamageModifier(53489, "Soul Barbs", "10% after entering or exiting shroud", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Soul_Barbs.png", GW2Builds.December2018Balance, GW2Builds.May2021Balance, DamageModifierMode.All),
            new BuffDamageModifier(53489, "Soul Barbs", "10% after entering or exiting shroud", DamageSource.NoPets, 10.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Necromancer, ByPresence, "https://wiki.guildwars2.com/images/b/bd/Soul_Barbs.png", GW2Builds.May2021Balance, GW2Builds.EndOfLife, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {     
                //forms
                new Buff("Lich Form",10631, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/ab/Lich_Form.png"),
                new Buff("Death Shroud", 790, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/f/f5/Death_Shroud.png"),
                //signets
                new Buff("Signet of Vampirism",21761, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Buff("Vampiric Mark",21766, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Buff("Signet of Vampirism (Shroud)",43885, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/73/Signet_of_Vampirism.png"),
                new Buff("Plague Signet",10630, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c5/Plague_Signet.png"),
                new Buff("Plague Sending",46832, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7d/Plague_Sending.png"),
                new Buff("Plague Signet (Shroud)",44164, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/c5/Plague_Signet.png"),
                new Buff("Signet of Spite",10621, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/df/Signet_of_Spite.png"),
                new Buff("Signet of Spite (Shroud)",43772, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/df/Signet_of_Spite.png"),
                new Buff("Signet of the Locust",10614, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a3/Signet_of_the_Locust.png"),
                new Buff("Signet of the Locust (Shroud)",40283, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/a/a3/Signet_of_the_Locust.png"),
                new Buff("Signet of Undeath",10610, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/9c/Signet_of_Undeath.png"),
                new Buff("Signet of Undeath (Shroud)",40583, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/9c/Signet_of_Undeath.png"),
                //skills
                new Buff("Spectral Walk",15083, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/33/Spectral_Walk.png", 0, GW2Builds.December2018Balance),
                new Buff("Spectral Walk",53476, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/33/Spectral_Walk.png", GW2Builds.December2018Balance, GW2Builds.EndOfLife),
                new Buff("Spectral Armor",10582, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d1/Spectral_Armor.png"),
                new Buff("Locust Swarm", 10567, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/77/Locust_Swarm.png"),
                //new Boon("Sand Cascade", 43759, BoonSource.Necromancer, BoonType.Duration, 1, BoonNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1e/Sand_Cascade.png"),
                //traits
                new Buff("Corrupter's Defense",30845, Source.Necromancer, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/11/Corrupter%27s_Fervor.png", 0, GW2Builds.October2019Balance),
                new Buff("Death's Carapace",30845, Source.Necromancer, BuffStackType.Stacking, 30, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/58/Death%27s_Carapace.png", GW2Builds.October2019Balance, GW2Builds.EndOfLife),
                new Buff("Flesh of the Master",13810, Source.Necromancer, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e9/Flesh_of_the_Master.png", 0, GW2Builds.October2019Balance),
                new Buff("Vampiric Aura", 30285, Source.Necromancer, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/d/da/Vampiric_Presence.png"),
                new Buff("Vampiric Strikes", 30398, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/da/Vampiric_Presence.png"),
                new Buff("Last Rites",29726, Source.Necromancer, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/1/1a/Last_Rites_%28effect%29.png"),
                new Buff("Soul Barbs",53489, Source.Necromancer, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/bd/Soul_Barbs.png"),
        };

        private static readonly HashSet<long> _shroudTransform = new HashSet<long>
        {
            10574, 10585, 
            30792, 30961, 
            62567, 62540
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
            (int)MinionID.BoneMinion
        };
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }
    }
}
