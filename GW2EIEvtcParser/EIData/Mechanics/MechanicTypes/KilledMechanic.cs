using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{

    internal class KilledMechanic : Mechanic
    {

        public KilledMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public KilledMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            foreach (AgentItem a in log.AgentData.GetNPCsByID((int)SkillId))
            {
                if (!regroupedMobs.TryGetValue(a.ID, out AbstractSingleActor amp))
                {
                    amp = log.FindActor(a, false);
                    if (amp == null)
                    {
                        continue;
                    }
                    regroupedMobs.Add(amp.ID, amp);
                }
                foreach (DeadEvent devt in log.CombatData.GetDeadEvents(a))
                {
                    mechanicLogs[this].Add(new MechanicEvent(devt.Time, this, amp));
                }
            }
        }
    }
}
