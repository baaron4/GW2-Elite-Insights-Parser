using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerSrcPlayerDstBuffApplyMechanic : PlayerSrcBuffApplyMechanic
    {

        public PlayerSrcPlayerDstBuffApplyMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffApplyChecker condition = null) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public PlayerSrcPlayerDstBuffApplyMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BuffApplyChecker condition = null) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        protected override void AddMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, BuffApplyEvent ba, Player p)
        {
            mechanicLogs[this].Add(new MechanicEvent(ba.Time, this, p));
            mechanicLogs[this].Add(new MechanicEvent(ba.Time, this, log.PlayerList.FirstOrDefault(x => x.AgentItem == ba.CreditedBy)));
        }
    }
}
