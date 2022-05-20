using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerStatusMechanic : Mechanic
    {

        public PlayerStatusMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(mechanicID, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public PlayerStatusMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            ShowOnTable = false;
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            foreach (Player p in log.PlayerList)
            {
                var cList = new List<long>();
                foreach (long mechanicID in MechanicIDs)
                {
                    switch (mechanicID)
                    {
                        case SkillIDs.Death:
                            cList = combatData.GetDeadEvents(p.AgentItem).Select(x => x.Time).ToList();
                            break;
                        case SkillIDs.Despawn:
                            cList = combatData.GetDespawnEvents(p.AgentItem).Select(x => x.Time).ToList();
                            break;
                        case SkillIDs.Respawn:
                            cList = combatData.GetSpawnEvents(p.AgentItem).Select(x => x.Time).ToList();
                            break;
                        case SkillIDs.Alive:
                            cList = combatData.GetAliveEvents(p.AgentItem).Select(x => x.Time).ToList();
                            break;
                        case SkillIDs.Down:
                            cList = combatData.GetDownEvents(p.AgentItem).Select(x => x.Time).ToList();
                            var downByVaporForm = combatData.GetBuffRemoveAllData(SkillIDs.VaporForm).Where(x => x.To == p.AgentItem).Select(x => x.Time).ToList();
                            foreach (long time in downByVaporForm)
                            {
                                cList.RemoveAll(x => Math.Abs(x - time) < 20);
                            }
                            break;
                        case SkillIDs.Resurrect:
                            cList = log.CombatData.GetAnimatedCastData(p.AgentItem).Where(x => x.SkillId == SkillIDs.Resurrect).Select(x => x.Time).ToList();
                            break;
                    }
                }
                foreach (long time in cList)
                {
                    mechanicLogs[this].Add(new MechanicEvent(time, this, p));
                }
            }
        }
    }
}
