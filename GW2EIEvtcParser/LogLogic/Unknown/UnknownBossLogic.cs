using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class UnknownBossLogic : UnknownEncounterLogic
{
    public UnknownBossLogic(int triggerID) : base(triggerID)
    {
        Extension = "boss";
        Icon = EncounterIconGeneric;
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            AgentItem? target = agentData.GetNPCsByID(GenericTriggerID).FirstOrDefault() ?? agentData.GetGadgetsByID(GenericTriggerID).FirstOrDefault();
            return GetFirstDamageEventTime(logData, agentData, combatData, target);
        }
        return GetGenericLogOffset(logData);
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        SetSuccessByDeath(Targets.Where(x => x.IsSpecies(GenericTriggerID)), combatData, logData, playerAgents, successHandler, true);
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        int id = GenericTriggerID;
        AgentItem? agentItem = agentData.GetNPCsByID(id).FirstOrDefault();
        // Trigger ID is not NPC
        if (agentItem == null)
        {
            agentItem = agentData.GetGadgetsByID(id).FirstOrDefault();
            if (agentItem != null)
            {
                _targets.Add(new NPC(agentItem));
            }
        }
        else
        {
            _targets.Add(new NPC(agentItem));
        }
        //
        FinalizeComputeLogTargets();
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        throw new InvalidOperationException("GetTargetIDs not valid for Unknown");
    }

    internal override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        throw new InvalidOperationException("GetFriendlyNPCIDs not valid for Unknown");
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        throw new InvalidOperationException("GetTrashMobsIDs not valid for Unknown");
    }

    internal override LogData.InstancePrivacyMode GetInstancePrivacyMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.InstancePrivacyMode.Unknown;
    }
}
