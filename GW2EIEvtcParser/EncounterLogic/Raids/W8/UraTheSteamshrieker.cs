
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class UraTheSteamshrieker : MountBalrior
{
    public UraTheSteamshrieker(int triggerID) : base(triggerID)
    {
        MechanicList.AddRange(new List<Mechanic>()
        {
            new PlayerDstBuffApplyMechanic(Deterrence, "Deterrence", new MechanicPlotlySetting(Symbols.Diamond, Colors.LightRed), "Pick-up Shard", "Picked up the Bloodstone Shard", "Bloodstone Shard Pick-up", 0),
            new PlayerDstBuffApplyMechanic(BloodstoneSaturation, "Bloodstone Saturation", new MechanicPlotlySetting(Symbols.Diamond, Colors.DarkPurple), "Dispel", "Used Dispel (SAK)", "Used Dispel", 0),
            new PlayerDstBuffApplyMechanic(PressureBlastTargetBuff, "Pressure Blast", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.LightBlue), "Pres.Blast.T", "Targeted by Pressure Blast (Bubbles)", "Pressure Blast Target", 0),
            new PlayerDstBuffApplyMechanic(PressureBlastBubbleBuff, "Pressure Blast", new MechanicPlotlySetting(Symbols.CircleOpen, Colors.Blue), "Pres.Blast.Up", "Lifted in a bubble by Pressure Blast", "Pressure Blast Bubble", 0),
            new PlayerDstEffectMechanic(EffectGUIDs.UraSteamPrisonIndicator, "Steam Prison", new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.LightOrange), "Ste.Prison.T", "Targeted by Steam Prison (Ring)", "Steam Prison Target (Ring)", 0),
        });
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

    protected override ReadOnlySpan<int> GetTargetsIDs()
    {
        return
        [
            (int)ArcDPSEnums.TargetID.Ura,
            (int)ArcDPSEnums.TrashID.ChampionFumaroller,
            (int)ArcDPSEnums.TrashID.EliteFumaroller,
        ];
    }

    protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
    {
        return
        [
            ArcDPSEnums.TrashID.SulfuricGeyser,
            ArcDPSEnums.TrashID.TitanspawnGeyser,
            ArcDPSEnums.TrashID.ToxicGeyser,
            ArcDPSEnums.TrashID.UraGadget_BloodstoneShard,
        ];
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new BuffGainCastFinder(UraDispelSAK, BloodstoneSaturation),
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
        var bloodstoneShards = combatData
            .Where(x => x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate && MaxHealthUpdateEvent.GetMaxHealth(x) == 14940)
            .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
            .Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxHeight == 300 && x.HitboxWidth == 2 && x.FirstAware > 0)
            .Distinct();
        refresh |= bloodstoneShards.Any();
        foreach (var shard in bloodstoneShards)
        {
            shard.OverrideID(ArcDPSEnums.TrashID.UraGadget_BloodstoneShard);
            shard.OverrideType(AgentItem.AgentType.NPC);
        }
        if (refresh)
        {
            agentData.Refresh();
        }
        ComputeFightTargets(agentData, combatData, extensions);

        int[] curFumarollers = [1, 1];
        foreach (SingleActor target in Targets)
        {
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TrashID.EliteFumaroller:
                    target.OverrideName("Elite " + target.Character + " " + curFumarollers[0]++);
                    break;
                case (int)ArcDPSEnums.TrashID.ChampionFumaroller:
                    target.OverrideName("Champion " + target.Character + " " + curFumarollers[1]++);
                    break;
                default:
                    break;
            }
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        switch (target.ID)
        {
            case (int)ArcDPSEnums.TargetID.Ura:
                // Slam - Cone - Third step of the attack
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.UraSlamCone, out var slams))
                {
                    foreach (EffectEvent effect in slams)
                    {
                        long duration = 2000;
                        long growing = effect.Time + duration;
                        (long start, long end) lifespan = effect.ComputeLifespan(log, duration);
                        lifespan.end = Math.Min(lifespan.end, ComputeEndCastTimeByBuffApplication(log, target, Stun, effect.Time, duration));
                        if (target.TryGetCurrentFacingDirection(log, effect.Time, out var facingDirection, duration))
                        {
                            var pie = (PieDecoration)new PieDecoration(1000, 60, lifespan, Colors.LightOrange, 0.2, new AgentConnector(target)).UsingRotationConnector(new AngleConnector(facingDirection));
                            replay.AddDecorationWithGrowing(pie, growing);
                        }
                    }
                }
                break;
            case (int)ArcDPSEnums.TrashID.ToxicGeyser:
                replay.Decorations.Add(new CircleDecoration(480, (target.FirstAware, target.LastAware), Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false));
                break;
            case (int)ArcDPSEnums.TrashID.SulfuricGeyser:
                replay.Decorations.Add(new CircleDecoration(580, (target.FirstAware, target.LastAware), Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false));
                break;
            case (int)ArcDPSEnums.TrashID.TitanspawnGeyser:

                break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);

        replay.AddOverheadIcons(player.GetBuffStatus(log, Deterrence, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.CrimsonAttunementOverhead);

        // Pressure Blast - Bubble AoE Indicator
        var pressureBlastTarget = player.GetBuffStatus(log, PressureBlastTargetBuff, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        foreach (var segment in pressureBlastTarget)
        {
            replay.AddDecorationWithGrowing(new CircleDecoration(180, segment.TimeSpan, Colors.LightOrange, 0.2, new AgentConnector(player)), segment.End);
        }

        // Pressure Blast - Bubble Lift Up
        var pressureBlastBubble = player.GetBuffStatus(log, PressureBlastBubbleBuff, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        foreach (var segment in pressureBlastBubble)
        {
            replay.Decorations.Add(new CircleDecoration(100, segment.TimeSpan, Colors.LightBlue, 0.2, new AgentConnector(player)));
        }

        // Steam Prison - Ring
        if (log.CombatData.TryGetEffectEventsByDstWithGUID(player.AgentItem, EffectGUIDs.UraSteamPrisonIndicator, out var steamPrisons))
        {
            foreach (EffectEvent effect in steamPrisons)
            {
                (long start, long end) lifespan = effect.ComputeLifespan(log, 3000);
                replay.Decorations.Add(new DoughnutDecoration(400, 500, lifespan, Colors.LightOrange, 0.2, new AgentConnector(player)));
            }
        }
    }
}
