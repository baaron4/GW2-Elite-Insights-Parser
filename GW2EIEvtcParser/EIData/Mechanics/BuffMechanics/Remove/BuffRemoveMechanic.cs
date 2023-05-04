using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class BuffRemoveMechanic : IDBasedMechanic<BuffRemoveAllEvent>
    {

        public BuffRemoveMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this(new long[] { mechanicID }, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public BuffRemoveMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        protected abstract AgentItem GetAgentItem(BuffRemoveAllEvent brae);
        protected abstract AbstractSingleActor GetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, AbstractSingleActor> regroupedMobs);

        protected virtual void AddMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, BuffRemoveAllEvent brae, AbstractSingleActor actor)
        {
            mechanicLogs[this].Add(new MechanicEvent(brae.Time, this, actor));
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (long mechanicID in MechanicIDs)
            {
                foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(mechanicID))
                {
                    if (c is BuffRemoveAllEvent brae && Keep(brae, log))
                    {
                        AbstractSingleActor amp = MechanicHelper.FindEnemyActor(log, GetAgentItem(brae), regroupedMobs);
                        if (amp != null)
                        {
                            AddMechanic(log, mechanicLogs, brae, amp);
                        }
                    }
                }
            }
        }

    }
}
