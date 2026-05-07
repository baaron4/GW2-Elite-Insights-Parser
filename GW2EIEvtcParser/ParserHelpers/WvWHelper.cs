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
        Tower,
        Keep,
        StonemistCastle
    }

    // To be filled with neutral, red, green, blue icon versions for each type
    private static readonly Dictionary<ObjectiveType, Dictionary<ObjectiveOwnership, string>> TypeIconsPerOwner = [];

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
    private static readonly Dictionary<int, Dictionary<int, WvWObjectiveData>> ObjectiveDataPerMapIDPerObjectiveID = [];

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
