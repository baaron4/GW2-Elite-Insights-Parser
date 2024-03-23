using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class BuffApplyMechanic : IDBasedMechanic<BuffApplyEvent>
    {

        public BuffApplyMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this(new long[] { mechanicID }, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public BuffApplyMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }



        protected abstract AgentItem GetAgentItem(BuffApplyEvent ba);
        protected abstract AbstractSingleActor GetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, AbstractSingleActor> regroupedMobs);

        protected virtual void AddMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, BuffApplyEvent ba, AbstractSingleActor actor)
        {
            mechanicLogs[this].Add(new MechanicEvent(ba.Time, this, actor));
        }


        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (long mechanicID in MechanicIDs)
            {
                foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(mechanicID))
                {
                    if (c is BuffApplyEvent ba && Keep(ba, log))
                    {
                        AbstractSingleActor amp = GetActor(log, GetAgentItem(ba), regroupedMobs);
                        if (amp != null)
                        {
                            AddMechanic(log, mechanicLogs, ba, amp);
                        }
                    }
                }
            }
        }
    }
}
