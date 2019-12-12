using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{

    public class SpawnMechanic : Mechanic
    {

        public SpawnMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public SpawnMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<ushort, AbstractSingleActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            foreach (AgentItem a in log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.ID == SkillId))
            {
                if (!regroupedMobs.TryGetValue(a.ID, out AbstractSingleActor amp))
                {
                    amp = log.FindActor(a, false);
                    regroupedMobs.Add(amp.ID, amp);
                }
                mechanicLogs[this].Add(new MechanicEvent(a.FirstAware, this, amp));
            }
        }
    }
}
