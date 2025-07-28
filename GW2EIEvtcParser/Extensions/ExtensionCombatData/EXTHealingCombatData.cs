using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions;

public class EXTHealingCombatData
{
    private readonly Dictionary<AgentItem, List<EXTHealingEvent>> _healData;
    private readonly Dictionary<AgentItem, List<EXTHealingEvent>> _healReceivedData;
    private readonly Dictionary<long, List<EXTHealingEvent>> _healDataByID;

    private readonly Dictionary<long, EXTHealingType> EncounteredIDs = []; //TODO(Rennorb) @perf

    private readonly IReadOnlyCollection<long> _hybridHealIDs;

    internal EXTHealingCombatData(Dictionary<AgentItem, List<EXTHealingEvent>> healData, Dictionary<AgentItem, List<EXTHealingEvent>> healReceivedData, Dictionary<long, List<EXTHealingEvent>> healDataByID, IReadOnlyCollection<long> hybridHealIDs)
    {
        _healData = healData;
        _healReceivedData = healReceivedData;
        _healDataByID = healDataByID;
        _hybridHealIDs = hybridHealIDs;
    }

    public IReadOnlyList<EXTHealingEvent> GetHealData(AgentItem key)
    {
        return CombatData.GetTimeValueOfEmpty(_healData, key);
    }
    public IReadOnlyList<EXTHealingEvent> GetHealReceivedData(AgentItem key)
    {
        return CombatData.GetTimeValueOfEmpty(_healReceivedData, key);
    }

    public IReadOnlyList<EXTHealingEvent> GetHealData(long key)
    {
        return _healDataByID.GetValueOrEmpty(key);
    }

    public EXTHealingType GetHealingType(long id, ParsedEvtcLog log)
    {
        if (_hybridHealIDs.Contains(id))
        {
            return EXTHealingType.Hybrid;
        }
        if (EncounteredIDs.TryGetValue(id, out EXTHealingType type))
        {
            return type;
        }
        if (log.CombatData.GetDamageData(id).Any(x => x.HealthDamage > 0 && !x.IsNotADamageEvent))
        {
            type = EXTHealingType.ConversionBased;
        }
        else
        {
            type = EXTHealingType.HealingPower;
        }
        EncounteredIDs[id] = type;
        return type;
    }

    public EXTHealingType GetHealingType(SkillItem skill, ParsedEvtcLog log)
    {
        return GetHealingType(skill.ID, log);
    }

    public EXTHealingType GetHealingType(Buff buff, ParsedEvtcLog log)
    {
        return GetHealingType(buff.ID, log);
    }

}
