using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class EffectByMechanic : EffectMechanic
    {

        protected override AgentItem GetAgentItem(EffectEvent effectEvt, AgentData agentData)
        {
            if (effectEvt.Src == ParserHelper._unknownAgent || effectEvt.Src.IsSpecy(ArcDPSEnums.NonIdentifiedAgent))
            {
                return agentData.GetNPCsByID(ArcDPSEnums.TrashID.Environment).FirstOrDefault();
            }
            return effectEvt.Src;
        }

        public EffectByMechanic(string effectGUID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, EffectChecker condition = null) : this(new string[] { effectGUID }, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public EffectByMechanic(string[] effectGUIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, EffectChecker condition = null) : base(effectGUIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }
    }
}
