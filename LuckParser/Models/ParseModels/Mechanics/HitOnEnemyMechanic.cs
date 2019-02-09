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

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, CheckTriggerCondition condition = null) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CheckTriggerCondition condition = null) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            MechanicData mechData = log.MechanicData;
            CombatData combatData = log.CombatData;
            HashSet<ushort> playersIds = log.PlayerIDs;
            IEnumerable<AgentItem> agents = log.AgentData.GetAgentsByID((ushort)SkillId);
            foreach (AgentItem a in agents)
            {
                List<CombatItem> combatitems = combatData.GetDamageTakenData(a.InstID, a.FirstAware, a.LastAware);
                foreach (CombatItem c in combatitems)
                {
                    if (c.IsBuff > 0 || !Keep(c))
                    {
                        continue;
                    }
                    foreach (Player p in log.PlayerList)
                    {
                        if (c.SrcInstid == p.InstID || c.SrcMasterInstid == p.InstID)
                        {
                            mechData[this].Add(new MechanicLog(log.FightData.ToFightSpace(c.Time), this, p));
                        }
                    }
                }
            }
        }
    }
}
