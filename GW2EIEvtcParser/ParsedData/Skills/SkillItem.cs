﻿using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using GW2EIGW2API.GW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.ParsedData;

public class SkillItem
{

    internal static (long, long) GetArcDPSCustomIDs(EvtcVersionEvent evtcVersion)
    {
        if (evtcVersion.Build >= ArcDPSBuilds.InternalSkillIDsChange)
        {
            return (ArcDPSDodge20220307, ArcDPSGenericBreakbar20220307);
        }
        else
        {
            return (ArcDPSDodge, ArcDPSGenericBreakbar);
        }
    }

    private static readonly Dictionary<long, string> _overrideNames = new()
    {
        { WeaponSwap, "Weapon Swap" },
        { Resurrect, "Resurrect" },
        { Bandage, "Bandage" },
        { ArcDPSDodge, "Dodge" },
        { ArcDPSDodge20220307, "Dodge" },
        { ArcDPSGenericBreakbar, "Generic Breakbar" },
        { ArcDPSGenericBreakbar20220307, "Generic Breakbar" },
        { WaterBlastCombo1, "Water Blast Combo" },
        { WaterBlastCombo2, "Water Blast Combo" },
        { WaterLeapCombo, "Water Leap Combo" },
        { LightningLeapCombo, "Lightning Leap Combo" },
        { MendingMight, "Mending Might" },
        { InvigoratingBond, "Invigorating Bond" },
        #region Sigils
        { WaveOfHealing_MinorSigilOfWater, "Wave of Healing (Minor Sigil of Water)" },
        { WaveOfHealing_MajorSigilOfWater, "Wave of Healing (Major Sigil of Water)" },
        { WaveOfHealing_SuperiorSigilOfWater, "Wave of Healing (Superior Sigil of Water)" },
        { WaveOfHealing_MinorSigilOfRenewal, "Wave of Healing (Minor Sigil of Renewal)" },
        { WaveOfHealing_MajorSigilOfRenewal, "Wave of Healing (Major Sigil of Renewal)" },
        { WaveOfHealing_SuperiorSigilOfRenewal, "Wave of Healing (Superior Sigil of Renewal)" },
        { FrostBurst_MinorSigilOfHydromancy, "Frost Burst (Minor Sigil of Hydromancy)" },
        { FrostBurst_MajorSigilOfHydromancy, "Frost Burst (Major Sigil of Hydromancy)" },
        { FrostBurst_SuperiorSigilOfHydromancy, "Frost Burst (Superior Sigil of Hydromancy)" },
        { RingOfEarth_MinorSigilOfGeomancy, "Ring of Earth (Minor Sigil of Geomancy)" },
        { RingOfEarth_MajorSigilOfGeomancy, "Ring of Earth (Major Sigil of Geomancy)" },
        { RingOfEarth_SuperiorSigilOfGeomancy, "Ring of Earth (Superior Sigil of Geomancy)" },
        { LightningStrike_SigilOfAir, "Lightning Strike (Minor/Major/Superior Sigil of Air)" },
        { FlameBlast_SigilOfFire, "Flame Blast (Minor/Major/Superior Sigil of Fire)" },
        { Snowball_SigilOfMischief, "Snowball (Minor/Major/Superior Sigil of Mischief)" },
        { SuperiorSigilOfSeverance, "Superior Sigil of Severance" },
        { MinorSigilOfDoom, "Minor Sigil of Doom" },
        { MajorSigilOfDoom, "Major Sigil of Doom" },
        { SuperiorSigilOfDoom, "Superior Sigil of Doom" },
        { MinorSigilOfBlood, "Minor Sigil of Blood" },
        { MajorSigilOfBlood, "Major Sigil of Blood" },
        { SuperiorSigilOfBlood, "Superior Sigil of Blood" },
        { MajorSigilOfLeeching, "Major Sigil of Leeching" },
        { SuperiorSigilOfLeeching, "Superior Sigil of Leeching" },
        { SuperiorSigilOfVision, "Superior Sigil of Vision" },
        { SuperiorSigilOfConcentration, "Superior Sigil of Concentration" },
        #endregion Sigils
        #region Runes
        { RuneOfNightmare, "Rune of the Nightmare" },
        { FrozenBurst_RuneOfIce, "Frozen Burst (Rune of the Ice)" },
        { HuntersCall_RuneOfMadKing, "Hunter's Call (Rune of the Mad King)" },
        { ArtilleryBarrage_RuneofCitadel, "Artillery Barrage (Rune of the Citadel)" },
        { HandOfGrenth_RuneOfGrenth, "Hand of Grenth (Rune of Grenth)" },
        #endregion Runes
        { PortalEntranceWhiteMantleWatchwork, "Portal Entrance" },
        { PortalExitWhiteMantleWatchwork, "Portal Exit" },
        { PlateOfSpicyMoaWingsGastricDistress, "Plate of Spicy Moa Wings (Gastric Distress)" },
        { ThrowGunkEttinGunk, "Throw Gunk (Ettin Gunk)" },
        { SmashBottle, "Smash (Bottle)" },
        { ThrowBottle, "Throw (Bottle)" },
        { ThrowNetUnderwaterNet, "Throw Net (Underwater Net)" },
        // Mounts
        #region Mounts
        { BondOfLifeSkill, "Bond of Life" },
        { BondOfVigorSkill, "Bond of Vigor" },
        { BondOfFaithSkill, "Bond of Faith" },
        { StealthMountSkill, "Stealth 2.0" },
        // Skyscale
        { SkyscaleSkill, "Skyscale" },
        { SkyscaleFireballSkill, "Fireball" },
        { SkyscaleBlastSkill, "Blast" },
        { SkyscaleBlastDamage, "Blast (Damage)" },
        #endregion Mounts
        #region Relics
        { RelicOfTheWizardsTower, "Relic of the Wizard's Tower" },
        { RelicOfIsgarrenTargetBuff, "Relic of Isgarren" },
        { RelicOfTheDragonhunterTargetBuff, "Relic of the Dragonhunter" },
        { RelicOfPeithaTargetBuff, "Relic of Peitha" },
        { RelicOfPeithaBlade, "Relic of Peitha (Blade)" },
        { RelicOfFireworksBuffLoss, "Relic of Fireworks (Buff Loss)" },
        { RelicOfTheFlockBarrier, "Relic of the Flock (Barrier)" },
        { RelicOfMercyHealing, "Relic of Mercy" },
        { MabonsStrength, "Relic of Mabon" },
        { NouryssHungerDamageBuff, "Relic of Nourys" },
        { RelicOfTheFoundingBarrier, "Relic of the Founding (Barrier)" },
        { RelicOfTheClawBuffLoss, "Relic of the Claw (Buff Loss)" },
        { RelicOfTheStormsingerChain, "Relic of the Stormsinger (Chain)" },
        #endregion Relics
        #region Elementalist
        { DualFireAttunement, "Dual Fire Attunement" },
        { FireWaterAttunement, "Fire Water Attunement" },
        { FireAirAttunement, "Fire Air Attunement" },
        { FireEarthAttunement, "Fire Earth Attunement" },
        { DualWaterAttunement, "Dual Water Attunement" },
        { WaterFireAttunement, "Water Fire Attunement" },
        { WaterAirAttunement, "Water Air Attunement" },
        { WaterEarthAttunement, "Water Earth Attunement" },
        { DualAirAttunement, "Dual Air Attunement" },
        { AirFireAttunement, "Air Fire Attunement" },
        { AirWaterAttunement, "Air Water Attunement" },
        { AirEarthAttunement, "Air Earth Attunement" },
        { DualEarthAttunement, "Dual Earth Attunement" },
        { EarthFireAttunement, "Earth Fire Attunement" },
        { EarthWaterAttunement, "Earth Water Attunement" },
        { EarthAirAttunement, "Earth Air Attunement" },
        { ShatteringIceDamage, "Shattering Ice (Hit)" },
        { ArcaneShieldDamage, "Arcane Shield (Explosion)" },
        { FirestormGlyphOfStormsOrFieryGreatsword, "Firestorm (Glyph of Storms / Fiery Greatsword)" },
        #endregion Elementalist
        #region Engineer
        { HealingMistOrSoothingDetonation, "Healing Mist or Soothing Detonation" },
        { MechCoreBarrierEngine, "Mech Core: Barrier Engine" },
        { MedBlasterHeal, "Med Blaster (Heal)" },
        { SoothingDetonation, "Soothing Detonation" },
        { HealingTurretHeal, "Healing Turret (Heal)" },
        { BladeBurstOrParticleAccelerator, "Blade Burst or Particle Accelerator" },
        { DetonateThrowMineOrMineField, "Detonate (Throw Mine / Mine Field)" },
        #endregion Engineer
        #region Guardian
        { SelflessDaring, "Selfless Daring" }, // The game maps this name incorrectly to "Selflessness Daring"
        { ProtectorsStrikeCounterHit, "Protector's Strike (Counter Hit)" },
        { HuntersVerdictPull, "Hunter's Verdict (Pull)" },
        { MantraOfSolace, "Mantra of Solace" },
        { RestoringReprieveOrRejunevatingRespite, "Restoring Reprieve or Rejunevating Respite" },
        { OpeningPassageOrClarifiedConclusion, "Opening Passage or Clarified Conclusion" },
        { PotentHasteOrOverwhelmingCelerity, "Potent Haste or Overwhelming Celerity" },
        { PortentOfFreedomOrUnhinderedDelivery, "Portent of Freedom or Unhindered Delivery" },
        { RushingJusticeStrike, "Rushing Justice (Hit)" },
        { ExecutionersCallingDualStrike, "Executioner's Calling (Dual Strike)" },
        { FireJurisdictionLevel1, "Fire Jurisdiction (Level 1)" },
        { FireJurisdictionLevel2, "Fire Jurisdiction (Level 2)" },
        { FireJurisdictionLevel3, "Fire Jurisdiction (Level 3)" },
        #endregion Guardian
        #region Mesmer
        { PowerReturn, "Power Return" },
        { PowerCleanse, "Power Cleanse" },
        { PowerBreak, "Power Break" },
        { PowerLock, "Power Lock" },
        { BlinkOrPhaseRetreat, "Blink or Phase Retreat" },
        { MirageCloakDodge, "Mirage Cloak" },
        { UnstableBladestormProjectiles, "Unstable Bladestorm (Projectile Hit)" },
        { PhantasmalBerserkerProjectileDamage, "Phantasmal Berserker (Greatsword Projectile Hit)" },
        { HealingPrism, "Healing Prism" },
        #endregion Mesmer
        #region Necromancer
        { DesertEmpowerment, "Desert Empowerment" },
        { SandCascadeBarrier, "Sand Cascade (Barrier)" },
        { SandFlare, "Sand Cascade" }, // Eliphas: What the hell?
        { SadisticSearingActivation, "Sadistic Searing (Activation)" },
        { MarkOfBloodOrChillblainsTrigger, "Mark of Blood / Chillblains" },
        #endregion Necromancer
        #region Ranger
        { WindborneNotes, "Windborne Notes" },
        { NaturalHealing, "Natural Healing" }, // The game does not map this one at all
        { LiveVicariously, "Live Vicariously" }, // The game maps this name incorrectly to "Vigorous Recovery"
        { EntangleDamage, "Entangle (Hit)" },
        { AstralWispAttachment, "Astral Wisp Attachment" },
        { GlyphOfUnityCA, "Glyph of Unity (CA)" },
        { BloodMoonDaze, "Blood Moon (Daze)" },
        { ChargeGazelleMergeSkill, "Charge (Travel)" },
        { ChargeGazelleMergeImpact, "Charge (Impact)" },
        { SmokeAssaultMergeHit, "Smoke Assault (Multi Hit)" },
        { OneWolfPackDamage, "One Wolf Pack (Hit)" },
        { OverbearingSmashLeap, "Overbearing Smash (Leap)" },
        { UnleashedOverbearingSmashLeap, "Unleashed Overbearing Smash (Leap)" },
        { RangerPetSpawned, "Ranger Pet Spawned" },
        { WolfsOnslaughtFollowUp, "Wolf's Onslaught (Follow Up)" },
        #endregion Ranger
        #region Revenant
        { EnergyExpulsion, "Energy Expulsion" },
        { RiftSlashRiftHit, "Rift Slash (Rift Hit)" },
        { UnrelentingAssaultMultihit, "Unrelenting Assault (Multi Hit)" },
        { ImpossibleOddsHit, "Impossible Odds (Hit)" },
        { EmbraceTheDarknessDamage, "Embrace the Darkness (Hit)" },
        { TrueNatureDragon, "True Nature - Dragon" },
        { TrueNatureDemon, "True Nature - Demon" },
        { TrueNatureDwarf, "True Nature - Dwarf" },
        { TrueNatureAssassin, "True Nature - Assassin" },
        { TrueNatureCentaur, "True Nature - Centaur" },
        { DarkrazorsDaringHit, "Darkrazor's Daring (Hit)" },
        { IcerazorsIreHit, "Icerazor's Ire (Hit)" },
        { PhantomsOnslaughtDamage, "Phantom's Onslaught (Hit)" },
        { KallaSummonsDespawnSkill, "Despawn" },
        { KallaSummonsSaluteAnimationSkill, "Salute" },
        { GenerousAbundanceCentaur, "Generous Abundance (Centaur)" },
        { GenerousAbundanceOther, "Generous Abundance (Other)" },
        { BlitzMinesDrop, "Blitz Mines (Drop)" },
        { BlitzMines, "Blitz Mines (Detonation)" },
        #endregion Revenant
        #region Thief
        { EscapistsFortitude, "Escapist's Fortitude" }, // The game maps this to the wrong skill
        { SoulStoneVenomSkill, "Soul Stone Venom" },
        { SoulStoneVenomStrike, "Soul Stone Venom (Hit)" },
        { BasiliskVenomStunBreakbarDamage, "Basilisk Venom (Stun)" },
        { TwilightComboSecondProjectile, "Twilight Combo (Secondary)" },
        { ThievesGuildMinionDespawnSkill, "Despawn" },
        { ImpairingDaggersHit1, "Impairing Daggers (Dagger Hit 1)" },
        { ImpairingDaggersHit2, "Impairing Daggers (Dagger Hit 2)" },
        { ImpairingDaggersHit3, "Impairing Daggers (Dagger Hit 3)" },
        { ImpairingDaggersDaredevilMinionHit1, "Impairing Daggers (Dagger Hit 1)" },
        { ImpairingDaggersDaredevilMinionHit2, "Impairing Daggers (Dagger Hit 2)" },
        { ImpairingDaggersDaredevilMinionHit3, "Impairing Daggers (Dagger Hit 3)" },
        { BoundHit, "Bound (Hit)" },
        { BarbedSpearMelee, "Barbed Spear (Melee)" },
        { BarbedSpearRanged, "Barbed Spear (Ranged)" },
        #endregion Thief
        #region Warrior
        { RushDamage, "Rush (Hit)" },
        { MightyThrowScatter, "Mighty Throw (Scattered Spear)" },
        { HarriersTossAdrenalineLevel1, "Harrier's Toss (Adrenaline Level 1)" },
        { HarriersTossAdrenalineLevel2, "Harrier's Toss (Adrenaline Level 2)" },
        { HarriersTossAdrenalineLevel3, "Harrier's Toss (Adrenaline Level 3)" },
        { BerserkEndSkill, "Berserk (End)" },
        #endregion Warrior
        // Special Forces Training Area
        { MushroomKingsBlessing, "Mushroom King's Blessing (PoV Only)" },
        #region Raids
        // Gorseval
        { GhastlyRampage,"Ghastly Rampage" },
        { ProtectiveShadow,"Protective Shadow" },
        { GhastlyRampageBegin,"Ghastly Rampage (Begin)" },
        // Sabetha
        { ShadowStepSabetha, "Shadow Step" },
        // Slothasor
        { TantrumSkill, "Tantrum Start" },
        { NarcolepsySkill, "Sleeping" },
        { FearMeSlothasor, "Fear Me!" },
        { PurgeSlothasor, "Purge" },
        // Bandit Trio
        { ThrowOilKeg, "Throw (Oil Keg)" },
        // Matthias
        { ShieldHuman, "Shield (Human)" },
        { AbominationTransformation, "Abomination Transformation" },
        { ShieldAbomination, "Shield (Abomination)" },
        // Escort
        { GlennaCap, "Capture" },
        { OverHere, "Over Here!" },
        // Xera
        { InterventionSAK, "Intervetion" },
        // Cairn
        { CelestialDashSAK, "Celestial Dash" },
        // Mursaar Overseer
        { ClaimSAK, "Claim" },
        { DispelSAK, "Dispel" },
        { ProtectSAK, "Protect" },
        // Soulless Horror
        { IssueChallengeSAK, "Issue Challenge" },
        // Broken King
        { NumbingBreachCast, "Numbing Breach (Cast)" },
        // Dhuum
        { MajorSoulSplit, "Major Soul Split" },
        { ExpelEnergySAK, "Expel Energy" },
        // Keep Construct
        { MagicBlastCharge, "Magic Blast Charge" },
        // Conjured Amalgamate
        { ConjuredSlashSAK, "Conjured Slash" },
        { ConjuredProtection, "Conjured Protection" },
        // Adina
        { DoubleRotatingEarthRays, "Double Rotating Earth Rays" },
        { TripleRotatingEarthRays, "Triple Rotating Earth Rays" },
        { Terraform, "Terraform" },
        // Sabir
        { RegenerativeBreakbar, "Regenerative Breakbar" },
        // Qadim the Peerless
        { RuinousNovaCharge, "Ruinous Nova Charge" },
        { FluxDisruptorActivateCast, "Flux Disruptor: Activate" },
        { FluxDisruptorDeactivateCast, "Flux Disruptor: Deactivate" },
        { PlayerLiftUpQadimThePeerless, "Player Lift Up Mechanic" },
        { UnleashSAK, "Unleash" },
        //{56036, "Magma Bomb" },
        { ForceOfRetaliationCast, "Force of Retaliation Cast" },
        { PeerlessQadimTPCenter, "Teleport Center" },
        { EatPylon, "Eat Pylon" },
        { BigMagmaDrop, "Big Magma Drop" },
        // Ura
        { UraDispelSAK, "Dispel" },
        #endregion Raids
        #region Strikes
        // Voice and Claw
        { KodanTeleport, "Kodan Teleport" },
        // Mai Trin (Aetherblade Hideout)
        { ReverseThePolaritySAK, "Reverse the Polarity!" },
        // Cerus
        // - Normal Mode
        { CrushingRegretNM, "Crushing Regret (NM)" },
        { WailOfDespairNM, "Wail of Despair (NM)" },
        { EnviousGazeNM, "Envious Gaze (NM)" },
        { MaliciousIntentNM, "Malicious Intent (NM)" },
        { InsatiableHungerSkillNM, "Insatiable Hunger (NM)" },
        { CryOfRageNM, "Cry of Rage (NM)" },
        // - Empowered Normal Mode
        { CrushingRegretEmpoweredNM, "Crushing Regret (Empowered NM)" },
        { WailOfDespairEmpoweredNM, "Wail of Despair (Empowered NM)" },
        { EnviousGazeEmpoweredNM, "Envious Gaze (Empowered NM)" },
        { MaliciousIntentEmpoweredNM, "Malicious Intent (Empowered NM)" },
        { InsatiableHungerEmpoweredSkillNM, "Insatiable Hunger (Empowered NM)" },
        { CryOfRageEmpoweredNM, "Cry of Rage (Empowered NM)" },
        // - Challenge Mode
        { CrushingRegretCM, "Crushing Regret (CM)" },
        { WailOfDespairCM, "Wail of Despair (CM)" },
        { EnviousGazeCM, "Envious Gaze (CM)" },
        { MaliciousIntentCM, "Malicious Intent (CM)" },
        { InsatiableHungerSkillCM, "Insatiable Hunger (CM)" },
        { CryOfRageCM, "Cry of Rage (CM)" },
        // - Empowered Challenge Mode
        { CrushingRegretEmpoweredCM, "Crushing Regret (Empowered CM)" },
        { WailOfDespairEmpoweredCM, "Wail of Despair (Empowered CM)" },
        { EnviousGazeEmpoweredCM, "Envious Gaze (Empowered CM)" },
        { MaliciousIntentEmpoweredCM, "Malicious Intent (Empowered CM)" },
        { InsatiableHungerEmpoweredSkillCM, "Insatiable Hunger (Empowered CM)" },
        { CryOfRageEmpoweredCM, "Cry of Rage (Empowered CM)" },
        // - Misc
        { PetrifySkill, "Petrify" },
        { EnragedSmashNM, "Enraged Smash (NM)" },
        { EnragedSmashCM, "Enraged Smash (CM)" },
        #endregion Strikes
        #region Fractals
        // Artsariiv
        { NovaLaunchSAK, "Nova Launch" },
        // Arkk
        { HypernovaLaunchSAK, "Hypernova Launch" },
        // Kanaxai
        { FrighteningSpeedWindup, "Frightening Speed (Windup)" },
        { FrighteningSpeedReturn, "Frightening Speed (Return)" },
        { DreadVisageKanaxaiSkill, "Dread Visage (Kanaxai)" },
        { DreadVisageKanaxaiSkillIsland, "Dread Visage (Kanaxai Island)" },
        { DreadVisageAspectSkill, "Dread Visage (Aspect)" },
        { RendingStormSkill, "Rending Storm (Axe)" },
        { GatheringShadowsSkill, "Gathering Shadows (Breakbar)" },
        #endregion Fractals
        #region WvW
        // World vs World
        { WvWSpendingSupplies, "Spending Supply (Building / Repairing)" },
        { WvWPickingUpSupplies, "Picking Up Supplies" },
        // - Arrow Cart
        { DeployArrowCart, "Deploy Arrow Cart" },
        { DeploySuperiorArrowCart, "Deploy Superior Arrow Cart" },
        { DeployGuildArrowCart, "Deploy Guild Arrow Cart" },
        // - Ballista
        { DeployBallista, "Deploy Ballista" },
        { DeploySuperiorBallista, "Deploy Superior Ballista" },
        { DeployGuildBallista, "Deploy Guild Ballista" },
        // - Catapult
        { DeployCatapult, "Deploy Catapult" },
        { DeploySuperiorCatapult, "Deploy Superior Catapult" },
        { DeployGuildCatapult, "Deploy Guild Catapult" },
        // - Flame Ram
        { DeployFlameRam, "Deploy Flame Ram" },
        { DeploySuperiorFlameRam, "Deploy Superior Flame Ram" },
        { DeployGuildFlameRam, "Deploy Guild Flame Ram" },
        // - Golem
        { DeployAlphaSiegeSuit, "Deploy Alpha Siege Golem" },
        { DeployOmegaSiegeSuit, "Deploy Omega Siege Golem" },
        { DeployGuildSiegeSuit, "Deploy Guild Siege Golem" },
        // - Shield Generator
        { DeployShieldGenerator, "Deploy Shield Generator" },
        { DeploySuperiorShieldGenerator, "Deploy Superior Shield Generator" },
        { DeployGuildShieldGenerator, "Deploy Guild Shield Generator" },
        // - Trebuchet
        { DeployTrebuchet, "Deploy Trebuchet" },
        { DeploySuperiorTrebuchet, "Deploy Superior Trebuchet" },
        { DeployGuildTrebuchet, "Deploy Guild Trebuchet" },
        #endregion WvW
    };

