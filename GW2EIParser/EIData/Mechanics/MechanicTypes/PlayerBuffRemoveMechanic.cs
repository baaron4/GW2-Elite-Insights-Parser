using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{

    public class PlayerBuffRemoveMechanic : BuffRemoveMechanic
    {

        public PlayerBuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, BuffRemoveChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public PlayerBuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffRemoveChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public PlayerBuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public PlayerBuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (Player p in log.PlayerList)
            {
                foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(SkillId))
                {
                    if (c is BuffRemoveManualEvent rme && p.AgentItem == rme.To && Keep(rme, log))
                    {
                        mechanicLogs[this].Add(new MechanicEvent(rme.Time, this, p));
                    }
                }
            }
        }
    }
}
