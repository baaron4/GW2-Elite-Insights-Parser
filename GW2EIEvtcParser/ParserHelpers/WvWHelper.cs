using System.Numerics;

namespace GW2EIEvtcParser;

public static class WvWHelper
{
    public enum ObjectiveOwnership
    {
        None,
        Red,
        Blue,
        Green
    }

    public enum ObjectiveType
    {
        Unknown,
        Camp,
        Ruins,
        Tower,
        Keep,
        Castle
    }

    public static string GetObjectiveTypeName(this ObjectiveType value)
    {
        switch (value)
        {
            case ObjectiveType.Camp:
                return "Camp";
            case ObjectiveType.Tower:
                return "Tower";
            case ObjectiveType.Ruins:
                return "Ruins";
            case ObjectiveType.Keep:
                return "Keep";
            case ObjectiveType.Castle:
                return "Castle";
        }
        return "None";
    }

    private static readonly Dictionary<ObjectiveType, IReadOnlyList<(uint Threshold, int Tier)>> TierThresholdPerType = new()
    {
        {
            ObjectiveType.Camp, new List<(uint Threshold, int Tier)>()
            {
                (25, 3),
                (20, 2),
                (15, 1),
            }
        },
        {
            ObjectiveType.Tower, new List<(uint Threshold, int Tier)>()
            {
                (35, 3),
                (20, 2),
                (15, 1),
            }
        },
        {
            ObjectiveType.Keep, new List<(uint Threshold, int Tier)>()
            {
                (50, 3),
                (30, 2),
                (20, 1),
            }
        },
        {
            ObjectiveType.Castle, new List<(uint Threshold, int Tier)>()
            {
                (90, 3),
                (60, 2),
                (40, 1),
            }
        },
    };

    /// <summary>
    /// Returns the tier of the objective for a given upgrade progress.
    /// Will return -1 is not applicable.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="upgradeProgress"></param>
    /// <returns></returns>
    internal static int GetObjectiveTier(ObjectiveType type, uint upgradeProgress)
    {
        if (TierThresholdPerType.TryGetValue(type, out var thresholds))
        {
            foreach (var (Threshold, Tier) in thresholds)
            {
                if (Threshold >= upgradeProgress)
                {
                    return Tier;
                }
            }
            return 0;
        }
        return -1;
    }

    // To be filled with neutral, red, green, blue icon versions for each type
    private static readonly Dictionary<ObjectiveType, Dictionary<ObjectiveOwnership, string>> TypeIconsPerOwner = new() {
        {ObjectiveType.Camp, new() {
            {ObjectiveOwnership.Red, "https://i.imgur.com/tRclXR9.png" },
            {ObjectiveOwnership.Green, "https://i.imgur.com/zTPNBd1.png" },
            {ObjectiveOwnership.Blue, "https://i.imgur.com/rGd2Xc0.png" },
            {ObjectiveOwnership.None, "https://render.guildwars2.com/file/015D365A08AAE105287A100AAE04529FDAE14155/102532.png" },
        }},
        {ObjectiveType.Ruins, new() {
            {ObjectiveOwnership.Red, "https://render.guildwars2.com/file/52B43242E55961770D78B80ED77BC764F0E57BF2/1635237.png" },
            {ObjectiveOwnership.Green, "https://render.guildwars2.com/file/52B43242E55961770D78B80ED77BC764F0E57BF2/1635237.png" },
            {ObjectiveOwnership.Blue, "https://render.guildwars2.com/file/52B43242E55961770D78B80ED77BC764F0E57BF2/1635237.png" },
            {ObjectiveOwnership.None, "https://render.guildwars2.com/file/52B43242E55961770D78B80ED77BC764F0E57BF2/1635237.png" },
        }},
        {ObjectiveType.Tower, new() {
            {ObjectiveOwnership.Red, "https://i.imgur.com/0NyXDA3.png" },
            {ObjectiveOwnership.Green, "https://i.imgur.com/Dyyk0zp.png" },
            {ObjectiveOwnership.Blue, "https://i.imgur.com/DbuFMzA.png" },
            {ObjectiveOwnership.None, "https://render.guildwars2.com/file/ABEC80C79576A103EA33EC66FCB99B77291A2F0D/102531.png" },
        }},
        {ObjectiveType.Keep, new() {
            {ObjectiveOwnership.Red, "https://i.imgur.com/IGcseHc.png" },
            {ObjectiveOwnership.Green, "https://i.imgur.com/F68fA1t.png" },
            {ObjectiveOwnership.Blue, "https://i.imgur.com/XMmTYn6.png" },
            {ObjectiveOwnership.None, "https://render.guildwars2.com/file/DB580419C8AD9449309A96C8E7C3D61631020EBB/102535.png" },
        }},
        {ObjectiveType.Castle, new() {
            {ObjectiveOwnership.Red, "https://i.imgur.com/npSLcOM.png" },
            {ObjectiveOwnership.Green, "https://i.imgur.com/M8BdXxX.png" },
            {ObjectiveOwnership.Blue, "https://i.imgur.com/VI79WOY.png" },
            {ObjectiveOwnership.None, "https://render.guildwars2.com/file/F0F1DA1C807444F4DF53090343F43BED02E50523/102608.png" },
        }},
    };

