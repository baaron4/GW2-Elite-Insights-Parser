using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{

    public class PlayerOnPlayerMechanic : BuffApplyMechanic
    {

        public PlayerOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, BuffApplyChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public PlayerOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffApplyChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public PlayerOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public PlayerOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<ushort, AbstractActor> regroupedMobs)
        {
            CombatData combatData = log.CombatData;
            foreach (Player p in log.PlayerList)
            {
                foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(SkillId))
                {
                    if (c is BuffApplyEvent ba && p.AgentItem == ba.To && Keep(ba, log))
                    {
                        mechanicLogs[this].Add(new MechanicEvent(ba.Time, this, p));
                        mechanicLogs[this].Add(new MechanicEvent(ba.Time, this, log.PlayerList.FirstOrDefault(x => x.AgentItem == ba.By)));
                    }

                }
            }
        }
    }
}
