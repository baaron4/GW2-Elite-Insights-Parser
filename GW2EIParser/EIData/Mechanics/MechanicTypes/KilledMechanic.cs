using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{

    public class KilledMechanic : Mechanic
    {

        public KilledMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public KilledMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            foreach (AgentItem a in log.AgentData.GetNPCsByID((int)SkillId))
            {
                if (!regroupedMobs.TryGetValue(a.ID, out AbstractSingleActor amp))
                {
                    amp = log.FindActor(a, false);
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
