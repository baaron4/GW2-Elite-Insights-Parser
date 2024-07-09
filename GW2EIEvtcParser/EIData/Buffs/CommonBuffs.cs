using System.Collections.Generic;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class CommonBuffs
    {

        internal static readonly List<Buff> Boons = new List<Buff>
        {
            new Buff("Might", Might, Source.Common, BuffStackType.Stacking, 25, BuffClassification.Boon, BuffImages.Might),
            new Buff("Fury", Fury, Source.Common, BuffStackType.Queue, 9, BuffClassification.Boon, BuffImages.Fury),
            new Buff("Quickness", Quickness, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, BuffImages.Quickness),
            new Buff("Alacrity", Alacrity, Source.Common, BuffStackType.Queue, 9, BuffClassification.Boon, BuffImages.Alacrity),
            new Buff("Protection", Protection, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, BuffImages.Protection),
            new Buff("Regeneration", Regeneration, Source.Common, BuffStackType.Regeneration, 5, BuffClassification.Boon, BuffImages.Regeneration),
            new Buff("Vigor", Vigor, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, BuffImages.Vigor),
            new Buff("Aegis", Aegis, Source.Common, BuffStackType.Queue, 9, BuffClassification.Boon, BuffImages.Aegis),
            new Buff("Stability", Stability, Source.Common, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Boon, BuffImages.Stability),
            new Buff("Swiftness", Swiftness, Source.Common, BuffStackType.Queue, 9, BuffClassification.Boon, BuffImages.Swiftness),
            new Buff("Retaliation", Retaliation, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, BuffImages.Retaliation).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new Buff("Resistance", Resistance, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, BuffImages.Resistance),
            new Buff("Resolution", Resolution, Source.Common, BuffStackType.Queue, 5, BuffClassification.Boon, BuffImages.Resolution).WithBuilds(GW2Builds.May2021Balance),
            //
            new Buff("Number of Boons", NumberOfBoons, Source.Common, BuffStackType.Stacking, 0, BuffClassification.Other, BuffImages.BoonDuration),
        };

        internal static readonly List<Buff> Conditions = new List<Buff>
        {
            new Buff("Bleeding", Bleeding, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, BuffImages.Bleeding),
            new Buff("Burning", Burning, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, BuffImages.Burning),
            new Buff("Confusion", Confusion, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, BuffImages.Confusion),
            new Buff("Poison", Poison, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, BuffImages.Poison),
            new Buff("Torment", Torment, Source.Common, BuffStackType.Stacking, 1500, BuffClassification.Condition, BuffImages.Torment),
            new Buff("Blind", Blind, Source.Common, BuffStackType.Queue, 9, BuffClassification.Condition, BuffImages.Blind),
            new Buff("Chilled", Chilled, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, BuffImages.Chilled),
            new Buff("Crippled", Crippled, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, BuffImages.Crippled),
            new Buff("Fear", Fear, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, BuffImages.Fear),
            new Buff("Immobile", Immobile, Source.Common, BuffStackType.Queue, 3, BuffClassification.Condition, BuffImages.Immobile),
            new Buff("Slow", Slow, Source.Common, BuffStackType.Queue, 9, BuffClassification.Condition, BuffImages.Slow),
            new Buff("Weakness", Weakness, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, BuffImages.Weakness),
            new Buff("Taunt", Taunt, Source.Common, BuffStackType.Queue, 5, BuffClassification.Condition, BuffImages.Taunt),
            new Buff("Vulnerability", Vulnerability, Source.Common, BuffStackType.Stacking, 25, BuffClassification.Condition, BuffImages.Vulnerability),
            //
            new Buff("Number of Conditions", NumberOfConditions, Source.Common, BuffStackType.Stacking, 0, BuffClassification.Other, BuffImages.ConditionDuration),
        };

        internal static readonly List<Buff> Commons = new List<Buff>
        {
            new Buff("Number of Active Combat Minions", NumberOfActiveCombatMinions, Source.Common, BuffStackType.Stacking, 99, BuffClassification.Other, BuffImages.RuneOfRanger),
            new Buff("Number of Clones", NumberOfClones, Source.Common, BuffStackType.Stacking, 99, BuffClassification.Other, BuffImages.RuneOfMesmer),
            new Buff("Number of Ranger Pets", NumberOfRangerPets, Source.Common, BuffStackType.Stacking, 99, BuffClassification.Other, BuffImages.RangerPets),
            new Buff("Downed", Downed, Source.Common, BuffClassification.Other, BuffImages.Downed),
            new Buff("Downed (Pet)", DownedPet, Source.Common, BuffClassification.Other, BuffImages.HealingPowerAttribute),
            new Buff("Exhaustion", Exhaustion, Source.Common, BuffStackType.Queue, 3, BuffClassification.Debuff, BuffImages.Exhaustion),
            new Buff("Stealth", Stealth, Source.Common, BuffStackType.Queue, 5, BuffClassification.Support, BuffImages.Stealth),
            new Buff("Hide in Shadows", HideInShadows, Source.Common, BuffStackType.Queue, 25, BuffClassification.Support, BuffImages.Stealth),
            new Buff("Revealed", Revealed, Source.Common, BuffClassification.Support, BuffImages.Revealed),
            new Buff("Superspeed", Superspeed, Source.Common, BuffClassification.Support, BuffImages.Superspeed)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.June2021Balance),
            new Buff("Superspeed", Superspeed, Source.Common, BuffStackType.Queue, 9, BuffClassification.Support, BuffImages.Superspeed)
                .WithBuilds(GW2Builds.June2021Balance),
            new Buff("Determined (762)", Determined762, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Determined (785)", Determined785, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Determined (788)", Determined788, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Resurrection", Resurrection, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Determined (895)", Determined895, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Determined (3892)", Determined3892, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Determined (31450)", Determined31450, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Determined (52271)", Determined52271, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Invulnerability (757)", Invulnerability757, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Invulnerability (56227)", Invulnerability56227, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Invulnerability (801)", Invulnerability801, Source.Common, BuffStackType.Queue, 25, BuffClassification.Other, BuffImages.Determined),
            new Buff("Spawn Protection?", SpawnProtection, Source.Common, BuffClassification.Other, BuffImages.Determined),
            new Buff("Stun", Stun, Source.Common, BuffClassification.Other, BuffImages.Stun),
            new Buff("Daze", Daze, Source.Common, BuffClassification.Other, BuffImages.Daze),
            new Buff("Exposed (48209)", Exposed48209, Source.Common, BuffClassification.Other, BuffImages.Exposed),
            new Buff("Exposed (31589)", Exposed31589, Source.Common, BuffClassification.Other, BuffImages.Exposed),
            new Buff("Old Exposed", OldExposed, Source.Common, BuffClassification.Other, BuffImages.Exposed),
            new Buff("Unblockable", Unblockable, Source.Common, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Offensive, BuffImages.Unblockable)
                .WithBuilds(GW2Builds.February2020Balance),
            new Buff("Encumbered", Encumbered, Source.Common, BuffStackType.Queue, 9, BuffClassification.Debuff, BuffImages.Encumbered),
            new Buff("Celeritas Spores", CeleritasSpores, Source.FightSpecific, BuffClassification.Other, BuffImages.SpeedMushroom),
            new Buff("Branded Accumulation", BrandedAccumulation, Source.Common, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.AchillesBane),
            new Buff("Revive Sickness", ReviveSickness, Source.Common, BuffClassification.Other, BuffImages.ReviveSickness),
            new Buff("Portal Interact Cooldown", PortalInteractCooldown, Source.Common, BuffClassification.Other, BuffImages.CooldownNegative),
            // Auras
            new Buff("Chaos Aura", ChaosAura, Source.Common, BuffClassification.Support, BuffImages.ChaosAura),
            new Buff("Fire Aura", FireAura, Source.Common, BuffClassification.Support, BuffImages.FireAura),
            new Buff("Frost Aura", FrostAura, Source.Common, BuffClassification.Support, BuffImages.FrostAura),
            new Buff("Light Aura", LightAura, Source.Common, BuffClassification.Support, BuffImages.LightAura),
            new Buff("Magnetic Aura", MagneticAura, Source.Common, BuffClassification.Support, BuffImages.MagneticAura),
            new Buff("Shocking Aura", ShockingAura, Source.Common, BuffClassification.Support, BuffImages.ShockingAura),
            new Buff("Dark Aura", DarkAura, Source.Common, BuffClassification.Support, BuffImages.DarkAura)
                .WithBuilds(GW2Builds.April2019Balance),
            // Race
            new Buff("Take Root", TakeRootBufft, Source.Common, BuffClassification.Other, BuffImages.TakeRoot),
            new Buff("Become the Bear", BecomeTheBear, Source.Common, BuffClassification.Other, BuffImages.BecomeBear),
            new Buff("Become the Raven", BecomeTheRaven, Source.Common, BuffClassification.Other, BuffImages.BecomeRaven),
            new Buff("Become the Snow Leopard", BecomeTheSnowLeopard, Source.Common, BuffClassification.Other, BuffImages.BecomeLeopard),
            new Buff("Become the Wolf", BecomeTheWolf, Source.Common, BuffClassification.Other, BuffImages.BecomeWolf),
            new Buff("Avatar of Melandru", AvatarOfMelandru, Source.Common, BuffClassification.Other, BuffImages.AvatarOfMelandru),
            new Buff("Power Suit", PowerSuit, Source.Common, BuffClassification.Other, BuffImages.PowerSuit),
            new Buff("Reaper of Grenth", ReaperOfGrenth, Source.Common, BuffClassification.Other, BuffImages.ReaperOfGrenth),
            new Buff("Charrzooka", Charrzooka, Source.Common, BuffClassification.Other, BuffImages.Charrzooka),
            // W4
            new Buff("Crystalline Heart", CrystallineHeart, Source.Common, BuffClassification.Other, BuffImages.CrystallineHeart),
            // Mounts
            new Buff("No Mount Use", NoMountUse, Source.Common, BuffClassification.Other, BuffImages.MountsDisabled),
            new Buff("Bond of Life", BondOfLifeBuff, Source.Common, BuffClassification.Other, BuffImages.SynchronizedVitality),
            new Buff("Bond of Vigor", BondOfVigorBuff, Source.Common, BuffClassification.Other, BuffImages.BondOfVigorEffect),
            new Buff("Evasion (Bond of Faith)", EvasionBondOfFaith, Source.Common, BuffClassification.Other, BuffImages.SteelAndFury),
            new Buff("Stealth (Mount Buff)", StealthMountBuff, Source.Common, BuffClassification.Other, BuffImages.StealthMountEffect),
            new Buff("Siege Ammo Available", SiegeAmmoAvailable, Source.Common, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.SiegeAmmoAvailable),
            new Buff("Open Access", OpenAccessBuff, Source.Common, BuffClassification.Other, BuffImages.OpenAccessEffect),
            // Gliding
            new Buff("Gliding Disabled", GlidingDisabled, Source.Common, BuffClassification.Debuff, BuffImages.GlidingDisabled),
            // Consumable Portal
            new Buff("Portal Weaving (Xera/Watchwork)", PortalWeavingWhiteMantleWatchwork, Source.Common, BuffClassification.Other, BuffImages.PortalEnter),
            new Buff("Portal Uses (Xera/Watchwork)", PortalUsesWhiteMantleWatchwork, Source.Common, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.PortalEnter),
            // Consumable Summons
            new Buff("Ogre Pet Whistle", OgrePetWhistleBuff, Source.Common, BuffClassification.Other, BuffImages.OgrePetWhistle),
            new Buff("Fire Elemental Powder", FireElementalPowderBuff, Source.Common, BuffClassification.Other, BuffImages.FireElementalPowder),
            new Buff("Sunspear Paragon Support", SunspearParagonSupportBuff, Source.Common, BuffClassification.Other, BuffImages.SunspearParagonSupport),
            new Buff("Raven Spirit Shadow", RavenSpiritShadowBuff, Source.Common, BuffClassification.Other, BuffImages.RavenSpiritShadow),
            // Primers
            new Buff("Metabolic Primer", MetabolicPrimer, Source.Common, BuffClassification.Other, BuffImages.MetabolicPrimer),
            new Buff("Utility Primer", UtilityPrimer, Source.Common, BuffClassification.Other, BuffImages.UtilityPrimer),
        };

        internal static readonly List<Buff> Gear = new List<Buff>
        {
            // Sigils
            new Buff("Superior Sigil of Concentration", SuperiorSigilOfConcentration, Source.Gear, BuffClassification.Gear, BuffImages.SuperiorSigilOfConcentration)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2018Rune),
            new Buff("Minor Sigil of Corruption", MinorSigilOfCorruption, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.MinorSigilOfCorruption),
            new Buff("Major Sigil of Corruption", MajorSigilOfCorruption, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.MajorSigilOfCorruption),
            new Buff("Superior Sigil of Corruption", SuperiorSigilOfCorruption, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SuperiorSigilOfCorruption),
            new Buff("Superior Sigil of Cruelty", SuperiorSigilOfCruelty, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SuperiorSigilOfCruelty),
            new Buff("Major Sigil of Life", MajorSigilOfLife, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.MajorSigilOfLife),
            new Buff("Superior Sigil of Life", SuperiorSigilOfLife, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SuperiorSigilOfLife),
            new Buff("Major Sigil of Perception", MajorSigilOfPerception, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.MajorSigilOfPerception),
            new Buff("Superior Sigil of Perception", SuperiorSigilOfPerception, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SuperiorSigilOfPerception),
            new Buff("Minor Sigil of Bloodlust", MinorSigilOfBloodlust, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.MinorSigilOfBloodlust),
            new Buff("Major Sigil of Bloodlust", MajorSigilOfBloodlust, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.MajorSigilOfBloodlust),
            new Buff("Superior Sigil of Bloodlust", SuperiorSigilOfBloodlust, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SuperiorSigilOfBloodlust),
            new Buff("Superior Sigil of Bounty", SuperiorSigilOfBounty, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SuperiorSigilOfBounty),
            new Buff("Minor Sigil of Benevolence", MinorSigilOfBenevolence, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.MinorSigilOfBenevolence),
            new Buff("Major Sigil of Benevolence", MajorSigilOfBenevolence, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.MajorSigilOfBenevolence),
            new Buff("Superior Sigil of Benevolence", SuperiorSigilOfBenevolence, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SuperiorSigilOfBenevolence),
            new Buff("Superior Sigil of Momentum", SuperiorSigilOfMomentum, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SuperiorSigilOfMomentum),
            new Buff("Superior Sigil of the Stars", SuperiorSigilOfTheStars, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.SuperiorSigilOfStars),
            new Buff("Superior Sigil of Severance", SuperiorSigilOfSeverance, Source.Gear, BuffClassification.Gear, BuffImages.SuperiorSigilOfSeverance),
            new Buff("Minor Sigil of Doom", MinorSigilOfDoom, Source.Gear, BuffClassification.Gear, BuffImages.SuperiorSigilOfDoom),
            new Buff("Major Sigil of Doom", MajorSigilOfDoom, Source.Gear, BuffClassification.Gear, BuffImages.SuperiorSigilOfDoom),
            new Buff("Superior Sigil of Doom", SuperiorSigilOfDoom, Source.Gear, BuffClassification.Gear, BuffImages.SuperiorSigilOfDoom),
            new Buff("Superior Sigil of Vision", SuperiorSigilOfVision, Source.Gear, BuffClassification.Gear, BuffImages.SuperiorSigilOfVision),
            new Buff("Leech (Major Sigil of Leeching)",  MajorSigilOfLeeching, Source.Gear, BuffClassification.Gear, BuffImages.LeechEffect),
            new Buff("Leech (Sigil / Runes)", LeechBuff, Source.Gear, BuffClassification.Gear, BuffImages.LeechEffect)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2018Rune), // Used to be on Runes of Scavenging (Builds 23057 - November2018Rune) and Vampirism (Builds StartOfLife - November2018Rune)
            new Buff("Leech (Superior Sigil of Leeching)", LeechBuff, Source.Gear, BuffClassification.Gear, BuffImages.LeechEffect)
                .WithBuilds(GW2Builds.November2018Rune),
            // Runes
            new Buff("Superior Rune of the Monk", SuperiorRuneOfTheMonk, Source.Gear, BuffStackType.Stacking, 10, BuffClassification.Gear, BuffImages.SuperiorRuneOfTheMonk)
                .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
            new Buff("Superior Rune of the Cavalier", SuperiorRuneOfTheCavalier, Source.Gear,BuffClassification.Gear, BuffImages.SuperiorRuneOfTheCavalier)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
            new Buff("Healing Glyph (Druid Runes)", HealingGlyph, Source.Gear, BuffClassification.Gear, BuffImages.ConsumeRation)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
            new Buff("Critical (Daredevil Runes)", Critical, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.Critical)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
            // Exotic Halloween Upgrades
            new Buff("Spectral Countenance", SpectralCountenance, Source.Gear, BuffClassification.Gear, BuffImages.OrbOfAscension),
            new Buff("Ghastly Appearance", GhastlyAppearance, Source.Gear, BuffClassification.Gear, BuffImages.OrbOfAscension),
            new Buff("Gourd Vibrations", GourdVibrations, Source.Gear, BuffClassification.Gear, BuffImages.OrbOfAscension),
            new Buff("Cat's Shadow", CatsShadow, Source.Gear, BuffClassification.Gear, BuffImages.OrbOfAscension),
            // Relics
            new Buff("Relic Player Buff (Dragonhunter / Isgarren / Peitha)", RelicTargetToPlayerBuff, Source.Gear, BuffStackType.StackingTargetUniqueSrc, 999, BuffClassification.Hidden, BuffImages.Unknown),
            new Buff("Relic of the Dragonhunter", RelicOfTheDragonhunterTargetBuff, Source.Gear, BuffStackType.StackingTargetUniqueSrc, 999, BuffClassification.Debuff, BuffImages.RelicOfTheDragonhunter), // Applied on target
            new Buff("Relic of the Aristocracy", RelicOfTheAristocracy, Source.Gear, BuffStackType.Stacking, 5, BuffClassification.Gear, BuffImages.RelicOfTheAristocracy),
            new Buff("Relic of the Monk", RelicOfTheMonk, Source.Gear, BuffStackType.Stacking, 10, BuffClassification.Gear, BuffImages.RelicOfTheMonk),
            new Buff("Relic of the Brawler", RelicOfTheBrawler, Source.Gear, BuffClassification.Gear, BuffImages.RelicOfTheBrawler),
            new Buff("Relic of the Thief", RelicOfTheThief, Source.Gear, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Gear, BuffImages.RelicOfTheThief),
            new Buff("Relic of Fireworks", RelicOfFireworks, Source.Gear, BuffClassification.Gear, BuffImages.RelicOfFireworks),
            new Buff("Relic of the Daredevil", RelicOfTheDaredevil, Source.Gear, BuffStackType.StackingConditionalLoss, 3, BuffClassification.Gear, BuffImages.RelicOfTheDaredevil),
            new Buff("Relic of the Deadeye", RelicOfTheDeadeye, Source.Gear, BuffClassification.Gear, BuffImages.RelicOfTheDeadeye),
            new Buff("Relic of the Firebrand", RelicOfTheFirebrand, Source.Gear, BuffClassification.Gear, BuffImages.RelicOfTheFirebrand),
            new Buff("Relic of the Herald", RelicOfTheHerald, Source.Gear, BuffStackType.Stacking, 10, BuffClassification.Gear, BuffImages.RelicOfTheHerald),
            new Buff("Relic of the Scourge", RelicOfTheScourge, Source.Gear, BuffStackType.Stacking, 10, BuffClassification.Gear, BuffImages.RelicOfTheScourge),
            new Buff("Relic of the Weaver", RelicOfTheWeaver, Source.Gear, BuffClassification.Gear, BuffImages.RelicOfTheWeaver),
            new Buff("Relic of the Zephyrite", RelicOfTheZephyrite, Source.Gear, BuffClassification.Gear, BuffImages.RelicOfTheZephyrite),
            new Buff("Relic of Cerus", RelicOfCerusBuff, Source.Gear, BuffClassification.Gear, BuffImages.RelicOfCerus),
            new Buff("Relic of Dagda", RelicOfDagdaBuff, Source.Gear, BuffStackType.Stacking, 1, BuffClassification.Gear, BuffImages.RelicOfDagda),
            new Buff("Relic of Isgarren", RelicOfIsgarrenTargetBuff, Source.Gear, BuffStackType.StackingTargetUniqueSrc, 999, BuffClassification.Debuff, BuffImages.RelicOfIsgarren),
            new Buff("Relic of Lyhr", RelicOfLyhr, Source.Gear, BuffClassification.Defensive, BuffImages.RelicOfLyhr),
            new Buff("Mabon's Strength", MabonsStrength, Source.Gear, BuffStackType.StackingConditionalLoss, 10, BuffClassification.Gear, BuffImages.RelicOfMabon),
            new Buff("Relic of Mabon", RelicOfMabon, Source.Gear, BuffClassification.Gear, BuffImages.RelicOfMabon),
            new Buff("Relic of Peitha", RelicOfPeithaTargetBuff, Source.Gear, BuffStackType.StackingTargetUniqueSrc, 999, BuffClassification.Debuff, BuffImages.RelicOfPeitha),
            new Buff("Relic of Vass", RelicOfVass, Source.Gear, BuffStackType.StackingConditionalLoss, 3, BuffClassification.Gear, BuffImages.RelicOfVass),
            new Buff("Nourys's Hunger (Active)", NouryssHungerStartGainingStacksBuff, Source.Gear, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.RelicOfNourys),
            new Buff("Nourys's Hunger (Stacks)", NouryssHungerStacksBuff, Source.Gear,  BuffStackType.StackingConditionalLoss, 10, BuffClassification.Gear, BuffImages.RelicOfNourys),
            new Buff("Nourys's Hunger (Damage Buff)", NouryssHungerDamageBuff, Source.Gear, BuffClassification.Gear, BuffImages.RelicOfNourys),
        };

    }
}
