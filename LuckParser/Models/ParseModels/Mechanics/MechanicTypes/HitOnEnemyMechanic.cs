using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    
    public class HitOnEnemyMechanic : Mechanic
    {

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, List<MechanicChecker> conditions, TriggerRule rule) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, conditions, rule)
        {
        }

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, List<MechanicChecker> conditions, TriggerRule rule) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, conditions, rule)
        {
        }

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicLog>> mechanicLogs, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            HashSet<ushort> playersIds = log.PlayerIDs;
            IEnumerable<AgentItem> agents = log.AgentData.GetAgentsByID((ushort)SkillId);
            foreach (AgentItem a in agents)
            {
                List<AbstractDamageEvent> combatitems = combatData.GetDamageTakenData(a);
                foreach (AbstractDamageEvent c in combatitems)
                {
                    if (c is NonDirectDamageEvent || !c.IsHit /*|| !Keep(c, log)*/ )
                    {
                        continue;
                    }
                    foreach (Player p in log.PlayerList)
                    {
                        if (c.From == p.AgentItem || c.MasterFrom == p.AgentItem )
                        {
                            mechanicLogs[this].Add(new MechanicLog(c.Time, this, p));
                        }
                    }
                }
            }
        }
    }
}
