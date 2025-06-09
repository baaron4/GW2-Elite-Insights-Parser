using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class CommonBuffs
{

    internal static readonly IReadOnlyList<Buff> Boons =
    [
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
        new Buff("Number of Boons", NumberOfBoons, Source.Common, BuffStackType.Stacking, 0, BuffClassification.Other, ParserIcons.BoonDuration),
    ];

    internal static readonly IReadOnlyList<Buff> Conditions =
    [
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
        new Buff("Number of Conditions", NumberOfConditions, Source.Common, BuffStackType.Stacking, 0, BuffClassification.Other, ParserIcons.ConditionDuration),
    ];

    internal static readonly IReadOnlyList<Buff> Commons =
    [
        new Buff("Number of Active Combat Minions", NumberOfActiveCombatMinions, Source.Common, BuffStackType.Stacking, 99, BuffClassification.Other, ItemImages.SuperiorRuneOfTheRanger),
        new Buff("Number of Clones", NumberOfClones, Source.Common, BuffStackType.Stacking, 99, BuffClassification.Other, ItemImages.SuperiorRuneOfTheMesmer),
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
        new Buff("Take Root", TakeRootBufft, Source.Common, BuffClassification.Other, SkillImages.TakeRoot),
        new Buff("Become the Bear", BecomeTheBear, Source.Common, BuffClassification.Other, SkillImages.BecomeBear),
        new Buff("Become the Raven", BecomeTheRaven, Source.Common, BuffClassification.Other, SkillImages.BecomeRaven),
        new Buff("Become the Snow Leopard", BecomeTheSnowLeopard, Source.Common, BuffClassification.Other, SkillImages.BecomeLeopard),
        new Buff("Become the Wolf", BecomeTheWolf, Source.Common, BuffClassification.Other, SkillImages.BecomeWolf),
        new Buff("Avatar of Melandru", AvatarOfMelandru, Source.Common, BuffClassification.Other, SkillImages.AvatarOfMelandru),
        new Buff("Power Suit", PowerSuit, Source.Common, BuffClassification.Other, SkillImages.PowerSuit),
        new Buff("Reaper of Grenth", ReaperOfGrenth, Source.Common, BuffClassification.Other, SkillImages.ReaperOfGrenth),
        new Buff("Charrzooka", Charrzooka, Source.Common, BuffClassification.Other, SkillImages.Charrzooka),
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
        // OW Consumables     
        new Buff("Lowland Expertise", LowlandExpertise, Source.Common, BuffStackType.Stacking, 7, BuffClassification.Other, BuffImages.LowlandExpertise),
        // Consumable Portal
        new Buff("Portal Weaving (Xera/Watchwork)", PortalWeavingWhiteMantleWatchwork, Source.Common, BuffClassification.Other, SkillImages.PortalEnter),
        new Buff("Portal Uses (Xera/Watchwork)", PortalUsesWhiteMantleWatchwork, Source.Common, BuffStackType.Stacking, 25, BuffClassification.Other, SkillImages.PortalEnter),
        // Consumable Summons
        new Buff("Ogre Pet Whistle", OgrePetWhistleBuff, Source.Common, BuffClassification.Other, ItemImages.OgrePetWhistle),
        new Buff("Fire Elemental Powder", FireElementalPowderBuff, Source.Common, BuffClassification.Other, ItemImages.FireElementalPowder),
        new Buff("Sunspear Paragon Support", SunspearParagonSupportBuff, Source.Common, BuffClassification.Other, ItemImages.SunspearParagonSupport),
        new Buff("Raven Spirit Shadow", RavenSpiritShadowBuff, Source.Common, BuffClassification.Other, ItemImages.RavenSpiritShadow),
        // Primers
        new Buff("Metabolic Primer", MetabolicPrimer, Source.Common, BuffClassification.Other, ItemImages.MetabolicPrimer),
        new Buff("Utility Primer", UtilityPrimer, Source.Common, BuffClassification.Other, ItemImages.UtilityPrimer),
    ];

    internal static readonly IReadOnlyList<Buff> Gear =
    [
        // Sigils
        new Buff("Superior Sigil of Concentration", SuperiorSigilOfConcentration, Source.Gear, BuffClassification.Gear, ItemImages.SuperiorSigilOfConcentration)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2018Rune),
        new Buff("Minor Sigil of Corruption", MinorSigilOfCorruption, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.MinorSigilOfCorruption),
        new Buff("Major Sigil of Corruption", MajorSigilOfCorruption, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.MajorSigilOfCorruption),
        new Buff("Superior Sigil of Corruption", SuperiorSigilOfCorruption, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.SuperiorSigilOfCorruption),
        new Buff("Superior Sigil of Cruelty", SuperiorSigilOfCruelty, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.SuperiorSigilOfCruelty),
        new Buff("Major Sigil of Life", MajorSigilOfLife, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.MajorSigilOfLife),
        new Buff("Superior Sigil of Life", SuperiorSigilOfLife, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.SuperiorSigilOfLife),
        new Buff("Major Sigil of Perception", MajorSigilOfPerception, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.MajorSigilOfPerception),
        new Buff("Superior Sigil of Perception", SuperiorSigilOfPerception, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.SuperiorSigilOfPerception),
        new Buff("Minor Sigil of Bloodlust", MinorSigilOfBloodlust, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.MinorSigilOfBloodlust),
        new Buff("Major Sigil of Bloodlust", MajorSigilOfBloodlust, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.MajorSigilOfBloodlust),
        new Buff("Superior Sigil of Bloodlust", SuperiorSigilOfBloodlust, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.SuperiorSigilOfBloodlust),
        new Buff("Superior Sigil of Bounty", SuperiorSigilOfBounty, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.SuperiorSigilOfBounty),
        new Buff("Minor Sigil of Benevolence", MinorSigilOfBenevolence, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.MinorSigilOfBenevolence),
        new Buff("Major Sigil of Benevolence", MajorSigilOfBenevolence, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.MajorSigilOfBenevolence),
        new Buff("Superior Sigil of Benevolence", SuperiorSigilOfBenevolence, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.SuperiorSigilOfBenevolence),
        new Buff("Superior Sigil of Momentum", SuperiorSigilOfMomentum, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.SuperiorSigilOfMomentum),
        new Buff("Superior Sigil of the Stars", SuperiorSigilOfTheStars, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, ItemImages.SuperiorSigilOfStars),
        new Buff("Superior Sigil of Severance", SuperiorSigilOfSeverance, Source.Gear, BuffClassification.Gear, ItemImages.SuperiorSigilOfSeverance),
        new Buff("Minor Sigil of Doom", MinorSigilOfDoom, Source.Gear, BuffClassification.Gear, ItemImages.SuperiorSigilOfDoom),
        new Buff("Major Sigil of Doom", MajorSigilOfDoom, Source.Gear, BuffClassification.Gear, ItemImages.SuperiorSigilOfDoom),
        new Buff("Superior Sigil of Doom", SuperiorSigilOfDoom, Source.Gear, BuffClassification.Gear, ItemImages.SuperiorSigilOfDoom),
        new Buff("Superior Sigil of Vision", SuperiorSigilOfVision, Source.Gear, BuffClassification.Gear, ItemImages.SuperiorSigilOfVision),
        new Buff("Leech (Major Sigil of Leeching)",  MajorSigilOfLeeching, Source.Gear, BuffClassification.Gear, ItemImages.LeechEffect),
        new Buff("Leech (Sigil / Runes)", LeechBuff, Source.Gear, BuffClassification.Gear, ItemImages.LeechEffect)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2018Rune), // Used to be on Runes of Scavenging (Builds 23057 - November2018Rune) and Vampirism (Builds StartOfLife - November2018Rune)
        new Buff("Leech (Superior Sigil of Leeching)", LeechBuff, Source.Gear, BuffClassification.Gear, ItemImages.LeechEffect)
            .WithBuilds(GW2Builds.November2018Rune),
        // Runes
        new Buff("Superior Rune of the Monk", SuperiorRuneOfTheMonk, Source.Gear, BuffStackType.Stacking, 10, BuffClassification.Gear, ItemImages.SuperiorRuneOfTheMonk)
            .WithBuilds(GW2Builds.November2018Rune, GW2Builds.SOTOReleaseAndBalance),
        new Buff("Superior Rune of the Cavalier", SuperiorRuneOfTheCavalier, Source.Gear,BuffClassification.Gear, ItemImages.SuperiorRuneOfTheCavalier)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
        new Buff("Healing Glyph (Druid Runes)", HealingGlyph, Source.Gear, BuffClassification.Gear, SkillImages.ConsumeRation)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
        new Buff("Critical (Daredevil Runes)", Critical, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Gear, BuffImages.Critical)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOReleaseAndBalance),
        // Exotic Halloween Upgrades
        new Buff("Spectral Countenance", SpectralCountenance, Source.Gear, BuffClassification.Gear, BuffImages.OrbOfAscension),
        new Buff("Ghastly Appearance", GhastlyAppearance, Source.Gear, BuffClassification.Gear, BuffImages.OrbOfAscension),
        new Buff("Gourd Vibrations", GourdVibrations, Source.Gear, BuffClassification.Gear, BuffImages.OrbOfAscension),
        new Buff("Cat's Shadow", CatsShadow, Source.Gear, BuffClassification.Gear, BuffImages.OrbOfAscension),
        // Relics
        new Buff("Relic Player Buff (Dragonhunter / Isgarren / Peitha)", RelicTargetToPlayerBuff, Source.Gear, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Hidden, BuffImages.Unknown),
        new Buff("Relic of the Dragonhunter", RelicOfTheDragonhunterTargetBuff, Source.Gear, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Debuff, ItemImages.RelicOfTheDragonhunter), // Applied on target
        new Buff("Relic of the Aristocracy", RelicOfTheAristocracy, Source.Gear, BuffStackType.Stacking, 5, BuffClassification.Gear, ItemImages.RelicOfTheAristocracy),
        new Buff("Relic of the Monk", RelicOfTheMonk, Source.Gear, BuffStackType.Stacking, 10, BuffClassification.Gear, ItemImages.RelicOfTheMonk),
        new Buff("Relic of the Brawler", RelicOfTheBrawler, Source.Gear, BuffClassification.Gear, ItemImages.RelicOfTheBrawler),
        new Buff("Relic of the Thief", RelicOfTheThief, Source.Gear, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Gear, ItemImages.RelicOfTheThief),
        new Buff("Relic of Fireworks", RelicOfFireworks, Source.Gear, BuffClassification.Gear, ItemImages.RelicOfFireworks),
        new Buff("Relic of the Daredevil", RelicOfTheDaredevil, Source.Gear, BuffStackType.StackingConditionalLoss, 3, BuffClassification.Gear, ItemImages.RelicOfTheDaredevil),
        new Buff("Relic of the Deadeye", RelicOfTheDeadeye, Source.Gear, BuffClassification.Gear, ItemImages.RelicOfTheDeadeye),
        new Buff("Relic of the Firebrand", RelicOfTheFirebrand, Source.Gear, BuffClassification.Gear, ItemImages.RelicOfTheFirebrand),
        new Buff("Relic of the Herald", RelicOfTheHerald, Source.Gear, BuffStackType.Stacking, 10, BuffClassification.Gear, ItemImages.RelicOfTheHerald),
        new Buff("Relic of the Scourge", RelicOfTheScourge, Source.Gear, BuffStackType.Stacking, 10, BuffClassification.Gear, ItemImages.RelicOfTheScourge),
        new Buff("Relic of the Weaver", RelicOfTheWeaver, Source.Gear, BuffClassification.Gear, ItemImages.RelicOfTheWeaver),
        new Buff("Relic of the Zephyrite", RelicOfTheZephyrite, Source.Gear, BuffClassification.Gear, ItemImages.RelicOfTheZephyrite),
        new Buff("Relic of Cerus", RelicOfCerusBuff, Source.Gear, BuffClassification.Gear, ItemImages.RelicOfCerus),
        new Buff("Relic of Dagda", RelicOfDagdaBuff, Source.Gear, BuffStackType.Stacking, 1, BuffClassification.Gear, ItemImages.RelicOfDagda),
        new Buff("Relic of Isgarren", RelicOfIsgarrenTargetBuff, Source.Gear, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Debuff, ItemImages.RelicOfIsgarren),
        new Buff("Relic of Lyhr", RelicOfLyhr, Source.Gear, BuffClassification.Defensive, ItemImages.RelicOfLyhr),
        new Buff("Mabon's Strength", MabonsStrength, Source.Gear, BuffStackType.StackingConditionalLoss, 10, BuffClassification.Gear, ItemImages.RelicOfMabon),
        new Buff("Relic of Mabon", RelicOfMabon, Source.Gear, BuffClassification.Gear, ItemImages.RelicOfMabon),
        new Buff("Relic of Peitha", RelicOfPeithaTargetBuff, Source.Gear, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Debuff, ItemImages.RelicOfPeitha),
        new Buff("Relic of Vass", RelicOfVass, Source.Gear, BuffStackType.StackingConditionalLoss, 3, BuffClassification.Gear, ItemImages.RelicOfVass),
        new Buff("Nourys's Hunger (Active)", NouryssHungerStartGainingStacksBuff, Source.Gear, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, ItemImages.RelicOfNourys),
        new Buff("Nourys's Hunger (Stacks)", NouryssHungerStacksBuff, Source.Gear,  BuffStackType.StackingConditionalLoss, 10, BuffClassification.Gear, ItemImages.RelicOfNourys),
        new Buff("Nourys's Hunger (Damage Buff)", NouryssHungerDamageBuff, Source.Gear, BuffClassification.Gear, ItemImages.RelicOfNourys),
        new Buff("Relic of the Stormsinger", RelicOfTheStormsingerBuff, Source.Gear, BuffClassification.Other, ItemImages.RelicOfTheStormsinger),
        new Buff("Relic of Sorrow", RelicOfSorrowBuff, Source.Gear, BuffClassification.Defensive, ItemImages.RelicOfTheSorrow),
        new Buff("Greer's Virulence", GreersVirulence, Source.Gear,  BuffStackType.StackingConditionalLoss, 5, BuffClassification.Gear, ItemImages.RelicOfTheBlightbringer),
        new Buff("Relic of the Claw", RelicOfTheClaw, Source.Gear, BuffClassification.Gear, ItemImages.RelicOfTheClaw),
        new Buff("Relic of Mount Balrior", RelicOfMountBalrior, Source.Gear, BuffClassification.Gear, ItemImages.RelicOfMountBalrior),
        new Buff("Relic of Thorns", RelicOfThorns, Source.Gear, BuffStackType.Stacking, 10, BuffClassification.Gear, ItemImages.RelicOfThorns),
        new Buff("Soul  of the Titan", SoulOfTheTitan, Source.Gear, BuffClassification.Gear, BuffImages.SoulOfTheTitan),
        new Buff("Titanic Potential", TitanicPotential, Source.Gear, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Gear, ItemImages.RelicOfTheLivingCity),
        new Buff("Bloodstone Volatility", BloodstoneVolatility, Source.Gear, BuffStackType.Stacking, 3, BuffClassification.Gear, ItemImages.RelicOfBloodstone),
        new Buff("Bloodstone Fervor", BloodstoneFervor, Source.Gear, BuffStackType.Stacking, 3, BuffClassification.Gear, ItemImages.RelicOfBloodstone),
        new Buff("Agony of the choir", AgonyOfTheChoir, Source.Gear, BuffStackType.Stacking, 25, BuffClassification.Debuff, BuffImages.SpectralAgony),
    ];

}
