using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerCastStartMechanic : CastMechanic
    {

        public PlayerCastStartMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, CastChecker condition) : this(mechanicID, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public PlayerCastStartMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public PlayerCastStartMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(mechanicID, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public PlayerCastStartMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CastChecker condition) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (Player p in log.PlayerList)
            {
                foreach (long mechanicID in MechanicIDs)
                {
                    foreach (AbstractCastEvent c in log.CombatData.GetAnimatedCastData(mechanicID))
                    {
                        if (c.Caster == p.AgentItem && Keep(c, log))
                        {
                            mechanicLogs[this].Add(new MechanicEvent(GetTime(c), this, p));

                        }
                    }
                }
            }
        }
    }
}
