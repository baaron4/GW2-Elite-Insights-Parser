using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class EffectMechanic : StringBasedMechanic<EffectEvent>
    {

        protected abstract AgentItem GetAgentItem(EffectEvent effectEvt, AgentData agentData);

        public EffectMechanic(string effectGUID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this(new string[] { effectGUID }, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public EffectMechanic(string[] effectGUIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(effectGUIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        protected void PlayerChecker(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs)
        {
            foreach (string guid in MechanicIDs)
            {
                if (log.CombatData.TryGetEffectEventsByGUID(guid, out IReadOnlyList<EffectEvent> effects))
                {
                    foreach (EffectEvent effectEvent in effects)
                    {
                        AgentItem agentItem = GetAgentItem(effectEvent, log.AgentData);
                        if (log.PlayerAgents.Contains(agentItem) && Keep(effectEvent, log))
                        {
                            InsertMechanic(log, mechanicLogs, effectEvent.Time, log.FindActor(agentItem));
                        }
                    }
                }
            }
        }

        protected void EnemyChecker(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (string guid in MechanicIDs)
            {
                if (log.CombatData.TryGetEffectEventsByGUID(guid, out IReadOnlyList<EffectEvent> effects))
                {
                    foreach (EffectEvent effectEvent in effects)
                    {
                        AgentItem agentItem = GetAgentItem(effectEvent, log.AgentData);
                        if (agentItem.IsSpecies(ArcDPSEnums.TrashID.Environment) && Keep(effectEvent, log))
                        {
                            InsertMechanic(log, mechanicLogs, effectEvent.Time, log.FindActor(agentItem, true));
                        }
                        else
                        {
                            AbstractSingleActor actor = MechanicHelper.FindEnemyActor(log, agentItem, regroupedMobs);
                            if (actor != null && Keep(effectEvent, log))
                            {
                                InsertMechanic(log, mechanicLogs, effectEvent.Time, actor);
                            }
                        }
                    }
                }
            }
        }
    }
}
