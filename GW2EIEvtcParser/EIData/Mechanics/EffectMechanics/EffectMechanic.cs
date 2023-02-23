using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class EffectMechanic : StringBasedMechanic
    {
        public delegate bool EffectChecker(EffectEvent ba, ParsedEvtcLog log);

        private readonly EffectChecker _triggerCondition = null;

        protected bool Keep(EffectEvent c, ParsedEvtcLog log)
        {
            return _triggerCondition == null || _triggerCondition(c, log);
        }

        protected abstract AgentItem GetAgentItem(EffectEvent effectEvt, AgentData agentData);

        public EffectMechanic(string effectGUID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, EffectChecker condition = null) : this(new string[] { effectGUID }, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public EffectMechanic(string[] effectGUIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, EffectChecker condition = null) : base(effectGUIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
        }

        protected void PlayerChecker(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs)
        {
            foreach (string guid in MechanicIDs)
            {
                EffectGUIDEvent effectGUID = log.CombatData.GetEffectGUIDEvent(guid);
                if (effectGUID != null)
                {
                    IReadOnlyList<EffectEvent> effects = log.CombatData.GetEffectEventsByEffectID(effectGUID.ContentID);
                    foreach (EffectEvent effectEvent in effects)
                    {
                        AgentItem agentItem = GetAgentItem(effectEvent, log.AgentData);
                        if (log.PlayerAgents.Contains(agentItem) && Keep(effectEvent, log))
                        {
                            mechanicLogs[this].Add(new MechanicEvent(effectEvent.Time, this, log.FindActor(agentItem)));
                        }
                    }
                }
            }
        }

        protected void EnemyChecker(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (string guid in MechanicIDs)
            {
                EffectGUIDEvent effectGUID = log.CombatData.GetEffectGUIDEvent(guid);
                if (effectGUID != null)
                {
                    IReadOnlyList<EffectEvent> effects = log.CombatData.GetEffectEventsByEffectID(effectGUID.ContentID);
                    foreach (EffectEvent effectEvent in effects)
                    {
                        AgentItem agentItem = GetAgentItem(effectEvent, log.AgentData);
                        if (agentItem.IsSpecy(ArcDPSEnums.TrashID.Environment) && Keep(effectEvent, log))
                        {
                            mechanicLogs[this].Add(new MechanicEvent(effectEvent.Time, this, log.FindActor(agentItem, true)));
                        } 
                        else
                        {
                            AbstractSingleActor actor = MechanicHelper.FindEnemyActor(log, agentItem, regroupedMobs);
                            if (actor != null && Keep(effectEvent, log))
                            {
                                mechanicLogs[this].Add(new MechanicEvent(effectEvent.Time, this, actor));
                            }
                        }
                    }
                }
            }
        }
    }
}
