using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerDstBuffRemoveMechanic : PlayerBuffRemoveMechanic<BuffRemoveAllEvent>
    {
        private bool _withMinions { get; set; }
        public PlayerDstBuffRemoveMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public PlayerDstBuffRemoveMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public PlayerDstBuffRemoveMechanic WithMinions(bool withMinions)
        {
            _withMinions = withMinions;
            return this;
        }

        private static bool OnlyMinionsChecker(BuffRemoveAllEvent brae, ParsedEvtcLog log)
        {
            return brae.To != brae.To.GetFinalMaster();
        }

        public PlayerDstBuffRemoveMechanic OnlyMinions(bool onlyMinions)
        {
            if (onlyMinions)
            {
                Checkers.Add(OnlyMinionsChecker);
            }
            else
            {
                Checkers.Remove(OnlyMinionsChecker);
            }
            return WithMinions(onlyMinions);
        }

        protected override AgentItem GetAgentItem(BuffRemoveAllEvent rae)
        {
            return _withMinions ? rae.To.GetFinalMaster() : rae.To;
        }
    }
}
