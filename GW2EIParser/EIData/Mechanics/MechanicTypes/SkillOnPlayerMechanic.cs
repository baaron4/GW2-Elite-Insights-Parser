using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{

    public class SkillOnPlayerMechanic : SkillMechanic
    {

        public SkillOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, SkillChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public SkillOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public SkillOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public SkillOnPlayerMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public override void CheckMechanic(ParsedLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<ushort, DummyActor> regroupedMobs)
        {
            foreach (Player p in log.PlayerList)
            {
                List<AbstractDamageEvent> combatitems = p.GetDamageTakenLogs(null, log, 0, log.FightData.FightDuration);
                foreach (AbstractDamageEvent c in combatitems)
                {
                    if (c.SkillId == SkillId && Keep(c, log))
                    {
                        mechanicLogs[this].Add(new MechanicEvent(c.Time, this, p));
                    }
                }
            }
        }
    }
}
