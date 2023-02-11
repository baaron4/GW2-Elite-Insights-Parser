using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class BuffRemoveMechanic : IDBasedMechanic
    {
        public delegate bool BuffRemoveChecker(BuffRemoveAllEvent rme, ParsedEvtcLog log);

        private readonly BuffRemoveChecker _triggerCondition = null;

        protected bool Keep(BuffRemoveAllEvent c, ParsedEvtcLog log)
        {
            return _triggerCondition != null ? _triggerCondition(c, log) : true;
        }

        public BuffRemoveMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffRemoveChecker condition = null) : this(new long[] { mechanicID }, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public BuffRemoveMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffRemoveChecker condition = null) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
        }
    }
}
