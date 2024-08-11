using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class GenericCombatEventListMechanic<T> : CombatEventListMechanic<T> where T : AbstractTimeCombatEvent
    {

        public GenericCombatEventListMechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, bool isEnemyMechanic, CombatEventsGetter getter) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, getter)
        {
            IsEnemyMechanic = isEnemyMechanic;
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            if (!IsEnemyMechanic)
            {
                foreach (Player p in log.PlayerList)
                {
                    foreach (T c in GetEvents(log, p.AgentItem))
                    {
                        if (Keep(c, log))
                        {
                            InsertMechanic(log, mechanicLogs, c.Time, p);
                        }
                    }
                }
            }
            else
            {
                foreach (AbstractSingleActor actor in log.FightData.Logic.Hostiles)
                {
                    foreach (T c in GetEvents(log, actor.AgentItem))
                    {
                        if (Keep(c, log))
                        {
                            AbstractSingleActor actorToUse = MechanicHelper.FindEnemyActor(log, actor.AgentItem, regroupedMobs);
                            // no need to null check, we are already iterating over an existing actor list
                            InsertMechanic(log, mechanicLogs, c.Time, actorToUse);
                        }
                    }
                }
            }
        }
    }
}
