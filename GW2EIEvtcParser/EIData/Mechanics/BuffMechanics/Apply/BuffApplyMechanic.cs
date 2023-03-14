using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class BuffApplyMechanic : IDBasedMechanic
    {
        public delegate bool BuffApplyChecker(BuffApplyEvent ba, ParsedEvtcLog log);

        private readonly BuffApplyChecker _triggerCondition = null;

        protected bool Keep(BuffApplyEvent c, ParsedEvtcLog log)
        {
            return _triggerCondition == null || _triggerCondition(c, log);
        }

        public BuffApplyMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffApplyChecker condition = null) : this(new long[] { mechanicID }, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public BuffApplyMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffApplyChecker condition = null) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
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
