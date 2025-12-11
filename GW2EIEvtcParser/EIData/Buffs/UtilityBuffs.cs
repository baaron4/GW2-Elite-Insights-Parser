using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class UtilityBuffs
{
    internal static readonly IReadOnlyList<Buff> Utilities =
    [     
        // UTILITIES 
        // 1h versions have the same ID as 30 min versions 
        new Buff("Diminished",Diminished, Source.Item, BuffClassification.Enhancement, ItemImages.Diminished),
        new Buff("Rough Sharpening Stone", RoughSharpeningStone, Source.Item, BuffClassification.Enhancement, ItemImages.RoughSharpeningStone),
        new Buff("Simple Sharpening Stone", SimpleSharpeningStone, Source.Item, BuffClassification.Enhancement, ItemImages.SimpleSharpeningStone),
        new Buff("Standard Sharpening Stone", StandardSharpeningStone, Source.Item, BuffClassification.Enhancement, ItemImages.StandardSharpeningStone),
        new Buff("Quality Sharpening Stone", QualitySharpeningStone, Source.Item, BuffClassification.Enhancement, ItemImages.QualitySharpeningStone),
        new Buff("Hardened Sharpening Stone", HardenedSharpeningStone, Source.Item, BuffClassification.Enhancement, ItemImages.HardenedSharpeningStone),
        new Buff("Superior Sharpening Stone", SuperiorSharpeningStone, Source.Item, BuffClassification.Enhancement, ItemImages.SuperiorSharpeningStone),
        new Buff("Apprentice Maintenance Oil", ApprenticeMaintenanceOil, Source.Item, BuffClassification.Enhancement, ItemImages.ApprenticeMaintenanceOil),
        new Buff("Journeyman Maintenance Oil", JourneymanMaintenanceOil, Source.Item, BuffClassification.Enhancement, ItemImages.JourneymanMaintenanceOil),
        new Buff("Standard Maintenance Oil", StandardMaintenanceOil, Source.Item, BuffClassification.Enhancement, ItemImages.StandardMaintenanceOil),
        new Buff("Artisan Maintenance Oil", ArtisanMaintenanceOil, Source.Item, BuffClassification.Enhancement, ItemImages.ArtisanMaintenanceOil),
        new Buff("Quality Maintenance Oil", QualityMaintenanceOil, Source.Item, BuffClassification.Enhancement, ItemImages.QualityMaintenanceOil),
        new Buff("Master Maintenance Oil", MasterMaintenanceOil, Source.Item, BuffClassification.Enhancement, ItemImages.MasterMaintenanceOil),
        new Buff("Apprentice Tuning Crystal", ApprenticeTuningCrystal, Source.Item, BuffClassification.Enhancement, ItemImages.ApprenticeTuningCrystal),
        new Buff("Journeyman Tuning Crystal", JourneymanTuningCrystal, Source.Item, BuffClassification.Enhancement, ItemImages.JourneymanTuningCrystal),
        new Buff("Standard Tuning Crystal", StandardTuningCrystal, Source.Item, BuffClassification.Enhancement, ItemImages.StandardTuningCrystal),
        new Buff("Artisan Tuning Crystal", ArtisanTuningCrystal, Source.Item, BuffClassification.Enhancement, ItemImages.ArtisanTuningCrystal),
        new Buff("Quality Tuning Crystal", QualityTuningCrystal, Source.Item, BuffClassification.Enhancement, ItemImages.QualityTuningCrystal),
        new Buff("Master Tuning Crystal", MasterTuningCrystal, Source.Item, BuffClassification.Enhancement, ItemImages.MasterTuningCrystal),
        new Buff("Compact Hardened Sharpening Stone", CompactHardenedSharpeningStone, Source.Item, BuffClassification.Enhancement, ItemImages.CompactHardenedSharpeningStone),
        new Buff("Tin of Fruitcake", TinOfFruitcake, Source.Item, BuffClassification.Enhancement, ItemImages.TinOfFruitcake),
        new Buff("Bountiful Sharpening Stone", BountifulSharpeningStone, Source.Item, BuffClassification.Enhancement, ItemImages.SuperiorSharpeningStone),
        new Buff("Toxic Sharpening Stone", ToxicSharpeningStone, Source.Item, BuffClassification.Enhancement, ItemImages.ToxicSharpeningStone),
        new Buff("Magnanimous Sharpening Stone", MagnanimousSharpeningStone, Source.Item, BuffClassification.Enhancement, ItemImages.MagnanimousSharpeningStone),
        new Buff("Corsair Sharpening Stone", CorsairSharpeningStone, Source.Item, BuffClassification.Enhancement, ItemImages.CorsairSharpeningStone),
        new Buff("Furious Sharpening Stone", FuriousSharpeningStone, Source.Item, BuffClassification.Enhancement, ItemImages.SuperiorSharpeningStone),
        new Buff("Holographic Super Cheese", HolographicSuperCheese, Source.Item, BuffClassification.Enhancement, ItemImages.HolographicSuperCheese),
        new Buff("Compact Quality Maintenance Oil", CompactQualityMaintenanceOil, Source.Item, BuffClassification.Enhancement, ItemImages.CompactQualityMaintenanceOil),
        new Buff("Peppermint Oil", PeppermintOil, Source.Item, BuffClassification.Enhancement, ItemImages.PeppermintOil),
        new Buff("Toxic Maintenance Oil", ToxicMaintenanceOil, Source.Item, BuffClassification.Enhancement, ItemImages.ToxicMaintenanceOil),
        new Buff("Magnanimous Maintenance Oil", MagnanimousMaintenanceOil, Source.Item, BuffClassification.Enhancement, ItemImages.MagnanimousMaintenanceOil),
        new Buff("Enhanced Lucent Oil", EnhancedLucentOil, Source.Item, BuffClassification.Enhancement, ItemImages.EnhancedLucentOil),
        new Buff("Potent Lucent Oil", PotentLucentOil, Source.Item, BuffClassification.Enhancement, ItemImages.PotentLucentOil),
        new Buff("Corsair Maintenance Oil", CorsairMaintenanceOil, Source.Item, BuffClassification.Enhancement, ItemImages.MagnanimousMaintenanceOil),
        new Buff("Furious Maintenance Oil", FuriousMaintenanceOil, Source.Item, BuffClassification.Enhancement, ItemImages.MasterMaintenanceOil),
        new Buff("Holographic Super Drumstick", HolographicSuperDrumstick, Source.Item, BuffClassification.Enhancement, ItemImages.HolographicSuperDrumstick),
        new Buff("Bountiful Maintenance Oil", BountifulMaintenanceOil, Source.Item, BuffClassification.Enhancement, ItemImages.MasterMaintenanceOil),
        new Buff("Compact Quality Tuning Crystal", CompactQualityTuningCrystal, Source.Item, BuffClassification.Enhancement, ItemImages.CompactQualityTuningCrystal),
        new Buff("Tuning Icicle", TuningIcicle, Source.Item, BuffClassification.Enhancement, ItemImages.TuningIcicle),
        new Buff("Bountiful Tuning Crystal", BountifulTuningCrystal, Source.Item, BuffClassification.Enhancement, ItemImages.MasterTuningCrystal),
        new Buff("Toxic Focusing Crystal", ToxicTuningCrystal, Source.Item, BuffClassification.Enhancement, ItemImages.ToxicTuningCrystal).WithBuilds(GW2Builds.StartOfLife, GW2Builds.November2024MountBalriorRelease),
        new Buff("Toxic Tuning Crystal", ToxicTuningCrystal, Source.Item, BuffClassification.Enhancement, ItemImages.ToxicTuningCrystal).WithBuilds(GW2Builds.November2024MountBalriorRelease),
        new Buff("Magnanimous Tuning Crystal", MagnanimousTuningCrystal, Source.Item, BuffClassification.Enhancement, ItemImages.MagnanimousTuningCrystal),
        new Buff("Furious Tuning Crystal", FuriousTuningCrystal, Source.Item, BuffClassification.Enhancement, ItemImages.MasterTuningCrystal),
        new Buff("Corsair Tuning Crystal", CorsairTuningCrystal, Source.Item, BuffClassification.Enhancement, ItemImages.CorsairTuningCrystal),
        new Buff("Holographic Super Apple", HolographicSuperApple, Source.Item, BuffClassification.Enhancement, ItemImages.HolographicSuperApple),
        new Buff("Sharpening Skull", SharpeningSkull, Source.Item, BuffClassification.Enhancement, ItemImages.SharpeningSkull),
        new Buff("Flask of Pumpkin Oil", FlaskOfPumpkinOil, Source.Item, BuffClassification.Enhancement, ItemImages.FlaskOfPumpkinOil),
        new Buff("Lump of Crystallized Nougat", LumpOfCrystallizedNougat, Source.Item, BuffClassification.Enhancement, ItemImages.LumpOfCrystallizedNougat),
        new Buff("Decade Enhancement", DecadeEnhancement, Source.Item, BuffClassification.Enhancement, ItemImages.DecadeEnhancement),
        new Buff("Sharpening Golem", SharpeningGolem, Source.Item, BuffClassification.Enhancement, ItemImages.NourishmentEffect),
        new Buff("Snow Diamond Ornament", SnowDiamondOrnament, Source.Item, BuffClassification.Enhancement, ItemImages.SnowDiamondOrnament),
    ];

    internal static readonly IReadOnlyList<Buff> OtherConsumables =
    [       
        // Reinforce Armor Canister / Anvil
        new Buff("Reinforced Armor", ReinforcedArmor, Source.Item, BuffClassification.OtherConsumable, ItemImages.ReinforcedArmor).WithBuilds(GW2Builds.June2022Balance),
        // Boosters
        new Buff("15% Speed Bonus", SpeedBonus15, Source.Item, BuffStackType.Queue, 9, BuffClassification.OtherConsumable, ItemImages.SpeedBonus15),
        new Buff("5% Damage Reduction", DamageReduction5, Source.Item, BuffStackType.Queue, 9, BuffClassification.OtherConsumable, ItemImages.DamageReduction5),
        new Buff("Healthful Rejuvenation", HealthfulRejuvenation, Source.Item, BuffStackType.Queue, 9, BuffClassification.OtherConsumable, ItemImages.HealthfulRejuvenation),
        new Buff("5% Damage Bonus", DamageBonus5, Source.Item, BuffStackType.Queue, 9, BuffClassification.OtherConsumable, ItemImages.DamageBonus5),
        new Buff("Guild Swiftness Banner Boost", GuildSwiftnessBannerBoost, Source.Item, BuffClassification.OtherConsumable, ItemImages.GuildSwiftnessBanner),
        // Fractals
        new Buff("Fractal Mobility", FractalMobility, Source.Item, BuffStackType.Stacking, 5, BuffClassification.OtherConsumable, ItemImages.FractalMobility),
        new Buff("Fractal Defensive", FractalDefensive, Source.Item, BuffStackType.Stacking, 5, BuffClassification.OtherConsumable, ItemImages.FractalDefensive),
        new Buff("Fractal Offensive", FractalOffensive, Source.Item, BuffStackType.Stacking, 5, BuffClassification.OtherConsumable, ItemImages.FractalOffensive),
        new Buff("Anguished Tear of Alba", AnguishedTearOfAlba, Source.Item, BuffClassification.OtherConsumable, BuffImages.AppliedFortitude),
        new Buff("Anguished Tear of Alba (Mastery)", AnguishedTearOfAlbaMastery, Source.Item, BuffClassification.OtherConsumable, BuffImages.AppliedFortitude),
        // Misc
        new Buff("Potion Of Karka Toughness", PotionOfKarkaToughness, Source.Item, BuffClassification.OtherConsumable, ItemImages.PowerfulPotionOfInquestSlaying),
        new Buff("Skale Venom (Consumable)", SkaleVenomConsumable, Source.Item, BuffClassification.OtherConsumable, ItemImages.WeakPotionOfNightmareCourtSlaying),
        new Buff("Swift Moa Feather", SwiftMoaFeather, Source.Item, BuffClassification.OtherConsumable, ItemImages.SwiftMoaFeather),
        new Buff("Fool's Dog Treat", FoolsDogTreat, Source.Item, BuffClassification.OtherConsumable, ItemImages.FoolsDogTreat),
        new Buff("Chatoyant Elixir", ChatoyantElixir, Source.Item, BuffClassification.OtherConsumable, ItemImages.PowerfulPotionOfDredgeSlaying),
    ];

    internal static readonly IReadOnlyList<Buff> Writs =
    [
        new Buff("Writ of Basic Strength", WritOfBasicStrength, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfBasicStrength),
        new Buff("Writ of Strength", WritOfStrength, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfStrength),
        new Buff("Writ of Studied Strength", WritOfStudiedStrength, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfStudiedStrength),
        new Buff("Writ of Calculated Strength", WritOfCalculatedStrength, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfCalculatedStrength),
        new Buff("Writ of Learned Strength", WritOfLearnedStrength, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfLearnedStrength),
        new Buff("Writ of Masterful Strength", WritOfMasterfulStrength, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfMasterfulStrength),
        new Buff("Writ of Basic Accuracy", WritOfBasicAccuracy, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfBasicAccuracy),
        new Buff("Writ of Accuracy", WritOfAccuracy, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfAccuracy),
        new Buff("Writ of Studied Accuracy", WritOfStudiedAccuracy, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfStudiedAccuracy),
        new Buff("Writ of Calculated Accuracy", WritOfCalculatedAccuracy, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfCalculatedAccuracy),
        new Buff("Writ of Learned Accuracy", WritOfLearnedAccuracy, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfLearnedAccuracy),
        new Buff("Writ of Masterful Accuracy", WritOfMasterfulAccuracy, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfMasterfulAccuracy),
        new Buff("Writ of Basic Malice", WritOfBasicMalice, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfBasicMalice),
        new Buff("Writ of Malice", WritOfMalice, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfMalice),
        new Buff("Writ of Studied Malice", WritOfStudiedMalice, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfStudiedMalice),
        new Buff("Writ of Calculated Malice", WritOfCalculatedMalice, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfCalculatedMalice),
        new Buff("Writ of Learned Malice", WritOfLearnedMalice, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfLearnedMalice),
        new Buff("Writ of Masterful Malice", WritOfMasterfulMalice, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfMasterfulMalice),
        new Buff("Writ of Basic Speed", WritOfBasicSpeed, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfBasicSpeed),
        new Buff("Writ of Studied Speed", WritOfStudiedSpeed, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfStudiedSpeed),
        new Buff("Writ of Masterful Speed", WritOfMasterfulSpeed, Source.Item, BuffClassification.Enhancement, ItemImages.WritOfMasterfulSpeed),
    ];

    internal static readonly IReadOnlyList<Buff> SlayingPotions =
    [
        // Branded
        new Buff("Minor Potion of Branded Slaying", MinorPotionOfBrandedSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfBrandedSlaying),
        new Buff("Powerful Potion of Branded Slaying", PowerfulPotionOfBrandedSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfBrandedSlaying),
        new Buff("Dragon Crystal Potion", DragonCrystalPotion, Source.Item, BuffClassification.Enhancement, ItemImages.DragonCrystalPotion),
        // Flame Legion
        new Buff("Weak Potion of Flame Legion Slaying", WeakPotionOfFlameLegionSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.WeakPotionOfFlameLegionSlaying),
        new Buff("Minor Potion of Flame Legion Slaying", MinorPotionOfFlameLegionSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfFlameLegionSlaying),
        new Buff("Potion of Flame Legion Slaying", PotionOfFlameLegionSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotionOfFlameLegionSlaying),
        new Buff("Strong Potion of Flame Legion Slaying", StrongPotionOfFlameLegionSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.StrongPotionOfFlameLegionSlaying),
        new Buff("Potent Potion of Flame Legion Slaying", PotentPotionOfFlameLegionSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfFlameLegionSlaying),
        new Buff("Powerful Potion of Flame Legion Slaying", PowerfulPotionOfFlameLegionSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfFlameLegionSlaying),
        // Halloween
        new Buff("Weak Potion of Halloween Slaying", WeakPotionOfHalloweenSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.WeakPotionOfHalloweenSlaying),
        new Buff("Minor Potion of Halloween Slaying", MinorPotionOfHalloweenSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfGrawlSlaying),
        new Buff("Potion of Halloween Slaying", PotionOfHalloweenSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfOgreSlaying),
        new Buff("Strong Potion of Halloween Slaying", StrongPotionOfHalloweenSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotionOfOgreSlaying),
        new Buff("Potent Potion of Halloween Slaying", PotentPotionOfHalloweenSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfOgreSlaying),
        new Buff("Powerful Potion of Halloween Slaying", PowerfulPotionOfHalloweenSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfHalloweenSlaying),
        // Centaur
        new Buff("Weak Potion of Centaur Slaying", WeakPotionOfCentaurSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.WeakPotionOfCentaurSlaying),
        new Buff("Minor Potion of Centaur Slaying", MinorPotionOfCentaurSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfCentaurSlaying),
        new Buff("Potion of Centaur Slaying", PotionOfCentaurSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotionOfCentaurSlaying),
        new Buff("Strong Potion of Centaur Slaying", StrongPotionOfCentaurSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.StrongPotionOfCentaurSlaying),
        new Buff("Potent Potion of Centaur Slaying", PotentPotionOfCentaurSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfCentaurSlaying),
        new Buff("Powerful Potion of Centaur Slaying", PowerfulPotionOfCentaurSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfCentaurSlaying),
        // Krait
        new Buff("Weak Potion of Krait Slaying", WeakPotionOfKraitSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.WeakPotionOfKraitSlaying),
        new Buff("Minor Potion of Krait Slaying", MinorPotionOfKraitSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfKraitSlaying),
        new Buff("Potion of Krait Slaying", PotionOfKraitSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotionOfKraitSlaying),
        new Buff("Strong Potion of Krait Slaying", StrongPotionOfKraitSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.StrongPotionOfKraitSlaying),
        new Buff("Potent Potion of Krait Slaying", PotentPotionOfKraitSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfKraitSlaying),
        new Buff("Powerful Potion of Krait Slaying", PowerfulPotionOfKraitSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfKraitSlaying),
        // Ogre
        new Buff("Weak Potion of Ogre Slaying", WeakPotionOfOgreSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.WeakPotionOfHalloweenSlaying),
        new Buff("Minor Potion of Ogre Slaying", MinorPotionOfOgreSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfOgreSlaying),
        new Buff("Potion of Ogre Slaying", PotionOfOgreSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotionOfOgreSlaying),
        new Buff("Strong Potion of Ogre Slaying", StrongPotionOfOgreSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.StrongPotionOfOgreSlaying),
        new Buff("Potent Potion of Ogre Slaying", PotentPotionOfOgreSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfOgreSlaying),
        new Buff("Powerful Potion of Ogre Slaying", PowerfulPotionOfOgreSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfOgreSlaying),
        // Elemental
        new Buff("Weak Potion of Elemental Slaying", WeakPotionOfElementalSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.WeakPotionOfElementalSlaying),
        new Buff("Minor Potion of Elemental Slaying", MinorPotionOfElementalSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfElementalSlaying),
        new Buff("Potion of Elemental Slaying", PotionOfElementalSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotionOfElementalSlaying),
        new Buff("Strong Potion of Elemental Slaying", StrongPotionOfElementalSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.StrongPotionOfElementalSlaying),
        new Buff("Potent Potion of Elemental Slaying", PotentPotionOfElementalSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfElementalSlaying),
        new Buff("Powerful Potion of Elemental Slaying", PowerfulPotionOfElementalSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfElementalSlaying),
        // Destroyer
        new Buff("Weak Potion of Destroyer Slaying", WeakPotionOfDestroyerSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.WeakPotionOfDestroyerSlaying),
        new Buff("Minor Potion of Destroyer Slaying", MinorPotionOfDestroyerSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfDestroyerSlaying),
        new Buff("Potion of Destroyer Slaying", PotionOfDestroyerSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotionOfDestroyerSlaying),
        new Buff("Strong Potion of Destroyer Slaying", StrongPotionOfDestroyerSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.StrongPotionOfDestroyerSlaying),
        new Buff("Potent Potion of Destroyer Slaying", PotentPotionOfDestroyerSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfDestroyerSlaying),
        new Buff("Powerful Potion of Destroyer Slaying", PowerfulPotionOfDestroyerSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfDestroyerSlaying),
        // Nightmare Court
        new Buff("Weak Potion of Nightmare Court Slaying", WeakPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.WeakPotionOfNightmareCourtSlaying),
        new Buff("Minor Potion of Nightmare Court Slaying", MinorPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfNightmareCourtSlaying),
        new Buff("Potion of Nightmare Court Slaying", PotionOfNightmareCourtSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotionOfNightmareCourtSlaying),
        new Buff("Strong Potion of Nightmare Court Slaying", StrongPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.StrongPotionOfNightmareCourtSlaying),
        new Buff("Potent Potion of Nightmare Court Slaying", PotentPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfNightmareCourtSlaying),
        new Buff("Powerful Potion of Nightmare Court Slaying", PowerfulPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfNightmareCourtSlaying),
        // Scarlet's Armies
        new Buff("Minor Potion of Slaying Scarlet's Armies", MinorPotionOfSlayingScarletsArmies, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfSlayingScarletsArmies),
        new Buff("Powerful Potion of Slaying Scarlet's Armies", PowerfulPotionOfSlayingScarletsArmies, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfDemonSlaying),
        // Undead
        new Buff("Weak Potion of Undead Slaying", WeakPotionOfUndeadSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.ThimbleOfLiquidKarma),
        new Buff("Minor Potion of Undead Slaying", MinorPotionOfUndeadSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.TasteOfLiquidKarma),
        new Buff("Potion of Undead Slaying", PotionOfUndeadSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.SwigOfLiquidKarma),
        new Buff("Strong Potion of Undead Slaying", StrongPotionOfUndeadSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.FlaskOfLiquidKarma),
        new Buff("Potent Potion of Undead Slaying", PotentPotionOfUndeadSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfUndeadSlaying),
        new Buff("Powerful Potion of Undead Slaying", PowerfulPotionOfUndeadSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfUndeadSlaying),
        // Dredge
        new Buff("Weak Potion of Dredge Slaying", WeakPotionOfDredgeSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.WeakPotionOfDredgeSlaying),
        new Buff("Minor Potion of Dredge Slaying", MinorPotionOfDredgeSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfDredgeSlaying),
        new Buff("Potion of Dredge Slaying", PotionOfDredgeSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotionOfDredgeSlaying),
        new Buff("Strong Potion of Dredge Slaying", StrongPotionOfDredgeSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.StrongPotionOfDredgeSlaying),
        new Buff("Potent Potion of Dredge Slaying", PotentPotionOfDredgeSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfDredgeSlaying),
        new Buff("Powerful Potion of Dredge Slaying", PowerfulPotionOfDredgeSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfDredgeSlaying),
        // Inquest
        new Buff("Weak Potion of Inquest Slaying", WeakPotionOfInquestSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.DripOfLiquidKarma),
        new Buff("Minor Potion of Inquest Slaying", MinorPotionOfInquestSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.SipOfLiquidKarma),
        new Buff("Potion of Inquest Slaying", PotionOfInquestSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.VialOfLiquidKarma),
        new Buff("Strong Potion of Inquest Slaying", StrongPotionOfInquestSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.GulpOfLiquidKarma),
        new Buff("Potent Potion of Inquest Slaying", PotentPotionOfInquestSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.JugOfLiquidKarma),
        new Buff("Powerful Potion of Inquest Slaying", PowerfulPotionOfInquestSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfInquestSlaying),
        // Demon
        new Buff("Weak Potion of Demon Slaying", WeakPotionOfDemonSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.WeakPotionOfDemonSlaying),
        new Buff("Minor Potion of Demon Slaying", MinorPotionOfDemonSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfDemonSlaying),
        new Buff("Potion of Demon Slaying", PotionOfDemonSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotionOfDemonSlaying),
        new Buff("Strong Potion of Demon Slaying", StrongPotionOfDemonSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.StrongPotionOfDemonSlaying),
        new Buff("Potent Potion of Demon Slaying", PotentPotionOfDemonSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfDemonSlaying),
        new Buff("Powerful Potion of Demon Slaying", PowerfulPotionOfDemonSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfDemonSlaying),
        // Grawl
        new Buff("Weak Potion of Grawl Slaying", WeakPotionOfGrawlSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.WeakPotionOfGrawlSlaying),
        new Buff("Minor Potion of Grawl Slaying", MinorPotionOfGrawlSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfGrawlSlaying),
        new Buff("Potion of Grawl Slaying", PotionOfGrawlSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotionOfGrawlSlaying),
        new Buff("Strong Potion of Grawl Slaying", StrongPotionOfGrawlSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.StrongPotionOfGrawlSlaying),
        new Buff("Potent Potion of Grawl Slaying", PotentPotionOfGrawlSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfGrawlSlaying),
        new Buff("Powerful Potion of Grawl Slaying", PowerfulPotionOfGrawlSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfGrawlSlaying),
        // Sons of Svanir
        new Buff("Weak Potion of Sons of Svanir Slaying", WeakPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MysteryTonicFurniture),
        new Buff("Minor Potion of Sons of Svanir Slaying", MinorPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.Tonic),
        new Buff("Potion of Sons of Svanir Slaying", PotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotionOfSonsOfSvanirSlaying),
        new Buff("Strong Potion of Sons of Svanir Slaying", StrongPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.StrongPotionOfSonsOfSvanirSlaying),
        new Buff("Potent Potion of Sons of Svanir Slaying", PotentPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfSonsOfSvanirSlaying),
        new Buff("Powerful Potion of Sons of Svanir Slaying", PowerfulPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfSonsOfSvanirSlaying),
        // Outlaw
        new Buff("Weak Potion of Outlaw Slaying", WeakPotionOfOutlawSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.WeakPotionOfOutlawSlaying),
        new Buff("Minor Potion of Outlaw Slaying", MinorPotionOfOutlawSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfOutlawSlaying),
        new Buff("Potion of Outlaw Slaying", PotionOfOutlawSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotionOfOutlawSlaying),
        new Buff("Strong Potion of Outlaw Slaying", StrongPotionOfOutlawSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.StrongPotionOfOutlawSlaying),
        new Buff("Potent Potion of Outlaw Slaying", PotentPotionOfOutlawSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfOutlawSlaying),
        new Buff("Powerful Potion of Outlaw Slaying", PowerfulPotionOfOutlawSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfOutlawSlaying),
        // Ice Brood
        new Buff("Weak Potion of Ice Brood Slaying", WeakPotionOfIceBroodSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.WeakPotionOfIceBroodSlaying),
        new Buff("Minor Potion of Ice Brood Slaying", MinorPotionOfIceBroodSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfIceBroodSlaying),
        new Buff("Potion of Ice Brood Slaying", PotionOfIceBroodSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotionOfIceBroodSlaying),
        new Buff("Strong Potion of Ice Brood Slaying", StrongPotionOfIceBroodSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.StrongPotionOfIceBroodSlaying),
        new Buff("Potent Potion of Ice Brood Slaying", PotentPotionOfIceBroodSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PotentPotionOfIceBroodSlaying),
        new Buff("Powerful Potion of Ice Brood Slaying", PowerfulPotionOfIceBroodSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfIceBroodSlaying),
        // Ghost
        new Buff("Extended Potion of Ghost Slaying", ExtendedPotionOfGhostSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.GlassOfButtermilk),
        // Hologram
        new Buff("Minor Potion of Hologram Slaying", MinorPotionOfHologramSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfHologramSlaying),
        // Karka
        new Buff("Potion Of Karka Slaying", PotionOfKarkaSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfInquestSlaying),
        // Mordrem
        new Buff("Minor Potion of Mordrem Slaying", MinorPotionOfMordremSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.MinorPotionOfMordremSlaying),
        new Buff("Powerful Potion of Mordrem Slaying", PowerfulPotionOfMordremSlaying, Source.Item, BuffClassification.Enhancement, ItemImages.PowerfulPotionOfMordremSlaying),
    ];

    internal static readonly IReadOnlyList<Buff> UtilityProcs =
    [
        // Buffs on revival
        new Buff("Ghoul's Grasp", GhoulsGrasp, Source.Item, BuffClassification.Support, ItemImages.GhoulsGrasp),
        new Buff("Feline Fury", FelineFury, Source.Item, BuffClassification.Support, ItemImages.FelineFury),
        new Buff("Pumpkin Prowess", PumpkinProwess, Source.Item, BuffClassification.Support, ItemImages.PumpkinProwess),
    ];
}
