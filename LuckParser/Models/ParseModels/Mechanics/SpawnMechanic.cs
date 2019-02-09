using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
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

        public override void CheckMechanic(ParsedLog log, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            MechanicData mechData = log.MechanicData;
            CombatData combatData = log.CombatData;
            HashSet<ushort> playersIds = log.PlayerIDs;
            foreach (AgentItem a in log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.ID == SkillId))
            {
                if (!regroupedMobs.TryGetValue(a.ID, out DummyActor amp))
                {
                    amp = new DummyActor(a);
                    regroupedMobs.Add(a.ID, amp);
                }
                mechData[this].Add(new MechanicLog(log.FightData.ToFightSpace(a.FirstAware), this, amp));
            }
        }
    }
}
