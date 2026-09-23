using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.MechanicIDs;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class RaidEncounterLogic : RaidLogic
{

    protected RaidEncounterLogic(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([
            new PlayerDstBuffApplyMechanic(ExposedPlayer, Mech_PlayerExposed, new (Symbols.TriangleLeft, Colors.Purple, 10), new ("Exposed", "Exposed Applied (Increased incoming damage)", "Exposed Applied"), Sev0),
            new PlayerDstBuffApplyMechanic(Debilitated, Mech_PlayerDebilitated, new (Symbols.TriangleDown, Colors.Purple, 10), new ("Debilitated", "Debilitated Applied (Reduced outgoing damage)", "Debilitated Applied"), Sev0),
            new PlayerDstBuffApplyMechanic(Infirmity, Mech_PlayerInfirmity, new (Symbols.TriangleUp, Colors.Purple, 10), new ("Infirmity", "Infirmity Applied (Reduced incoming healing)", "Infirmity Applied"), Sev0),
        ])
        );
        LogCategoryInformation.Category = LogCategory.RaidEncounter;
        LogID |= LogIDs.LogMasks.RaidEncounterMask;
    }

    protected void CheckPostEODRewardSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        if (IsInstance)
        {
            successHandler.SetSuccess(true, GetFinalMapChangeTime(logData, combatData));
            return;
        }
        IReadOnlyList<RewardEvent> rewards = combatData.GetRewardEvents();
        RewardEvent? reward = rewards.FirstOrDefault(x => x.RewardType == RewardTypes.PostEoDRaidEncounterReward && x.Time > logData.LogStart);
        if (reward != null)
        {
            successHandler.SetSuccess(true, reward.Time);
        }
        else
        {
            NoBouncyChestGenericCheckSucess(combatData, agentData, logData, playerAgents, successHandler);
        }
    }
}
