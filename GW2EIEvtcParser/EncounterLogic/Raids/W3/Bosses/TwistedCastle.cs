using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class TwistedCastle : StrongholdOfTheFaithful
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new PlayerDstBuffApplyMechanic(SpatialDistortion, new MechanicPlotlySetting(Symbols.Circle,Colors.Magenta), "Statue TP", "Teleported by Statue", "Statue Teleport", 500),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(StillWaters, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Magenta), "Still Waters (Immunity)", "Used a fountain for immunity", "Still Waters (Immunity)", 0)
                    .UsingChecker((evt, log) => log.CombatData.GetBuffDataByIDByDst(SoothingWaters, evt.To).Any(x => x is BuffApplyEvent ba && Math.Abs(ba.Time - evt.Time) < 500)),
                new PlayerDstBuffApplyMechanic(StillWaters, new MechanicPlotlySetting(Symbols.StarDiamond,Colors.Magenta), "Still Waters (Removal)", "Used a fountain for stack removal", "Still Waters (Removal)", 0)
                    .UsingChecker((evt, log) => !log.CombatData.GetBuffDataByIDByDst(SoothingWaters, evt.To).Any(x => x is BuffApplyEvent ba && Math.Abs(ba.Time - evt.Time) < 500)),
            ]),
            new PlayerDstBuffApplyMechanic(Madness, new MechanicPlotlySetting(Symbols.Square,Colors.LightPurple), "Madness", "Stacking debuff", "Madness", 0),
            new PlayerDstBuffApplyMechanic(ChaoticHaze, new MechanicPlotlySetting(Symbols.Hexagon,Colors.Red), "Chaotic Haze", "Damaging Debuff from bombardement", "Chaotic Haze", 500),
        ]);
    public TwistedCastle(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "twstcstl";
        GenericFallBackMethod = FallBackMethod.None;
        Targetless = true;
        Icon = EncounterIconTwistedCastle;
        EncounterCategoryInformation.InSubCategoryOrder = 2;
        EncounterID |= 0x000003;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayTwistedCastle,
                        (774, 1000),
                        (-8058, -4321, 819, 7143)/*,
                        (-12288, -27648, 12288, 27648),
                        (1920, 12160, 2944, 14464)*/);
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        RewardEvent? reward = GetOldRaidReward2Event(combatData, fightData.FightStart, fightData.LogEnd); ;
        if (reward != null)
        {
            fightData.SetSuccess(true, reward.Time);
        }
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            IReadOnlyList<AgentItem> statues = agentData.GetNPCsByID(TargetID.HauntingStatue);
            long start = long.MaxValue;
            foreach (AgentItem statue in statues)
            {
                CombatItem? enterCombat = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.EnterCombat && x.SrcMatchesAgent(statue));
                if (enterCombat != null)
                {
                    start = Math.Min(start, enterCombat.Time);
                }
            }
            return start < long.MaxValue ? start : GetGenericFightOffset(fightData);
        }
        return GetGenericFightOffset(fightData);
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        // To investigate
        return FightData.EncounterStartStatus.Normal;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Twisted Castle", Spec.NPC, TargetID.DummyTarget, true);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
           TargetID.HauntingStatue,
           //ParseEnum.TrashIDS.CastleFountain
        ];
    }

    internal override void ComputeNPCCombatReplayActors(NPC npc, ParsedEvtcLog log, CombatReplay replay)
    {
        switch (npc.ID)
        {
            case (int)TargetID.HauntingStatue:
                var lifespan = ((int)replay.TimeOffsets.start, (int)replay.TimeOffsets.end);
                if (replay.Rotations.Count != 0)
                {
                    replay.Decorations.Add(new ActorOrientationDecoration(lifespan, npc.AgentItem));
                }
                break;
            //case (ushort)ParseEnum.TrashIDS.CastleFountain:
            //    break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);
        // Madness - 0 to 29 nothing, 30 to 59 Silver, 60 to 89 Gold, 90 to 99 Red
        IEnumerable<Segment> madnesses = player.GetBuffStatus(log, Madness).Where(x => x.Value > 0);
        foreach (Segment segment in madnesses)
        {
            if (segment.Value >= 90)
            {
                replay.Decorations.AddOverheadIcon(segment, player, ParserIcons.MadnessRedOverhead);
            }
            else if (segment.Value >= 60)
            {
                replay.Decorations.AddOverheadIcon(segment, player, ParserIcons.MadnessGoldOverhead);
            }
            else if (segment.Value >= 30)
            {
                replay.Decorations.AddOverheadIcon(segment, player, ParserIcons.MadnessSilverOverhead);
            }
        }
    }

    internal override int GetTriggerID()
    {
        return (int)TargetID.HauntingStatue;
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Twisted Castle";
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        if (log.FightData.Success)
        {
            if (log.CombatData.GetBuffData(AchievementEligibilityMildlyInsane).Any())
            {
                InstanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityMildlyInsane));
            }
            else if (CustomCheckMildlyInsaneEligibility(log))
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIDs[AchievementEligibilityMildlyInsane], 1));
            }
        }
    }

    private static bool CustomCheckMildlyInsaneEligibility(ParsedEvtcLog log)
    {
        foreach (Player player in log.PlayerList)
        {
            IReadOnlyDictionary<long, BuffGraph> bgms = player.GetBuffGraphs(log);
            if (bgms != null && bgms.TryGetValue(Madness, out var bgm))
            {
                if (bgm.Values.Any(x => x.Value >= 99)) { return false; }
            }
        }
        return true;
    }
}
