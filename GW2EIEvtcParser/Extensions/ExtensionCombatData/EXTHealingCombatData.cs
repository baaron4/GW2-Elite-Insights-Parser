using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTHealingCombatData
    {
        private readonly Dictionary<AgentItem, List<EXTAbstractHealingEvent>> _healData;
        private readonly Dictionary<AgentItem, List<EXTAbstractHealingEvent>> _healReceivedData;
        private readonly Dictionary<long, List<EXTAbstractHealingEvent>> _healDataById;

        private readonly Dictionary<long, EXTHealingType> EncounteredIDs = new Dictionary<long, EXTHealingType>();

        private readonly HashSet<long> HybridHealIDs;

        internal EXTHealingCombatData(Dictionary<AgentItem, List<EXTAbstractHealingEvent>> healData, Dictionary<AgentItem, List<EXTAbstractHealingEvent>> healReceivedData, Dictionary<long, List<EXTAbstractHealingEvent>> healDataById, HashSet<long> hybridHealIDs)
        {
            _healData = healData;
            _healReceivedData = healReceivedData;
            _healDataById = healDataById;
            HybridHealIDs = hybridHealIDs;
        }

        public IReadOnlyList<EXTAbstractHealingEvent> GetHealData(AgentItem key)
        {
            if (_healData.TryGetValue(key, out List<EXTAbstractHealingEvent> res))
            {
                return res;
            }
            return new List<EXTAbstractHealingEvent>();
        }
        public IReadOnlyList<EXTAbstractHealingEvent> GetHealReceivedData(AgentItem key)
        {
            if (_healReceivedData.TryGetValue(key, out List<EXTAbstractHealingEvent> res))
            {
                return res;
            }
            return new List<EXTAbstractHealingEvent>();
        }

        public IReadOnlyList<EXTAbstractHealingEvent> GetHealData(long key)
        {
            if (_healDataById.TryGetValue(key, out List<EXTAbstractHealingEvent> res))
            {
                return res;
            }
            return new List<EXTAbstractHealingEvent>();
        }

        public EXTHealingType GetHealingType(long id, ParsedEvtcLog log)
        {
            if (HybridHealIDs.Contains(id))
            {
                return EXTHealingType.Hybrid;
            }
            if (EncounteredIDs.TryGetValue(id, out EXTHealingType type))
            {
                return type;
            }
            if (log.CombatData.GetDamageData(id).Any(x => x.HealthDamage > 0 && !x.DoubleProcHit))
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
}
