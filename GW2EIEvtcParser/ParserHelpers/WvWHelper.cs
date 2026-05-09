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
                return "Resource Camp";
            case ObjectiveType.Tower:
                return "Tower";
            case ObjectiveType.Ruins:
                return "Ruins";
            case ObjectiveType.Keep:
                return "Camp";
            case ObjectiveType.Castle:
                return "Stonemist Castle";
        }
        return "None";
    }

    // To be filled with neutral, red, green, blue icon versions for each type
    private static readonly Dictionary<ObjectiveType, Dictionary<ObjectiveOwnership, string>> TypeIconsPerOwner = new() {
        {ObjectiveType.Camp, new() {
            {ObjectiveOwnership.Red, "https://render.guildwars2.com/file/015D365A08AAE105287A100AAE04529FDAE14155/102532.png" },
            {ObjectiveOwnership.Green, "https://render.guildwars2.com/file/015D365A08AAE105287A100AAE04529FDAE14155/102532.png" },
            {ObjectiveOwnership.Blue, "https://render.guildwars2.com/file/015D365A08AAE105287A100AAE04529FDAE14155/102532.png" },
            {ObjectiveOwnership.None, "https://render.guildwars2.com/file/015D365A08AAE105287A100AAE04529FDAE14155/102532.png" },
        }},
        {ObjectiveType.Ruins, new() {
            {ObjectiveOwnership.Red, "https://render.guildwars2.com/file/52B43242E55961770D78B80ED77BC764F0E57BF2/1635237.png" },
            {ObjectiveOwnership.Green, "https://render.guildwars2.com/file/52B43242E55961770D78B80ED77BC764F0E57BF2/1635237.png" },
            {ObjectiveOwnership.Blue, "https://render.guildwars2.com/file/52B43242E55961770D78B80ED77BC764F0E57BF2/1635237.png" },
            {ObjectiveOwnership.None, "https://render.guildwars2.com/file/52B43242E55961770D78B80ED77BC764F0E57BF2/1635237.png" },
        }},
        {ObjectiveType.Tower, new() {
            {ObjectiveOwnership.Red, "https://render.guildwars2.com/file/ABEC80C79576A103EA33EC66FCB99B77291A2F0D/102531.png" },
            {ObjectiveOwnership.Green, "https://render.guildwars2.com/file/ABEC80C79576A103EA33EC66FCB99B77291A2F0D/102531.png" },
            {ObjectiveOwnership.Blue, "https://render.guildwars2.com/file/ABEC80C79576A103EA33EC66FCB99B77291A2F0D/102531.png" },
            {ObjectiveOwnership.None, "https://render.guildwars2.com/file/ABEC80C79576A103EA33EC66FCB99B77291A2F0D/102531.png" },
        }},
        {ObjectiveType.Keep, new() {
            {ObjectiveOwnership.Red, "https://render.guildwars2.com/file/DB580419C8AD9449309A96C8E7C3D61631020EBB/102535.png" },
            {ObjectiveOwnership.Green, "https://render.guildwars2.com/file/DB580419C8AD9449309A96C8E7C3D61631020EBB/102535.png" },
            {ObjectiveOwnership.Blue, "https://render.guildwars2.com/file/DB580419C8AD9449309A96C8E7C3D61631020EBB/102535.png" },
            {ObjectiveOwnership.None, "https://render.guildwars2.com/file/DB580419C8AD9449309A96C8E7C3D61631020EBB/102535.png" },
        }},
        {ObjectiveType.Castle, new() {
            {ObjectiveOwnership.Red, "https://render.guildwars2.com/file/F0F1DA1C807444F4DF53090343F43BED02E50523/102608.png" },
            {ObjectiveOwnership.Green, "https://render.guildwars2.com/file/F0F1DA1C807444F4DF53090343F43BED02E50523/102608.png" },
            {ObjectiveOwnership.Blue, "https://render.guildwars2.com/file/F0F1DA1C807444F4DF53090343F43BED02E50523/102608.png" },
            {ObjectiveOwnership.None, "https://render.guildwars2.com/file/F0F1DA1C807444F4DF53090343F43BED02E50523/102608.png" },
        }},
    };

    internal class WvWObjectiveData
    {
        public readonly ObjectiveType Type;
        public readonly Vector3 Position;

        public bool IsUnknown => Type == ObjectiveType.Unknown;

        public WvWObjectiveData(ObjectiveType type, Vector3 position)
        {
            Type = type;
            Position = position;
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
            {39, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Spiritholm
            {38, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Woodhaven
            {37, new WvWObjectiveData(ObjectiveType.Keep, new()) }, // Blue Garrison
            {40, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Dawn's Eyrie
            {52, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Godslore 
            {51, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Stargrove 

            {64, new WvWObjectiveData(ObjectiveType.Ruins, new()) }, // Baeur 
            {65, new WvWObjectiveData(ObjectiveType.Ruins, new()) }, // Orchard 
            {63, new WvWObjectiveData(ObjectiveType.Ruins, new()) }, // Battle 
            {66, new WvWObjectiveData(ObjectiveType.Ruins, new()) }, // Carver 
            {62, new WvWObjectiveData(ObjectiveType.Ruins, new()) }, // Lost Prayers 

            {33, new WvWObjectiveData(ObjectiveType.Keep, new()) }, // Ascension
            {35, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Redbriar
            {53, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Redvale

            {32, new WvWObjectiveData(ObjectiveType.Keep, new()) }, // Askalion
            {32, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Greenlake
            {50, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Greenwater

            {34, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Demesne
        } },
        {MapIDs.GreenAlpineBorderland, new()
        {

            {39, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Titanpaw
            {38, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Sunnyhill
            {37, new WvWObjectiveData(ObjectiveType.Keep, new()) }, // Green Garrison
            {40, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Cragtop
            {52, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Faithleap 
            {51, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Foghaven 

            {64, new WvWObjectiveData(ObjectiveType.Ruins, new(6691.21f, 13343.1f, -393f)) }, // Gzertzz 
            {65, new WvWObjectiveData(ObjectiveType.Ruins, new()) }, // Cohen 
            {63, new WvWObjectiveData(ObjectiveType.Ruins, new()) }, // Norfolk 
            {66, new WvWObjectiveData(ObjectiveType.Ruins, new()) }, // Patrick 
            {62, new WvWObjectiveData(ObjectiveType.Ruins, new()) }, // Fallen

            {33, new WvWObjectiveData(ObjectiveType.Keep, new()) }, // Dradfall Bay
            {35, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Bluebriar
            {53, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Bluevale

            {32, new WvWObjectiveData(ObjectiveType.Keep, new()) }, // Shadaran Hills
            {32, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Redlake
            {50, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Redwater

            {34, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Hero's Lodge
        } },
        {MapIDs.RedDesertBorderland, new()
        {
             {99, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Hamm
            {102, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // O'del
            {37, new WvWObjectiveData(ObjectiveType.Keep, new()) }, // Stoic Rampart
            {104, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Eternal Necropolis
            {115, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Boettiger 
            {109, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Roy's 

            {122, new WvWObjectiveData(ObjectiveType.Ruins, new()) }, // Tilly 
            {119, new WvWObjectiveData(ObjectiveType.Ruins, new()) }, // Bearce 
            {120, new WvWObjectiveData(ObjectiveType.Ruins, new()) }, // Zak 
            {121, new WvWObjectiveData(ObjectiveType.Ruins, new()) }, // Darra 
            {118, new WvWObjectiveData(ObjectiveType.Ruins, new()) }, // Higgins' 

            {106, new WvWObjectiveData(ObjectiveType.Keep, new()) }, // Blistering
            {110, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Parched
            {101, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Mclain

            {114, new WvWObjectiveData(ObjectiveType.Keep, new()) }, // Osprey
            {105, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Crankshaft
            {100, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Bauer

            {116, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Dustwhisper
        } },
        {MapIDs.EternalBattleground, new()
        {

            {6, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Speldan
            {17, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Mendon
            {18, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Anzalias
            {1, new WvWObjectiveData(ObjectiveType.Keep, new()) }, // Red Overlook
            {20, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Veloka
            {19, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Ogrewatch
            {5, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Pangloss

            {8, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Umberglade
            {22, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Bravost
            {21, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Durios
            {2, new WvWObjectiveData(ObjectiveType.Keep, new()) }, // Blue Valley
            {15, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Langor
            {16, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Quentin
            {7, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Danelon

            {4, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Golanta
            {13, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Jerrifer
            {14, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Klovan
            {3, new WvWObjectiveData(ObjectiveType.Keep, new()) }, // Green Lowlands
            {11, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Aldon
            {12, new WvWObjectiveData(ObjectiveType.Tower, new()) }, // Wildcreek
            {10, new WvWObjectiveData(ObjectiveType.Camp, new()) }, // Rogue

            {9, new WvWObjectiveData(ObjectiveType.Castle, new()) }, // Stonemist Castle
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
