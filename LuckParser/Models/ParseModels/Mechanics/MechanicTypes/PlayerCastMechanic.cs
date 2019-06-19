using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    
    public class PlayerCastMechanic : CastMechanic
    {

        public PlayerCastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, bool end, List<CastChecker> conditions, TriggerRule rule) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, end, conditions, rule)
        {
        }

        public PlayerCastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, bool end) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, end)
        {
        }

        public PlayerCastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, bool end) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, end)
        {
        }

        public PlayerCastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, bool end, List<CastChecker> conditions, TriggerRule rule) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, end, conditions, rule)
        {
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            foreach (Player p in log.PlayerList)
            {
                foreach (AbstractCastEvent c in log.CombatData.GetCastDataById(SkillId))
                {
                    if (c.Caster == p.AgentItem && Keep(c, log))
                    {
                        mechanicLogs[this].Add(new MechanicEvent(EndCast ? c.Time + c.ActualDuration : c.Time, this, p));

                    }
                }
            }
        }
    }
}
