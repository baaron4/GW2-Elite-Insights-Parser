using System.Collections.Generic;
using GW2EIEvtcParser.EIData.Buffs;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class UtilityBuffs
    {
        internal static readonly List<Buff> Utilities = new List<Buff>
        {     
            // UTILITIES 
            // 1h versions have the same ID as 30 min versions 
            new Buff("Diminished",Diminished, Source.Item, BuffClassification.Enhancement, BuffImages.Diminished),
            new Buff("Rough Sharpening Stone", RoughSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.RoughSharpeningStone),
            new Buff("Simple Sharpening Stone", SimpleSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.SimpleSharpeningStone),
            new Buff("Standard Sharpening Stone", StandardSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.StandardSharpeningStone),
            new Buff("Quality Sharpening Stone", QualitySharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.QualitySharpeningStone),
            new Buff("Hardened Sharpening Stone", HardenedSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.HardenedSharpeningStone),
            new Buff("Superior Sharpening Stone", SuperiorSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.SuperiorSharpeningStone),
            new Buff("Apprentice Maintenance Oil", ApprenticeMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.ApprenticeMaintenanceOil),
            new Buff("Journeyman Maintenance Oil", JourneymanMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.JourneymanMaintenanceOil),
            new Buff("Standard Maintenance Oil", StandardMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.StandardMaintenanceOil),
            new Buff("Artisan Maintenance Oil", ArtisanMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.ArtisanMaintenanceOil),
            new Buff("Quality Maintenance Oil", QualityMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.QualityMaintenanceOil),
            new Buff("Master Maintenance Oil", MasterMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.MasterMaintenanceOil),
            new Buff("Apprentice Tuning Crystal", ApprenticeTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.ApprenticeTuningCrystal),
            new Buff("Journeyman Tuning Crystal", JourneymanTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.JourneymanTuningCrystal),
            new Buff("Standard Tuning Crystal", StandardTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.StandardTuningCrystal),
            new Buff("Artisan Tuning Crystal", ArtisanTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.ArtisanTuningCrystal),
            new Buff("Quality Tuning Crystal", QualityTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.QualityTuningCrystal),
            new Buff("Master Tuning Crystal", MasterTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.MasterTuningCrystal),
            new Buff("Compact Hardened Sharpening Stone", CompactHardenedSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.CompactHardenedSharpeningStone),
            new Buff("Tin of Fruitcake", TinOfFruitcake, Source.Item, BuffClassification.Enhancement, BuffImages.TinOfFruitcake),
            new Buff("Bountiful Sharpening Stone", BountifulSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.SuperiorSharpeningStone),
            new Buff("Toxic Sharpening Stone", ToxicSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.ToxicSharpeningStone),
            new Buff("Magnanimous Sharpening Stone", MagnanimousSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.MagnanimousSharpeningStone),
            new Buff("Corsair Sharpening Stone", CorsairSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.CorsairSharpeningStone),
            new Buff("Furious Sharpening Stone", FuriousSharpeningStone, Source.Item, BuffClassification.Enhancement, BuffImages.SuperiorSharpeningStone),
            new Buff("Holographic Super Cheese", HolographicSuperCheese, Source.Item, BuffClassification.Enhancement, BuffImages.HolographicSuperCheese),
            new Buff("Compact Quality Maintenance Oil", CompactQualityMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.CompactQualityMaintenanceOil),
            new Buff("Peppermint Oil", PeppermintOil, Source.Item, BuffClassification.Enhancement, BuffImages.PeppermintOil),
            new Buff("Toxic Maintenance Oil", ToxicMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.ToxicMaintenanceOil),
            new Buff("Magnanimous Maintenance Oil", MagnanimousMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.MagnanimousMaintenanceOil),
            new Buff("Enhanced Lucent Oil", EnhancedLucentOil, Source.Item, BuffClassification.Enhancement, BuffImages.EnhancedLucentOil),
            new Buff("Potent Lucent Oil", PotentLucentOil, Source.Item, BuffClassification.Enhancement, BuffImages.PotentLucentOil),
            new Buff("Corsair Maintenance Oil", CorsairMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.MagnanimousMaintenanceOil),
            new Buff("Furious Maintenance Oil", FuriousMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.MasterMaintenanceOil),
            new Buff("Holographic Super Drumstick", HolographicSuperDrumstick, Source.Item, BuffClassification.Enhancement, BuffImages.HolographicSuperDrumstick),
            new Buff("Bountiful Maintenance Oil", BountifulMaintenanceOil, Source.Item, BuffClassification.Enhancement, BuffImages.MasterMaintenanceOil),
            new Buff("Compact Quality Tuning Crystal", CompactQualityTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.CompactQualityTuningCrystal),
            new Buff("Tuning Icicle", TuningIcicle, Source.Item, BuffClassification.Enhancement, BuffImages.TuningIcicle),
            new Buff("Bountiful Tuning Crystal", BountifulTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.MasterTuningCrystal),
            new Buff("Toxic Focusing Crystal", ToxicFocusingCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.ToxicFocusingCrystal),
            new Buff("Magnanimous Tuning Crystal", MagnanimousTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.MagnanimousTuningCrystal),
            new Buff("Furious Tuning Crystal", FuriousTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.MasterTuningCrystal),
            new Buff("Corsair Tuning Crystal", CorsairTuningCrystal, Source.Item, BuffClassification.Enhancement, BuffImages.CorsairTuningCrystal),
            new Buff("Holographic Super Apple", HolographicSuperApple, Source.Item, BuffClassification.Enhancement, BuffImages.HolographicSuperApple),
            new Buff("Sharpening Skull", SharpeningSkull, Source.Item, BuffClassification.Enhancement, BuffImages.SharpeningSkull),
            new Buff("Flask of Pumpkin Oil", FlaskOfPumpkinOil, Source.Item, BuffClassification.Enhancement, BuffImages.FlaskOfPumpkinOil),
            new Buff("Lump of Crystallized Nougat", LumpOfCrystallizedNougat, Source.Item, BuffClassification.Enhancement, BuffImages.LumpOfCrystallizedNougat),
            new Buff("Decade Enhancement", DecadeEnhancement, Source.Item, BuffClassification.Enhancement, BuffImages.DecadeEnhancement),
        };

        internal static readonly List<Buff> OtherConsumables = new List<Buff>
        {       
            //
            new Buff("Reinforced Armor", ReinforcedArmor, Source.Item, BuffClassification.OtherConsumable, BuffImages.ReinforcedArmor, GW2Builds.June2022Balance, GW2Builds.EndOfLife),
            //
            new Buff("15% Speed Bonus", SpeedBonus15, Source.Item, BuffStackType.Queue, 9, BuffClassification.OtherConsumable, BuffImages.SpeedBonus15),
            new Buff("5% Damage Reduction", DamageReduction5, Source.Item, BuffStackType.Queue, 9, BuffClassification.OtherConsumable, BuffImages.DamageReduction5),
            new Buff("Healthful Rejuvenation", HealthfulRejuvenation, Source.Item, BuffStackType.Queue, 9, BuffClassification.OtherConsumable, BuffImages.HealthfulRejuvenation),
            new Buff("5% Damage Bonus", DamageBonus5, Source.Item, BuffStackType.Queue, 9, BuffClassification.OtherConsumable, BuffImages.DamageBonus5),
            //
            new Buff("Skale Venom (Consumable)", SkaleVenomConsumable, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfNightmareCourtSlaying),
            new Buff("Swift Moa Feather", SwiftMoaFeather, Source.Item, BuffClassification.OtherConsumable, BuffImages.SwiftMoaFeather),
            new Buff("Fool's Dog Treat", FoolsDogTreat, Source.Item, BuffClassification.Nourishment, BuffImages.FoolsDogTreat),
            new Buff("Chatoyant Elixir", ChatoyantElixir, Source.Item, BuffClassification.Nourishment, BuffImages.PowerfulPotionOfDredgeSlaying),
        };

        internal static readonly List<Buff> Writs = new List<Buff>
        {
            new Buff("Writ of Basic Strength", WritOfBasicStrength, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfBasicStrength),
            new Buff("Writ of Strength", WritOfStrength, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfStrength),
            new Buff("Writ of Studied Strength", WritOfStudiedStrength, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfStudiedStrength),
            new Buff("Writ of Calculated Strength", WritOfCalculatedStrength, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfCalculatedStrength),
            new Buff("Writ of Learned Strength", WritOfLearnedStrength, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfLearnedStrength),
            new Buff("Writ of Masterful Strength", WritOfMasterfulStrength, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfMasterfulStrength),
            new Buff("Writ of Basic Accuracy", WritOfBasicAccuracy, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfBasicAccuracy),
            new Buff("Writ of Accuracy", WritOfAccuracy, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfAccuracy),
            new Buff("Writ of Studied Accuracy", WritOfStudiedAccuracy, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfStudiedAccuracy),
            new Buff("Writ of Calculated Accuracy", WritOfCalculatedAccuracy, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfCalculatedAccuracy),
            new Buff("Writ of Learned Accuracy", WritOfLearnedAccuracy, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfLearnedAccuracy),
            new Buff("Writ of Masterful Accuracy", WritOfMasterfulAccuracy, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfMasterfulAccuracy),
            new Buff("Writ of Basic Malice", WritOfBasicMalice, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfBasicMalice),
            new Buff("Writ of Malice", WritOfMalice, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfMalice),
            new Buff("Writ of Studied Malice", WritOfStudiedMalice, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfStudiedMalice),
            new Buff("Writ of Calculated Malice", WritOfCalculatedMalice, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfCalculatedMalice),
            new Buff("Writ of Learned Malice", WritOfLearnedMalice, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfLearnedMalice),
            new Buff("Writ of Masterful Malice", WritOfMasterfulMalice, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfMasterfulMalice),
            new Buff("Writ of Basic Speed", WritOfBasicSpeed, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfBasicSpeed),
            new Buff("Writ of Studied Speed", WritOfStudiedSpeed, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfStudiedSpeed),
            new Buff("Writ of Masterful Speed", WritOfMasterfulSpeed, Source.Item, BuffClassification.Enhancement, BuffImages.WritOfMasterfulSpeed),
        };

        internal static readonly List<Buff> Potions = new List<Buff>
        {
            // Slaying Potions
            // Branded
            new Buff("Minor Potion of Branded Slaying", MinorPotionOfBrandedSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfBrandedSlaying),
            new Buff("Powerful Potion of Branded Slaying", PowerfulPotionOfBrandedSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfBrandedSlaying),
            new Buff("Dragon Crystal Potion", DragonCrystalPotion, Source.Item, BuffClassification.OtherConsumable, BuffImages.DragonCrystalPotion),
            // Flame Legion
            new Buff("Weak Potion of Flame Legion Slaying", WeakPotionOfFlameLegionSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfFlameLegionSlaying),
            new Buff("Minor Potion of Flame Legion Slaying", MinorPotionOfFlameLegionSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfFlameLegionSlaying),
            new Buff("Potion of Flame Legion Slaying", PotionOfFlameLegionSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfFlameLegionSlaying),
            new Buff("Strong Potion of Flame Legion Slaying", StrongPotionOfFlameLegionSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfFlameLegionSlaying),
            new Buff("Potent Potion of Flame Legion Slaying", PotentPotionOfFlameLegionSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfFlameLegionSlaying),
            new Buff("Powerful Potion of Flame Legion Slaying", PowerfulPotionOfFlameLegionSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfFlameLegionSlaying),
            // Halloween
            new Buff("Weak Potion of Halloween Slaying", WeakPotionOfHalloweenSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfHalloweenSlaying),
            new Buff("Minor Potion of Halloween Slaying", MinorPotionOfHalloweenSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfGrawlSlaying),
            new Buff("Potion of Halloween Slaying", PotionOfHalloweenSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfOgreSlaying),
            new Buff("Strong Potion of Halloween Slaying", StrongPotionOfHalloweenSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfOgreSlaying),
            new Buff("Potent Potion of Halloween Slaying", PotentPotionOfHalloweenSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfOgreSlaying),
            new Buff("Powerful Potion of Halloween Slaying", PowerfulPotionOfHalloweenSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfHalloweenSlaying),
            // Centaur
            new Buff("Weak Potion of Centaur Slaying", WeakPotionOfCentaurSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfCentaurSlaying),
            new Buff("Minor Potion of Centaur Slaying", MinorPotionOfCentaurSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfCentaurSlaying),
            new Buff("Potion of Centaur Slaying", PotionOfCentaurSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfCentaurSlaying),
            new Buff("Strong Potion of Centaur Slaying", StrongPotionOfCentaurSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfCentaurSlaying),
            new Buff("Potent Potion of Centaur Slaying", PotentPotionOfCentaurSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfCentaurSlaying),
            new Buff("Powerful Potion of Centaur Slaying", PowerfulPotionOfCentaurSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfCentaurSlaying),
            // Krait
            new Buff("Weak Potion of Krait Slaying", WeakPotionOfKraitSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfKraitSlaying),
            new Buff("Minor Potion of Krait Slaying", MinorPotionOfKraitSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfKraitSlaying),
            new Buff("Potion of Krait Slaying", PotionOfKraitSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfKraitSlaying),
            new Buff("Strong Potion of Krait Slaying", StrongPotionOfKraitSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfKraitSlaying),
            new Buff("Potent Potion of Krait Slaying", PotentPotionOfKraitSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfKraitSlaying),
            new Buff("Powerful Potion of Krait Slaying", PowerfulPotionOfKraitSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfKraitSlaying),
            // Ogre
            new Buff("Weak Potion of Ogre Slaying", WeakPotionOfOgreSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfHalloweenSlaying),
            new Buff("Minor Potion of Ogre Slaying", MinorPotionOfOgreSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfOgreSlaying),
            new Buff("Potion of Ogre Slaying", PotionOfOgreSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfOgreSlaying),
            new Buff("Strong Potion of Ogre Slaying", StrongPotionOfOgreSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfOgreSlaying),
            new Buff("Potent Potion of Ogre Slaying", PotentPotionOfOgreSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfOgreSlaying),
            new Buff("Powerful Potion of Ogre Slaying", PowerfulPotionOfOgreSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfOgreSlaying),
            // Elemental
            new Buff("Weak Potion of Elemental Slaying", WeakPotionOfElementalSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfElementalSlaying),
            new Buff("Minor Potion of Elemental Slaying", MinorPotionOfElementalSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfElementalSlaying),
            new Buff("Potion of Elemental Slaying", PotionOfElementalSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfElementalSlaying),
            new Buff("Strong Potion of Elemental Slaying", StrongPotionOfElementalSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfElementalSlaying),
            new Buff("Potent Potion of Elemental Slaying", PotentPotionOfElementalSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfElementalSlaying),
            new Buff("Powerful Potion of Elemental Slaying", PowerfulPotionOfElementalSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfElementalSlaying),
            // Destroyer
            new Buff("Weak Potion of Destroyer Slaying", WeakPotionOfDestroyerSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfDestroyerSlaying),
            new Buff("Minor Potion of Destroyer Slaying", MinorPotionOfDestroyerSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfDestroyerSlaying),
            new Buff("Potion of Destroyer Slaying", PotionOfDestroyerSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfDestroyerSlaying),
            new Buff("Strong Potion of Destroyer Slaying", StrongPotionOfDestroyerSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfDestroyerSlaying),
            new Buff("Potent Potion of Destroyer Slaying", PotentPotionOfDestroyerSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfDestroyerSlaying),
            new Buff("Powerful Potion of Destroyer Slaying", PowerfulPotionOfDestroyerSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfDestroyerSlaying),
            // Nightmare Court
            new Buff("Weak Potion of Nightmare Court Slaying", WeakPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfNightmareCourtSlaying),
            new Buff("Minor Potion of Nightmare Court Slaying", MinorPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfNightmareCourtSlaying),
            new Buff("Potion of Nightmare Court Slaying", PotionOfNightmareCourtSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfNightmareCourtSlaying),
            new Buff("Strong Potion of Nightmare Court Slaying", StrongPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfNightmareCourtSlaying),
            new Buff("Potent Potion of Nightmare Court Slaying", PotentPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfNightmareCourtSlaying),
            new Buff("Powerful Potion of Nightmare Court Slaying", PowerfulPotionOfNightmareCourtSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfNightmareCourtSlaying),
            // Scarlet's Armies
            new Buff("Minor Potion of Slaying Scarlet's Armies", MinorPotionOfSlayingScarletsArmies, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfSlayingScarletsArmies),
            new Buff("Powerful Potion of Slaying Scarlet's Armies", PowerfulPotionOfSlayingScarletsArmies, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfDemonSlaying),
            // Undead
            new Buff("Weak Potion of Undead Slaying", WeakPotionOfUndeadSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.ThimbleOfLiquidKarma),
            new Buff("Minor Potion of Undead Slaying", MinorPotionOfUndeadSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.TasteOfLiquidKarma),
            new Buff("Potion of Undead Slaying", PotionOfUndeadSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.SwigOfLiquidKarma),
            new Buff("Strong Potion of Undead Slaying", StrongPotionOfUndeadSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.FlaskOfLiquidKarma),
            new Buff("Potent Potion of Undead Slaying", PotentPotionOfUndeadSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfUndeadSlaying),
            new Buff("Powerful Potion of Undead Slaying", PowerfulPotionOfUndeadSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfUndeadSlaying),
            // Dredge
            new Buff("Weak Potion of Dredge Slaying", WeakPotionOfDredgeSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfDredgeSlaying),
            new Buff("Minor Potion of Dredge Slaying", MinorPotionOfDredgeSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfDredgeSlaying),
            new Buff("Potion of Dredge Slaying", PotionOfDredgeSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfDredgeSlaying),
            new Buff("Strong Potion of Dredge Slaying", StrongPotionOfDredgeSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfDredgeSlaying),
            new Buff("Potent Potion of Dredge Slaying", PotentPotionOfDredgeSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfDredgeSlaying),
            new Buff("Powerful Potion of Dredge Slaying", PowerfulPotionOfDredgeSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfDredgeSlaying),
            // Inquest
            new Buff("Weak Potion of Inquest Slaying", WeakPotionOfInquestSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.DripOfLiquidKarma),
            new Buff("Minor Potion of Inquest Slaying", MinorPotionOfInquestSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.SipOfLiquidKarma),
            new Buff("Potion of Inquest Slaying", PotionOfInquestSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.VialOfLiquidKarma),
            new Buff("Strong Potion of Inquest Slaying", StrongPotionOfInquestSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.GulpOfLiquidKarma),
            new Buff("Potent Potion of Inquest Slaying", PotentPotionOfInquestSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.JugOfLiquidKarma),
            new Buff("Powerful Potion of Inquest Slaying", PowerfulPotionOfInquestSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfInquestSlaying),
            // Demon
            new Buff("Weak Potion of Demon Slaying", WeakPotionOfDemonSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfDemonSlaying),
            new Buff("Minor Potion of Demon Slaying", MinorPotionOfDemonSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfDemonSlaying),
            new Buff("Potion of Demon Slaying", PotionOfDemonSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfDemonSlaying),
            new Buff("Strong Potion of Demon Slaying", StrongPotionOfDemonSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfDemonSlaying),
            new Buff("Potent Potion of Demon Slaying", PotentPotionOfDemonSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfDemonSlaying),
            new Buff("Powerful Potion of Demon Slaying", PowerfulPotionOfDemonSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfDemonSlaying),
            // Grawl
            new Buff("Weak Potion of Grawl Slaying", WeakPotionOfGrawlSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfGrawlSlaying),
            new Buff("Minor Potion of Grawl Slaying", MinorPotionOfGrawlSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfGrawlSlaying),
            new Buff("Potion of Grawl Slaying", PotionOfGrawlSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfGrawlSlaying),
            new Buff("Strong Potion of Grawl Slaying", StrongPotionOfGrawlSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfGrawlSlaying),
            new Buff("Potent Potion of Grawl Slaying", PotentPotionOfGrawlSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfGrawlSlaying),
            new Buff("Powerful Potion of Grawl Slaying", PowerfulPotionOfGrawlSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfGrawlSlaying),
            // Sons of Svanir
            new Buff("Weak Potion of Sons of Svanir Slaying", WeakPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MysteryTonicFurniture),
            new Buff("Minor Potion of Sons of Svanir Slaying", MinorPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.Tonic),
            new Buff("Potion of Sons of Svanir Slaying", PotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfSonsOfSvanirSlaying),
            new Buff("Strong Potion of Sons of Svanir Slaying", StrongPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfSonsOfSvanirSlaying),
            new Buff("Potent Potion of Sons of Svanir Slaying", PotentPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfSonsOfSvanirSlaying),
            new Buff("Powerful Potion of Sons of Svanir Slaying", PowerfulPotionOfSonsOfSvanirSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfSonsOfSvanirSlaying),
            // Outlaw
            new Buff("Weak Potion of Outlaw Slaying", WeakPotionOfOutlawSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfOutlawSlaying),
            new Buff("Minor Potion of Outlaw Slaying", MinorPotionOfOutlawSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfOutlawSlaying),
            new Buff("Potion of Outlaw Slaying", PotionOfOutlawSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfOutlawSlaying),
            new Buff("Strong Potion of Outlaw Slaying", StrongPotionOfOutlawSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfOutlawSlaying),
            new Buff("Potent Potion of Outlaw Slaying", PotentPotionOfOutlawSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfOutlawSlaying),
            new Buff("Powerful Potion of Outlaw Slaying", PowerfulPotionOfOutlawSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfOutlawSlaying),
            // Ice Brood
            new Buff("Weak Potion of Ice Brood Slaying", WeakPotionOfIceBroodSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.WeakPotionOfIceBroodSlaying),
            new Buff("Minor Potion of Ice Brood Slaying", MinorPotionOfIceBroodSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfIceBroodSlaying),
            new Buff("Potion of Ice Brood Slaying", PotionOfIceBroodSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotionOfIceBroodSlaying),
            new Buff("Strong Potion of Ice Brood Slaying", StrongPotionOfIceBroodSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.StrongPotionOfIceBroodSlaying),
            new Buff("Potent Potion of Ice Brood Slaying", PotentPotionOfIceBroodSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PotentPotionOfIceBroodSlaying),
            new Buff("Powerful Potion of Ice Brood Slaying", PowerfulPotionOfIceBroodSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfIceBroodSlaying),
            // Ghost
            new Buff("Extended Potion of Ghost Slaying", ExtendedPotionOfGhostSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.GlassOfButtermilk),
            // Hologram
            new Buff("Minor Potion of Hologram Slaying", MinorPotionOfHologramSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfHologramSlaying),
            // Karka
            new Buff("Potion Of Karka Slaying", PotionOfKarkaSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfInquestSlaying),
            // Mordrem
            new Buff("Minor Potion of Mordrem Slaying", MinorPotionOfMordremSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.MinorPotionOfMordremSlaying),
            new Buff("Powerful Potion of Mordrem Slaying", PowerfulPotionOfMordremSlaying, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfMordremSlaying),
            // Fractals
            new Buff("Fractal Mobility", FractalMobility, Source.Item, BuffStackType.Stacking, 5, BuffClassification.OtherConsumable, BuffImages.FractalMobility),
            new Buff("Fractal Defensive", FractalDefensive, Source.Item, BuffStackType.Stacking, 5, BuffClassification.OtherConsumable, BuffImages.FractalDefensive),
            new Buff("Fractal Offensive", FractalOffensive, Source.Item, BuffStackType.Stacking, 5, BuffClassification.OtherConsumable, BuffImages.FractalOffensive),
            // Misc
            new Buff("Potion Of Karka Toughness", PotionOfKarkaToughness, Source.Item, BuffClassification.OtherConsumable, BuffImages.PowerfulPotionOfInquestSlaying),
        };

    }
}
