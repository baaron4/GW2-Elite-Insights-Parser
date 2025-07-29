using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData;

internal class MarkerCastFinder : CheckedCastFinder<MarkerEvent>
{
    protected bool Minions = false;
    private readonly GUID _markerGUID;
    private int _speciesID = 0;

    public MarkerCastFinder WithMinions()
    {
        Minions = true;
        return this;
    }

    protected AgentItem GetAgent(MarkerEvent markerEvent)
    {
        return Minions ? GetKeyAgent(markerEvent).GetFinalMaster() : GetKeyAgent(markerEvent);
    }

    protected virtual AgentItem GetKeyAgent(MarkerEvent markerEvent)
    {
        return markerEvent.Src;
    }

    public MarkerCastFinder(long skillID, GUID markerGUID) : base(skillID)
    {
        UsingNotAccurate();
        UsingEnable((combatData) => combatData.HasEffectData);
        _markerGUID = markerGUID;
    }

    internal MarkerCastFinder UsingSrcBaseSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.Src.BaseSpec == spec);
        return this;
    }

    internal MarkerCastFinder UsingSrcNotBaseSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.Src.BaseSpec != spec);
        return this;
    }

    internal MarkerCastFinder UsingSrcSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.Src.Spec == spec);
        return this;
    }

    internal MarkerCastFinder UsingSrcNotSpecChecker(Spec spec)
    {
        UsingChecker((evt, combatData, agentData, skillData) => evt.Src.Spec != spec);
        return this;
    }

    internal MarkerCastFinder UsingSrcSpecsChecker(HashSet<Spec> specs)
    {
        UsingChecker((evt, combatData, agentData, skillData) => specs.Contains(evt.Src.Spec));
        return this;
    }
    internal MarkerCastFinder UsingSrcNotSpecsChecker(HashSet<Spec> specs)
    {
        UsingChecker((evt, combatData, agentData, skillData) => !specs.Contains(evt.Src.Spec));
        return this;
    }

    internal MarkerCastFinder UsingAgentRedirectionIfUnknown(int speciesID)
    {
        _speciesID = speciesID;
        return this;
    }

    public override List<InstantCastEvent> ComputeInstantCast(CombatData combatData, SkillData skillData, AgentData agentData)
    {
        var res = new List<InstantCastEvent>();
        var markerGUIDEvent = combatData.GetMarkerGUIDEvent(_markerGUID);
        var markers = combatData.GetMarkerEventsByMarkerID(markerGUIDEvent.ContentID).GroupBy(GetAgent);
        foreach (var group in markers)
        {
            if (group.Key.IsUnknown)
            {
                continue;
            }
            long lastTime = int.MinValue;
            foreach (MarkerEvent markerEvent in group)
            {
                if (CheckCondition(markerEvent, combatData, agentData, skillData))
                {
                    if (markerEvent.Time - lastTime < ICD)
                    {
                        lastTime = markerEvent.Time;
                        continue;
                    }
                    lastTime = markerEvent.Time;
                    var caster = group.Key;
                    if (_speciesID > 0 && caster.IsUnamedSpecies())
                    {
                        AgentItem? agent = agentData.GetNPCsByID(_speciesID).FirstOrDefault(x => x.LastAware >= markerEvent.Time && x.FirstAware <= markerEvent.Time);
                        if (agent != null)
                        {
                            caster = agent;
                        }
                    }
                    res.Add(new InstantCastEvent(GetTime(markerEvent, GetKeyAgent(markerEvent), combatData), skillData.Get(SkillID), caster));
                }
            }
        }
        return res;
    }
}
