using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIGW2API;
using GW2EIGW2API.GW2API;
using static System.Net.WebRequestMethods;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.ParsedData
{
    public class SkillItem
    {

        internal static (long, long) GetArcDPSCustomIDs(int evtcVersion)
        {
            if (evtcVersion >= ArcDPSBuilds.InternalSkillIDsChange)
            {
                return (ArcDPSDodge20220307, ArcDPSGenericBreakbar20220307);
            }
            else
            {
                return (ArcDPSDodge, ArcDPSGenericBreakbar);
            }
        }

        private static readonly Dictionary<long, string> _overrideNames = new Dictionary<long, string>()
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
            { SigilOfWater, "Sigil of Water" },
            { RuneOfNightmare, "Rune of the Nightmare" },
            { PortalEntranceWhiteMantleWatchwork, "Portal Entrance" },
            { PortalExitWhiteMantleWatchwork, "Portal Exit" },
            { PlateOfSpicyMoaWingsGastricDistress, "Plate of Spicy Moa Wings (Gastric Distress)" },
            { ThrowGunkEttinGunk, "Throw Gunk (Ettin Gunk)" },
            // Elementalist
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
            // Engineer
            { HealingMistOrSoothingDetonation, "Healing Mist or Soothing Detonation" },
            { MechCoreBarrierEngine, "Mech Core: Barrier Engine" },
            // Guardian
            { SelflessDaring, "Selfless Daring" }, // The game maps this name incorrectly to "Selflessness Daring"
            { ProtectorsStrikeCounterHit, "Protector's Strike (Counter Hit)" },
            { MantraOfSolace, "Mantra of Solace" },
            { RestoringReprieveOrRejunevatingRespite, "Restoring Reprieve or Rejunevating Respite" },
            { OpeningPassageOrClarifiedConclusion, "Opening Passage or Clarified Conclusion" },
            { PotentHasteOrOverwhelmingCelerity, "Potent Haste or Overwhelming Celerity" },
            { PortentOfFreedomOrUnhinderedDelivery, "Portent of Freedom or Unhindered Delivery" },
            { RushingJusticeStrike, "Rushing Justice (Hit)" },
            { ExecutionersCallingDualStrike, "Executioner's Calling (Dual Strike)" },
            // Mesmer
            { PowerReturn, "Power Return" },
            { PowerCleanse, "Power Cleanse" },
            { PowerBreak, "Power Break" },
            { PowerLock, "Power Lock" },
            { BlinkOrPhaseRetreat, "Blink or Phase Retreat" },
            { MirageCloakDodge, "Mirage Cloak" },
            { UnstableBladestormProjectiles, "Unstable Bladestorm (Projectile Hit)" },
            { PhantasmalBerserkerProjectileDamage, "Phantasmal Berserker (Greatsword Projectile Hit)" },
            // Necromancer
            { DesertEmpowerment, "Desert Empowerment" },
            // Ranger
	        { NaturalHealing, "Natural Healing" }, // The game does not map this one at all
            { LiveVicariously, "Live Vicariously" }, // The game maps this name incorrectly to "Vigorous Recovery"
            { AstralWispAttachment, "Astral Wisp Attachment" },
            { GlyphOfUnityCA, "Glyph of Unity (CA)" },
            { ChargeGazelleMergeTravel, "Charge (Travel Hit)" },
            { SmokeAssaultMergeHit, "Smoke Assault (Multi Hit)" },
            { OneWolfPackDamage, "One Wolf Pack (Hit)" },
            { OverbearingSmashLeap, "Overbearing Smash (Leap)" },
            // Revenant
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
            // Thief
	        { EscapistsFortitude, "Escapist's Fortitude" }, // The game maps this to the wrong skill
            { SoulStoneVenomSkill, "Soul Stone Venom" },
            { SoulStoneVenomStrike, "Soul Stone Venom (Hit)" },
            { BasiliskVenomStunBreakbarDamage, "Basilisk Venom (Stun)" },
            // Warrior
            { RushDamage, "Rush (Hit)" },
            // Special Forces Training Area
            { MushroomKingsBlessing, "Mushroom King's Blessing (PoV Only)" },
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
            // Matthias
            { ShieldHuman, "Shield (Human)" },
            { AbominationTransformation, "Abomination Transformation" },
            { ShieldAbomination, "Shield (Abomination)" },
            // Escort
            { GlennaCap, "Capture" },
            // Generic
            //{-5, "Phase out" },
            // Deimos
            //{-6, "Roleplay" },
            // Dhuum
            { MajorSoulSplit, "Major Soul Split" },
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
            //{56036, "Magma Bomb" },
            { ForceOfRetaliationCast, "Force of Retaliation Cast" },
            { PeerlessQadimTPCenter, "Teleport Center" },
            { EatPylon, "Eat Pylon" },
            { BigMagmaDrop, "Big Magma Drop" },
            // Voice and Claw
            { KodanTeleport, "Kodan Teleport" },
        };

        private static readonly Dictionary<long, string> _overrideIcons = new Dictionary<long, string>()
        {
            { WeaponSwap, "https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png" },
            { Resurrect, "https://wiki.guildwars2.com/images/3/3d/Downed_ally.png" },
            { Bandage, "https://wiki.guildwars2.com/images/0/0c/Bandage.png" },
            { ArcDPSGenericBreakbar, "https://wiki.guildwars2.com/images/a/ae/Unshakable.png" },
            { ArcDPSDodge, "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png" },
            { ArcDPSGenericBreakbar20220307, "https://wiki.guildwars2.com/images/a/ae/Unshakable.png" },
            { ArcDPSDodge20220307, "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png" },
            { WaterBlastCombo1, "https://wiki.guildwars2.com/images/thumb/f/f3/Healing.png/30px-Healing.png" },
            { WaterBlastCombo2, "https://wiki.guildwars2.com/images/thumb/f/f3/Healing.png/30px-Healing.png" },
            { LightningStrikeSigil, "https://wiki.guildwars2.com/images/c/c3/Superior_Sigil_of_Air.png" },
            { FlameBlastSigil, "https://wiki.guildwars2.com/images/5/56/Superior_Sigil_of_Fire.png" },
            { SigilOfEarth, "https://wiki.guildwars2.com/images/4/43/Superior_Sigil_of_Geomancy.png" },
            { SigilOfHydromancy, "https://wiki.guildwars2.com/images/3/33/Superior_Sigil_of_Hydromancy.png" },
            { SigilOfWater, "https://wiki.guildwars2.com/images/f/f9/Superior_Sigil_of_Water.png" },
            { RuneOfTormenting, "https://wiki.guildwars2.com/images/e/ec/Superior_Rune_of_Tormenting.png" },
            { RuneOfNightmare, "https://wiki.guildwars2.com/images/2/2e/Superior_Rune_of_the_Nightmare.png" },
            { PortalEntranceWhiteMantleWatchwork, "https://wiki.guildwars2.com/images/4/43/Watchwork_Portal_Device.png" },
            { PortalExitWhiteMantleWatchwork, "https://wiki.guildwars2.com/images/4/43/Watchwork_Portal_Device.png" },
            { PlateOfSpicyMoaWingsGastricDistress, BuffImages.PlateOfSpicyMoaWings },
            { ThrowGunkEttinGunk, "https://wiki.guildwars2.com/images/d/d9/Throw_Gunk.png" },
            //{41243, "https://wiki.guildwars2.com/images/f/fb/Full_Counter.png" },
            //{10281, "https://wiki.guildwars2.com/images/9/91/Illusionary_Riposte.png" },
            //{38769, "https://wiki.guildwars2.com/images/4/48/Phantasmal_Swordsman.png" },
            // Freezie
            { FireSnowball, "https://wiki.guildwars2.com/images/d/d0/Fire_Snowball.png" },
            // Escort
            { HaresSpeedSkill, "https://wiki.guildwars2.com/images/0/05/Hare%27s_Speed.png" },
            // Mounts
            { TailSpin, "https://wiki.guildwars2.com/images/f/f2/Tail_Spin.png" },
            // Slothasor
            { Eat, "https://wiki.guildwars2.com/images/7/7b/Eat.png" },
            // Eater of Souls
            { ReclaimedEnergySkill, BuffImages.ReclaimedEnergy },
            // Eyes of Judgment
            { ThrowLight, "https://wiki.guildwars2.com/images/8/8c/Throw_Light.png" },
            { Flare, "https://wiki.guildwars2.com/images/5/54/Flare.png" },
            // Elementalist
            { DualFireAttunement, "https://wiki.guildwars2.com/images/b/b4/Fire_Attunement.png" },
            { FireWaterAttunement, "https://i.imgur.com/ar8Hn8G.png" },
            { FireAirAttunement, "https://i.imgur.com/YU31LwG.png" },
            { FireEarthAttunement, "https://i.imgur.com/64g3rto.png" },
            { DualWaterAttunement, "https://wiki.guildwars2.com/images/3/31/Water_Attunement.png" },
            { WaterFireAttunement, "https://i.imgur.com/H1peqpz.png" },
            { WaterAirAttunement, "https://i.imgur.com/Gz1XwEw.png" },
            { WaterEarthAttunement, "https://i.imgur.com/zqX3y4c.png" },
            { DualAirAttunement, "https://wiki.guildwars2.com/images/9/91/Air_Attunement.png" },
            { AirFireAttunement, "https://i.imgur.com/4ekncW5.png" },
            { AirWaterAttunement, "https://i.imgur.com/HIcUaXG.png" },
            { AirEarthAttunement, "https://i.imgur.com/MCCrMls.png" },
            { DualEarthAttunement, "https://wiki.guildwars2.com/images/a/a8/Earth_Attunement.png" },
            { EarthFireAttunement, "https://i.imgur.com/Vgu0B54.png" },
            { EarthWaterAttunement, "https://i.imgur.com/exrTKSW.png" },
            { EarthAirAttunement, "https://i.imgur.com/Z3P8cPa.png" },
            { EarthenBlast, "https://wiki.guildwars2.com/images/e/e2/Earthen_Blast.png" },
            { ElectricDischarge, "https://wiki.guildwars2.com/images/a/a4/Electric_Discharge.png" },
            { LightningJolt, "https://wiki.guildwars2.com/images/4/4b/Overload_Air.png" },
            { ShatteringIceDamage, "https://wiki.guildwars2.com/images/6/63/Shattering_Ice.png" },
            { ArcaneShieldDamage, BuffImages.ArcaneShield },
            // Engineer
            { ShredderGyroDamage, "https://render.guildwars2.com/file/E60C094A2349552EA6F6250D9B14E69BE91E4468/1128595.png" },
            { HealingMistOrSoothingDetonation, "https://i.imgur.com/cS05J70.png" },
            { ThermalReleaseValve, "https://wiki.guildwars2.com/images/0/0c/Thermal_Release_Valve.png" },
            { RefractionCutterBlade, "https://wiki.guildwars2.com/images/1/10/Refraction_Cutter.png" },
            { MechCoreBarrierEngine, "https://wiki.guildwars2.com/images/d/da/Mech_Core-_Barrier_Engine.png" },
            { JumpShotEOD, "https://wiki.guildwars2.com/images/b/b5/Jump_Shot.png" },
            // Guardian
            { ProtectorsStrikeCounterHit, "https://wiki.guildwars2.com/images/e/e0/Protector%27s_Strike.png" },
            { SwordOfJusticeDamage, "https://wiki.guildwars2.com/images/8/81/Sword_of_Justice.png" },
            { GlacialHeart, "https://wiki.guildwars2.com/images/4/4f/Glacial_Heart.png" },
            { ShatteredAegis, "https://wiki.guildwars2.com/images/d/d0/Shattered_Aegis.png" },
            { SelflessDaring, "https://wiki.guildwars2.com/images/9/9c/Selfless_Daring.png" },
            { Chapter1SearingSpell, "https://wiki.guildwars2.com/images/d/d3/Chapter_1-_Searing_Spell.png" },
            { Chapter2IgnitingBurst, "https://wiki.guildwars2.com/images/5/53/Chapter_2-_Igniting_Burst.png" },
            { Chapter3HeatedRebuke,  "https://wiki.guildwars2.com/images/e/e7/Chapter_3-_Heated_Rebuke.png" },
            { Chapter4ScorchedAftermath, "https://wiki.guildwars2.com/images/c/c9/Chapter_4-_Scorched_Aftermath.png" },
            { EpilogueAshesOfTheJust, "https://wiki.guildwars2.com/images/6/6d/Epilogue-_Ashes_of_the_Just.png" },
            { Chapter1DesertBloomHeal, "https://wiki.guildwars2.com/images/f/fd/Chapter_1-_Desert_Bloom.png" },
            { Chapter1DesertBloomSkill, "https://wiki.guildwars2.com/images/f/fd/Chapter_1-_Desert_Bloom.png" },
            { Chapter2RadiantRecovery, "https://wiki.guildwars2.com/images/9/95/Chapter_2-_Radiant_Recovery.png" },
            { Chapter3AzureSun, "https://wiki.guildwars2.com/images/b/bf/Chapter_3-_Azure_Sun.png" },
            { Chapter4ShiningRiver, "https://wiki.guildwars2.com/images/1/16/Chapter_4-_Shining_River.png" },
            { EpilogueEternalOasis, "https://wiki.guildwars2.com/images/5/5f/Epilogue-_Eternal_Oasis.png" },
            { Chapter1UnflinchingCharge, "https://wiki.guildwars2.com/images/3/30/Chapter_1-_Unflinching_Charge.png" },
            { Chapter2DaringChallenge,  "https://wiki.guildwars2.com/images/7/79/Chapter_2-_Daring_Challenge.png" },
            { Chapter3ValiantBulwark,  "https://wiki.guildwars2.com/images/7/73/Chapter_3-_Valiant_Bulwark.png" },
            { Chapter4StalwartStand, "https://wiki.guildwars2.com/images/8/89/Chapter_4-_Stalwart_Stand.png" },
            { EpilogueUnbrokenLines, "https://wiki.guildwars2.com/images/d/d8/Epilogue-_Unbroken_Lines.png" },
            { FlameRushOld, "https://wiki.guildwars2.com/images/a/a8/Flame_Rush.png" },
            { FlameSurgeOld, "https://wiki.guildwars2.com/images/7/7e/Flame_Surge.png" },
            { MantraOfTruthDamage, "https://wiki.guildwars2.com/images/f/ff/Echo_of_Truth.png" },
            { RestoringReprieveOrRejunevatingRespite, "https://i.imgur.com/RUJNIoM.png" },
            { OpeningPassageOrClarifiedConclusion, "https://i.imgur.com/2M93tOd.png" },
            { PotentHasteOrOverwhelmingCelerity, "https://i.imgur.com/vBBKfGz.png" },
            { PortentOfFreedomOrUnhinderedDelivery, "https://i.imgur.com/b6RUVTr.png" },            
            { RushingJusticeStrike, "https://wiki.guildwars2.com/images/7/74/Rushing_Justice.png" },
            { ExecutionersCallingDualStrike, "https://wiki.guildwars2.com/images/d/da/Executioner%27s_Calling.png" },
            { AdvancingStrikeSkill, "https://wiki.guildwars2.com/images/6/6b/Advancing_Strike.png" },
            // Mesmer
            { HealingPrism, "https://wiki.guildwars2.com/images/f/f4/Healing_Prism.png" },
            { SignetOfTheEther, "https://wiki.guildwars2.com/images/7/7a/Signet_of_the_Ether.png" },
            { BlinkOrPhaseRetreat, "https://i.imgur.com/yxMF7D1.png" },
            { MirageCloakDodge, "https://wiki.guildwars2.com/images/a/a5/Mirage_Cloak_%28effect%29.png" },
            { UnstableBladestormProjectiles, "https://wiki.guildwars2.com/images/9/93/Unstable_Bladestorm.png" },
            { PhantasmalBerserkerProjectileDamage, "https://wiki.guildwars2.com/images/e/e8/Phantasmal_Berserker.png" },
            { PhantasmalBerserkerPhantasmDamage, "https://wiki.guildwars2.com/images/e/e8/Phantasmal_Berserker.png" },
            // Necromancer
            { LifeFromDeath, "https://wiki.guildwars2.com/images/5/5e/Life_from_Death.png" },
            { ChillingNova, "https://wiki.guildwars2.com/images/8/82/Chilling_Nova.png" },
            { ManifestSandShadeShadeHit, "https://wiki.guildwars2.com/images/a/a4/Manifest_Sand_Shade.png" },
            { NefariousFavorShadeHit, "https://wiki.guildwars2.com/images/8/83/Nefarious_Favor.png" },
            { SandCascadeBarrier, "https://wiki.guildwars2.com/images/1/1e/Sand_Cascade.png" },
            { SandCascadeShadeHit, "https://wiki.guildwars2.com/images/1/1e/Sand_Cascade.png" },
            { GarishPillarHit, "https://wiki.guildwars2.com/images/4/40/Garish_Pillar.png" },
            { GarishPillarShadeHit, "https://wiki.guildwars2.com/images/4/40/Garish_Pillar.png" },
            { DesertShroudHit, "https://wiki.guildwars2.com/images/0/08/Desert_Shroud.png" },
            { SandstormShroudHit, "https://wiki.guildwars2.com/images/3/34/Sandstorm_Shroud.png" },
            { SandFlare, "https://wiki.guildwars2.com/images/f/f0/Sand_Flare.png" },
            { DesertEmpowerment, "https://wiki.guildwars2.com/images/c/c3/Desert_Empowerment.png" },
            { CascadingCorruption, "https://wiki.guildwars2.com/images/9/9e/Cascading_Corruption.png" },
            { DeathlyHaste, "https://wiki.guildwars2.com/images/9/9d/Deathly_Haste.png" },
            { DoomApproaches, "https://wiki.guildwars2.com/images/2/28/Doom_Approaches.png" },
            { UnstableExplosion, "https://wiki.guildwars2.com/images/c/c9/Mark_of_Horror.png" },
            // Ranger
            { WindborneNotes, "https://wiki.guildwars2.com/images/8/84/Windborne_Notes.png" },
            { OpeningStrike, "https://wiki.guildwars2.com/images/9/9e/Opening_Strike.png" },
            { RuggedGrowth, "https://wiki.guildwars2.com/images/7/73/Rugged_Growth.png" },
            { AquaSurge, "https://wiki.guildwars2.com/images/0/07/Aqua_Surge.png" },
            { SignetOfRenewalEffect, "https://wiki.guildwars2.com/images/1/11/Signet_of_Renewal.png" },
            { EntangleDamage, "https://wiki.guildwars2.com/images/6/67/Entangle.png" },
            { SpiritOfNature, "https://wiki.guildwars2.com/images/a/a3/Spirit_of_Nature.png" },
            { ConsumingBite, "https://wiki.guildwars2.com/images/6/68/Consuming_Bite.png" },
            { NarcoticSpores,  "https://wiki.guildwars2.com/images/8/84/Narcotic_Spores.png" },
            { CripplingAnguish, "https://wiki.guildwars2.com/images/c/c8/Crippling_Anguish.png" },
            { KickGazelle, "https://wiki.guildwars2.com/images/b/bc/Kick_%28gazelle%29.png" },
            { ChargeGazelle, "https://wiki.guildwars2.com/images/a/af/Charge_%28gazelle%29.png" },
            { HeadbuttGazelle, "https://wiki.guildwars2.com/images/8/82/Headbutt_%28gazelle%29.png" },
            { SolarBeam, "https://wiki.guildwars2.com/images/a/a9/Solar_Beam.png" },
            { AstralWispAttachment, "https://wiki.guildwars2.com/images/f/ff/Astral_Wisp.png" },
            { CultivatedSynergyPlayer, "https://wiki.guildwars2.com/images/a/a2/Cultivated_Synergy.png" },
            { CultivatedSynergyPet, "https://wiki.guildwars2.com/images/a/a2/Cultivated_Synergy.png" },
            { LiveVicariously, "https://wiki.guildwars2.com/images/6/64/Live_Vicariously.png" },
            { NaturalHealing, "https://wiki.guildwars2.com/images/c/c1/Natural_Healing_%28ranger_trait%29.png" },
            { GlyphOfUnityCA, "https://wiki.guildwars2.com/images/4/4c/Glyph_of_Unity_%28Celestial_Avatar%29.png" },
            { ChargeGazelleMergeTravel, "https://wiki.guildwars2.com/images/a/af/Charge_%28gazelle%29.png" },
            { SmokeAssaultMergeHit, "https://wiki.guildwars2.com/images/9/92/Smoke_Assault.png" },
            { OneWolfPackDamage, "https://wiki.guildwars2.com/images/3/3b/One_Wolf_Pack.png" },
            { PredatorsCunning, "https://wiki.guildwars2.com/images/c/cc/Predator%27s_Cunning.png" },
            { OverbearingSmashLeap, "https://wiki.guildwars2.com/images/9/9a/Overbearing_Smash.png" },
            // Revenant
            { RiftSlashRiftHit, "https://wiki.guildwars2.com/images/a/a8/Rift_Slash.png" },
            { UnrelentingAssaultMultihit, "https://wiki.guildwars2.com/images/e/e9/Unrelenting_Assault.png" },
            { EnchantedDaggers2, "https://wiki.guildwars2.com/images/f/fa/Enchanted_Daggers.png" },
            { ImpossibleOddsHit, "https://wiki.guildwars2.com/images/8/87/Impossible_Odds.png" },
            { EmbraceTheDarknessDamage, "https://wiki.guildwars2.com/images/5/51/Embrace_the_Darkness.png" },
            { NaturalHarmony, "https://wiki.guildwars2.com/images/d/d9/Natural_Harmony.png" },
            { ProjectTranquility, "https://wiki.guildwars2.com/images/e/e7/Project_Tranquility.png" },
            { VentarisWill, "https://wiki.guildwars2.com/images/b/b6/Ventari%27s_Will.png" },
            { DarkrazorsDaringHit, "https://wiki.guildwars2.com/images/7/77/Darkrazor%27s_Daring.png" },
            { IcerazorsIreHit, "https://wiki.guildwars2.com/images/2/2d/Icerazor%27s_Ire.png" },
            { BreakrazorsBastion, "https://wiki.guildwars2.com/images/a/a7/Breakrazor%27s_Bastion.png" },
            { PhantomsOnslaughtDamage, "https://wiki.guildwars2.com/images/b/b2/Phantom%27s_Onslaught.png" },
            { DeathDropDodge, "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png" },
            { SaintsShieldDodge, "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png" },
            { ImperialImpactDodge, "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png" },
            { LesserBanishEnchantment, "https://wiki.guildwars2.com/images/e/ec/Banish_Enchantment.png" },
            // Thief
            { ThrowMagneticBomb, "https://wiki.guildwars2.com/images/e/e7/Throw_Magnetic_Bomb.png" },
            { DetonatePlasma, "https://wiki.guildwars2.com/images/3/3d/Detonate_Plasma.png" },
            { UnstableArtifact, "https://wiki.guildwars2.com/images/d/dd/Unstable_Artifact.png" },
            { MindShock, "https://wiki.guildwars2.com/images/e/e6/Mind_Shock.png" },
            { SoulStoneVenomSkill, "https://wiki.guildwars2.com/images/d/d6/Soul_Stone_Venom.png" },
            { SoulStoneVenomStrike, "https://wiki.guildwars2.com/images/d/d6/Soul_Stone_Venom.png" },
            { Pitfall, "https://wiki.guildwars2.com/images/6/67/Pitfall.png" },
            { BasiliskVenomStunBreakbarDamage, "https://wiki.guildwars2.com/images/3/3a/Basilisk_Venom.png" },
            { Bound, BuffImages.BoundingDodger },
            { ImpalingLotus, BuffImages.LotusTraining },
            { Dash, BuffImages.UnhinderedCombatant },
            { TwilightComboSecondProjectile, "https://wiki.guildwars2.com/images/d/dc/Twilight_Combo.png" },
            { HauntShot, "https://wiki.guildwars2.com/images/b/be/Haunt_Shot.png" },
            { GraspingShadows, "https://wiki.guildwars2.com/images/e/ef/Grasping_Shadows.png" },
            { DawnsRepose, "https://wiki.guildwars2.com/images/3/31/Dawn%27s_Repose.png" },
            { EternalNight, "https://wiki.guildwars2.com/images/7/7b/Eternal_Night.png" },
            // Warrior
            { LossAversion, "https://wiki.guildwars2.com/images/8/85/Loss_Aversion.png" },
            { KingOfFires, "https://wiki.guildwars2.com/images/7/70/King_of_Fires.png" },
            { DragonSlashBoost, "https://wiki.guildwars2.com/images/7/75/Dragon_Slash%E2%80%94Boost.png" },
            { DragonSlashForce, "https://wiki.guildwars2.com/images/b/b5/Dragon_Slash%E2%80%94Force.png" },
            { DragonSlashReach, "https://wiki.guildwars2.com/images/e/eb/Dragon_Slash%E2%80%94Reach.png" },
            { Triggerguard, "https://wiki.guildwars2.com/images/4/4e/Triggerguard.png" },
            { FlickerStep, "https://wiki.guildwars2.com/images/d/de/Flicker_Step.png" },
            { ExplosiveThrust, "https://wiki.guildwars2.com/images/9/99/Explosive_Thrust.png" },
            { SteelDivide, "https://wiki.guildwars2.com/images/9/9a/Steel_Divide.png" },
            { SwiftCut, "https://wiki.guildwars2.com/images/e/e3/Swift_Cut.png" },
            { BloomingFire, "https://wiki.guildwars2.com/images/d/d0/Blooming_Fire.png" },
            { ArtillerySlash, "https://wiki.guildwars2.com/images/6/68/Artillery_Slash.png" },
            { CycloneTrigger, "https://wiki.guildwars2.com/images/6/6c/Cyclone_Trigger.png" },
            { BreakStep, "https://wiki.guildwars2.com/images/7/76/Break_Step.png" },
            { RushDamage, "https://wiki.guildwars2.com/images/4/42/Rush.png" },
            { DragonspikeMineDamage, "https://wiki.guildwars2.com/images/3/3e/Dragonspike_Mine.png" },
            // Special Action Keys
            // - Training Area
            { MushroomKingsBlessing, "https://wiki.guildwars2.com/images/8/86/Cap_Hop.png" },
            { WeaponStow, "https://i.imgur.com/K7taOUe.png" },
            { WeaponDraw, "https://i.imgur.com/7TAlNtd.png" },
            // - Icebrood Saga
            { SpiritNovaTier1, "https://wiki.guildwars2.com/images/1/16/Spirit_Nova.png" },
            { SpiritNovaTier2, "https://wiki.guildwars2.com/images/1/16/Spirit_Nova.png" },
            { SpiritNovaTier3, "https://wiki.guildwars2.com/images/1/16/Spirit_Nova.png" },
            { SpiritNovaTier4, "https://wiki.guildwars2.com/images/1/16/Spirit_Nova.png" },
            { NightTerrorTier1, "https://wiki.guildwars2.com/images/0/03/Night_Terror.png" },
            { NightTerrorTier2, "https://wiki.guildwars2.com/images/0/03/Night_Terror.png" },
            { NightTerrorTier3, "https://wiki.guildwars2.com/images/0/03/Night_Terror.png" },
            { NightTerrorTier4, "https://wiki.guildwars2.com/images/0/03/Night_Terror.png" },
            { ShatteredPsycheTier1, "https://wiki.guildwars2.com/images/6/68/Shattered_Psyche.png" },
            { ShatteredPsycheTier2, "https://wiki.guildwars2.com/images/6/68/Shattered_Psyche.png" },
            { ShatteredPsycheTier3, "https://wiki.guildwars2.com/images/6/68/Shattered_Psyche.png" },
            { ShatteredPsycheTier4, "https://wiki.guildwars2.com/images/6/68/Shattered_Psyche.png" },
            // - Sabetha
            { SapperBombSkill, "https://wiki.guildwars2.com/images/b/ba/Sapper_Bomb.png" },
            // - Cairn
            { CelestialDash, "https://wiki.guildwars2.com/images/5/56/Celestial_Dash.png" },
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
            { UnleashSAK, "https://wiki.guildwars2.com/images/9/99/Touch_of_the_Sun.png" },
        };

        private static readonly Dictionary<long, ulong> _nonCritable = new Dictionary<long, ulong>
        {
            { LightningStrikeSigil, GW2Builds.StartOfLife }, 
            { FlameBlastSigil, GW2Builds.StartOfLife },
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

        private const string DefaultIcon = "https://render.guildwars2.com/file/1D55D34FB4EE20B1962E315245E40CA5E1042D0E/62248.png";

        // Fields
        public long ID { get; }
        //public int Range { get; private set; } = 0;
        public bool AA { get; }

        public bool IsSwap => ID == WeaponSwap || ElementalistHelper.IsElementalSwap(ID) || RevenantHelper.IsLegendSwap(ID) || HarbingerHelper.IsHarbingerShroudTransform(ID);
        public bool IsDodge(SkillData skillData) => IsAnimatedDodge(skillData) || ID == MirageCloakDodge;
        public bool IsAnimatedDodge(SkillData skillData) => ID == skillData.DodgeId || VindicatorHelper.IsVindicatorDodge(ID);
        public string Name { get; }
        public string Icon { get; }
        private readonly WeaponDescriptor _weaponDescriptor;
        public bool IsWeaponSkill => _weaponDescriptor != null;
        internal GW2APISkill ApiSkill { get; }
        private SkillInfoEvent _skillInfo { get; set; }

        internal const string DefaultName = "UNKNOWN";

        public bool UnknownSkill => Name == DefaultName;

        // Constructor

        internal SkillItem(long ID, string name, GW2APIController apiController)
        {
            this.ID = ID;
            Name = name.Replace("\0", "");
            ApiSkill = apiController.GetAPISkill(ID);
            //
            if (_overrideNames.TryGetValue(ID, out string overrideName))
            {
                Name = overrideName;
            } 
            else if (ApiSkill != null && (UnknownSkill || Name.All(char.IsDigit)))
            {
                Name = ApiSkill.Name;
            }
            if (_overrideIcons.TryGetValue(ID, out string icon))
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
                _weaponDescriptor = new WeaponDescriptor(ApiSkill);
            }
            AA = (ApiSkill?.Slot == "Weapon_1" || ApiSkill?.Slot == "Downed_1");
            if (AA)
            {
                if (ApiSkill.Categories != null)
                {
                    AA = AA && !ApiSkill.Categories.Contains("StealthAttack") && !ApiSkill.Categories.Contains("Ambush"); // Ambush in case one day it's added
                }
                if (ApiSkill.Description != null)
                {
                    AA = AA && !ApiSkill.Description.Contains("Ambush.");
                }
            }
        }

        public static bool CanCrit(long id, ulong gw2Build)
        {
            if (_nonCritable.TryGetValue(id, out ulong build))
            {
                return gw2Build < build;
            }
            return true;
        }

        internal int FindFirstWeaponSet(List<int> swaps)
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
            bool keep = swapped == WeaponSetIDs.FirstLandSet || swapped == WeaponSetIDs.SecondLandSet || swapped == WeaponSetIDs.FirstWaterSet || swapped == WeaponSetIDs.SecondWaterSet;
            if (_weaponDescriptor == null || !keep || !validForCurrentSwap)
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
}
