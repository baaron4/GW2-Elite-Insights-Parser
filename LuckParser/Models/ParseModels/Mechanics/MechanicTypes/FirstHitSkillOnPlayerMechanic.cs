using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    
    public class FirstHitSkillOnPlayerMechanic : DamageMechanic
    {

        private readonly Dictionary<AgentItem, AbstractDamageEvent> _firstHits = new Dictionary<AgentItem, AbstractDamageEvent>();

        public FirstHitSkillOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, List<DamageChecker> conditions, TriggerRule rule) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, conditions, rule)
        {
        }

        public FirstHitSkillOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, List<DamageChecker> conditions, TriggerRule rule) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, conditions, rule)
        {
        }

        public FirstHitSkillOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public FirstHitSkillOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        private AbstractDamageEvent GetFirstHit(AgentItem src, ParsedLog log)
        {
            if (!_firstHits.TryGetValue(src, out AbstractDamageEvent evt))
            {
                AbstractDamageEvent res = log.CombatData.GetDamageData(src).Where(x => x.SkillId == SkillId).FirstOrDefault();
                _firstHits[src] = res;
                return res;
            }
            return evt;
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            AgentData agentData = log.AgentData;
            foreach (Player p in log.PlayerList)
            {
                List<AbstractDamageEvent> combatitems = combatData.GetDamageTakenData(p.AgentItem);
                foreach (AbstractDamageEvent c in combatitems)
                {
                    if ( !(c.SkillId == SkillId) || !c.HasHit || !Keep(c, log))
                    {
                        continue;
                    }

                    if (GetFirstHit(c.From, log) != c) continue;

                    mechanicLogs[this].Add(new MechanicEvent(c.Time, this, p));
                }
            }
        }
    }
}
