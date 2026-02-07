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
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.AchievementEligibilityIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class GuardiansGlade : VisionsOfEternityRaidEncounter
{
    internal readonly MechanicGroup Mechanics = new([
        new PlayerDstBuffApplyMechanic(FixatedKela, new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "Fixated", "Fixated by Kela","Fixated", 0),
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
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
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

    internal static IReadOnlyList<SubPhasePhaseData> ComputePhases(ParsedEvtcLog log, SingleActor kela, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<SubPhasePhaseData>(10);
        var kelaCasts = kela.GetAnimatedCastEvents(log, encounterPhase.Start, encounterPhase.End);
        var burrowCast = kelaCasts.Where(x => x.SkillID == KelaBurrow).ToList();
        var kelaNonBurrowRelatedCast = kelaCasts.Where(x => x.SkillID != KelaBurrow && x.SkillID != KelaAmbush1 && x.SkillID != KelaAmbush2 && x.SkillID != log.SkillData.DodgeID && x.ActualDuration > ServerDelayConstant).ToList();
        // Candidate phases
        var kelaPhases = GetSubPhasesByInvul(log, KelaBurrowed, kela, true, true, encounterPhase.Start, encounterPhase.End, false);
        List<SubPhasePhaseData> candidateMainPhases = [];
        List<SubPhasePhaseData> candidateStormPhases = [];
        for (int i = 0; i < kelaPhases.Count; i++)
        {
            SubPhasePhaseData subPhase = kelaPhases[i];
            subPhase.AddParentPhase(encounterPhase);
            if ((i % 2) == 0)
            {
                candidateMainPhases.Add(subPhase);
            }
            else
            {
                candidateStormPhases.Add(subPhase);
            }
            subPhase.AddTarget(kela, log);
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
                var burrowCastQuickly = burrowCast.Any(x => x.Time <= previousBurrowEnd + 5000 && x.Time >= previousBurrowEnd);
                var nonBurrowCast = kelaNonBurrowRelatedCast.Any(x => x.Time >= previousBurrowEnd - ServerDelayConstant && x.Time <= nextBurrowStart + ServerDelayConstant);
                if (!nonBurrowCast && burrowCastQuickly)
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
        // Main phases
        var mainPhaseCount = 1;
        foreach (var candidateMainPhase in candidateMainPhases)
        {
            if (!stormPhases.Any(x => x.InInterval((candidateMainPhase.Start + candidateMainPhase.End) / 2)))
            {
                candidateMainPhase.Name = "Phase " + (mainPhaseCount++);
                phases.Add(candidateMainPhase);
            }
        }
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        var kela = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.KelaSeneschalOfWaves)) ?? throw new MissingKeyActorsException("Kela not found");
        var phases = GetInitialPhase(log);
        var fullFightPhase = (EncounterPhaseData)phases[0];
        fullFightPhase.AddTarget(kela, log);
        phases.AddRange(ComputePhases(log, kela, fullFightPhase, requirePhases));
        return phases;
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.Mode.Normal;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
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
            var decoration = new CircleDecoration(100, seg, Colors.Orange, 0.1, new AgentConnector(p.AgentItem));
            replay.Decorations.Add(decoration);
        }
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

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        switch (target.ID)
        {
            case (int)TargetID.KelaSeneschalOfWaves:
                {
                    // Burrowed
                    foreach (var burrowed in target.GetBuffStatus(log, KelaBurrowed).Where(x => x.Value > 0))
                    {
                        var decoration = new CircleDecoration(320, burrowed, Colors.Orange, 0.2, new AgentConnector(target));
                        replay.Decorations.Add(decoration);
                        replay.Decorations.AddOverheadIcon(burrowed, target, ParserIcons.RedArrowDownOverhead);
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
                        replay.Decorations.Add(decoration);
                    }
                    break;
                }
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        // Claw Slam (frontal)
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GuardiansGaleClawSlamIndicator, out var clawSlams))
        {
            foreach (var effect in clawSlams)
            {
                var decoration = new PieDecoration(600, 135, effect.ComputeLifespan(log, 3000), Colors.Orange, 0.2, new PositionConnector(effect.Position))
                   .UsingRotationConnector(new AngleConnector(effect.Rotation.Z + 90));
                environmentDecorations.Add(decoration);
            }
        }

        // Stomp
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GuardiansGaleStompLeftIndicator, out var stompLeft))
        {
            foreach (var effect in stompLeft)
            {
                var decoration = new PieDecoration(600, 180, effect.ComputeLifespan(log, 2000), Colors.Orange, 0.2, new PositionConnector(effect.Position))
                   .UsingRotationConnector(new AngleConnector(effect.Rotation.Z + 90));
                environmentDecorations.Add(decoration);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GuardiansGaleStompRightIndicator, out var stompRight))
        {
            foreach (var effect in stompRight)
            {
                var decoration = new PieDecoration(600, 180, effect.ComputeLifespan(log, 2000), Colors.Orange, 0.2, new PositionConnector(effect.Position))
                   .UsingRotationConnector(new AngleConnector(effect.Rotation.Z - 90));
                environmentDecorations.Add(decoration);
            }
        }

        // Sand
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

        // Scalding Wave
        const uint waveLength = 5000;
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GuardiansGaleScaldingWaveIndicator, out var waveIndicators))
        {
            foreach (var effect in waveIndicators)
            {
                var position = new PositionConnector(effect.Position).WithOffset(new Vector3(0f, -0.5f * waveLength, 0f), true);
                var decoration = new RectangleDecoration(2000, waveLength, effect.ComputeLifespan(log, 4000), Colors.LightOrange, 0.2, position)
                   .UsingRotationConnector(new AngleConnector(effect.Rotation.Z));
                environmentDecorations.Add(decoration);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GuardiansGaleScaldingWave, out var waves))
        {
            foreach (var effect in waves)
            {
                var position = new PositionConnector(effect.Position).WithOffset(new Vector3(0f, -0.5f * waveLength, 0f), true);
                var decoration = new RectangleDecoration(2000, waveLength, effect.ComputeLifespan(log, 4666), Colors.Red, 0.2, position)
                   .UsingRotationConnector(new AngleConnector(effect.Rotation.Z));
                environmentDecorations.Add(decoration);
            }
        }

        // Lightning Strike
        const float ground = -1700f; // ignore effects significantly above ground
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GuardiansGaleLightningStrikeIndicator, out var lightningIndicators))
        {
            foreach (var effect in lightningIndicators.Where(x => x.isBelowHeight(ground, 100)))
            {
                var (start, end) = effect.ComputeLifespan(log, 1200);
                var decoration = new CircleDecoration(190, effect.ComputeLifespan(log, 1200), Colors.LightOrange, 0.2, new PositionConnector(effect.Position))
                    .UsingFilled(false);
                environmentDecorations.AddWithFilledWithGrowing(decoration, true, end);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.GuardiansGaleLightningStrikeHit, out var lightningHits))
        {
            foreach (var effect in lightningHits.Where(x => x.isBelowHeight(ground, 100)))
            {
                var lifespan = (effect.Time, effect.Time + 100); // actual effect duration 1s
                var decoration = new CircleDecoration(190, lifespan, Colors.Red, 0.2, new PositionConnector(effect.Position));
                environmentDecorations.Add(decoration);
            }
        }
    }

    private static void AddSandDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer decorations, IReadOnlyList<EffectEvent> effects, long defaultDuration, Color color, double opacity, bool filled)
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
            decorations.Add(decoration);
            mergedStart = long.MaxValue;
        }
    }
}
