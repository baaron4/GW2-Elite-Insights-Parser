namespace GW2EIGW2API.GW2API;

public class GW2APISkill : GW2APIBaseItem
{
    public string Name;
    public string Description;
    public string Icon;
    public string ChatLink;
    public string Type;
    public string WeaponType;
    public IReadOnlyList<string> Professions;
    public IReadOnlyList<string> Flags;
    public string Slot;
    public IReadOnlyList<GW2APIFact> Facts;
    public IReadOnlyList<GW2APITraitedFact> TraitedFacts;
    public IReadOnlyList<string> Categories;
    public string Attunement;
    public int Cost;
    public string DualWield;
    public int FlipSkill;
    public int Initiative;
    public int NextChain;
    public int PrevChain;
    public IReadOnlyList<long> TransformSkills;
    public IReadOnlyList<long> BundleSkills;

    public int ToolbeltSkill;
}

