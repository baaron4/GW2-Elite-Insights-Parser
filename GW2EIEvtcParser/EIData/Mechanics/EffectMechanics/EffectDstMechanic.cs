using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class EffectDstMechanic : EffectMechanic
    {

        protected override AgentItem GetAgentItem(EffectEvent effectEvt, AgentData agentData)
        {
            if (!effectEvt.IsAroundDst || effectEvt.Dst == ParserHelper._unknownAgent || effectEvt.Dst.IsSpecy(ArcDPSEnums.NonIdentifiedAgent))
            {
                return agentData.GetNPCsByID(ArcDPSEnums.TrashID.Environment).FirstOrDefault();
            }
            return effectEvt.Dst;
        }

        public EffectDstMechanic(string effectGUID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, EffectChecker condition = null) : this(new string[] { effectGUID }, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public EffectDstMechanic(string[] effectGUIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, EffectChecker condition = null) : base(effectGUIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
    }
}
