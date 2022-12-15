using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class HitOnEnemySpeciesMechanic : SkillMechanic
    {

        public HitOnEnemySpeciesMechanic(long speciesID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(speciesID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public HitOnEnemySpeciesMechanic(long[] speciesIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(speciesIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
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
