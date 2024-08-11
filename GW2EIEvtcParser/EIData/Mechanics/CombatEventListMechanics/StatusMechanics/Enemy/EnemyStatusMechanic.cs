using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class EnemyStatusMechanic<T> : StatusMechanic<T> where T : AbstractStatusEvent
    {

        public EnemyStatusMechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CombatEventsGetter getter) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, getter)
        {
            IsEnemyMechanic = true;
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
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
