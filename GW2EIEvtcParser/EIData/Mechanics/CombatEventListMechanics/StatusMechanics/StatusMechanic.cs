using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class StatusMechanic<T> : CombatEventListMechanic<T> where T : AbstractStatusEvent
    {

        public StatusMechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CombatEventsGetter getter) : base(inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, getter)
        {
        }
    }
}
