using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class HitOnEnemyMechanic : SkillMechanic
    {

        public HitOnEnemyMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, SkillChecker condition) : this(mechanicID, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public HitOnEnemyMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public HitOnEnemyMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(mechanicID, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public HitOnEnemyMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            foreach (long mechanicID in MechanicIDs)
            {
                IEnumerable<AgentItem> agents = log.AgentData.GetNPCsByID((int)mechanicID);
                foreach (AgentItem a in agents)
                {
                    IReadOnlyList<AbstractHealthDamageEvent> combatitems = combatData.GetDamageTakenData(a);
                    foreach (AbstractHealthDamageEvent c in combatitems)
                    {
                        if (c is DirectHealthDamageEvent && c.HasHit && Keep(c, log))
                        {
                            foreach (AbstractSingleActor actor in log.Friendlies)
                            {
                                if (c.From.GetFinalMaster() == actor.AgentItem)
                                {
                                    mechanicLogs[this].Add(new MechanicEvent(c.Time, this, actor));
                                }
                            }
                        }

                    }
                }
            }
            
        }
    }
}
