using System.Diagnostics.CodeAnalysis;

namespace GW2EIEvtcParser.ParsedData;

public class SkillData
{
    public IReadOnlyCollection<SkillItem> AllSkills => _skills.Values;
    // Fields
    private readonly Dictionary<long, SkillItem> _skills = [];
    private readonly GW2EIGW2API.GW2APIController _apiController;
    public readonly long DodgeID;
    public readonly long GenericBreakbarID;

    private readonly ulong _gw2Build;
    private readonly EvtcVersionEvent _evtcVersionEvent;
    // Public Methods

    internal SkillData(GW2EIGW2API.GW2APIController apiController, EvtcVersionEvent evtcVersion, ulong gw2Build)
    {
        _apiController = apiController;
        (DodgeID, GenericBreakbarID) = SkillItem.GetArcDPSCustomIDs(evtcVersion);
        _gw2Build = gw2Build;
        _evtcVersionEvent = evtcVersion;
    }

    public SkillItem Get(long ID)
    {
        if (_skills.TryGetValue(ID, out var value))
        {
            return value;
        }
        Add(ID, SkillItem.DefaultName);
        return _skills[ID];
    }


    internal bool TryGet(long ID, [NotNullWhen(true)] out SkillItem? skillItem)
    {
        return _skills.TryGetValue(ID, out skillItem);
    }

    internal HashSet<long> NotAccurate = [];

    public bool IsNotAccurate(long ID)
    {
        return NotAccurate.Contains(ID);
    }

    internal HashSet<long> GearProc = [];
    public bool IsGearProc(long ID)
    {
        return GearProc.Contains(ID);
    }

    internal HashSet<long> TraitProc = [];
    public bool IsTraitProc(long ID)
    {
        return TraitProc.Contains(ID);
    }

    internal HashSet<long> UnconditionalProc = [];
    public bool IsUnconditionalProc(long ID)
    {
        return UnconditionalProc.Contains(ID);
    }

    internal void Add(long id, string name)
    {
        _skills.TryAdd(id, new SkillItem(id, name, _apiController, _gw2Build));
    }

}
