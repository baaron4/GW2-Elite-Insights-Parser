using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using static GW2EIEvtcParser.AchievementEligibilityIDs;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class GuardiansGlade : VisionsOfEternityRaidEncounter
{
    private static bool IsFirstBee(long time, AgentItem agent, ParsedEvtcLog log)
    {
        foreach (var player in log.PlayerList)
        {
            if (agent.Is(player.AgentItem) || !player.InAwareTimes(time))
            {
                continue;
            }
            if (player.HasBuff(log, BitingSwarm, time - 2800, 2800))
            {
                return false;
            }
        }
        return true;
    }

    internal readonly MechanicGroup Mechanics = new([
        // Kela Stomp Left / Right
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic([KelaStompLeft, KelaStompRight], new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Orange), "Stomp.H", "Hit by Stomp", "Stomp Hit", 0)
                .WithStabilitySubMechanic(
                    new SubMechanic(new MechanicPlotlySetting(Symbols.TriangleLeftOpen, Colors.Orange), "Stomp.CC", "CC by Stomp", "Stomp CC", 0),
                    false
                ),
        ]),
        // Kela Claw Slam
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(KelaClawSlam, new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Orange), "ClawSlam.H", "Hit by ClawSlam", "ClawSlam Hit", 0)
                .WithStabilitySubMechanic(
                    new SubMechanic(new MechanicPlotlySetting(Symbols.TriangleUpOpen, Colors.Orange), "ClawSlam.CC", "CC by ClawSlam", "ClawSlam CC", 0),
                    false
                ),
        ]),
        // Lightning Strike
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(KelaLightningStrike, new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "LightStk.H", "Hit by Lightning Strike", "Lightning Strike Hit", 0)
                .WithStabilitySubMechanic(
                    new SubMechanic(new MechanicPlotlySetting(Symbols.CircleOpen, Colors.LightOrange), "LightStk.CC", "CC by Lightning Strike", "Lightning Strike CC", 10),
                    false
                ),
        ]),
        // Crocodilian Razortooth Tackle
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(CrocodilianRazortoothTackle, new MechanicPlotlySetting(Symbols.TriangleDown, Colors.Red), "CrocTackle.H", "Hit by Crocodilian Razortooth Tackle", "Croc Tackle Hit", 0)
                .WithStabilitySubMechanic(
                    new SubMechanic(new MechanicPlotlySetting(Symbols.TriangleDownOpen, Colors.Red), "CrocTackle.CC", "CC by Crocodilian Razortooth Tackle", "Croc Tackle CC", 0),
                    false
                ),
        ]),
        // Tornado
        new MechanicGroup([
            new PlayerDstHealthDamageHitMechanic(KelaTornado, new MechanicPlotlySetting(Symbols.YUp, Colors.Grey), "Tornado.H", "Hit by Tornado", "Tornado Hit", 0)
                .WithStabilitySubMechanic(
                    new SubMechanic(new MechanicPlotlySetting(Symbols.YUpOpen, Colors.Grey), "Tornado.CC", "CC by Tornado", "Tornado CC", 0),
                    false
                ),
        ]),
        new PlayerDstHealthDamageHitMechanic([KelaAmbush1, KelaAmbush2], new MechanicPlotlySetting(Symbols.CircleX, Colors.Red), "Ambush.H", "Hit by Ambush", "Ambush Hit", 0),
        new PlayerDstHealthDamageHitMechanic([KelaTantrum1, KelaTantrum2], new MechanicPlotlySetting(Symbols.Square, Colors.BreakbarActiveBlue), "Tantrum.H", "Hit by Tantrum", "Tantrum", 0),
        new PlayerDstHealthDamageHitMechanic(ScaldingWave, new MechanicPlotlySetting(Symbols.Star, Colors.Blue), "ScalWave.H", "Hit by Scalding Wave", "Scalding Wave Hit", 0),
        new PlayerDstBuffApplyMechanic(FixatedKela, new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "Fixated", "Fixated by Kela", "Kela Fixated", 0),
        new PlayerDstBuffApplyMechanic(Hunted, new MechanicPlotlySetting(Symbols.Star, Colors.Red), "Croc.Fix", "Fixated by Crocodilian Razortooth", "Croc Fixated", 0),
        new PlayerDstBuffApplyMechanic(ShreddedArmor, new MechanicPlotlySetting(Symbols.Octagon, Colors.LightRed), "ShredArmor.A", "Applied Shredded Armor", "Shredded Armor Applied", 0),
        // Loose Sand
        new MechanicGroup([
            new PlayerDstBuffApplyMechanic(LooseSand, new MechanicPlotlySetting(Symbols.Bowtie, Colors.LightPurple), "LooSand.A", "Applied Loose Sand", "Loose Sand Applied", 0),
            new MechanicGroup([
                new AchievementEligibilityMechanic(Ach_Surefooted, new MechanicPlotlySetting(Symbols.Bowtie, Colors.DarkPurple), "Achiv.Sand.L", "Achievement Eligibility: Surefooted Lost", "Achiv: Surefooted Lost", 0)
                    .UsingChecker((evt, log) => evt.Lost),
                new AchievementEligibilityMechanic(Ach_Surefooted, new MechanicPlotlySetting(Symbols.Bowtie, Colors.Purple), "Achiv.Sand.K", "Achievement Eligibility: Surefooted Kept", "Achiv: Surefooted Kept", 0)
                    .UsingChecker((evt, log) => !evt.Lost)
            ]),
        ]),
        // Biting Swarm
        new MechanicGroup([
            new PlayerDstBuffApplyMechanic(BitingSwarm, new MechanicPlotlySetting(Symbols.Diamond, Colors.Orange), "Bee", "Biting Swarm Application", "Biting Swarm", 0)
                .WithSubMechanic(new SubMechanic(new MechanicPlotlySetting(Symbols.Diamond, Colors.Orange), "Bee.First", "Biting Swarm First Application", "First Biting Swarm", 0), (time, actor, log) => IsFirstBee(time, actor.AgentItem, log))
                .WithSubMechanic(new SubMechanic(new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Orange), "Bee.Cntmntd", "Contaminated by Bitting Swarm", "Contaminated by Bitting Swarm ", 0), (time, actor, log) => !IsFirstBee(time, actor.AgentItem, log))
                .UsingIgnored()
        ]),
        new EnemyDstBuffApplyMechanic(RelentlessSpeed, new MechanicPlotlySetting(Symbols.Hourglass, Colors.Blue), "Speed", "Gained Relentless Speed", "Relentless Speed Applied", 0),
        // Eating
        new MechanicGroup([
            new EnemySrcHealthDamageMechanic(ArcDPSGenericKill, new MechanicPlotlySetting(Symbols.StarDiamond, Colors.Red), "Ate Croc", "Ate a Crocodilian Razortooth", "Ate Croc", 0)
                .UsingChecker((hde, log) => hde.To.IsSpecies(TargetID.DownedEliteCrocodilianRazortooth)),
            new EnemySrcHealthDamageMechanic(ArcDPSGenericKill, new MechanicPlotlySetting(Symbols.StarDiamondOpen, Colors.Red), "Ate Artifact", "Ate the Cursed Artifact", "Ate Artifact", 0)
                .UsingChecker((hde, log) => hde.To.IsSpecies(TargetID.CursedArtifact_NPC)),
            new MechanicGroup([
                new EnemySrcHealthDamageMechanic(ArcDPSGenericKill, new MechanicPlotlySetting(Symbols.StarSquare, Colors.Red), "Ate Player", "Ate a Player", "Ate Player", 0)
                    .UsingChecker((hde, log) => hde.To.IsPlayer),
                new PlayerDstHealthDamageMechanic(ArcDPSGenericKill, new MechanicPlotlySetting(Symbols.StarTriangleUp, Colors.Red), "Player Eaten", "Player Eaten", "Player Eaten", 0)
                    .UsingChecker((hde, log) => hde.From.IsSpecies(TargetID.KelaSeneschalOfWaves))
                    .WithSubMechanic(
                        new SubMechanic(new MechanicPlotlySetting(Symbols.StarTriangleUpOpen, Colors.Red), "Tank Eaten", "Tank Eaten", "Tank Eaten", 0)
                        , (time, agent, log) => agent.HasBuff(log, FixatedKela, time - 50)
                    ),
            ]),
        ]),
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
            (1149, 1149),
            (-23041.832, 10959.9775, -18941.832, 15059.9775));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayGuardiansGlade, crMap);
        return crMap;
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(KelaAura, KelaAura),
            new MinionSpawnCastFinder(ThrowRelic, (int)TargetID.CursedArtifact_NPC),
        ];
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData, GW2APIController apiController)
    {
        return "Guardian's Glade";
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.KelaSeneschalOfWaves,
            TargetID.ExecutorOfWaves,
            TargetID.EliteCrocodilianRazortooth,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.DownedEliteCrocodilianRazortooth,
            TargetID.VeteranCrocodilianRazortooth,
            TargetID.GuardiansGladeTornado,
            TargetID.CursedArtifact_NPC,
        ];
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericLogOffset(logData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            long enterCombatTime = GetEnterCombatTime(logData, agentData, combatData, logStartNPCUpdate.Time, GenericTriggerID, logStartNPCUpdate.DstAgent);
            var firstFixation = combatData.FirstOrDefault(x => x.IsBuffApply() && x.SkillID == FixatedKela);
            // If fixation is missing or first seen fixation is after boss enters combat, fallback to enterCombatTime, log will be seen as late start
            if (firstFixation == null || firstFixation.Time > enterCombatTime)
            {
                return enterCombatTime;
            }
            return firstFixation.Time;
        }
        return startToUse;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindChestGadgets([
            (ChestID.GrandRaidKelaChest, GrandRaidChestKelaPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100)),
        ], agentData, combatData);

        OverrideGenericKillAmbushKillingBlows(agentData, combatData);

        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);

        RenameCrocodilianRazortooth(Targets);

        /*var chest = agentData.GetGadgetsByID(ChestID.GrandRaidKelaChest).FirstOrDefault();
        if (chest != null)
        {
            var kela = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.KelaSeneschalOfWaves)) ?? throw new MissingKeyActorsException("Kela not found");
            // Chest can be used for success if it appears after kela
            if (kela.FirstAware < chest.FirstAware + 5000)
            {
                ChestID = ChestID.GrandRaidKelaChest;
            }
        }*/
    }

    internal static IReadOnlyList<SubPhasePhaseData> ComputePhases(ParsedEvtcLog log, SingleActor kela, IReadOnlyList<SingleActor> targets, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var eliteCrocs = targets.Where(x => x.IsSpecies(TargetID.EliteCrocodilianRazortooth));
        var crabs = targets.Where(x => x.IsSpecies(TargetID.ExecutorOfWaves));
        var phases = new List<SubPhasePhaseData>(10);
        var kelaCasts = kela.GetAnimatedCastEvents(log, encounterPhase.Start, encounterPhase.End);
        var burrowOrAmbushCast = kelaCasts.Where(x => x.SkillID == KelaBurrow || x.SkillID == KelaAmbush1 || x.SkillID == KelaAmbush2).ToList();
        var kelaNonStormRelatedCast = kelaCasts.Where(x => x.SkillID != KelaBurrow && x.SkillID != KelaAmbush1 && x.SkillID != KelaAmbush2 && x.SkillID != log.SkillData.DodgeID && x.ActualDuration > ServerDelayConstant).ToList();
        // Candidate phases
        var kelaPhases = GetSubPhasesByInvul(log, KelaBurrowed, kela, true, true, encounterPhase.Start, encounterPhase.End, false);
        List<SubPhasePhaseData> candidateMainPhases = [];
        List<SubPhasePhaseData> candidateStormPhases = [];
        for (int i = 0; i < kelaPhases.Count; i++)
        {
            SubPhasePhaseData subPhase = kelaPhases[i];
            subPhase.AddParentPhase(encounterPhase);
            subPhase.AddTarget(kela, log);
            if ((i % 2) == 0)
            {
                candidateMainPhases.Add(subPhase);
            }
            else
            {
                candidateStormPhases.Add(subPhase);
            }
        }
        // Split phases
        var stormPhaseCount = 1;
        List<SubPhasePhaseData> stormPhases = new(10);
        SubPhasePhaseData? curStormPhase = null;
        foreach (var candidateStormPhase in candidateStormPhases)
        {
            if (curStormPhase == null)
            {
                curStormPhase = candidateStormPhase;
                curStormPhase.Name = "Storm Phase " + (stormPhaseCount++);
                phases.Add(curStormPhase);
                stormPhases.Add(curStormPhase);
            }
            else
            {
                var nextBurrowStart = candidateStormPhase.Start;
                var previousBurrowEnd = curStormPhase.End;
                var burrowOrAmbushCastQuickly = burrowOrAmbushCast.Any(x => x.Time <= previousBurrowEnd + 5000 && x.Time >= previousBurrowEnd);
                var nonStormCast = kelaNonStormRelatedCast.Any(x => x.Time >= previousBurrowEnd - ServerDelayConstant && x.Time <= nextBurrowStart + ServerDelayConstant);
                if (!nonStormCast && burrowOrAmbushCastQuickly)
                {
                    curStormPhase.OverrideEnd(candidateStormPhase.End);
                }
                else
                {
                    curStormPhase = candidateStormPhase;
                    phases.Add(curStormPhase);
                    curStormPhase.Name = "Storm Phase " + (stormPhaseCount++);
                    stormPhases.Add(curStormPhase);
                }
            }
        }
        // Handle edges cases where Kela is killed before it can burrow again, creating a "fake" dps phase
        var lastStormPhase = stormPhases.LastOrDefault();
        if (lastStormPhase != null)
        {
            var followingMainphase = candidateMainPhases.FirstOrDefault(x => x.Start >= lastStormPhase.End);
            if (followingMainphase != null)
            {
                var fightEnd = followingMainphase.End;
                var previousBurrowEnd = lastStormPhase.End;
                var burrowOrAmbushCastQuickly = burrowOrAmbushCast.Any(x => x.Time <= previousBurrowEnd + 5000 && x.Time >= previousBurrowEnd);
                var nonStormCast = kelaNonStormRelatedCast.Any(x => x.Time >= previousBurrowEnd - ServerDelayConstant && x.Time <= fightEnd + ServerDelayConstant);
                if (!nonStormCast && burrowOrAmbushCastQuickly)
                {
                    lastStormPhase.OverrideEnd(followingMainphase.End);
                    candidateMainPhases.Remove(followingMainphase);
                }
            }
        }
        foreach (var stormPhase in stormPhases)
        {
            stormPhase.AddTargets(crabs, log, PhaseData.TargetPriority.NonBlocking);
            stormPhase.AddTargets(eliteCrocs, log, PhaseData.TargetPriority.NonBlocking);
        }
        // Main phases
        var mainPhaseCount = 1;
        foreach (var candidateMainPhase in candidateMainPhases)
        {
            if (!stormPhases.Any(x => x.InInterval((candidateMainPhase.Start + candidateMainPhase.End) / 2)))
            {
                candidateMainPhase.Name = "Phase " + (mainPhaseCount++);
                candidateMainPhase.AddTargets(crabs, log, PhaseData.TargetPriority.NonBlocking);
                phases.Add(candidateMainPhase);
            }
        }
        return phases;
    }

    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>() {
            { TargetID.KelaSeneschalOfWaves, 0 },
            { TargetID.EliteCrocodilianRazortooth, 1 },
            { TargetID.ExecutorOfWaves, 1 },
        };
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        var kela = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.KelaSeneschalOfWaves)) ?? throw new MissingKeyActorsException("Kela not found");
        var phases = GetInitialPhase(log);
        var fullFightPhase = (EncounterPhaseData)phases[0];
        fullFightPhase.AddTarget(kela, log);
        phases.AddRange(ComputePhases(log, kela, Targets, fullFightPhase, requirePhases));
        return phases;
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        SingleActor kela = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.KelaSeneschalOfWaves)) ?? throw new MissingKeyActorsException("Kela not found");
        return (kela.GetHealth(combatData) > 70e6) ? LogData.Mode.CM : LogData.Mode.Normal;
    }

    internal override LogData.StartStatus GetLogStartStatus(CombatData combatData, AgentData agentData, LogData logData)
    {
        var genericStatus = base.GetLogStartStatus(combatData, agentData, logData);
        if (genericStatus != LogData.StartStatus.Normal)
        {
            return genericStatus;
        }
        var firstFixation = combatData.GetBuffApplyData(FixatedKela).FirstOrDefault();
        if (firstFixation != null)
        {
            return firstFixation.Time <= logData.LogStart ? LogData.StartStatus.Normal : LogData.StartStatus.Late;
        }
        return LogData.StartStatus.Late;
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


    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
        // Fixated
        var kelaPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID).ToList();
        var fixateds = p.GetBuffStatus(log, FixatedKela).Where(x => x.Value > 0);
        foreach (Segment seg in fixateds)
        {
            var kela = kelaPhases.FirstOrDefault(x => x.IntersectsWindow(seg.Start, seg.End))?.Targets.First(x => x.Key.IsSpecies(TargetID.KelaSeneschalOfWaves)).Key;
            if (kela != null)
            {
                replay.Decorations.AddTether(seg.Start, seg.End, p.AgentItem, kela.AgentItem, Colors.FixationPurple.WithAlpha(0.3).ToString());
            }
            replay.Decorations.AddOverheadIcon(seg, p, ParserIcons.FixationPurpleOverhead);
        }

        // Biting Swarm
        var bitingSwarms = p.GetBuffStatus(log, BitingSwarm).Where(x => x.Value > 0);
        foreach (var seg in bitingSwarms)
        {
            var decoration = new CircleDecoration(100, seg, Colors.Orange, 0.3, new AgentConnector(p.AgentItem));
            replay.Decorations.AddWithBorder(decoration, Colors.Orange, 0.2);
        }

        // Crocodilian Razortooth Fixation
        var crocHunted = GetBuffApplyRemoveSequencePerInstanceID(log.CombatData, Hunted, p.AgentItem, true).ToList();
        foreach (List<BuffEvent> eventsList in crocHunted)
        {
            // TODO - Investigate why some croc do not have a tether
            replay.Decorations.AddTethers(eventsList, Colors.Red, 0.2);
            foreach (BuffEvent ev in eventsList.Where(x => x is BuffApplyEvent))
            {
                // Adjust the fixation end time, in game it's not removed correctly when the croc dies.
                var lifespan = (ev.Time, ev.By.LastAware);
                replay.Decorations.Add(new IconOverheadDecoration(ParserIcons.FixationRedOverhead, 20, 1, lifespan, new AgentConnector(p)));
            }
        }
    }
    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        switch (target.ID)
        {
            case (int)TargetID.KelaSeneschalOfWaves:
                {
                    // Burrowed
                    foreach (var burrowed in target.GetBuffStatus(log, KelaBurrowed).Where(x => x.Value > 0))
                    {
                        var decoration = new CircleDecoration(320, burrowed, Colors.Orange, 0.3, new AgentConnector(target));
                        var ring = new CircleDecoration(450, burrowed, Colors.Red, 0.5, new AgentConnector(target)).UsingFilled(false);
                        replay.Decorations.Add(decoration);
                        //replay.Decorations.Add(ring); // TODO: Size to be verified, death most likely snaps at ambush cast start
                        replay.Decorations.AddOverheadIcon(burrowed, target, ParserIcons.RedArrowDownOverhead);
                    }

                    // Tantrum Breakbar
                    var breakbarUpdates = target.GetBreakbarPercentUpdates(log);
                    var (_, breakbarActives, _, _) = target.GetBreakbarStatus(log);
                    foreach (var segment in breakbarActives)
                    {
                        replay.Decorations.AddActiveBreakbar(segment.TimeSpan, target, breakbarUpdates);
                    }

                    // Tantrum Range
                    var tantrumCasts = target.GetAnimatedCastEvents(log).Where(x => x.SkillID == KelaTantrum1 || x.SkillID == KelaTantrum2).ToList();
                    foreach (var cast in tantrumCasts)
                    {
                        var decoration = new CircleDecoration(1800, (cast.Time, cast.EndTime), Colors.DarkRed, 0.8, new AgentConnector(target)).UsingFilled(false);
                        replay.Decorations.Add(decoration);
                    }
                    break;
                }
            case (int)TargetID.GuardiansGladeTornado:
                {
                    // Tornado (visual representation via applied buff)
                    var start = log.CombatData.GetBuffApplyDataByIDByDst(TornadoEffects, target.AgentItem).FirstOrDefault()?.Time;
                    var end = log.CombatData.GetDespawnEvents(target.AgentItem).FirstOrDefault()?.Time ?? target.LastAware;
                    if (start != null)
                    {
                        var decoration = new CircleDecoration(235, (start.Value, end), Colors.BlueishGrey, 0.2, new AgentConnector(target));
                        replay.Decorations.AddWithBorder(decoration, Colors.Orange, 0.2);
                    }
                    break;
                }
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }
        (long start, long end) lifespan;

        // Claw Slam (frontal)
        long slamDuration = 2520; // Effect duration lasts longer than when the damage event happens.
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GuardiansGaleClawSlamIndicator, out var clawSlams))
        {
            foreach (var effect in clawSlams)
            {
                lifespan = (effect.Time, effect.Time + slamDuration);
                var decoration = (PieDecoration)new PieDecoration(600, 135, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position))
                   .UsingRotationConnector(new AngleConnector(effect.Rotation.Z + 90));
                environmentDecorations.AddWithFilledWithGrowing(decoration, true, lifespan.end);
            }
        }

        // Stomp
        long stompDuration = 1520; // Effect duration lasts longer than when the damage event happens.
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GuardiansGaleStompLeftIndicator, out var stompLeft))
        {
            foreach (var effect in stompLeft)
            {
                lifespan = (effect.Time, effect.Time + stompDuration);
                var decoration = (PieDecoration)new PieDecoration(600, 180, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position))
                   .UsingRotationConnector(new AngleConnector(effect.Rotation.Z + 90));
                environmentDecorations.AddWithFilledWithGrowing(decoration, true, lifespan.end);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GuardiansGaleStompRightIndicator, out var stompRight))
        {
            foreach (var effect in stompRight)
            {
                lifespan = (effect.Time, effect.Time + stompDuration);
                var decoration = (PieDecoration)new PieDecoration(600, 180, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position))
                   .UsingRotationConnector(new AngleConnector(effect.Rotation.Z + 90));
                environmentDecorations.AddWithFilledWithGrowing(decoration, true, lifespan.end);
            }
        }

        // Sand
        if (log.CombatData.GetGW2BuildEvent().Build >= GW2Builds.February2026GuardiansGladeCMReleaseAndMinorBalance)
        {
            foreach (var agent in log.AgentData.GetNPCsByIDs([TargetID.ExecutorOfWaves, TargetID.KelaSeneschalOfWavesSand]))
            {
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(agent, EffectGUIDs.GuardiansGaleSandBorder, out var borders))
                {
                    AddSandDecorations(log, environmentDecorations, borders, 2500, Colors.Yellow, 0.1, true, Colors.Red, 0.2);
                }
            }
        } 
        else
        {
            foreach (var agent in log.AgentData.GetNPCsByIDs([TargetID.ExecutorOfWaves, TargetID.KelaSeneschalOfWavesSand]))
            {
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(agent, EffectGUIDs.GuardiansGaleSand, out var sands))
                {
                    AddSandDecorations(log, environmentDecorations, sands, 2500, Colors.Yellow, 0.1, true);
                }
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(agent, EffectGUIDs.GuardiansGaleSandBorder, out var borders))
                {
                    AddSandDecorations(log, environmentDecorations, borders, 3500, Colors.Red, 0.2, false);
                }
            }
        }

        // Scalding Wave
        const uint waveLength = 5000;
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GuardiansGaleScaldingWaveIndicator, out var waveIndicators))
        {
            foreach (var effect in waveIndicators)
            {
                var position = new PositionConnector(effect.Position).WithOffset(new Vector3(0f, -0.5f * waveLength, 0f), true);
                lifespan = effect.ComputeLifespanWithSecondaryEffect(log, EffectGUIDs.GuardiansGaleScaldingWave);
                var decoration = new RectangleDecoration(2000, waveLength, lifespan, Colors.LightOrange, 0.2, position)
                   .UsingRotationConnector(new AngleConnector(effect.Rotation.Z));
                environmentDecorations.Add(decoration);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GuardiansGaleScaldingWave, out var waves))
        {
            foreach (var effect in waves)
            {
                // TODO - Investigate if it's possible to update the behaviour of the wave to be a moving rectangle.
                // If it's possible, update the indicator above to last since its spawn time until the wave ends.
                var position = new PositionConnector(effect.Position).WithOffset(new Vector3(0f, -0.5f * waveLength, 0f), true);
                var decoration = new RectangleDecoration(2000, waveLength, effect.ComputeLifespan(log, 4666), Colors.LightBlue, 0.2, position)
                   .UsingRotationConnector(new AngleConnector(effect.Rotation.Z));
                environmentDecorations.Add(decoration);
            }
        }

        // Lightning Strike
        const float ground = -1700f; // ignore effects significantly above ground
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GuardiansGaleLightningStrikeIndicator, out var lightningIndicators))
        {
            foreach (var effect in lightningIndicators.Where(x => x.IsBelowHeight(ground, 100)))
            {
                lifespan = effect.ComputeLifespan(log, 1200);
                var decoration = new CircleDecoration(190, lifespan, Colors.LightOrange, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.AddWithFilledWithGrowing(decoration, true, lifespan.end);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GuardiansGaleLightningStrikeHit, out var lightningHits))
        {
            foreach (var effect in lightningHits.Where(x => x.IsBelowHeight(ground, 100)))
            {
                lifespan = (effect.Time, effect.Time + 100); // actual effect duration 1s
                var decoration = new CircleDecoration(190, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.Add(decoration);
            }
        }
    }
    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }

        var surefooted = new List<AchievementEligibilityEvent>();
        var kelaPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID && x.IsCM && x.IntersectsWindow(p.FirstAware, p.LastAware)).ToHashSet();
        var sandApplications = log.CombatData.GetBuffApplyDataByIDByDst(LooseSand, p.AgentItem);

        foreach (AbstractBuffApplyEvent apply in sandApplications)
        {
            // If the Executor is alive, Loose Sand is applied by the Executor and you lose the eligibility.
            // If the Executor is dead, Loose Sand is applied by Kela and you do NOT lose the eligibility.
            // This will require a GW2 build check in case it gets fixed in game.
            if (apply.By.IsSpecies(TargetID.ExecutorOfWaves))
            {
                InsertAchievementEligibityEventAndRemovePhase(kelaPhases, surefooted, apply.Time, Ach_Surefooted, p);
            }
        }

        AddSuccessBasedAchievementEligibityEvents(kelaPhases, surefooted, Ach_Surefooted, p);
        achievementEligibilityEvents.AddRange(surefooted);
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }

        var encounterPhases = log.LogData.GetEncounterPhases(log).Where(x => x.ID == LogID);
        var tackleCasts = log.CombatData.GetAnimatedCastData(CrocodilianRazortoothTackle)
                .Where(x => x.Caster.IsAnySpecies([TargetID.VeteranCrocodilianRazortooth, TargetID.EliteCrocodilianRazortooth])).ToList();

        foreach (var phase in encounterPhases)
        {
            if (!phase.Success || !phase.IsCM)
            {
                continue;
            }

            if (!tackleCasts.Any(x => phase.InInterval(x.Time)))
            {
                instanceBuffs.Add(new(log.Buffs.BuffsByIDs[AchievementEligibilitySeeYouLaterAlligator], 1, phase));
            }
        }
    }

    private static void AddSandDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer decorations, IReadOnlyList<EffectEvent> effects, long defaultDuration, Color color, double opacity, bool filled, Color? borderColor = null, double borderOpacity = 0)
    {
        const uint maxInterval = 2500; // usually 2s interval
        const uint baseRadius = 240;

        var mergedStart = long.MaxValue;
        for (var i = 0; i < effects.Count; i++)
        {
            var effect = effects[i];
            var (start, end) = effect.ComputeDynamicLifespan(log, defaultDuration);
            var next = effects.ElementAtOrDefault(i + 1);
            if (next?.Time < end + maxInterval) // avoid overlap and pulsing by using start of next as end
            {
                if (next.Scale == effect.Scale) // merge same scale with next
                {
                    mergedStart = Math.Min(mergedStart, start);
                    continue;
                }
                end = next.Time;
            }
            start = Math.Min(start, mergedStart);
            var radius = (uint)(effect.Scale * baseRadius);
            var decoration = new CircleDecoration(radius, (start, end), color, opacity, new PositionConnector(effect.Position))
                .UsingFilled(filled);
            if (borderColor != null)
            {
                decorations.AddWithBorder(decoration, borderColor, borderOpacity);
            }
            else
            {
                decorations.Add(decoration);
            }
            mergedStart = long.MaxValue;
        }
    }

    private static void RenameCrocodilianRazortooth(IReadOnlyList<SingleActor> targets)
    {
        foreach (SingleActor actor in targets)
        {
            switch (actor.ID)
            {
                case (int)TargetID.VeteranCrocodilianRazortooth:
                    actor.OverrideName("Veteran " + actor.Character);
                    break;
                case (int)TargetID.EliteCrocodilianRazortooth:
                    actor.OverrideName("Elite " + actor.Character);
                    break;
                case (int)TargetID.DownedEliteCrocodilianRazortooth:
                    actor.OverrideName("Downed Elite " + actor.Character);
                    break;
            }
        }
    }

    /// <summary>
    /// When Kela ambushes a player or a crocodilian razortooth, sometimes the killing blow has no source attached.<br></br>
    /// We redirect the source of the combat item to Kela.
    /// </summary>
    private static void OverrideGenericKillAmbushKillingBlows(AgentData agentData, List<CombatItem> combatData)
    {
        var kelas = agentData.GetNPCsByID(TargetID.KelaSeneschalOfWaves);

        IReadOnlyList<AgentItem> downedCrocs = agentData.GetNPCsByID(TargetID.DownedEliteCrocodilianRazortooth);
        foreach (AgentItem croc in downedCrocs)
        {
            var kela = kelas.FirstOrDefault(x => x.InAwareTimes(croc));
            if (kela != null)
            {
                IEnumerable<CombatItem> items = combatData.Where(x => x.IsDamage() && x.DstMatchesAgent(croc) && x.SrcInstid == 0 && x.SkillID == ArcDPSGenericKill);
                foreach (CombatItem item in items)
                {
                    item.OverrideSrcAgent(kela);
                }
            }
        }

        List<AgentItem> eatableIFFFoeAgents = [
            ..agentData.GetNPCsByID(TargetID.CursedArtifact_NPC),
            ..agentData.GetAgentByType(AgentItem.AgentType.Player)
        ];
        foreach (AgentItem eatableAgent in eatableIFFFoeAgents)
        {
            var kela = kelas.FirstOrDefault(x => x.InAwareTimes(eatableAgent));
            if (kela != null)
            {
                IEnumerable<CombatItem> items = combatData.Where(x => x.IsDamage() && x.DstMatchesAgent(eatableAgent)
                    && x.SrcInstid == 0 && x.SkillID == ArcDPSGenericKill
                    && x.IFF == IFF.Foe);
                foreach (CombatItem item in items)
                {
                    item.OverrideSrcAgent(kela);
                }
            }
        }
    }
}
