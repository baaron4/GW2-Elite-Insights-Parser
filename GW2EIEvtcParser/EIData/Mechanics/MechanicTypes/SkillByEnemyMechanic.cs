using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class SkillByEnemyMechanic : SkillMechanic
    {

        public SkillByEnemyMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, SkillChecker condition) : this(mechanicID, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public SkillByEnemyMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public SkillByEnemyMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(mechanicID, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public SkillByEnemyMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (long mechanicID in MechanicIDs)
            {
                foreach (AbstractHealthDamageEvent c in log.CombatData.GetDamageData(mechanicID))
                {
                    AbstractSingleActor amp = null;
                    if (Keep(c, log))
                    {
                        if (!regroupedMobs.TryGetValue(c.From.ID, out amp))
                        {
                            amp = log.FindActor(c.From, true);
                            if (amp == null)
                            {
                                continue;
                            }
                            regroupedMobs.Add(amp.ID, amp);
                        }
                    }
                    if (amp != null)
                    {
                        mechanicLogs[this].Add(new MechanicEvent(c.Time, this, amp));
                    }
                }
            }
        }
    }
}
