namespace GW2EIEvtcParser.LogLogic;

public class LogCategories
{
    internal enum LogCategory
    {
        Fractal,
        RaidEncounter,
        RaidWing,
        WvW,
        Golem,
        Story,
        OpenWorld,
        Convergence,
        UnknownEncounter,
        Unknown,
    };

    internal enum SubLogCategory
    {
        //Raids
        SpiritVale,
        SalvationPass,
        StrongholdOfTheFaithful,
        BastionOfThePenitent,
        HallOfChains,
        MythwrightGambit,
        TheKeyOfAhdashim,
        MountBalrior,
        //Fractals
        CaptainMaiTrinBossFractal,
        Deepstone,
        Nightmare,
        ShatteredObservatory,
        SunquaPeak,
        SilentSurf,
        LonelyTower,
        Kinfall,
        //
        Grothmar,
        Bjora,
        Drizzlewood,
        Cantha,
        SotO,
        VisionsOfEternity,
        //
        GreenAlpineBorderlands,
        BlueAlpineBorderlands,
        RedDesertBorderlands,
        EternalBattlegrounds,
        EdgeOfTheMists,
        ObsidianSanctum,
        ArmisticeBastion,
        GuildHall,
        //
        Golem,
        //
        Story,
        //
        OpenWorld,
        //
        OuterNayosConvergence,
        MountBalriorConvergence,
        //
        UnknownEncounter,
        Unknown
    };
    internal LogCategory Category = LogCategory.Unknown;
    internal SubLogCategory SubCategory = SubLogCategory.Unknown;

    internal int InSubCategoryOrder = 0;

    public int CompareTo(LogCategories other)
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
        switch (Category)
        {
            case LogCategory.RaidWing:
                return "Raid Wing";
            case LogCategory.RaidEncounter:
                return "Raid Encounter";
            case LogCategory.WvW:
                return "World vs Word";
            case LogCategory.UnknownEncounter:
                return "Unknown Encounter";
            default:
                return Category.ToString();
        }
    }

    public string GetSubCategoryName()
    {
        switch (SubCategory)
        {
            case SubLogCategory.SpiritVale:
                return "Spirit Vale";
            case SubLogCategory.SalvationPass:
                return "Salvation Pass";
            case SubLogCategory.StrongholdOfTheFaithful:
                return "Stronghold of the Faithful";
            case SubLogCategory.BastionOfThePenitent:
                return "Bastion of the Penitent";
            case SubLogCategory.HallOfChains:
                return "Hall of Chains";
            case SubLogCategory.MythwrightGambit:
                return "Mythwright Gambit";
            case SubLogCategory.TheKeyOfAhdashim:
                return "The Key of Ahdashim";
            case SubLogCategory.MountBalrior:
                return "Mount Balrior";
            //
            case SubLogCategory.CaptainMaiTrinBossFractal:
                return "Captain Mai Trin Boss Fractal";
            case SubLogCategory.ShatteredObservatory:
                return "Shattered Observatory";
            case SubLogCategory.SunquaPeak:
                return "Sunqua Peak";
            case SubLogCategory.SilentSurf:
                return "Silent Surf";
            case SubLogCategory.LonelyTower:
                return "Lonely Tower";
            //
            case SubLogCategory.UnknownEncounter:
                return "Unknown Encounter";
            //
            case SubLogCategory.EternalBattlegrounds:
                return "Eternal Battlegrounds";
            case SubLogCategory.ObsidianSanctum:
                return "Obsidian Sanctum";
            case SubLogCategory.RedDesertBorderlands:
                return "Red Desert Borderlands";
            case SubLogCategory.GreenAlpineBorderlands:
                return "Green Alpine Borderlands";
            case SubLogCategory.BlueAlpineBorderlands:
                return "Blue Alpine Borderlands";
            case SubLogCategory.EdgeOfTheMists:
                return "Edge of the Mists";
            case SubLogCategory.ArmisticeBastion:
                return "Armistice Bastion";
            //
            case SubLogCategory.Grothmar:
                return "Grothmar Valley";
            case SubLogCategory.Bjora:
                return "Bjora Marches";
            case SubLogCategory.Drizzlewood:
                return "Drizzlewood Coast";
            case SubLogCategory.SotO:
                return "Secret of the Obscure";
            case SubLogCategory.VisionsOfEternity:
                return "Visions of Eternity";
            default:
                return SubCategory.ToString();
        }
    }

}
