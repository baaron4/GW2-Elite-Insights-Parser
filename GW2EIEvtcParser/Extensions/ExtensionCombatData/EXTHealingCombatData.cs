using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.Extensions.HealingStatsExtensionHandler;

namespace GW2EIEvtcParser.Extensions
{
    public class EXTHealingCombatData
    {
        private readonly Dictionary<AgentItem, List<EXTAbstractHealingEvent>> _healData;
        private readonly Dictionary<AgentItem, List<EXTAbstractHealingEvent>> _healReceivedData;
        private readonly Dictionary<long, List<EXTAbstractHealingEvent>> _healDataById;

        // to be filled
        private static readonly HashSet<long> HybridHealIDs = new HashSet<long>()
        {

        };

        private readonly Dictionary<long, EXTHealingType> EncounteredIDs = new Dictionary<long, EXTHealingType>();

        internal EXTHealingCombatData(Dictionary<AgentItem, List<EXTAbstractHealingEvent>> healData, Dictionary<AgentItem, List<EXTAbstractHealingEvent>> healReceivedData, Dictionary<long, List<EXTAbstractHealingEvent>> healDataById) 
        {
            _healData = healData;
            _healReceivedData = healReceivedData;
            _healDataById = healDataById;
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

        public EXTHealingType GetHealingType(SkillItem skill, ParsedEvtcLog log)
        {
            if (HybridHealIDs.Contains(skill.ID))
            {
                return EXTHealingType.HealingPower;
            }
            if (EncounteredIDs.TryGetValue(skill.ID, out EXTHealingType type))
            {
                return type;
            }
            if (log.CombatData.GetDamageData(skill.ID).Any(x => !x.DoubleProcHit))
            {
                type = EXTHealingType.ConversionBased;
            } 
            else
            {
                type = EXTHealingType.HealingPower;
            }
            EncounteredIDs[skill.ID] = type;
            return type;
        }

    }
}