    private static readonly Dictionary<long, string> _overrideIcons = new()
    {
        { WeaponSwap, SkillImages.WeaponSwap },
        { WeaponStow, SkillImages.WeaponStow },
        { WeaponDraw, SkillImages.WeaponDraw },
        { Resurrect, SkillImages.Resurrect },
        { Bandage, SkillImages.Bandage },
        { LevelUp, ParserIcons.LevelUp },
        { LevelUp2, ParserIcons.LevelUp },
        { ArcDPSGenericBreakbar, ParserIcons.Breakbar },
        { ArcDPSDodge, SkillImages.Dodge },
        { ArcDPSGenericBreakbar20220307, ParserIcons.Breakbar },
        { ArcDPSDodge20220307, SkillImages.Dodge },
        { Poisoned, BuffImages.Poison },
        { WhirlingAssault, SkillImages.WhirlingAssault },
        #region ComboIcons
        // Combos
        { WaterBlastCombo1, ParserIcons.Healing },
        { WaterBlastCombo2, ParserIcons.Healing },
        { WaterLeapCombo, ParserIcons.Healing },
        { WaterWhirlCombo, ParserIcons.Healing },
        { LeechingBolt1, ParserIcons.Healing },
        { LeechingBolt2, ParserIcons.Healing },
        { PoisonLeapCombo, ParserIcons.Combo },
        { PoisonBlastCombo, ParserIcons.Combo },
        { PoisonBlastCombo2, ParserIcons.Combo },
        { PoisonWhirlCombo, ParserIcons.Combo },
        { LightningLeapCombo, ParserIcons.Combo },
        { LightningWhirlCombo, ParserIcons.Combo },
        { DarkWhirlCombo, ParserIcons.Combo },
        { DarkBlastCombo, ParserIcons.Combo },
        { DarkBlastCombo2, ParserIcons.Combo },
        { FireWhirlCombo, ParserIcons.Combo },
        { IceWhirlCombo, ParserIcons.Combo },
        { ChaosWhirlCombo, ParserIcons.Combo },
        { SmokeWhirlCombo, ParserIcons.Combo },
        { LightWhirlCombo, ParserIcons.Combo },
        #endregion ComboIcons
        #region ItemIcons
        { LightningStrike_SigilOfAir, ItemImages.SuperiorSigilOfAir },
        { FlameBlast_SigilOfFire, ItemImages.SuperiorSigilOfFire },
        { RingOfEarth_MinorSigilOfGeomancy, ItemImages.MinorSigilOfGeomancy },
        { RingOfEarth_MajorSigilOfGeomancy, ItemImages.MajorSigilOfGeomancy },
        { RingOfEarth_SuperiorSigilOfGeomancy, ItemImages.SuperiorSigilOfGeomancy },
        { FrostBurst_MinorSigilOfHydromancy, ItemImages.MinorSigilOfHydromancy },
        { FrostBurst_MajorSigilOfHydromancy, ItemImages.MajorSigilOfHydromancy },
        { FrostBurst_SuperiorSigilOfHydromancy, ItemImages.SuperiorSigilOfHydromancy },
        { WaveOfHealing_MinorSigilOfWater, ItemImages.MinorSigilOfWater },
        { WaveOfHealing_MajorSigilOfWater, ItemImages.MajorSigilOfWater },
        { WaveOfHealing_SuperiorSigilOfWater, ItemImages.SuperiorSigilOfWater },
        { WaveOfHealing_MinorSigilOfRenewal, ItemImages.MinorSigilOfRenewal },
        { WaveOfHealing_MajorSigilOfRenewal, ItemImages.MajorSigilOfRenewal },
        { WaveOfHealing_SuperiorSigilOfRenewal, ItemImages.SuperiorSigilOfRenewal },
        { MajorSigilOfRestoration, ItemImages.MajorSigilOfRestoration },
        { SuperiorSigilOfRestoration, ItemImages.SuperiorSigilOfRestoration },
        { SuperiorSigilOfSeverance, ItemImages.SuperiorSigilOfSeverance },
        { MinorSigilOfDoom, ItemImages.MinorSigilOfDoom },
        { MajorSigilOfDoom, ItemImages.MajorSigilOfDoom },
        { SuperiorSigilOfDoom, ItemImages.SuperiorSigilOfDoom },
        { MinorSigilOfBlood, ItemImages.MinorSigilOfBlood },
        { MajorSigilOfBlood, ItemImages.MajorSigilOfBlood },
        { SuperiorSigilOfBlood, ItemImages.SuperiorSigilOfBlood },
        { MajorSigilOfLeeching, ItemImages.MajorSigilOfLeeching },
        { SuperiorSigilOfLeeching, ItemImages.SuperiorSigilOfLeeching },
        { Snowball_SigilOfMischief, ItemImages.SuperiorSigilOfMischief },
        { SuperiorSigilOfVision, ItemImages.SuperiorSigilOfVision },
        { SuperiorSigilOfConcentration, ItemImages.SuperiorSigilOfConcentration },
        { SuperiorSigilOfDraining, ItemImages.SuperiorSigilOfConcentration },
        { RuneOfTormenting, ItemImages.SuperiorRuneOfTormenting },
        { RuneOfNightmare, ItemImages.SuperiorRuneOfTheNightmare },
        { SuperiorRuneOfTheDolyak, ItemImages.SuperiorRuneOfTheDolyak },
        { FrozenBurst_RuneOfIce, ItemImages.SuperiorRuneOfTheIce },
        { HuntersCall_RuneOfMadKing, ItemImages.SuperiorRuneOfTheMadKing },
        { ArtilleryBarrage_RuneofCitadel, ItemImages.SuperiorRuneOfTheCitadel },
        { HandOfGrenth_RuneOfGrenth, ItemImages.SuperiorRuneOfGrenth },
        { PortalEntranceWhiteMantleWatchwork, ItemImages.WatchworkPortalDevice },
        { PortalExitWhiteMantleWatchwork, ItemImages.WatchworkPortalDevice },
        { PlateOfSpicyMoaWingsGastricDistress, ItemImages.PlateOfSpicyMoaWings },
        { ThrowGunkEttinGunk, SkillImages.ThrowGunk },
        { SmashBottle, SkillImages.SmashBottle },
        { ThrowBottle, SkillImages.ThrowBottle },
        { ThrowNetUnderwaterNet, SkillImages.NetShot },
#endregion ItemIcons
        #region MountIcons
        { BondOfLifeSkill, SkillImages.BondOfLife },
        { BondOfVigorSkill, SkillImages.BondOfVigor },
        { BondOfFaithSkill, SkillImages.BondOfFaith },
        { StealthMountSkill, SkillImages.StealthMount },
        // Skyscale
        { SkyscaleSkill, SkillImages.Skyscale },
        { SkyscaleFireballSkill, SkillImages.SkyscaleFireball },
        { SkyscaleFireballDamage, SkillImages.SkyscaleFireball },
        { SkyscaleBlastSkill, SkillImages.SkyscaleBlast },
        // Raptor
        { RaptorTailSpin, SkillImages.RaptorTailSpin },
        // Warclaw
        { WarclawBattleMaulSkill, SkillImages.WarclawBattleMaul },
        { WarclawBattleMaulDamage, SkillImages.WarclawBattleMaul },
        { WarclawLance, SkillImages.WarclawLance },
        { WarclawChainPull1, SkillImages.WarclawChainPull },
        { WarclawChainPull2, SkillImages.WarclawChainPull },
        #endregion MountIcons
        #region RelicIcons
        // Relics
        { RelicOfTheAfflicted, ItemImages.RelicOfTheAfflicted },
        { RelicOfTheCitadel, ItemImages.RelicOfTheCitadel },
        { RelicOfMercyHealing, ItemImages.RelicOfMercy },
        { RelicOfTheFlock, ItemImages.RelicOfTheFlock },
        { RelicOfTheFlockBarrier, ItemImages.RelicOfTheFlock },
        { RelicOfTheIce, ItemImages.RelicOfTheIce },
        { RelicOfTheKrait, ItemImages.RelicOfTheKrait },
        { RelicOfTheNightmare, ItemImages.RelicOfTheNightmare },
        { RelicOfTheSunless, ItemImages.RelicOfTheSunless },
        { RelicOfAkeem, ItemImages.RelicOfAkeem },
        { RelicOfTheWizardsTower, ItemImages.RelicOfTheWizardsTower },
        { RelicOfTheMirage, ItemImages.RelicOfTheMirage },
        { RelicOfCerusHit, ItemImages.RelicOfCerus },
        { RelicOfDagdaHit, ItemImages.RelicOfDagda },
        { RelicOfFireworks, ItemImages.RelicOfFireworks },
        { RelicOfVass, ItemImages.RelicOfVass },
        { RelicOfTheFirebrand, ItemImages.RelicOfTheFirebrand },
        { RelicOfIsgarrenTargetBuff, ItemImages.RelicOfIsgarren },
        { RelicOfTheDragonhunterTargetBuff, ItemImages.RelicOfTheDragonhunter },
        { RelicOfPeithaTargetBuff, ItemImages.RelicOfPeitha },
        { RelicOfPeithaBlade, ItemImages.RelicOfPeitha },
        { MabonsStrength, ItemImages.RelicOfMabon },
        { NouryssHungerDamageBuff, ItemImages.RelicOfNourys },
        { RelicOfFireworksBuffLoss, ItemImages.RelicOfFireworksLoss },
        { RelicOfKarakosaHealing, ItemImages.RelicOfKarakosa },
        { RelicOfNayosHealing, ItemImages.RelicOfNayos },
        { RelicOfTheDefenderHealing, ItemImages.RelicOfTheDefender },
        { RelicOfTheFoundingBarrier, ItemImages.RelicOfTheFounding },
        { RelicOfTheTwinGenerals, ItemImages.RelicOfTheTwinGenerals },
        { RelicOfTheClawBuffLoss, ItemImages.RelicOfTheClawLoss },
        { RelicOfTheClaw, ItemImages.RelicOfTheClaw },
        { RelicOfTheBlightbringer, ItemImages.RelicOfTheBlightbringer },
        { RelicOfSorrowBuff, ItemImages.RelicOfTheSorrow },
        { RelicOfSorrowHeal, ItemImages.RelicOfTheSorrow },
        { RelicOfTheStormsingerChain, ItemImages.RelicOfTheStormsinger },
        { RelicOfTheBeehive, ItemImages.RelicOfTheBeehive },
        { RelicOfMountBalrior, ItemImages.RelicOfMountBalrior },
#endregion RelicIcons
        #region ElementalistIcons
        { DualFireAttunement, SkillImages.FireAttunement },
        { FireWaterAttunement, SkillImages.FireWaterAttunement },
        { FireAirAttunement, SkillImages.FireAirAttunement },
        { FireEarthAttunement, SkillImages.FireEarthAttunement },
        { DualWaterAttunement, SkillImages.WaterAttunement },
        { WaterFireAttunement, SkillImages.WaterFireAttunement },
        { WaterAirAttunement, SkillImages.WaterAirAttunement },
        { WaterEarthAttunement, SkillImages.WaterEarthAttunement },
        { DualAirAttunement, SkillImages.AirAttunement },
        { AirFireAttunement, SkillImages.AirFireAttunement },
        { AirWaterAttunement, SkillImages.AirWaterAttunement },
        { AirEarthAttunement, SkillImages.AirEarthAttunement },
        { DualEarthAttunement, SkillImages.EarthAttunement },
        { EarthFireAttunement, SkillImages.EarthFireAttunement },
        { EarthWaterAttunement, SkillImages.EarthWaterAttunement },
        { EarthAirAttunement, SkillImages.EarthAirAttunement },
        { EarthenBlast, TraitImages.EarthenBlast },
        { ElectricDischarge, TraitImages.ElectricDischarge },
        { LightningJolt, SkillImages.OverloadAir },
        { ShatteringIceDamage, SkillImages.ShatteringIce },
        { ArcaneShieldDamage, SkillImages.ArcaneShield },
        { ConeOfColdHealing, SkillImages.CondOfCold },
        { HealingRipple, TraitImages.HealingRipple },
        { HealingRain, SkillImages.HealingRain },
        { ElementalRefreshmentBarrier, TraitImages.ElementalRefreshment },
        { FirestormGlyphOfStormsOrFieryGreatsword, SkillImages.FirestormGlyphOfStormsOrFieryGreatsword },
        { LightningStrikeWvW, SkillImages.LightningStrike },
        { ChainLightningWvW, SkillImages.ChainLightning },
        { FlameBurstWvW, SkillImages.FlameBurst },
        { MistForm, SkillImages.MistForm },
        { VaporForm, SkillImages.VaporForm },
        { FieryRushLeap, SkillImages.FieryRush },
        { LesserCleansingFire, SkillImages.CleansingFire },
        { FlashFreezeDelayed, SkillImages.FlashFreeze },
#endregion  ElementalistIcons
        #region EngineerIcons
        { ShredderGyroHit, SkillImages.ShredderGyro },
        { ShredderGyroDamage, SkillImages.ShredderGyro },
        { HealingMistOrSoothingDetonation, SkillImages.HealingMistOrSoothingDetonation },
        { ThermalReleaseValve, TraitImages.ThermalReleaveValve },
        { RefractionCutterBlade, SkillImages.RefractionCutter },
        { MechCoreBarrierEngine, TraitImages.MechCoreBarrierEngine },
        { JumpShotEOD, SkillImages.JumpShot },
        { MedBlasterHeal, SkillImages.MedBlaster },
        { SoothingDetonation, TraitImages.SoothingDetonation },
        { HealingTurretHeal, SkillImages.HealingTurret },
        { HardStrikeJadeMech, SkillImages.HardStrikeJadeMech },
        { HeavySmashJadeMech, SkillImages.HeavySmashJadeMech },
        { TwinStrikeJadeMech, SkillImages.TwinStrikeJadeMech },
        { RecallMech_MechSkill, SkillImages.RecallMech },
        { RapidRegeneration, TraitImages.RapidRegeneration },
        { BladeBurstOrParticleAccelerator, SkillImages.BladeBurstOrParticleAccelerator },
        { MedicalDispersionFieldHeal, TraitImages.MedicalDispersionField },
        { ImpactSavantBarrier, TraitImages.ImpactSavant },
        { JumpShotWvW, SkillImages.JumpShot },
        { NetAttack, SkillImages.NetAttack },
        { FlameTurretDamage, SkillImages.FlameTurret },
        { StaticShield, SkillImages.StaticShield },
        { NetTurretDamageUW, SkillImages.NetTurret },
        { JadeEnergyShot1JadeMech, SkillImages.Anchor },
        { JadeEnergyShot2JadeMech, SkillImages.Anchor },
        { RifleBurstGrenadeDamage, SkillImages.GrenadeBarrage },
        { GyroExplosion, SkillImages.FunctionGyro },
        { DetonateThrowMineOrMineField, SkillImages.DetonateMineField },
        { ConduitSurge, SkillImages.ConduitSurge },
        { CrashDown2, SkillImages.CrashDown },
        { SystemShockerBarrier, TraitImages.SystemShocker },
#endregion EngineerIcons
            #region GuardianIcons
            { ProtectorsStrikeCounterHit, SkillImages.ProtectorsStrike },
            { SwordOfJusticeDamage, SkillImages.SwordOfJustice },
            { GlacialHeart, TraitImages.GlacialHeart },
            { GlacialHeartHeal, TraitImages.GlacialHeart },
            { ShatteredAegis, TraitImages.ShatteredAegis },
            { SelflessDaring, TraitImages.SelflessDaring },
            { HuntersVerdictPull, SkillImages.HuntersVerdict },
            { Chapter1SearingSpell, SkillImages.Chapter1SearingSpell },
            { Chapter2IgnitingBurst, SkillImages.Chapter2IgnitingBurst },
            { Chapter3HeatedRebuke,  SkillImages.Chapter3HeatedRebuke },
            { Chapter4ScorchedAftermath, SkillImages.Chapter4ScorchedAftermath },
            { EpilogueAshesOfTheJust, SkillImages.EpilogueAshesOfTheJust },
            { Chapter1DesertBloomHeal, SkillImages.Chapter1DesertBloom },
            { Chapter1DesertBloomSkill, SkillImages.Chapter1DesertBloom },
            { Chapter2RadiantRecovery, SkillImages.Chapter2RadiantRecovery },
            { Chapter2RadiantRecoveryHealing, SkillImages.Chapter2RadiantRecovery },
            { Chapter3AzureSun, SkillImages.Chapter3AzureSun },
            { Chapter4ShiningRiver, SkillImages.Chapter4ShiningRiver },
            { EpilogueEternalOasis, SkillImages.EpilogueEternalOasis },
            { Chapter1UnflinchingCharge, SkillImages.Chapter1UnflinchingCharge },
            { Chapter2DaringChallenge,  SkillImages.Chapter2DaringChallenge },
            { Chapter3ValiantBulwark, SkillImages.Chapter3ValiantBulwark },
            { Chapter4StalwartStand, SkillImages.Chapter4StalwartStand },
            { EpilogueUnbrokenLines, SkillImages.EpilogueUnbrokenLines },
            { FlameRushOld, SkillImages.FlameRush_Old },
            { FlameSurgeOld, SkillImages.FlameSurge_Old },
            { MantraOfTruthDamage, SkillImages.EchoOfTruth },
            { RestoringReprieveOrRejunevatingRespite, SkillImages.RestoringReprieveOrRejunevatingRespite },
            { OpeningPassageOrClarifiedConclusion, SkillImages.OpeningPassageOrClarifiedConclusion },
            { PotentHasteOrOverwhelmingCelerity, SkillImages.PotentHasteOrOverwhelmingCelerity },
            { PortentOfFreedomOrUnhinderedDelivery, SkillImages.PortentOfFreedomOrUnhinderedDelivery },
            { RushingJusticeStrike, SkillImages.RushingJustice },
            { ExecutionersCallingDualStrike, SkillImages.ExecutionersCalling },
            { AdvancingStrikeSkill, SkillImages.AdvancingStrike },
            { SwordOfJusticeSomething, SkillImages.SwordOfJustice },
            { ShieldOfTheAvengerSomething, SkillImages.ShieldOfTheAvenger },
            { ShieldOfTheAvengerShatter, SkillImages.ShieldOfTheAvenger },
            { VirtueOfJusticePassiveBurn, SkillImages.VirtueOfJustice },
            { HuntersWardImpacts, SkillImages.HuntersWard },
            { FireJurisdictionLevel1, SkillImages.FireJurisdiction },
            { FireJurisdictionLevel2, SkillImages.FireJurisdiction },
            { FireJurisdictionLevel3, SkillImages.FireJurisdiction },
            { DaybreakingSlashWave, SkillImages.DaybreakingSlash },
            { BindingBladeSelf, SkillImages.BindingBlade },
            { ReceiveTheLightPulse, SkillImages.ReceiveTheLight },
            { SolarStormIlluminatedHealing, SkillImages.SolarStorm },
#endregion GuardianIcons
            #region MesmerIcons
            { HealingPrism, TraitImages.HealingPrism },
            { SignetOfTheEther, SkillImages.SignetOfTheEther },
            { BlinkOrPhaseRetreat, SkillImages.BlinkOrPhaseRetreat },
            { MirageCloakDodge, SkillImages.MirageCloak },
            { UnstableBladestormProjectiles, SkillImages.UnstableBladestorm },
            { PhantasmalBerserkerProjectileDamage, SkillImages.PhantasmalBerserker },
            { PhantasmalBerserkerPhantasmDamage, SkillImages.PhantasmalBerserker },
            { WindsOfChaosStaffClone, SkillImages.WindOfChaos },
            { IllusionaryUnload, SkillImages.Unload },
            { IllusionaryRiposteHit, SkillImages.IllusionaryRiposte },
            { IllusionarySwordAttack, SkillImages.MageStrike },
            { BlurredFrenzySwordman, SkillImages.BlurredFrenzy },
            { MindSlashSwordClone, SkillImages.MindSlash },
            { MindGashSwordClone, SkillImages.MindGash },
            { MindStabSwordClone, SkillImages.MindStab },
            { WhirlingDefensesIllusionaryWarden, SkillImages.WhirlingDefense },
            { CutterBurst, SkillImages.FlyingCutter },
            { CutterBurstMindblade, SkillImages.FlyingCutter },
            { RestorativeMantras, TraitImages.RestorativeMantras },
            { SignetOfTheEtherHeal, SkillImages.SignetOfTheEther },
            { IllusionaryInspiration, TraitImages.IllusionaryInspiration },
            { BackFire, SkillImages.PhantasmalMage },
            { MageStrike, SkillImages.MageStrike },
            { IllusionaryLeap, SkillImages.IllusionaryLeap },
            { DisenchantingBolt, SkillImages.PhantasmalDisenchanter },
            { IllusionaryCounterHit, SkillImages.IllusionaryCounter },
            { SpatialSurgeAdditional, SkillImages.SpatialSurge },
            { EtherBolt, SkillImages.EherBolt },
            { PhantasmalWhalersVolley, SkillImages.PhantasmalWhaler },
            { IllusionOfLifeBuff, SkillImages.IllusionOfLife },
            { MindBlast, SkillImages.MindBlast },
            { PowerBlock, TraitImages.PowerBlock },
            { PhantasmalMageAttack,  SkillImages.PhantasmalMage },
            { PhantasmalDuelistAttack, SkillImages.PhantasmalDuelist },
            { FlyingCutterExtra, SkillImages.FlyingCutter },
            { EchoOfMemoryExtra, SkillImages.EchoOfMemory },
            { SplitSurgeSecondaryBeams, SkillImages.SplitSurge },
            { PersistenceOfMemory, TraitImages.PersistenceOfMemory },
            { FriendlyFireIllu, SkillImages.FriendlyFire },
            { PhantasmalSharpshooterAttack,SkillImages.PhantasmalSharpshooter },
            #endregion  MesmerIcons
            #region NecromancerIcons
            { LifeFromDeath, TraitImages.LifeFromDeath },
            { ChillingNova, TraitImages.ChillingNova },
            { ManifestSandShadeShadeHit, SkillImages.ManifestSandShade },
            { NefariousFavorSomething, SkillImages.NefariousFavor },
            { NefariousFavorShadeHit, SkillImages.NefariousFavor },
            { SandCascadeBarrier, SkillImages.SandCascade },
            { SandCascadeShadeHit, SkillImages.SandCascade },
            { GarishPillarHit, SkillImages.GarishPillars },
            { GarishPillarShadeHit, SkillImages.GarishPillars },
            { DesertShroudHit, SkillImages.DesertShroud },
            { SandstormShroudHit, SkillImages.SandstormShroud },
            { SandFlare, SkillImages.SandFlare },
            { DesertEmpowerment, TraitImages.DesertEmpowerment },
            { CascadingCorruption, TraitImages.CascadingCorruption },
            { DeathlyHaste, TraitImages.DeathlyHaste },
            { ApproachingDoom, TraitImages.DoomApproaches },
            { UnstableExplosion, SkillImages.SummonMadness },
            { SadisticSearing, TraitImages.SadisticSearing },
            { SadisticSearingActivation, TraitImages.SadisticSearing },
            { SoulEater, TraitImages.SoulEater },
            { LesserSignetOfVampirism, SkillImages.SignetOfVampirism },
            { SignetOfVampirismSkill2, SkillImages.SignetOfVampirism },
            { SignetOfVampirismHeal2, SkillImages.SignetOfVampirism },
            { SignetOfVampirismHeal, SkillImages.SignetOfVampirism },
            { LifeTransferSomething, SkillImages.LifeTransfer },
            { MarkOfBloodOrChillblainsTrigger, SkillImages.MarkOfBloodOrChillblains },
            { VampiricStrikes, TraitImages.VampiricPresence },
            { LifeBlast, SkillImages.LifeBlast },
            { FiendLeechWvW, SkillImages.SummonBloodFiend },
            { BoneSlash, SkillImages.BoneSlash },
            { NecroticBite1, SkillImages.NecroticBite },
            { NecroticBite2, SkillImages.NecroticBite },
            { UnholyFeastSomething, SkillImages.UnholyFeast },
            { VampiricStrikes2, TraitImages.Vampiric },
            { TaintedShacklesTicks, SkillImages.TaintedShackles },
            { BloodBank, TraitImages.BloodBank },
            { ShamblingSlash, SkillImages.ShamblingSlash },
            { AuguryOfDeath, TraitImages.AuguryOfDeath },
            { SoulSpiralHeal, SkillImages.SoulSpiral },
#endregion  NecromancerIcons
            #region RangerIcons
            // Ranger
            { WindborneNotes, TraitImages.WindborneNotes },
            { InvigoratingBond, TraitImages.InvigoratingBond },
            { OpeningStrike, TraitImages.OpeningStrike },
            { RuggedGrowth, TraitImages.RuggedGrowth },
            { AquaSurge_Player, SkillImages.AquaSurge },
            { AquaSurge_WaterSpiritNPC, SkillImages.AquaSurge },
            { SolarFlare_Player, SkillImages.SolarFlare },
            { SolarFlare_SunSpiritNPC, SkillImages.SolarFlare },
            { Quicksand_Player, SkillImages.Quicksand },
            { Quicksand_StoneSpiritNPC, SkillImages.Quicksand },
            { ColdSnap_Player, SkillImages.ColdSnap},
            { ColdSnap_FrostSpiritNPC, SkillImages.ColdSnap },
            { CallLightning_Player, SkillImages.CallLightning },
            { CallLightning_StormSpiritNPC, SkillImages.CallLightning },
            { NaturesRenewal_Player, SkillImages.NaturesRenewal },
            { NaturesRenewal_SpiritOfNatureRenewalNPC, SkillImages.NaturesRenewal },
            { NaturesRenewalHealing, SkillImages.NaturesRenewal },
            { SignetOfRenewalBuff, SkillImages.SignetOfRenewal },
            { EntangleDamage, SkillImages.Entangle },
            { SpiritOfNature, SkillImages.SpiritOfNature },
            { ConsumingBite, SkillImages.ConsumingBite },
            { NarcoticSpores,  SkillImages.NarcoticSpores },
            { CripplingAnguish, SkillImages.CripplingAnguish },
            { KickGazelle, SkillImages.KickGazelle },
            { ChargeGazelle, SkillImages.ChargeGazelle },
            { HeadbuttGazelle, SkillImages.HeadbuttGazelle },
            { SolarBeam, SkillImages.SolarBeam },
            { AstralWispAttachment, SkillImages.AstralWisp },
            { CultivatedSynergyPlayer, TraitImages.CultivatedSynergy },
            { CultivatedSynergyPet, TraitImages.CultivatedSynergy },
            { BloodMoonDaze, TraitImages.BloodMoon },
            { LiveVicariously, TraitImages.LiveVicariously },
            { NaturalHealing, TraitImages.NaturalHealing },
            { GlyphOfUnityCA, SkillImages.GlyphOfUnityCelestialAvatar },
            { ChargeGazelleMergeImpact, SkillImages.ChargeGazelle },
            { SmokeAssaultMergeHit, SkillImages.SmokeAssault },
            { OneWolfPackDamage, SkillImages.OneWolfPack },
            { PredatorsCunning, TraitImages.PredatorsCunning },
            { OverbearingSmashLeap, SkillImages.OverbearingSmash },
            { UnleashedOverbearingSmashLeap, SkillImages.UnleashedOverbearingSmash },
            { RangerPetSpawned, SkillImages.PetSpawn },
            { QuickDraw, TraitImages.QuickDraw },
            { WingSwipeWyvern, SkillImages.WingSwipeWyvern },
            { WingBuffetWyvern, SkillImages.WingBuffetWyvern },
            { TailLashWyvern, SkillImages.TailLashWyvern },
            { TailLashDevourer, SkillImages.TailLashDevourer },
            { TwinDartsDevourer, SkillImages.TwinDartsDevourer },
            { RetreatDevourer, SkillImages.RetreatDevourer },
            { BlackHoleMinion, SkillImages.BlackHole },
            { JacarandasEmbraceMinion, SkillImages.JacarandasEmbrace },
            { CallLightningJacaranda, SkillImages.CallLightningJacaranda },
            { RootSlap, SkillImages.RootSlap },
            { Peck, SkillImages.Peck },
            { FrenziedAttack, SkillImages.FrenziedAttack },
            { SignetOfRenewalHeal, SkillImages.SignetOfRenewal },
            { HeavyShotTurtle, SkillImages.HeavyShotTurtle },
            { JadeCannonTurtle, SkillImages.JadeCannonTurtle },
            { SlamTurtle, SkillImages.SlamTurtle },
            { Swoop, SkillImages.Swoop },
            { TailSwipePet, SkillImages.TailSwipePet },
            { BiteCanine, SkillImages.BiteCanine },
            { BrutalChargeCanine, SkillImages.BrutalChargeCanine },
            { BiteDrake, SkillImages.BiteDrake },
            { BiteBear, SkillImages.BiteBear },
            { BiteSmokescale, SkillImages.BiteSmokescale },
            { SmokeAssaultSmokescaleSkill, SkillImages.SmokeAssault },
            { SmokeAssaultSmokescaleDamage, SkillImages.SmokeAssault },
            { MaulPorcine, SkillImages.MaulPorcine },
            { JabPorcine, SkillImages.JabPorcine },
            { BrutalChargePorcine, SkillImages.BrutalChargePorcine },
            { SlashBear, SkillImages.SlashBear },
            { SlashBird, SkillImages.SlashBird },
            { SwoopBird, SkillImages.SwoopBird },
            { BiteFeline, SkillImages.BiteFeline },
            { SlashFeline, SkillImages.MaulFeline },
            { MaulFeline, SkillImages.MaulFeline },
            { CripplingLeap, SkillImages.CripplingLeap },
            { HornetStingWvW, SkillImages.HornetSting },
            { LongRangeShotWvW, SkillImages.LongRangeShot},
            { PointBlankShotWvW, SkillImages.PointBlankShot },
            { RapidFireWvW, SkillImages.RapidFire },
            { ColdSnapFrostSpirit, SkillImages.ColdSnap },
            { QuakeStoneSpirit, SkillImages.Quicksand },
            { ExplodingSpore, SkillImages.ExplodingSpore },
            { VenomousOutburstPet, SkillImages.VenoumousOutburst },
            { EnvelopingHazePet, SkillImages.EnvelopingHaze },
            { RendingVinesPet, SkillImages.RendingVines },
            { GlyphOfUnitySomething, SkillImages.GlyphOfUnity },
            { CripplingTalonWvW, SkillImages.CripplingTalon },
            { HiltBashWvW, SkillImages.HiltBash },
            { HarmonicCry, SkillImages.HarmonicCry },
            { QuickeningScreech, SkillImages.QuickeningScreech },
            { TakedownSmokescale, SkillImages.TakedownSmokescale },
            { PhasePounceWhiteTiger, SkillImages.PhasePounceWhiteTiger },
            { PhotosynthesizeJacaranda, SkillImages.PhotosynthesizeJacaranda },
            { EvilEye, SkillImages.EvilEyeDemon },
            { TormentingVisionSpinegazer, SkillImages.TormentingVisionSpinegazer },
            { WolfsOnslaughtFollowUp, SkillImages.WolfsOnslaught },
            { EletroctuteJuvenileSkyChak, SkillImages.EletroctuteJuvenileSkyChak },
            #endregion RangerIcons
            #region RevenantIcons
            { RiftSlashRiftHit, SkillImages.RiftSlash },
            { UnrelentingAssaultMultihit, SkillImages.UnrelentingAssault },
            { EnchantedDaggers2, SkillImages.EnchantedDaggers },
            { ImpossibleOddsHit, SkillImages.ImpossibleOdds },
            { EmbraceTheDarknessDamage, SkillImages.EmbraceTheDarkness },
            { NaturalHarmony, SkillImages.NaturalHarmony },
            { NaturalHarmonyHeal, SkillImages.NaturalHarmony },
            { ProjectTranquility, SkillImages.ProjectTranquility },
            { ProjectTranquilityHeal, SkillImages.ProjectTranquility },
            { VentarisWill, SkillImages.VentarisWill },
            { DarkrazorsDaringHit, SkillImages.DarkrazorsDaring },
            { DarkrazorsDaringSkillMinion, SkillImages.DarkrazorsDaring },
            { DarkrazorsDaringSkillMinionReworked, SkillImages.DarkrazorsDaring },
            { IcerazorsIreHit, SkillImages.IcerazorsIre },
            { IcerazorsIreSkillMinion, SkillImages.IcerazorsIre },
            { IcerazorsIreSkillMinionReworked,SkillImages.IcerazorsIre },
            { BreakrazorsBastionMinion, SkillImages.BreakrazorsBastion },
            { BreakrazorsBastionSkillMinionReworked, SkillImages.BreakrazorsBastion },
            { RazorclawsRageSkillMinion, SkillImages.RazorclawsRage },
            { RazorclawsRageHitReworked, SkillImages.RazorclawsRage },
            { RazorclawsRageSkillMinionReworked, SkillImages.RazorclawsRage },
            { SoulcleavesSummitSkillMinion, SkillImages.SoulcleavesSummit },
            { SoulcleavesSummitHitReworked, SkillImages.SoulcleavesSummit },
            { KallaSummonsSaluteAnimationSkill, BuffImages.RoyalDecree },
            { KallaSummonsDespawnSkill, BuffImages.Downed },
            { PhantomsOnslaughtDamage, SkillImages.PhantomsOnslaught },
            { DeathDropDodge, SkillImages.Dodge },
            { SaintsShieldDodge, SkillImages.Dodge },
            { ImperialImpactDodge, SkillImages.Dodge },
            { LesserBanishEnchantment, SkillImages.BanishEnchantement },
            { BalanceInDiscord, TraitImages.BalanceInDiscord },
            { HealersGift, TraitImages.HealersGift },
            { EnergyExpulsionHeal, SkillImages.EnergyExpulsion },
            { PurifyingEssenceHeal, SkillImages.PurifyingEssence },
            { HealingOrbRevenant, SkillImages.RejuvenatingAssault },
            { WordsOfCensure, TraitImages.WordsOfCensure },
            { GenerousAbundanceCentaur, TraitImages.GenerousAbundance },
            { GenerousAbundanceOther, TraitImages.GenerousAbundance },
            { GlaringResolve, TraitImages.GlaringResolve },
            { ElevatedCompassion, TraitImages.ElevatedCompassion },
            { FrigidBlitzExtra, SkillImages.FrigidBlitz },
            { EchoingEruptionExtra, SkillImages.EchoingEruption },
            { PhaseTraversal2, SkillImages.PhaseTraversal },
            { CoalescenceOfRuinExtra, SkillImages.CoalescenceOfRuin },
            { AbyssalRazeUnleash, SkillImages.AbyssalRaze },
            { AbyssalStrike_SecondHit, SkillImages.AbyssalStrike },
            { BlitzMinesDrop, SkillImages.BlitzMines },
            #endregion RevenantIcons
            #region ThiefIcons
            { ThrowMagneticBomb, SkillImages.ThrowMagneticBomb },
            { DetonatePlasma, SkillImages.DetonatePlasma },
            { UnstableArtifact, SkillImages.UnstableArtifact },
            { SoulStoneVenomSkill, SkillImages.SoulStoneVenom },
            { SoulStoneVenomStrike, SkillImages.SoulStoneVenom },
            { Pitfall, SkillImages.Pitfall },
            { BasiliskVenomStunBreakbarDamage, SkillImages.BasiliskVenom },
            { Bound, TraitImages.BoundingDodger },
            { ImpalingLotus, TraitImages.LotusTraining },
            { Dash, TraitImages.UnhinderedCombatant },
            { TwilightComboSecondProjectile, SkillImages.TwilightCombo },
            { ShadowFlareDeadeyeMinion, SkillImages.ShadowFlare },
            { DoubleTapDeadeyeMinion, SkillImages.ShadowFlare },
            { BrutalAimDeadeyeMinion, SkillImages.BrutalAim },
            { DeathsJudgmentDeadeyeMinion, SkillImages.DeathsJudgment },
            { UnloadThiefMinion, SkillImages.Unload },
            { BlackPowderThiefMinion, SkillImages.BlackPowder },
            { TwistingFangThiefMinion1, SkillImages.TwistingFangs },
            { TwistingFangThiefMinion2, SkillImages.TwistingFangs },
            { TwistingFangThiefMinion3, SkillImages.TwistingFangs },
            { ScorpionWireThiefMinion, SkillImages.ScorpionWire },
            { TripleThreatSpecterMinion, SkillImages.TripleThreat },
            { ShadowBoltSpecterMinion, SkillImages.ShadowBolt },
            { DoubleBoltSpecterMinion, SkillImages.DoubleBolt },
            { TripleBoltSpecterMinion, SkillImages.TripleBolt },
            { ImpairingDaggersDaredevilMinionHit1, SkillImages.ImpairingDaggers },
            { ImpairingDaggersDaredevilMinionHit2, SkillImages.ImpairingDaggers },
            { ImpairingDaggersDaredevilMinionHit3, SkillImages.ImpairingDaggers },
            { ImpairingDaggersDaredevilMinionSkill, SkillImages.ImpairingDaggers },
            { ImpairingDaggersHit1, SkillImages.ImpairingDaggers },
            { ImpairingDaggersHit2, SkillImages.ImpairingDaggers },
            { ImpairingDaggersHit3, SkillImages.ImpairingDaggers },
            { VaultDaredevilMinion, SkillImages.Vault },
            { WeakeningChargeDaredevilMinion, SkillImages.WeakeningWhirl },
            { BoundHit, TraitImages.BoundingDodger },
            { ThievesGuildMinionDespawnSkill, BuffImages.Downed },
            { FlankingStrikeWvW, SkillImages.FlankingStrike },
            { BlindingPowderWvW, SkillImages.BlindingPowder },
            { BlackPowderWvW, SkillImages.BlackPowder },
            { ShadowAssault, SkillImages.ShadowAssault },
            { ShadowShot, SkillImages.ShadowShot },
            { ShadowStrike, SkillImages.ShadowAssault },
            { InfiltratorsStrikeSomething, SkillImages.InfiltratorsStrike },
            { BarbedSpearRanged, SkillImages.BarbedSpear },
            { TraversingDuskHeal, TraitImages.TraversingDusk },
            { DarkSaviorHealing, TraitImages.ShadowSavior },
            { ShieldingRestorationBarrier, TraitImages.ShieldingRestoration },
            #endregion ThiefIcons
            #region WarriorIcons
            { MendingMight, TraitImages.MendingMight },
            { LossAversion, TraitImages.LossAversion },
            { KingOfFires, TraitImages.KingOfFires },
            { DragonSlashBoost, SkillImages.DragonSlashBoost },
            { DragonSlashForce, SkillImages.DragonSlashForce },
            { DragonSlashReach, SkillImages.DragonSlashReach },
            { Triggerguard, SkillImages.Triggerguard },
            { FlickerStep, SkillImages.FlickerStep },
            { ExplosiveThrust, SkillImages.ExplosiveThrust },
            { SteelDivide, SkillImages.SteelDivide },
            { SwiftCut, SkillImages.SwiftCut },
            { BloomingFire, SkillImages.BloomingFire },
            { ArtillerySlash, SkillImages.ArtillerySlash },
            { CycloneTrigger, SkillImages.CycloneTrigger },
            { BreakStep, SkillImages.BreakStep },
            { RushDamage, SkillImages.Rush },
            { DragonspikeMineDamage, SkillImages.DragonspikeMine },
            { FullCounterHit, SkillImages.FullCounter },
            { AimedShotWvW, SkillImages.AimedShot },
            { ChargeWarhornWvW, SkillImages.ChargeWarhorn },
            { MaceSmashWvW1, SkillImages.MaceSmash },
            { MaceSmashWvW2, SkillImages.MaceSmash },
            { CrushingBlowWvW1, SkillImages.CrushingBlow },
            { CrushingBlowWvW2, SkillImages.CrushingBlow },
            { PulverizeWvW1, SkillImages.Pulverize },
            { PulverizeWvW2, SkillImages.Pulverize },
            { BannerOfDefenseBarrier, SkillImages.BannerOfDefense },
            { ShieldBashWvW, SkillImages.ShieldBash },
            { BolaShotWvW, SkillImages.BolaShot },
            { ThrowBolasWvW, SkillImages.ThrowBolas },
            { CallToArmsWvW, SkillImages.CallOfValor },
            { RifleButtWvW, SkillImages.RifleButt },
            { VolleyWvW, SkillImages.Volley },
            { BleedingShotWvW, SkillImages.FierceShot },
            { BolaTossWvW, SkillImages.BolaShot },
            { MagebaneTetherBuff, TraitImages.MagebaneTether },
            { MagebaneTetherSkill, TraitImages.MagebaneTether },
            { EnchantmentCollapse, TraitImages.EnchantmentCollapse },
            { LineBreakerHeal, SkillImages.LineBreaker },
            { VigorousShouts, TraitImages.VigorousShouts },
            { MightyThrowScatter, SkillImages.MightyThrow },
            { WildThrowExtra, SkillImages.WildThrow },
            { SpearmarshalsSupportBombard, SkillImages.SpearmarshalsSupport },
            { ShrugItOffHeal, TraitImages.ShrugItOff },
            { BerserkEndSkill, SkillImages.BerserkerEnd },
            #endregion WarriorIcons
            #region EncounterIcons
            // Silent Surf Fractal
            { GrapplingHook, SkillImages.ScorpionWire },
            { Parachute, "https://wiki.guildwars2.com/images/f/fd/Feathers_%28skill%29.png" },
            { BlackPowderCharge, "https://wiki.guildwars2.com/images/7/75/Powder_Keg_%28skill%29.png" },
            { FlareSilentSurf, "https://wiki.guildwars2.com/images/2/21/Reclaimed_Energy.png" },
            // Special Action Keys
            // - Training Area
            { MushroomKingsBlessing, "https://wiki.guildwars2.com/images/8/86/Cap_Hop.png" },
            // - Icebrood Saga
            { SpiritNovaTier1, BuffImages.SpiritNova },
            { SpiritNovaTier2, BuffImages.SpiritNova },
            { SpiritNovaTier3, BuffImages.SpiritNova },
            { SpiritNovaTier4, BuffImages.SpiritNova },
            { NightTerrorTier1, BuffImages.NightTerror },
            { NightTerrorTier2, BuffImages.NightTerror },
            { NightTerrorTier3, BuffImages.NightTerror },
            { NightTerrorTier4, BuffImages.NightTerror },
            { ShatteredPsycheTier1, BuffImages.ShatteredPsyche },
            { ShatteredPsycheTier2, BuffImages.ShatteredPsyche },
            { ShatteredPsycheTier3, BuffImages.ShatteredPsyche },
            { ShatteredPsycheTier4, BuffImages.ShatteredPsyche },
            // - Sabetha
            { SapperBombSkill, BuffImages.SapperBomb },
            // - Slothasor
            { PurgeSlothasor, "https://wiki.guildwars2.com/images/a/aa/Purge.png" },
            { Eat, "https://wiki.guildwars2.com/images/7/7b/Eat.png" },
            // - Bandit Trio
            { Beehive, BuffImages.ThrowJar },
            { ThrowOilKeg, "https://wiki.guildwars2.com/images/5/5f/Throw_Keg.png" },
            // - Matthias
            { UnstableBloodMagic, "https://wiki.guildwars2.com/images/a/aa/Purge.png" },
            // - Escort Glenna
            { OverHere, "https://wiki.guildwars2.com/images/b/b7/Over_Here%21.png" },
            { HaresSpeedSkill, "https://wiki.guildwars2.com/images/0/05/Hare%27s_Speed.png" },
            // - Xera
            { InterventionSAK, "https://wiki.guildwars2.com/images/3/3f/Intervention.png" },
            // - Cairn
            { CelestialDashSAK, SkillImages.CelestialDash },
            // - Mursaar Overseer
            { ClaimSAK, BuffImages.Claim },
            { DispelSAK, BuffImages.Dispel },
            { ProtectSAK, BuffImages.Protect },
            // - Soulless Horror
            { IssueChallengeSAK, "https://wiki.guildwars2.com/images/1/13/Rally_the_Crowd.png" },
            // Eater of Souls
            { ReclaimedEnergySkill, BuffImages.ReclaimedEnergy },
            // Eyes of Judgment
            { ThrowLight, "https://wiki.guildwars2.com/images/8/8c/Throw_Light.png" },
            { Flare, "https://wiki.guildwars2.com/images/5/54/Flare.png" },
            // - Dhuum
            { ExpelEnergySAK, "https://wiki.guildwars2.com/images/c/c1/Core_Capture.png" },
            // - Conjured Amalgamate
            { ConjuredSlashPlayer, "https://wiki.guildwars2.com/images/5/59/Conjured_Slash.png" },
            { ConjuredProtection, "https://wiki.guildwars2.com/images/0/02/Conjured_Protection.png" },
            { ConjuredSlashSAK, "https://wiki.guildwars2.com/images/5/59/Conjured_Slash.png" },
            { ConjuredProtectionSAK, "https://wiki.guildwars2.com/images/0/02/Conjured_Protection.png" },
            // - Sabir
            { FlashDischargeSAK, "https://wiki.guildwars2.com/images/5/59/Flash_Discharge.png" },
            // - Qadim the Peerless
            { FluxDisruptorActivateCast, "https://wiki.guildwars2.com/images/d/d5/Flux_Disruptor-_Activate.png" },
            { FluxDisruptorDeactivateCast, "https://wiki.guildwars2.com/images/3/34/Flux_Disruptor-_Deactivate.png" },
            { PlayerLiftUpQadimThePeerless, ParserIcons.GenericBlueArrowUp },
            { UnleashSAK, "https://wiki.guildwars2.com/images/9/99/Touch_of_the_Sun.png" },
            // - Ura
            { UraDispelSAK, "https://wiki.guildwars2.com/images/0/07/Consume_Bloodstone_Fragment.png" },
            // - Mai Trin (Aetherble Hideout)
            { ReverseThePolaritySAK, "https://wiki.guildwars2.com/images/f/f8/Prod.png" },
            // - Dadga (Cosmic Observatory)
            { PurifyingLight, "https://wiki.guildwars2.com/images/1/1e/Purifying_Light_%28Dagda%29.png" },
            // - Artsariiv
            { NovaLaunchSAK, SkillImages.CelestialDash },
            // - Arkk
            { HypernovaLaunchSAK, SkillImages.CelestialDash },
            // Freezie
            { FireSnowball, "https://wiki.guildwars2.com/images/d/d0/Fire_Snowball.png" },
            // Generic Encounter Skills
            // - Ura
            { ThrummingPresenceDamage, BuffImages.ConjuredBarrier },
            { ThrummingPresenceBuff, BuffImages.ConjuredBarrier },
            #endregion  EncounterIcons
            #region WvWIcons
            { WvWSpendingSupplies, "https://wiki.guildwars2.com/images/b/b7/Repair_Master.png" },
            { WvWPickingUpSupplies, "https://wiki.guildwars2.com/images/9/94/Supply.png" },
            { DeployTrapWvW, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { ImprovedPouredTar, "https://wiki.guildwars2.com/images/1/1a/Pour_Tar.png" },
            { EngulfingBurningOil, "https://wiki.guildwars2.com/images/6/6d/Pour_Oil.png" },
            { FirePackedIncendiaryShells, "https://wiki.guildwars2.com/images/e/e0/Fire_Incendiary_Shells.png" },
            { HollowedBoulderShot, "https://wiki.guildwars2.com/images/4/4d/Fire_Boulder.png" },
            { FireHealingOasis, "https://wiki.guildwars2.com/images/4/49/Healing_Oasis.png" },
            { SmokeScreen, "https://wiki.guildwars2.com/images/0/0a/Smoke_Screen_%28engineer_skill%29.png" },
            { SiegeBubble, "https://wiki.guildwars2.com/images/4/4d/Siege_Bubble.png" },
            { EjectionSeat, "https://wiki.guildwars2.com/images/b/ba/Eject.png" },
            { PunchGolem, "https://wiki.guildwars2.com/images/8/8f/Punch.png" },
            { BombShell, "https://wiki.guildwars2.com/images/b/b0/Bomb_Shell.png" },
            { GatlingFists1, "https://wiki.guildwars2.com/images/8/89/Gatling_Fists.png" },
            { GatlingFists2, "https://wiki.guildwars2.com/images/8/89/Gatling_Fists.png" },
            { BoilingOil, "https://wiki.guildwars2.com/images/6/6d/Pour_Oil.png" },
            // - Cannon
            { FireCannon, "https://wiki.guildwars2.com/images/5/5e/Fire_%28Cannon%29.png" },
            { FireCannonStrips1, "https://wiki.guildwars2.com/images/5/5e/Fire_%28Cannon%29.png" },
            { FireCannonStrips2, "https://wiki.guildwars2.com/images/5/5e/Fire_%28Cannon%29.png" },
            { FireCannonRadius, "https://wiki.guildwars2.com/images/5/5e/Fire_%28Cannon%29.png" },
            { Grapeshot, "https://wiki.guildwars2.com/images/4/46/Fire_Grapeshot.png" },
            { GrapeshotDamage, "https://wiki.guildwars2.com/images/4/46/Fire_Grapeshot.png" },
            { GrapeshotDamageDoubleBleeds1, "https://wiki.guildwars2.com/images/4/46/Fire_Grapeshot.png" },
            { GrapeshotDamageDoubleBleeds2, "https://wiki.guildwars2.com/images/4/46/Fire_Grapeshot.png" },
            { IceShot1, "https://wiki.guildwars2.com/images/9/9f/Ice_Shot.png" },
            { IceShot2, "https://wiki.guildwars2.com/images/9/9f/Ice_Shot.png" },
            { IceShotDamage, "https://wiki.guildwars2.com/images/9/9f/Ice_Shot.png" },
            { IceShotRadiusDamage, "https://wiki.guildwars2.com/images/9/9f/Ice_Shot.png" },
            // - Mortar
            { TurnLeftMortar, "https://wiki.guildwars2.com/images/4/4c/Turn_Left.png" },
            { TurnRightMortar, "https://wiki.guildwars2.com/images/4/4c/Turn_Right.png" },
            { ConcussionBarrageDamage, "https://wiki.guildwars2.com/images/e/e0/Fire_Incendiary_Shells.png" },
            { ConcussionBarrageSkill, "https://wiki.guildwars2.com/images/e/e0/Fire_Incendiary_Shells.png" },
            { FireExplosiveShellsDamage, "https://wiki.guildwars2.com/images/2/2a/Fire_Exploding_Shells.png" },
            { FireExplosiveShellsSkill, "https://wiki.guildwars2.com/images/2/2a/Fire_Exploding_Shells.png" },
            // - Arrow Cart
            { DeployArrowCart, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { DeploySuperiorArrowCart, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { DeployGuildArrowCart, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { VolleyArrowCart, "https://wiki.guildwars2.com/images/b/be/Fire_%28Arrow_Cart%29.png" },
            { FireImprovedArrows, "https://wiki.guildwars2.com/images/b/be/Fire_%28Arrow_Cart%29.png" },
            { FireDistantVolley, "https://wiki.guildwars2.com/images/b/be/Fire_%28Arrow_Cart%29.png" },
            { FireDevastatingArrows, "https://wiki.guildwars2.com/images/b/be/Fire_%28Arrow_Cart%29.png" },
            { CripplingVolley, "https://wiki.guildwars2.com/images/6/63/Fire_Crippling_Arrows.png" },
            { FireImprovedCripplingArrows, "https://wiki.guildwars2.com/images/6/63/Fire_Crippling_Arrows.png" },
            { FireReapingArrow, "https://wiki.guildwars2.com/images/6/63/Fire_Crippling_Arrows.png" },
            { FireStaggeringArrows, "https://wiki.guildwars2.com/images/6/63/Fire_Crippling_Arrows.png" },
            { FireSufferingArrows, "https://wiki.guildwars2.com/images/6/63/Fire_Crippling_Arrows.png" },
            { BarbedVolley, "https://wiki.guildwars2.com/images/6/63/Fire_Crippling_Arrows.png" },
            { FireImprovedBarbedArrows, "https://wiki.guildwars2.com/images/f/f2/Fire_Barbed_Arrows.png" },
            { FirePenetratingSniperArrows, "https://wiki.guildwars2.com/images/f/f2/Fire_Barbed_Arrows.png" },
            { FireExsanguinatingArrows, "https://wiki.guildwars2.com/images/f/f2/Fire_Barbed_Arrows.png" },
            { FireMercilessArrows, "https://wiki.guildwars2.com/images/f/f2/Fire_Barbed_Arrows.png" },
            { ToxicUnveilingVolley, "https://wiki.guildwars2.com/images/a/a4/Toxic_Unveiling_Volley.png" },
            // - Ballista
            { DeployBallista, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { DeploySuperiorBallista, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { DeployGuildBallista, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { BallistaBolt, "https://wiki.guildwars2.com/images/8/8f/Fire_%28Ballista%29.png" },
            { SwiftBoltDamage, "https://wiki.guildwars2.com/images/8/8f/Fire_%28Ballista%29.png" },
            { SniperBoltDamage, "https://wiki.guildwars2.com/images/8/8f/Fire_%28Ballista%29.png" },
            { ImprovedShatteringBoltDamage, "https://wiki.guildwars2.com/images/6/6b/Fire_Shattering_Bolt.png" },
            { GreaterShatteringBoltDamage, "https://wiki.guildwars2.com/images/6/6b/Fire_Shattering_Bolt.png" },
            { ReinforcedShotDamage, "https://wiki.guildwars2.com/images/8/89/Fire_Reinforced_Shot.png" },
            { GreaterReinforcedShotDamage, "https://wiki.guildwars2.com/images/8/89/Fire_Reinforced_Shot.png" },
            { ImprovedReinforcedShotDamage, "https://wiki.guildwars2.com/images/8/89/Fire_Reinforced_Shot.png" },
            { AntiairBallistaBoltDamage1, "https://wiki.guildwars2.com/images/5/50/Antiair_Bolt.png" },
            { AntiairBallistaBoltDamage2, "https://wiki.guildwars2.com/images/5/50/Antiair_Bolt.png" },
            { AntiairBallistaBoltDamage3, "https://wiki.guildwars2.com/images/5/50/Antiair_Bolt.png" },
            { AntiairBallistaBoltDamage4, "https://wiki.guildwars2.com/images/5/50/Antiair_Bolt.png" },
            // - Catapult
            { DeployCatapult, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { DeploySuperiorCatapult, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { DeployGuildCatapult, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { TurnLeftCatapult, "https://wiki.guildwars2.com/images/4/4c/Turn_Left.png" },
            { TurnRightCatapult, "https://wiki.guildwars2.com/images/4/4c/Turn_Right.png" },
            { FireBoulderSkill, "https://wiki.guildwars2.com/images/4/4d/Fire_Boulder.png" },
            { FireBoulder, "https://wiki.guildwars2.com/images/4/4d/Fire_Boulder.png" },
            { HeavyBoulderShot1, "https://wiki.guildwars2.com/images/4/4d/Fire_Boulder.png" },
            { HeavyBoulderShot2, "https://wiki.guildwars2.com/images/4/4d/Fire_Boulder.png" },
            { FireHeavyBoulder1, "https://wiki.guildwars2.com/images/4/4d/Fire_Boulder.png" },
            { FireHeavyBoulder2, "https://wiki.guildwars2.com/images/4/4d/Fire_Boulder.png" },
            { FireLargeHeavyBoulderSkill, "https://wiki.guildwars2.com/images/4/4d/Fire_Boulder.png" },
            { FireLargeHeavyBoulder, "https://wiki.guildwars2.com/images/4/4d/Fire_Boulder.png" },
            { FireHollowedBoulderSkill, "https://wiki.guildwars2.com/images/4/4d/Fire_Boulder.png" },
            { FireHollowedBoulder, "https://wiki.guildwars2.com/images/4/4d/Fire_Boulder.png" },
            { GravelShotSkill, "https://wiki.guildwars2.com/images/d/dd/Fire_Gravel.png" },
            { GravelShot2, "https://wiki.guildwars2.com/images/d/dd/Fire_Gravel.png" },
            { GravelShot3, "https://wiki.guildwars2.com/images/d/dd/Fire_Gravel.png" },
            { GravelShot4, "https://wiki.guildwars2.com/images/d/dd/Fire_Gravel.png" },
            { RendingGravelSkill, "https://wiki.guildwars2.com/images/d/dd/Fire_Gravel.png" },
            { FireHollowedGravel, "https://wiki.guildwars2.com/images/d/dd/Fire_Gravel.png" },
            { FireHollowedGravelSkill, "https://wiki.guildwars2.com/images/d/dd/Fire_Gravel.png" },
            { FireLargeRendingGravelSkill, "https://wiki.guildwars2.com/images/d/dd/Fire_Gravel.png" },
            { HollowedGravelShot, "https://wiki.guildwars2.com/images/d/dd/Fire_Gravel.png" },
            // - Flame Ram
            { DeployFlameRam, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { DeploySuperiorFlameRam, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { DeployGuildFlameRam, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { Ram, SkillImages.FireTrebuchet },
            { AcceleratedRam, SkillImages.FireTrebuchet },
            { ImpactSlam, "https://wiki.guildwars2.com/images/1/1f/Impact_Slam.png" },
            { WeakeningFlameBlast, "https://wiki.guildwars2.com/images/f/f8/Ring_of_Fire.png" },
            { IntenseFlameBlast, "https://wiki.guildwars2.com/images/f/f8/Ring_of_Fire.png" },
            // - Golem
            { DeployAlphaSiegeSuit, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { DeployOmegaSiegeSuit, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { DeployGuildSiegeSuit, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { PunchSiegeGolem, "https://wiki.guildwars2.com/images/8/8f/Punch.png" },
            { RocketPunch1, "https://wiki.guildwars2.com/images/6/6b/Rocket_Punch.png" },
            { RocketPunch2, "https://wiki.guildwars2.com/images/6/6b/Rocket_Punch.png" },
            { WhirlingAssaultSiegeGolem, "https://wiki.guildwars2.com/images/8/8b/Whirling_Assault.png" },
            { WhirlingInferno, "https://wiki.guildwars2.com/images/1/1b/Whirling_Inferno.png" },
            { HealingShieldBubble, "https://wiki.guildwars2.com/images/e/e3/Shield_Bubble.png" },
            { PullSiegeGolem, "https://wiki.guildwars2.com/images/4/41/Pull_%28Siege_Golem%29.png" },
            { RocketSalvo, "https://wiki.guildwars2.com/images/8/80/Rocket_Salvo.png" },
            { EjectSiegeGolem, "https://wiki.guildwars2.com/images/b/ba/Eject.png" },
            // - Shield Generator
            { DeployShieldGenerator, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { DeploySuperiorShieldGenerator, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { DeployGuildShieldGenerator, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { ForceBallDamage1, "https://wiki.guildwars2.com/images/f/f9/Force_Ball.png" },
            { ForceBallDamage2, "https://wiki.guildwars2.com/images/f/f9/Force_Ball.png" },
            { ForceBallDamage3, "https://wiki.guildwars2.com/images/f/f9/Force_Ball.png" },
            { ForceBallDamage4, "https://wiki.guildwars2.com/images/f/f9/Force_Ball.png" },
            { ForceWall1, "https://wiki.guildwars2.com/images/3/36/Force_Wall.png" },
            { ForceWall2, "https://wiki.guildwars2.com/images/3/36/Force_Wall.png" },
            // - Trebuchet
            { DeployTrebuchet, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { DeploySuperiorTrebuchet, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { DeployGuildTrebuchet, "https://wiki.guildwars2.com/images/b/bb/Deploy_Siege.png" },
            { TurnLeftTrebuchet, "https://wiki.guildwars2.com/images/4/4c/Turn_Left.png" },
            { TurnRightTrebuchet, "https://wiki.guildwars2.com/images/4/4c/Turn_Right.png" },
            { FireTrebuchetSkill, SkillImages.FireTrebuchet },
            { FireColossalExplosiveShot, SkillImages.FireTrebuchet },
            { FireCorrosiveShot1, SkillImages.FireTrebuchet },
            { FireCorrosiveShot2, SkillImages.FireTrebuchet },
            { FireMegaExplosiveShot1, SkillImages.FireTrebuchet },
            { FireMegaExplosiveShot2, SkillImages.FireTrebuchet },
            { FireMegaExplosiveShot3, SkillImages.FireTrebuchet },
            { FireTrebuchetDamage1, SkillImages.FireTrebuchet },
            { FireTrebuchetDamage2, SkillImages.FireTrebuchet },
            { FireTrebuchetDamage3, SkillImages.FireTrebuchet },
            { FireTrebuchetDamage4, SkillImages.FireTrebuchet },
            { FirePutridCow, "https://wiki.guildwars2.com/images/0/00/Fire_Rotting_Cow.png" },
            { FireRottingCow1, "https://wiki.guildwars2.com/images/0/00/Fire_Rotting_Cow.png" },
            { FireRottingCow2, "https://wiki.guildwars2.com/images/0/00/Fire_Rotting_Cow.png" },
            { FireBloatedPutridCow, "https://wiki.guildwars2.com/images/0/00/Fire_Rotting_Cow.png" },
            { RottenCow1, "https://wiki.guildwars2.com/images/0/00/Fire_Rotting_Cow.png" },
            { RottenCow2, "https://wiki.guildwars2.com/images/0/00/Fire_Rotting_Cow.png" },
            { RottenCow3, "https://wiki.guildwars2.com/images/0/00/Fire_Rotting_Cow.png" },
            { RottenCow4, "https://wiki.guildwars2.com/images/0/00/Fire_Rotting_Cow.png" },
            { RottenCow5, "https://wiki.guildwars2.com/images/0/00/Fire_Rotting_Cow.png" },
            { FireHealingOasisSkill, "https://wiki.guildwars2.com/images/4/49/Healing_Oasis.png" },
            { FireHealingOasisHealing, "https://wiki.guildwars2.com/images/4/49/Healing_Oasis.png" },
            // - Dragon Banner
            { DragonsMight, "https://wiki.guildwars2.com/images/9/9b/Dragon%27s_Might.png" },
            { DragonTalon1, "https://wiki.guildwars2.com/images/e/e9/Dragon_Talon.png" },
            { DragonTalon2, "https://wiki.guildwars2.com/images/e/e9/Dragon_Talon.png" },
            { DragonTalon3, "https://wiki.guildwars2.com/images/e/e9/Dragon_Talon.png" },
            { DragonBlast, "https://wiki.guildwars2.com/images/3/3e/Dragon_Blast.png" },
            { DragonsWings, "https://wiki.guildwars2.com/images/4/45/Dragon%27s_Wings.png" },
            { DragonsBreath, "https://wiki.guildwars2.com/images/2/29/Dragon%27s_Breath_%28skill%29.png" },
            // - Turtle Banner
            { ShellShock, "https://wiki.guildwars2.com/images/2/2f/Shell_Shock_%28skill%29.png" },
            { Shellter, "https://wiki.guildwars2.com/images/a/a4/Shell-ter.png" },
            { SavedByTheShell, "https://wiki.guildwars2.com/images/f/fc/Saved_by_the_Shell.png" },
            { ShellWall, "https://wiki.guildwars2.com/images/f/f5/Shell_Wall.png" },
            { BombShellRiverOfSouls, "https://wiki.guildwars2.com/images/b/b0/Bomb_Shell.png" },
            // - Centaur Banner
            { SpiritCentaur, "https://wiki.guildwars2.com/images/9/95/Spirit_Centaur.png" },
            { StampedeOfArrows, "https://wiki.guildwars2.com/images/f/f8/Stampede_of_Arrows.png" },
            { CentaurDash, "https://wiki.guildwars2.com/images/6/6b/Centaur_Dash.png" },
            { SpikeBarricade, "https://wiki.guildwars2.com/images/3/3f/Spike_Barricade.png" },
            { OutrunACentaur, "https://wiki.guildwars2.com/images/7/7d/Outrun_a_Centaur.png" },
            { RendingThrashCentaurBannerSkill, BuffImages.RendingThrashCentaurBanner },
            { CripplingStrikeCentaurBannerSkill, BuffImages.CripplingStrikeCentaurBanner },
            { KickDustCentaurBannerSkill, BuffImages.KickDustCentaurBanner },
            #endregion WvWIcons
            #region FinisherIcons         
            { Finisher1, ItemImages.BasicFinisher },
            { Finisher2, ItemImages.BasicFinisher },
            { RabbitRankFinisher, ItemImages.RabbitRankFinisher },
            { DeerRankFinisher, ItemImages.DeerRankFinisher },
            { DolyakRankFinisher, ItemImages.DolyakRankFinisher },
            { WolfRankFinisher, ItemImages.WolfRankFinisher },
            { TigerRankFinisher, ItemImages.TigerRankFinisher },
            { BearRankFinisher, ItemImages.BearRankFinisher },
            { SharkRankFinisher, ItemImages.SharkRankFinisher },
            { PhoenixRankFinisher, ItemImages.PhoenixRankFinisher },
            { DragonRankFinisher, ItemImages.DragonRankFinisher },
            { MordremRabbitFinisher, ItemImages.MordremRabbitFinisher },
            { MordremDeerFinisher, ItemImages.MordremDeerFinisher },
            { MordremDolyakFinisher, ItemImages.MordremDolyakFinisher },
            { MordremWolfFinisher, ItemImages.MordremWolfFinisher },
            { MordremTigerFinisher, ItemImages.MordremTigerFinisher },
            { MordremBearFinisher, ItemImages.MordremBearFinisher },
            { MordremSharkFinisher, ItemImages.MordremSharkFinisher },
            { MordremPhoenixFinisher, ItemImages.MordremPhoenixFinisher },
            { MordremDragonFinisher, ItemImages.MordremDragonFinisher },
            { WvWGoldenDolyakFinisher, ItemImages.WvWGoldenDolyakFinisher },
            { WvWSilverDolyakFinisher, ItemImages.WvWSilverDolyakFinisher },
            { ChineseWorldTournamentFinisher, ItemImages.ChineseWorldTournamentFinisher },
            { NorthAmericanWorldTournamentFinisher, ItemImages.NorthAmericanWorldTournamentFinisher },
            { EuropeanWorldTournamentFinisher, ItemImages.EuropeanWorldTournamentFinisher },
            { AscalonianLeaderFinisher, ItemImages.AscalonianLeaderFinisher },
            { CuteQuagganFinisher, ItemImages.CuteQuagganFinisher },
            { BirthdayFinisher, ItemImages.BirthdayFinisher },
            { ChoyaFinisher, ItemImages.ChoyaFinisher },
            { CowFinisher, ItemImages.CowFinisher },
            { GiftFinisher, ItemImages.GiftFinisher },
            { GolemPummelerFinisher, ItemImages.GolemPummelerFinisher },
            { GraveFinisher, ItemImages.GraveFinisher },
            { GreatJungleWurmFinisher, ItemImages.GreatJungleWurmFinisher },
            { GuildFlagFinisher, ItemImages.GuildFlagFinisher },
            { GuildShieldFinisher, ItemImages.GuildShieldFinisher },
            { HiddenMinstrelFinisher, ItemImages.HiddenMinstrelFinisher },
            { LeyLineFinisher, ItemImages.LeyLineFinisher },
            { LlamaFinisher, ItemImages.LlamaFinisher },
            { MadKingFinisher, ItemImages.MadKingFinisher },
            { MysticalDragonFinisher, ItemImages.MysticalDragonFinisher },
            { RainbowUnicornFinisher, ItemImages.RainbowUnicornFinisher },
            { RevenantFinisher, ItemImages.RevenantFinisher },
            { SandsharkFinisher, ItemImages.SandsharkFinisher },
            { ScarecrowFinisher, ItemImages.ScarecrowFinisher },
            { SnowGlobeFinisher, ItemImages.SnowGlobeFinisher },
            { SnowmanFinisher1, ItemImages.SnowmanFinisher },
            { SnowmanFinisher2, ItemImages.SnowmanFinisher },
            { SuperExplosiveFinisher, ItemImages.SuperExplosiveFinisher },
            { TwistedWatchworkFinisher, ItemImages.TwistedWatchworkFinisher },
            { WhumpTheGiantFinisher, ItemImages.WhumpTheGiantFinisher },
            { WizardLightningFinisher, ItemImages.WizardLightningFinisher },
            { SpectreFinisher, ItemImages.SpectreFinisher},
            { VigilMegalaserFinisher, ItemImages.VigilMegalaserFinisher },
            { ThornrootFinisher, ItemImages.ThornrootFinisher },
            { SanctifiedFinisher, ItemImages.SanctifiedFinisher },
            { MartialFinisher, ItemImages.MartialFinisher },
            { ToxicOffshootFinisher, ItemImages.ToxicOffshootFinisher },
            { SkrittScavengerFinisher, ItemImages.SkrittScavengerFinisher },
            { ChickenadoFinisher, ItemImages.ChickenadoFinisher },
            { PactFleetFinisher, ItemImages.PactFleetFinisher },
            { RealmPortalSpikeFinisher, ItemImages.RealmPortalSpikeFinisher },
            { AvatarOfDeathFinisher, ItemImages.AvatarOfDeathFinisher },
            { HonorGuardFinisher, ItemImages.HonorGuardFinisher },
	        #endregion FinisherIcons
    };

    private static readonly Dictionary<long, ulong> _nonCritable = new()
    {
        { LightningStrike_SigilOfAir, GW2Builds.StartOfLife },
        { FlameBlast_SigilOfFire, GW2Builds.StartOfLife },
        { FireAttunementSkill, GW2Builds.December2018Balance },
        { Mug, GW2Builds.StartOfLife },
        { PulmonaryImpactSkill, GW2Builds.HoTRelease },
        { ConjuredSlashPlayer, GW2Builds.StartOfLife },
        { LightningJolt, GW2Builds.StartOfLife },
        { Sunspot, GW2Builds.December2018Balance },
        { EarthenBlast, GW2Builds.December2018Balance },
        { ChillingNova, GW2Builds.December2018Balance },
        { LesserBanishEnchantment, GW2Builds.December2018Balance },
        { LesserEnfeeble, GW2Builds.December2018Balance },
        { SpitefulSpirit, GW2Builds.December2018Balance },
        { LesserSpinalShivers, GW2Builds.December2018Balance },
        { PowerBlock, GW2Builds.December2018Balance },
        { ShatteredAegis, GW2Builds.December2018Balance },
        { GlacialHeart, GW2Builds.December2018Balance },
        { ThermalReleaseValve, GW2Builds.December2018Balance },
        { LossAversion, GW2Builds.December2018Balance },
        { Epidemic, GW2Builds.May2017Balance },
    };

    private const string DefaultIcon = SkillImages.MonsterSkill;

    // Fields
    public readonly long ID;
    //public int Range { get; private set; } = 0;
    private readonly bool AA;

    public bool IsSwap => ID == WeaponSwap || ElementalistHelper.IsElementalSwap(ID) || RevenantHelper.IsLegendSwap(ID) || NecromancerHelper.IsDeathShroudTransform(ID) || HarbingerHelper.IsHarbingerShroudTransform(ID);
    public bool IsDodge(SkillData skillData) => IsAnimatedDodge(skillData) || ID == MirageCloakDodge;
    public bool IsAnimatedDodge(SkillData skillData) => ID == skillData.DodgeId || VindicatorHelper.IsVindicatorDodge(ID);
    public bool IsAutoAttack(ParsedEvtcLog log) => AA || FirebrandHelper.IsAutoAttack(log, ID) || BladeswornHelper.IsAutoAttack(log, ID);
    public readonly string Name = "";
    public readonly string Icon = "";
    private readonly WeaponDescriptor? _weaponDescriptor;
    public bool IsWeaponSkill => _weaponDescriptor != null;
    internal readonly GW2APISkill? ApiSkill;
    private SkillInfoEvent? _skillInfo;

    internal const string DefaultName = "UNKNOWN";

    public bool UnknownSkill => Name == DefaultName;

    // Constructor

    [Obsolete("Dont use this, testing only")] //TODO(Rennorb) @cleanup
    public SkillItem(bool swap) { ID = swap ? WeaponSwap : default; }

    internal SkillItem(long ID, string name, GW2APIController apiController)
    {
        this.ID = ID;
        Name = name.Replace("\0", "");
        ApiSkill = apiController.GetAPISkill(ID);
        //
        if (_overrideNames.TryGetValue(ID, out var overrideName))
        {
            Name = overrideName;
        }
        else if (ApiSkill != null && (UnknownSkill || Name.All(char.IsDigit)))
        {
            Name = ApiSkill.Name;
        }
        if (_overrideIcons.TryGetValue(ID, out var icon))
        {
            Icon = icon;
        }
        else
        {
            Icon = ApiSkill != null ? ApiSkill.Icon : DefaultIcon;
        }
        if (ApiSkill != null && ApiSkill.Type == "Weapon"
            && ApiSkill.WeaponType != "None" && ApiSkill.Professions.Count > 0
            && WeaponDescriptor.IsWeaponSlot(ApiSkill.Slot))
        {
            // Special handling of specter shroud as it is not done in the same way 
            var isSpecterShroud = ApiSkill.Professions.Contains("Thief") && ApiSkill.Facts.Any(x => x.Text != null && x.Text.Contains("Tethered Ally"));
            if (!isSpecterShroud)
            {
                _weaponDescriptor = new WeaponDescriptor(ApiSkill);
            }
        }
        AA = (ApiSkill?.Slot == "Weapon_1" || ApiSkill?.Slot == "Downed_1");
        if (AA)
        {
            if (ApiSkill?.Categories != null)
            {
                AA = AA && !ApiSkill.Categories.Contains("StealthAttack") && !ApiSkill.Categories.Contains("Ambush"); // Ambush in case one day it's added
            }
            if (ApiSkill?.Description != null)
            {
                AA = AA && !ApiSkill.Description.Contains("Ambush.");
            }
        }
#if DEBUG
        Name = ID + "-" + Name;
#endif
    }

    public static bool CanCrit(long id, ulong gw2Build)
    {
        if (_nonCritable.TryGetValue(id, out ulong build))
        {
            return gw2Build < build;
        }
        return true;
    }

    internal int FindFirstWeaponSet(IReadOnlyList<(int to, int from)> swaps)
    {
        int swapped = WeaponSetIDs.NoSet;
        // we started on a proper weapon set
        if (_weaponDescriptor != null)
        {
            swapped = _weaponDescriptor.FindFirstWeaponSet(swaps);
        }
        return swapped;
    }

    internal bool EstimateWeapons(WeaponSets weaponSets, int swapped, bool validForCurrentSwap)
    {
        bool keep = WeaponSetIDs.IsWeaponSet(swapped);
        if (_weaponDescriptor == null || !keep || !validForCurrentSwap || ApiSkill == null)
        {
            return false;
        }
        weaponSets.SetWeapons(_weaponDescriptor, ApiSkill, swapped);
        return true;
    }

    internal void AttachSkillInfoEvent(SkillInfoEvent skillInfo)
    {
        if (ID == skillInfo.SkillID)
        {
            _skillInfo = skillInfo;
        }
    }

    // Public Methods

    /*public void SetCCAPI()//this is 100% off the GW2 API is not a reliable source of finding skill CC
    {
        CC = 0;
        if (_apiSkill != null)
        {
            GW2APISkillDetailed apiskilldet = (GW2APISkillDetailed)_apiSkill;
            GW2APISkillCheck apiskillchec = (GW2APISkillCheck)_apiSkill;
            GW2APIfacts[] factsList = apiskilldet != null ? apiskilldet.facts : apiskillchec.facts;
            bool daze = false;
            bool stun = false;
            bool knockdown = false;
            bool flaot = false;
            bool knockback = false;
            bool launch = false;
            bool pull = false;
           
            foreach (GW2APIfacts fact in factsList)
            {
                if (daze == false)
                {
                    if (fact.text == "Daze" || fact.status == "Daze")
                    {
                        if (fact.duration < 1)
                        {
                            CC += 100;
                        }
                        else
                        {
                            CC += fact.duration * 100;
                        }
                        daze = true;
                    }

                }
                if (stun == false)
                {
                    if (fact.text == "Stun" || fact.status == "Stun")
                    {
                        if (fact.duration < 1)
                        {
                            CC += 100;
                        }
                        else
                        {
                            CC += fact.duration * 100;
                        }
                        stun = true;
                    }
                }
                if (knockdown == false)
                {
                    if (fact.text == "Knockdown" || fact.status == "Knockdown")
                    {
                        if (fact.duration < 1)
                        {
                            CC += 100;
                        }
                        else
                        {
                            CC += fact.duration * 100;
                        }
                        knockdown = true;
                    }
                }
                if (launch == false)
                {
                    if (fact.text == "Launch" || fact.status == "Launch")
                    {

                        CC += 232;//Wiki says either 232 or 332 based on duration? launch doesn't provide duration in api however
                       
                        launch = true;
                    }
                }
                if (knockback == false)
                {
                    if (fact.text == "Knockback" || fact.status == "Knockback")
                    {

                        CC += 150;//always 150 unless special case of 232 for ranger pet?
                        knockback = true;
                    }
                }
                if (pull == false)
                {
                    if (fact.text == "Pull" || fact.status == "Pull")
                    {

                        CC += 150;

                        pull = true;
                    }
                }
                if (flaot == false)
                {
                    if (fact.text == "Float" || fact.status == "Float")
                    {
                        if (fact.duration < 1)
                        {
                            CC += 100;
                        }
                        else
                        {
                            CC += fact.duration * 100;
                        }
                        flaot = true;
                    }
                }
                if (fact.text == "Stone Duration" || fact.status == "Stone Duration")
                {
                    if (fact.duration < 1)
                    {
                        CC += 100;
                    }
                    else
                    {
                        CC += fact.duration * 100;
                    }
                    
                }

            
            }
            if (ID == 30725)//toss elixir x
            {
                CC = 300;
            }
            if (ID == 29519)//MOA signet
            {
                CC = 1000;
            }
           
        }
    }*/
}
