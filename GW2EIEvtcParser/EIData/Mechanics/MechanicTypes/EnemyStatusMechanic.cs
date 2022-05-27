using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class EnemyStatusMechanic<T> : StatusMechanic<T> where T : AbstractStatusEvent
    {

        public EnemyStatusMechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, StatusGetter getter, StatusChecker condition = null) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, getter, condition)
        {
            IsEnemyMechanic = true;
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            var list = new List<AbstractSingleActor>(log.FightData.Logic.Targets);
            list.AddRange(log.FightData.Logic.TrashMobs);
            foreach (AbstractSingleActor actor in list)
            {
                foreach (T c in GetEvents(log, actor.AgentItem))
                {
                    if (Keep(c, log))
                    {
                        AbstractSingleActor actorToUse = EnemyMechanicHelper.FindActor(log, actor.AgentItem, regroupedMobs);
                        // no need to null check, we are already iterating over an existing actor list
                        mechanicLogs[this].Add(new MechanicEvent(c.Time, this, actorToUse));
                    }
                }
            }
        }
    }
}
