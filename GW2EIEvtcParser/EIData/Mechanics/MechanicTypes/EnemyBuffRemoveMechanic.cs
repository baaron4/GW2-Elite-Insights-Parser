using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class EnemyBuffRemoveMechanic : BuffRemoveMechanic
    {

        public EnemyBuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, BuffRemoveChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public EnemyBuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffRemoveChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
            IsEnemyMechanic = true;
        }

        public EnemyBuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public EnemyBuffRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (AbstractBuffEvent c in log.CombatData.GetBuffData(SkillId))
            {
                AbstractSingleActor amp = null;
                if (c is BuffRemoveAllEvent rea && Keep(rea, log))
                {
                    if (!regroupedMobs.TryGetValue(rea.To.ID, out amp))
                    {
                        amp = log.FindActor(rea.To, true);
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
