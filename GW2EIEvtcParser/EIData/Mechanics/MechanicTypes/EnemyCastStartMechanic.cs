using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class EnemyCastStartMechanic : CastMechanic
    {

        public EnemyCastStartMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, CastChecker condition = null) : this(mechanicID, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public EnemyCastStartMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CastChecker condition = null) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
            IsEnemyMechanic = true;
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (long mechanicID in MechanicIDs)
            {
                foreach (AbstractCastEvent c in log.CombatData.GetAnimatedCastData(mechanicID))
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
}
