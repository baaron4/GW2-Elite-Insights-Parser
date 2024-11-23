
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class UraTheSteamshrieker : MountBalrior
{
    public UraTheSteamshrieker(int triggerID) : base(triggerID)
    {
        Extension = "ura";
        Icon = EncounterIconUra;
        EncounterCategoryInformation.InSubCategoryOrder = 2;
        EncounterID |= 0x000003;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayUratheSteamshrieker,
                        (1833, 1824),
                        (2900, 6600, 8355, 12028));
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Ura, the Steamshrieker";
    }
    protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
    {
        return
        [
            ArcDPSEnums.TrashID.Fumaroller,
            ArcDPSEnums.TrashID.SulfuricGeyser,
            ArcDPSEnums.TrashID.TitanspawnGeyser,
            ArcDPSEnums.TrashID.ToxicGeyser,
            ArcDPSEnums.TrashID.UraGadget,
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var sulfuricEffectGUID = combatData
            .Where(x => x.IsStateChange == ArcDPSEnums.StateChange.EffectIDToGUID && EffectGUIDs.UraSulfuricGeyserSpawn.Equals(x.SrcAgent, x.DstAgent))
            .Select(x => new EffectGUIDEvent(x, evtcVersion))
            .FirstOrDefault();
        bool refresh = false;
        if (sulfuricEffectGUID != null)
        {
            var sulfuricAgents = combatData
                .Where(x => x.IsEffect && x.SkillID == sulfuricEffectGUID.ContentID)
                .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
                .Where(x => x.Type == AgentItem.AgentType.Gadget)
                .Distinct();
            refresh |= sulfuricAgents.Any();
            foreach (var sulfuricAgent in sulfuricAgents)
            {
                sulfuricAgent.OverrideID(ArcDPSEnums.TrashID.SulfuricGeyser);
                sulfuricAgent.OverrideType(AgentItem.AgentType.NPC);
            }
        }
        var toxicEffectGUID = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.EffectIDToGUID && EffectGUIDs.UraToxicGeyserSpawn.Equals(x.SrcAgent, x.DstAgent)).Select(x => new EffectGUIDEvent(x, evtcVersion)).FirstOrDefault();
        if (toxicEffectGUID != null)
        {
            var toxicAgents = combatData
                .Where(x => x.IsEffect && x.SkillID == toxicEffectGUID.ContentID)
                .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
                .Where(x => x.Type == AgentItem.AgentType.Gadget)
                .Distinct();
            refresh |= toxicAgents.Any();
            foreach (var toxicAgent in toxicAgents)
            {
                toxicAgent.OverrideID(ArcDPSEnums.TrashID.ToxicGeyser);
                toxicAgent.OverrideType(AgentItem.AgentType.NPC);
            }
        }
        // At this point, toxic and sulfur ones are properly flaggued 
        // This seems to miss some titan geysers, investigate why some are different (no proper max health)
        var titanGeysers = combatData
            .Where(x => x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate && MaxHealthUpdateEvent.GetMaxHealth(x) == 448200)
            .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
            .Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth > 100)
            .Distinct();
        refresh |= titanGeysers.Any();
        foreach (var titanAgent in titanGeysers)
        {
            titanAgent.OverrideID(ArcDPSEnums.TrashID.TitanspawnGeyser);
            titanAgent.OverrideType(AgentItem.AgentType.NPC);
        }
        var uras = combatData
            .Where(x => x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate && MaxHealthUpdateEvent.GetMaxHealth(x) == 14940)
            .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
            .Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxHeight == 300 && x.HitboxWidth == 2 && x.FirstAware > 0)
            .Distinct();
        refresh |= uras.Any();
        foreach (var ura in uras)
        {
            ura.OverrideID(ArcDPSEnums.TrashID.UraGadget);
            ura.OverrideType(AgentItem.AgentType.NPC);
        }
        if (refresh)
        {
            agentData.Refresh();
        }
        ComputeFightTargets(agentData, combatData, extensions);
    }

}
