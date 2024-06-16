using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class DstEffectMechanic : EffectMechanic
    {

        protected override AgentItem GetAgentItem(EffectEvent effectEvt, AgentData agentData)
        {
            if (!effectEvt.IsAroundDst || effectEvt.Dst.IsUnamedSpecies())
            {
                return agentData.GetNPCsByID(ArcDPSEnums.TrashID.Environment).FirstOrDefault();
            }
            return effectEvt.Dst;
        }

        public DstEffectMechanic(string effectGUID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this(new string[] { effectGUID }, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public DstEffectMechanic(string[] effectGUIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(effectGUIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
    }
}
