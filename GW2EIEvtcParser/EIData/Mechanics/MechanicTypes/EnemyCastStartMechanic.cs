using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class EnemyCastStartMechanic : CastMechanic
    {

        public EnemyCastStartMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, CastChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public EnemyCastStartMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CastChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
            IsEnemyMechanic = true;
        }

        public EnemyCastStartMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public EnemyCastStartMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            IsEnemyMechanic = true;
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (AbstractCastEvent c in log.CombatData.GetAnimatedCastData(SkillId))
            {
                AbstractSingleActor amp = null;
                if (Keep(c, log))
                {
                    if (!regroupedMobs.TryGetValue(c.Caster.ID, out amp))
                    {
                        amp = log.FindActor(c.Caster, true);
                        if (amp == null)
                        {
                            continue;
                        }
                        regroupedMobs.Add(amp.ID, amp);
                    }
                }
                if (amp != null)
                {
                    mechanicLogs[this].Add(new MechanicEvent(GetTime(c), this, amp));
                }
            }
        }
    }
}
