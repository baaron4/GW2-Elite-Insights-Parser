using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class EnemyCastEndMechanic : EnemyCastStartMechanic
    {
        protected override long GetTime(AbstractCastEvent evt)
        {
            return evt.EndTime;
        }

        public EnemyCastEndMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, CastChecker condition) : this(mechanicID, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public EnemyCastEndMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CastChecker condition) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public EnemyCastEndMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(mechanicID, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public EnemyCastEndMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
    }
}
