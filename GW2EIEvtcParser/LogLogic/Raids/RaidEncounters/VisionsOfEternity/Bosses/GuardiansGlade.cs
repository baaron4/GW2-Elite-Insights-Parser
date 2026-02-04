using System.Diagnostics;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class GuardiansGlade : VisionsOfEternityRaidEncounter
{
    internal readonly MechanicGroup Mechanics = new
    ([
        
    ]);
    
    public GuardiansGlade(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Icon = EncounterIconGuardiansGlade;
        Extension = "guardglade";
        GenericFallBackMethod = FallBackMethod.None;
        LogCategoryInformation.InSubCategoryOrder = 0;
        LogID |= 0x000001;
    }
    
    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
            (800, 800),
            (0, 0, 0, 0));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayGuardiansGlade, crMap);
        return crMap;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Guardian's Glade";
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.KelaSeneschalOfWaves,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.DownedEliteCrocodilianRazortooth,
            TargetID.EliteCrocodilianRazortooth,
            TargetID.VeteranCrocodilianRazortooth,
            TargetID.ExecutorOfWaves,
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        return base.GetPhases(log, requirePhases);
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.Mode.Normal;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        base.CheckSuccess(combatData, agentData, logData, playerAgents, successHandler);
        if (!successHandler.Success)
        {
            var kela = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.KelaSeneschalOfWaves)) ?? throw new MissingKeyActorsException("Kela not found");
            var determined762Applies = combatData.GetBuffApplyDataByIDByDst(Determined762, kela.AgentItem);
            if (determined762Applies.Count == 1)
            {
                successHandler.SetSuccess(true, determined762Applies[0].Time);
            }
        }
    }
}
