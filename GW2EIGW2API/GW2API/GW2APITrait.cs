namespace GW2EIGW2API.GW2API;

public class GW2APITrait : GW2APIBaseItem
{
    public string Name;
    public string Icon;
    public string Description;
    public int Specialization;
    public int Tier;
    public string Slot;
    public IReadOnlyList<GW2APIFact> Facts;
    public IReadOnlyList<GW2APITraitedFact> TraitedFacts;
    public IReadOnlyList<GW2APISkill> Skills;
}

