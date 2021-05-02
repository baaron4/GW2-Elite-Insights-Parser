namespace GW2EIEvtcParser.EncounterLogic
{
    public class EncounterCategory
    {
        internal enum FightCategory
        {
            Fractal,
            Strike,
            Raid,
            WvW,
            Golem,
            UnknownEncounter,
            Unknown
        };

        internal enum SubFightCategory
        {
            //Raids
            SpiritVale,
            SalvationPass,
            StrongholdOfTheFaithful,
            BastionOfThePenitent,
            HallOfChains,
            MythwrightGambit,
            TheKeyOfAhdashim,
            //Fractals
            Nightmare,
            ShatteredObservatory,
            SunquaPeak,
            // 
            Grothmar,
            Bjora,
            Drizzlewood,
            //
            GreenAlpineBorderlands,
            BlueAlpineBorderlands,
            RedDesertBorderlands,
            EternalBattlegrounds,
            EdgeOfTheMists,
            ObsidianSanctum,
            ArmisticeBastion,
            //
            Golem,
            //
            UnknownEncounter,
            Unknown
        };
        internal FightCategory Category { get; set; } = FightCategory.Unknown;
        internal SubFightCategory SubCategory { get; set; } = SubFightCategory.Unknown;

        internal int InSubCategoryOrder { get; set; } = 0;

        public int CompareTo(EncounterCategory other)
        {
            int catCompare = Category.CompareTo(other.Category);
            if (catCompare == 0)
            {
                int subCatCompare = SubCategory.CompareTo(other.SubCategory);
                /*if (subCatCompare == 0)
                {
                    return InSubCategoryOrder.CompareTo(other.InSubCategoryOrder);
                }*/
                return subCatCompare;
            }
            return catCompare;
        }

        public string GetCategoryName()
        {
            switch(Category)
            {
                case FightCategory.Strike:
                    return "Strike Mission";
                case FightCategory.WvW:
                    return "World vs Word";
                case FightCategory.UnknownEncounter:
                    return "Unknown Encounter";
                default:
                    return Category.ToString();
            }
        }

        public string GetSubCategoryName()
        {
            switch (SubCategory)
            {
                case SubFightCategory.SpiritVale:
                    return "Spirit Vale";
                case SubFightCategory.SalvationPass:
                    return "Salvation Pass";
                case SubFightCategory.StrongholdOfTheFaithful:
                    return "Stronghold of the Faithful";
                case SubFightCategory.BastionOfThePenitent:
                    return "Bastion of the Penitent";
                case SubFightCategory.HallOfChains:
                    return "Hall of Chains";
                case SubFightCategory.MythwrightGambit:
                    return "Mythwright Gambit";
                case SubFightCategory.TheKeyOfAhdashim:
                    return "The Key of Ahdashim";
                // 
                case SubFightCategory.ShatteredObservatory:
                    return "Shattered Observatory";
                case SubFightCategory.SunquaPeak:
                    return "Sunqua Peak";
                // 
                case SubFightCategory.UnknownEncounter:
                    return "Unknown Encounter";
                //
                case SubFightCategory.EternalBattlegrounds:
                    return "Eternal Battlegrounds";
                case SubFightCategory.ObsidianSanctum:
                    return "Obsidian Sanctum";
                case SubFightCategory.RedDesertBorderlands:
                    return "Red Desert Borderlands";
                case SubFightCategory.GreenAlpineBorderlands:
                    return "Green Alpine Borderlands";
                case SubFightCategory.BlueAlpineBorderlands:
                    return "Blue Alpine Borderlands";
                case SubFightCategory.EdgeOfTheMists:
                    return "Edge of the Mists";
                case SubFightCategory.ArmisticeBastion:
                    return "Armistice Bastion";
                //
                case SubFightCategory.Grothmar:
                    return "Grothmar Valley";
                case SubFightCategory.Bjora:
                    return "Bjora Marches";
                case SubFightCategory.Drizzlewood:
                    return "Drizzlewood Coast";
                default:
                    return Category.ToString();
            }
        }

    }
}