    internal class WvWObjectiveData
    {
        public readonly ObjectiveType Type;
        public readonly Vector3 ContinentPosition;

        public bool IsUnknown => Type == ObjectiveType.Unknown;

        public WvWObjectiveData(ObjectiveType type, Vector3 continentPosition)
        {
            Type = type;
            ContinentPosition = continentPosition;
        }

        public string GetIcon(ObjectiveOwnership ownership)
        {
            if (TypeIconsPerOwner.TryGetValue(Type, out var ownerDict))
            {
                if (ownerDict.TryGetValue(ownership, out var icon))
                {
                    return icon;
                }
            }
            return "";
        }
    }

    // to be filled for every wvw map and for every objective id in said map
    private static readonly Dictionary<int, Dictionary<int, WvWObjectiveData>> ObjectiveDataPerMapIDPerObjectiveID = new()
    {
        {MapIDs.BlueAlpineBorderland, new()
        {
            {39, new WvWObjectiveData(ObjectiveType.Camp, new(14082.8f, 11228.4f, -3676.89f)) }, // Spiritholm
            {38, new WvWObjectiveData(ObjectiveType.Tower, new(13444.8f, 12078.2f, -3758.65f)) }, // Woodhaven
            {37, new WvWObjectiveData(ObjectiveType.Keep, new(14056.6f, 12430.9f, -2800.76f)) }, // Blue Garrison
            {40, new WvWObjectiveData(ObjectiveType.Tower, new(14683.4f, 12030.3f, -4839.9f)) }, // Dawn's Eyrie
            {52, new WvWObjectiveData(ObjectiveType.Camp, new(  13211.9f, 12195.7f, -46.1562f)) }, // Godslore 
            {51, new WvWObjectiveData(ObjectiveType.Camp, new(15025.5f, 12168.3f, -1533.33f)) }, // Stargrove 

            {64, new WvWObjectiveData(ObjectiveType.Ruins, new(13859.2f, 12703.1f, -393f)) }, // Baeur 
            {65, new WvWObjectiveData(ObjectiveType.Ruins, new(14327.1f, 12757, -711)) }, // Orchard 
            {63, new WvWObjectiveData(ObjectiveType.Ruins, new(13761f, 13074.7f, -0.649902f)) }, // Battle 
            {66, new WvWObjectiveData(ObjectiveType.Ruins, new(14362.3f, 13112.1f, -2742f)) }, // Carver 
            {62, new WvWObjectiveData(ObjectiveType.Ruins, new(14065.1f, 13339.5f, -1168f)) }, // Lost Prayers 

            {33, new WvWObjectiveData(ObjectiveType.Keep, new(13035.1f, 12956.6f, -300.694f)) }, // Ascension
            {35, new WvWObjectiveData(ObjectiveType.Tower, new(13688.9f, 13339f, -1892.9f)) }, // Redbriar
            {53, new WvWObjectiveData(ObjectiveType.Camp, new(13262.3f, 13457.1f, -687.909f)) }, // Redvale

            {32, new WvWObjectiveData(ObjectiveType.Keep, new(15252.2f, 12880.7f, -3107.91f)) }, // Askalion
            {36, new WvWObjectiveData(ObjectiveType.Tower, new(14581f, 13409.9f, -1821.91f)) }, // Greenlake
            {50, new WvWObjectiveData(ObjectiveType.Camp, new(15015.7f, 13502.9f, -10.3619f)) }, // Greenwater

            {34, new WvWObjectiveData(ObjectiveType.Camp, new(14083.2f, 14033.2f, -307.1f)) }, // Demesne
        } },
        {MapIDs.GreenAlpineBorderland, new()
        {

            {39, new WvWObjectiveData(ObjectiveType.Camp, new(6914.78f, 11868.4f, -3676.89f)) }, // Titanpaw
            {38, new WvWObjectiveData(ObjectiveType.Tower, new(6276.77f, 12718.2f, -3758.65f)) }, // Sunnyhill
            {37, new WvWObjectiveData(ObjectiveType.Keep, new(6888.59f, 13070.9f, -2800.76f)) }, // Green Garrison
            {40, new WvWObjectiveData(ObjectiveType.Tower, new(7515.42f, 12670.3f, -4839.9f)) }, // Cragtop
            {52, new WvWObjectiveData(ObjectiveType.Camp, new(6043.87f, 12835.7f, -46.1562f)) }, // Faithleap 
            {51, new WvWObjectiveData(ObjectiveType.Camp, new(7857.45f, 12808.3f, -1533.33f)) }, // Foghaven 

            {64, new WvWObjectiveData(ObjectiveType.Ruins, new(6691.21f, 13343.1f, -393f)) }, // Gzertzz 
            {65, new WvWObjectiveData(ObjectiveType.Ruins, new(7159.09f, 13397, -711)) }, // Cohen 
            {63, new WvWObjectiveData(ObjectiveType.Ruins, new(6593, 13714.7f, -0.649902f)) }, // Norfolk 
            {66, new WvWObjectiveData(ObjectiveType.Ruins, new(7194.27f, 13752.1f, -2742)) }, // Patrick 
            {62, new WvWObjectiveData(ObjectiveType.Ruins, new(6897.13f, 13979.5f, -1168)) }, // Fallen

            {33, new WvWObjectiveData(ObjectiveType.Keep, new(5867.06f, 13596.6f, -300.694f)) }, // Dradfall Bay
            {35, new WvWObjectiveData(ObjectiveType.Tower, new(6520.89f, 13979, -1892.9f)) }, // Bluebriar
            {53, new WvWObjectiveData(ObjectiveType.Camp, new(6094.29f, 14097.1f, -687.909f)) }, // Bluevale

            {32, new WvWObjectiveData(ObjectiveType.Keep, new(8084.25f, 13520.7f, -3107.91f)) }, // Shadaran Hills
            {36, new WvWObjectiveData(ObjectiveType.Tower, new(7413.05f, 14049.9f, -1821.91f)) }, // Redlake
            {50, new WvWObjectiveData(ObjectiveType.Camp, new(7847.75f, 14142.9f, -10.3619f)) }, // Redwater

            {34, new WvWObjectiveData(ObjectiveType.Camp, new(6915.25f, 14673.2f, -307.1f)) }, // Hero's Lodge
        } },
        {MapIDs.RedDesertBorderland, new()
        {
            {99, new WvWObjectiveData(ObjectiveType.Camp, new(10743.8f,  9492.51f, -2955)) }, // Hamm
            {102, new WvWObjectiveData(ObjectiveType.Tower, new(9831.82f, 9507.67f, -2897.5f)) }, // O'del
            {113, new WvWObjectiveData(ObjectiveType.Keep, new(10776.6f, 10120.4f, -4120.01f)) }, // Stoic Rampart
            {104, new WvWObjectiveData(ObjectiveType.Tower, new(11739.2f, 9654.33f, -4452.81f)) }, // Eternal Necropolis
            {115, new WvWObjectiveData(ObjectiveType.Camp, new(9310.12f, 10008, -1283.35f)) }, // Boettiger 
            {109, new WvWObjectiveData(ObjectiveType.Camp, new(12097.5f, 10018.3f, -1025.05f)) }, // Roy's 

            {122, new WvWObjectiveData(ObjectiveType.Ruins, new(10725.3f, 10453.5f, -235.954f)) }, // Tilly 
            {119, new WvWObjectiveData(ObjectiveType.Ruins, new(10446.7f, 10761.6f, -620.871f)) }, // Bearce 
            {120, new WvWObjectiveData(ObjectiveType.Ruins, new(10989.5f, 10778.3f, -941.543f)) }, // Zak 
            {121, new WvWObjectiveData(ObjectiveType.Ruins, new(10399.4f, 11059.5f, -1255.37f)) }, // Darra 
            {118, new WvWObjectiveData(ObjectiveType.Ruins, new(10913.3f, 11198.2f, -992.897f)) }, // Higgins' 

            {106, new WvWObjectiveData(ObjectiveType.Keep, new(9327.72f, 10634.1f, -3714.37f)) }, // Blistering
            {110, new WvWObjectiveData(ObjectiveType.Tower, new(10243.9f, 11331.3f, -5557.72f)) }, // Parched
            {101, new WvWObjectiveData(ObjectiveType.Camp, new(9584.13f, 11316.1f, -3877.82f)) }, // Mclain

            {114, new WvWObjectiveData(ObjectiveType.Keep, new(12203, 10706.2f, -4254.64f)) }, // Osprey
            {105, new WvWObjectiveData(ObjectiveType.Tower, new(11256.9f, 11551.1f, -5219.09f)) }, // Crankshaft
            {100, new WvWObjectiveData(ObjectiveType.Camp, new(11891.4f, 11286.6f, -4736.73f)) }, // Bauer

            {116, new WvWObjectiveData(ObjectiveType.Camp, new(10754.8f, 11854.4f, -2801.74f)) }, // Dustwhisper
        } },
        {MapIDs.EternalBattleground, new()
        {

            {6, new WvWObjectiveData(ObjectiveType.Camp, new(9841.05f, 13545.8f, -508.295f)) }, // Speldan
            {17, new WvWObjectiveData(ObjectiveType.Tower, new(10256.6f, 13514.4f, -2015.34f)) }, // Mendon
            {18, new WvWObjectiveData(ObjectiveType.Tower, new(10188.8f, 14082.3f, -1657.95f)) }, // Anzalias
            {1, new WvWObjectiveData(ObjectiveType.Keep, new(10763.6f, 13655.8f, -2464.89f)) }, // Red Overlook
            {20, new WvWObjectiveData(ObjectiveType.Tower, new(11090.4f, 13488.2f, -2569.23f)) }, // Veloka
            {19, new WvWObjectiveData(ObjectiveType.Tower, new(10965.2f, 14054.6f, -1847.47f)) }, // Ogrewatch
            {5, new WvWObjectiveData(ObjectiveType.Camp, new(11279.8f, 13736.8f, -835.691f)) }, // Pangloss

            {8, new WvWObjectiveData(ObjectiveType.Camp, new(11565.5f, 14444.8f, -302.91f)) }, // Umberglade
            {22, new WvWObjectiveData(ObjectiveType.Tower, new(11766.3f, 14793.5f, -2133.39f)) }, // Bravost
            {21, new WvWObjectiveData(ObjectiveType.Tower, new(11156.4f, 14527.8f, -1622.95f)) }, // Durios
            {2, new WvWObjectiveData(ObjectiveType.Keep, new(11496.5f, 15120.6f, -1786.97f)) }, // Blue Valley
            {15, new WvWObjectiveData(ObjectiveType.Tower, new(11452.7f, 15490.7f, -2246.3f)) }, // Langor
            {16, new WvWObjectiveData(ObjectiveType.Tower, new(10850.1f, 15224.4f, -1052.29f)) }, // Quentin
            {7, new WvWObjectiveData(ObjectiveType.Camp, new(11037.9f, 15556.2f, -483.931f)) }, // Danelon

            {4, new WvWObjectiveData(ObjectiveType.Camp, new(10202.6f, 15437.1f, -79.961f)) }, // Golanta
            {13, new WvWObjectiveData(ObjectiveType.Tower, new(9805.96f, 15406.4f, -1659.98f)) }, // Jerrifer
            {14, new WvWObjectiveData(ObjectiveType.Tower, new(10171.8f, 15081.8f, -495.673f)) }, // Klovan
            {3, new WvWObjectiveData(ObjectiveType.Keep, new(9604.47f, 15129.9f, -906.09f)) }, // Green Lowlands
            {11, new WvWObjectiveData(ObjectiveType.Tower, new(9413.84f, 14792.8f, -1313.37f)) }, // Aldon
            {12, new WvWObjectiveData(ObjectiveType.Tower, new(9906.21f, 14624.6f, -1014.99f)) }, // Wildcreek
            {10, new WvWObjectiveData(ObjectiveType.Camp, new(9570.97f, 14423.2f, -700f)) }, // Rogue

            {9, new WvWObjectiveData(ObjectiveType.Castle, new(10606.3f, 14580.3f, -1536.93f)) }, // Stonemist Castle
        } },
    };

    internal static WvWObjectiveData? GetObjectiveData(int mapID, int objectiveID)
    {
        if (ObjectiveDataPerMapIDPerObjectiveID.TryGetValue(mapID, out var objectiveDataPerObjectiveID))
        {
            if (objectiveDataPerObjectiveID.TryGetValue(objectiveID, out var objectiveData))
            {
                return objectiveData;
            }
        }
        return null;
    }
}
