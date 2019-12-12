using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{

    public class HitOnEnemyMechanic : SkillMechanic
    {

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, SkillChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public HitOnEnemyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<ushort, AbstractSingleActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            IEnumerable<AgentItem> agents = log.AgentData.GetNPCsByID((ushort)SkillId);
            foreach (AgentItem a in agents)
            {
                List<AbstractDamageEvent> combatitems = combatData.GetDamageTakenData(a);
                foreach (AbstractDamageEvent c in combatitems)
                {
                    if (c is DirectDamageEvent && c.HasHit && Keep(c, log))
                    {
                        foreach (Player p in log.PlayerList)
                        {
                            if (c.From == p.AgentItem || c.From.Master == p.AgentItem)
                            {
                                mechanicLogs[this].Add(new MechanicEvent(c.Time, this, p));
                            }
                        }
                    }

                }
            }
        }
    }
}
