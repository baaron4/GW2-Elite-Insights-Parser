using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class SrcEffectMechanic : EffectMechanic
    {

        protected override AgentItem GetAgentItem(EffectEvent effectEvt, AgentData agentData)
        {
            if (effectEvt.Src.IsUnamedSpecies())
            {
                return agentData.GetNPCsByID(ArcDPSEnums.TrashID.Environment).FirstOrDefault();
            }
            return effectEvt.Src;
        }

        public SrcEffectMechanic(string effectGUID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : this(new string[] { effectGUID }, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public SrcEffectMechanic(string[] effectGUIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(effectGUIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
    }
}
