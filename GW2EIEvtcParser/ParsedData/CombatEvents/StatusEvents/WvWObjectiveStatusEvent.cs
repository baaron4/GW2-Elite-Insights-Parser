using System.Numerics;
using static GW2EIEvtcParser.WvWHelper;

namespace GW2EIEvtcParser.ParsedData;

public class WvWObjectiveStatusEvent
{
    public readonly int MapID;
    public readonly int ObjectiveID;

    public ObjectiveType ObjectiveType => _objectiveData != null ? _objectiveData.Type : ObjectiveType.Unknown;

    public readonly List<(uint TeamID, long Time)> Owners = [];

    private readonly WvWObjectiveData? _objectiveData;

    internal bool IsUnknown => _objectiveData == null || _objectiveData.IsUnknown;

    internal WvWObjectiveStatusEvent(CombatItem evtcItem)
    {
        MapID = evtcItem.Value;
        ObjectiveID = evtcItem.BuffDmg;
        Owners.Add((evtcItem.SkillID, evtcItem.Time));
        _objectiveData = GetObjectiveData(MapID, ObjectiveID);
    }

    internal void AddOwners(WvWObjectiveStatusEvent other)
    {
        Owners.AddRange(other.Owners);
    }

    internal string GetIcon(ParsedEvtcLog log, uint teamID)
    {
        if (_objectiveData == null)
        {
            return "";
        }
        var wvwTeams = log.CombatData.GetWvWTeamsEvent();
        if (wvwTeams == null)
        {
            return "";
        }
        var ownership = ObjectiveOwnership.None;
        if (teamID == wvwTeams.RedTeamID)
        {
            ownership = ObjectiveOwnership.Red;
        }
        else if (teamID == wvwTeams.BlueTeamID)
        {
            ownership = ObjectiveOwnership.Blue;
        }
        else if (teamID == wvwTeams.GreenTeamID)
        {
            ownership = ObjectiveOwnership.Green;
        }
        return _objectiveData.GetIcon(ownership);
    }

    public Vector3 GetPosition()
    {
        if (_objectiveData == null)
        {
            return new Vector3();
        }
        return new (_objectiveData.Position.X, _objectiveData.Position.Y, _objectiveData.Position.Z);
    }

}
