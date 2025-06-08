using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class UnknownFightLogic : FightLogic
{
    public UnknownFightLogic(int triggerID) : base(triggerID)
    {
        Extension = "boss";
        Icon = EncounterIconGeneric;
        EncounterCategoryInformation.Category = FightCategory.UnknownEncounter;
        EncounterCategoryInformation.SubCategory = SubFightCategory.UnknownEncounter;
    }

    internal override void UpdatePlayersSpecAndGroup(IReadOnlyList<Player> players, CombatData combatData, FightData fightData)
    {
        // We don't know how an unknown fight could operate.
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            AgentItem? target = agentData.GetNPCsByID(GenericTriggerID).FirstOrDefault() ?? agentData.GetGadgetsByID(GenericTriggerID).FirstOrDefault();
            return GetFirstDamageEventTime(fightData, agentData, combatData, target);
        }
        return GetGenericFightOffset(fightData);
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
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
        FinalizeComputeFightTargets();
    }

    protected override IReadOnlyList<TargetID> GetUniqueNPCIDs()
    {
        throw new InvalidOperationException("UniqueNPCIDs not valid for Unknown");
    }

    protected override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        throw new InvalidOperationException("GetTargetIDs not valid for Unknown");
    }

    protected override IReadOnlyList<TargetID> GetFriendlyNPCIDs()
    {
        throw new InvalidOperationException("GetFriendlyNPCIDs not valid for Unknown");
    }

    protected override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        throw new InvalidOperationException("GetTrashMobsIDs not valid for Unknown");
    }

    internal override FightData.InstancePrivacyMode GetInstancePrivacyMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return FightData.InstancePrivacyMode.Unknown;
    }
}
