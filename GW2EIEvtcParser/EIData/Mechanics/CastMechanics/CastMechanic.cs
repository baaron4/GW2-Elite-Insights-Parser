using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class CastMechanic : IDBasedMechanic<AbstractCastEvent>
    {

        protected abstract long GetTime(AbstractCastEvent evt);

        protected abstract AbstractSingleActor GetActor(ParsedEvtcLog log, AgentItem agentItem, Dictionary<int, AbstractSingleActor> regroupedMobs);

        public CastMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this(new long[] { mechanicID }, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public CastMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
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
