using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class CastMechanic : IDBasedMechanic
    {
        public delegate bool CastChecker(AbstractCastEvent ce, ParsedEvtcLog log);

        private readonly CastChecker _triggerCondition = null;

        protected bool Keep(AbstractCastEvent c, ParsedEvtcLog log)
        {
            return _triggerCondition == null || _triggerCondition(c, log);
        }

        protected abstract long GetTime(AbstractCastEvent evt);

        protected abstract AbstractSingleActor GetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, AbstractSingleActor> regroupedMobs);

        public CastMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CastChecker condition = null) : this(new long[] { mechanicID }, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public CastMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CastChecker condition = null) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (long mechanicID in MechanicIDs)
            {
                foreach (AbstractCastEvent c in log.CombatData.GetAnimatedCastData(mechanicID))
                {
                    if (Keep(c, log))
                    {
                        AbstractSingleActor amp = GetActor(log, c.Caster, regroupedMobs);
                        if (amp != null)
                        {
                            mechanicLogs[this].Add(new MechanicEvent(GetTime(c), this, amp));
                        }
                    }
                }
            }

        }

    }
}
