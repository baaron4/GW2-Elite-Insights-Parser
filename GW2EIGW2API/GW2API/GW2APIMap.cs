namespace GW2EIGW2API.GW2API;

public class GW2APIMap : GW2APIBaseItem
{
    public string Name;
    public int MinLevel;
    public int MaxLevel;
    public int DefaultFloor;
    public string Type;
    public IReadOnlyList<int> Floors;
    public int RegionId;
    public string RegionName;
    public int ContinentId;
    public string ContinentName;
    public IReadOnlyList<IReadOnlyList<int>> MapRect;
    public IReadOnlyList<IReadOnlyList<int>> ContinentRect;
}

