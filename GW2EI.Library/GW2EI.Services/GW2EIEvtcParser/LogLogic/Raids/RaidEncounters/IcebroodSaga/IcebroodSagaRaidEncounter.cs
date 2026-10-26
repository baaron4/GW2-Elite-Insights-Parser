using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class IcebroodSagaRaidEncounter : RaidEncounterLogic
{
    public IcebroodSagaRaidEncounter(int triggerID) : base(triggerID)
    {
        LogID |= LogIDs.RaidEncounterMasks.IBSMask;
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        if (IsInstance)
        {
            successHandler.SetSuccess(true, GetFinalMapChangeTime(logData, combatData));
            return;
        }
        var sraidEncounterRewardIDs = new HashSet<ulong>
        {
            RewardIDs.ShiverpeaksPassChests,
            RewardIDs.KodansOldAndCurrentChest,
            RewardIDs.KodansCurrentChest1,
            RewardIDs.KodansCurrentRepeatableChest,
            RewardIDs.KodansCurrentChest2,
            RewardIDs.FraenirRepeatableChest,
            RewardIDs.WhisperRepeatableChest,
            RewardIDs.BoneskinnerRepeatableChest,
        };
        IReadOnlyList<RewardEvent> rewards = combatData.GetRewardEvents();
        RewardEvent? reward = rewards.FirstOrDefault(x => sraidEncounterRewardIDs.Contains(x.RewardID) && x.Time > logData.LogStart);
        if (reward != null)
        {
            successHandler.SetSuccess(true, reward.Time);
        }
        else
        {
            NoBouncyChestGenericCheckSucess(combatData, agentData, logData, playerAgents, successHandler);
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);

        long[] skills = [
            SpiritNovaTier1, SpiritNovaTier2, SpiritNovaTier3, SpiritNovaTier4,
            NightTerrorTier1, NightTerrorTier2, NightTerrorTier3, NightTerrorTier4,
            ShatteredPsycheTier1, ShatteredPsycheTier2, ShatteredPsycheTier3, ShatteredPsycheTier4,
        ];

        var casts = p.GetAnimatedCastEvents(log).Where(cast => skills.Any(x => x == cast.SkillID));

        foreach (var cast in casts)
        {
            if (p.TryGetCurrentFacingDirection(log, cast.Time, out var facing, 500))
            {
                var agentConnector = new AgentConnector(p);
                var angleConnector = new AngleConnector(facing.Value);
                (long, long) lifespan = (cast.ExpectedEndTime - 50, cast.ExpectedEndTime + 150); // Brief indicator
                Color color = Colors.Black;

                switch (cast.SkillID)
                {
                    case SpiritNovaTier1:
                    case SpiritNovaTier2:
                    case SpiritNovaTier3:
                    case SpiritNovaTier4:
                        color = Colors.Red;
                        break;
                    case NightTerrorTier1:
                    case NightTerrorTier2:
                    case NightTerrorTier3:
                    case NightTerrorTier4:
                        color = Colors.Green;
                        break;
                    case ShatteredPsycheTier1:
                    case ShatteredPsycheTier2:
                    case ShatteredPsycheTier3:
                    case ShatteredPsycheTier4:
                        color = Colors.Blue;
                        break;
                }

                // Tooltip says 1200 range, width is assumed.
                replay.Decorations.Add(new RectangleDecoration(1200, 100, lifespan, color, 0.1, agentConnector.WithOffset(new(600, 0, 0), true)).UsingRotationConnector(angleConnector));
            }
        }
    }
}
