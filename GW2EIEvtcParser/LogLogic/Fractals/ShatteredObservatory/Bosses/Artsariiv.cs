using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Artsariiv : ShatteredObservatory
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup(
        [
            new PlayerDstHealthDamageHitMechanic(VaultArtsariiv, new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Yellow), "Vault", "Vault from Big Adds","Vault (Add)", 0),
            new PlayerDstHealthDamageHitMechanic(SlamArtsariiv, new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Slam", "Slam (Vault) from Boss","Vault (Arts)", 0),
            new PlayerDstHealthDamageHitMechanic(TeleportLunge, new MechanicPlotlySetting(Symbols.StarTriangleDownOpen,Colors.LightOrange), "3 Jump", "Triple Jump Mid->Edge","Triple Jump", 0),
            new PlayerDstHealthDamageHitMechanic(AstralSurge, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Floor Circle", "Different sized spiraling circles","1000 Circles", 0),
            new PlayerDstHealthDamageHitMechanic([RedMarble1, RedMarble2], new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Marble", "Red KD Marble after Jump","Red Marble", 0),
            new SpawnMechanic((int)TargetID.Spark, new MechanicPlotlySetting(Symbols.Star,Colors.Teal),"Spark","Spawned a Spark (missed marble)", "Spark",0),
        ]);
    public Artsariiv(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "arts";
        Icon = EncounterIconArtsariiv;
        LogCategoryInformation.InSubCategoryOrder = 1;
        LogID |= 0x000002;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (914, 914),
                        (8991, 112, 11731, 2812));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayArtsariiv, crMap);
        return crMap;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.Artsariiv,
            TargetID.CloneArtsariiv
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        var trashIDs = new List<TargetID>(5 + base.GetTrashMobsIDs().Count);
        trashIDs.AddRange(base.GetTrashMobsIDs());
        trashIDs.Add(TargetID.TemporalAnomalyArtsariiv);
        trashIDs.Add(TargetID.Spark);
        trashIDs.Add(TargetID.SmallArtsariiv);
        trashIDs.Add(TargetID.MediumArtsariiv);
        trashIDs.Add(TargetID.BigArtsariiv);

        return trashIDs;
    }

    internal static List<PhaseData> ComputePhases(ParsedEvtcLog log, SingleActor artsariiv, IReadOnlyList<SingleActor> targets, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<PhaseData>(5);
        phases.AddRange(GetPhasesByInvul(log, Determined762, artsariiv, true, true, encounterPhase.Start, encounterPhase.End));
        for (int i = 0; i < phases.Count; i++)
        {
            var phaseIndex = i + 1;
            PhaseData phase = phases[i];
            phase.AddParentPhase(encounterPhase);
            if (phaseIndex % 2 == 0)
            {
                phase.Name = "Split " + (phaseIndex) / 2;
                AddTargetsToPhaseAndFit(phase, targets, [TargetID.CloneArtsariiv], log);
            }
            else
            {
                phase.Name = "Phase " + (phaseIndex + 1) / 2;
                phase.AddTarget(artsariiv, log);
            }
        }
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        // generic method for fractals
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor artsariiv = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Artsariiv)) ?? throw new MissingKeyActorsException("Artsariiv not found");
        phases[0].AddTarget(artsariiv, log);
        phases.AddRange(ComputePhases(log, artsariiv, Targets, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    static readonly (string, Vector2)[] CloneLocations =
    [
        ("M" , new(10357.898f, 1466.580f)),
        ("NE", new(11431.998f, 2529.760f)),
        ("NW", new(9286.878f, 2512.429f)),
        ("SW", new(9284.729f, 392.916f)),
        ("SE", new(11422.698f, 401.501f)),
        ("N" , new(10369.498f, 2529.010f)),
        ("E" , new(11_432.598f, 1460.400f)),
        ("S" , new(10_388.698f, 390.419f)),
        ("W" , new(9295.668f, 1450.060f)),
    ];
    protected override HashSet<int> IgnoreForAutoNumericalRenaming()
    {
        return [
            (int)TargetID.CloneArtsariiv
        ];
    }

    internal static bool DetectCloneArtsariivs(EvtcVersionEvent evtcVersion, AgentData agentData, List<CombatItem> combatData)
    {
        var artsariivMarkerGUID = combatData
            .Where(x => x.IsStateChange == StateChange.IDToGUID &&
                GetContentLocal((byte)x.OverstackValue) == ContentLocal.Marker &&
                MarkerGUIDs.ArtsariivTripleLaserEyeMarker.Equals(x.SrcAgent, x.DstAgent))
            .Select(x => new MarkerGUIDEvent(x, evtcVersion))
            .FirstOrDefault();
        if (artsariivMarkerGUID != null)
        {
            var markedsArtsariivs = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.Marker && x.Value == artsariivMarkerGUID.ContentID).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Distinct();
            foreach (AgentItem artsariiv in agentData.GetNPCsByID(TargetID.Artsariiv))
            {
                if (!markedsArtsariivs.Any(x => x.Is(artsariiv)))
                {
                    artsariiv.OverrideID(TargetID.CloneArtsariiv, agentData);
                }
            }
            return true;
        }
        return false;
    }

    internal static void RenameSmallArtsariivs(IReadOnlyList<NPC> trashMobs)
    {
        foreach (NPC trashMob in trashMobs)
        {
            if (trashMob.IsSpecies(TargetID.SmallArtsariiv))
            {
                trashMob.OverrideName("Small " + trashMob.Character);
            }
            if (trashMob.IsSpecies(TargetID.MediumArtsariiv))
            {
                trashMob.OverrideName("Medium " + trashMob.Character);
            }
            if (trashMob.IsSpecies(TargetID.BigArtsariiv))
            {
                trashMob.OverrideName("Big " + trashMob.Character);
            }
        }
    }

    internal static void RenameCloneArtsariivs(IReadOnlyList<SingleActor> targets, List<CombatItem> combatData)
    {
        var nameCount = new Dictionary<string, int> {
                { "M", 1 }, { "NE", 1 }, { "NW", 1 }, { "SW", 1 }, { "SE", 1 }, // both split clones start at 1
                { "N", 2 }, { "E", 2 }, { "S", 2 }, { "W", 2 }, // second split clones start at 2
        };

        foreach (var target in targets)
        {
            if (target.IsSpecies(TargetID.CloneArtsariiv))
            {
                string? suffix = AddNameSuffixBasedOnInitialPosition(target, combatData, CloneLocations);
                if (suffix != null && nameCount.ContainsKey(suffix))
                {
                    // deduplicate name
                    target.OverrideName(target.Character + " " + (nameCount[suffix]++));
                }
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (!DetectCloneArtsariivs(evtcVersion, agentData, combatData))
        {
            // Legacy
            var targetArtsariiv = FindTargetArtsariiv(agentData);
            foreach (AgentItem artsariiv in agentData.GetNPCsByID(TargetID.Artsariiv))
            {
                if (!artsariiv.Is(targetArtsariiv))
                {
                    artsariiv.OverrideID(TargetID.CloneArtsariiv, agentData);
                }
            }
        }
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        RenameSmallArtsariivs(TrashMobs);
        RenameCloneArtsariivs(Targets, combatData);
    }

    internal override LogData.LogMode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        return LogData.LogMode.CMNoName;
    }

    static private AgentItem FindTargetArtsariiv(AgentData agentData)
    {
        // cc artsariiv clones have the same species id, find target with longest aware time
        return agentData.GetNPCsByID(TargetID.Artsariiv).MaxBy(x => x.LastAware - x.FirstAware) ?? throw new MissingKeyActorsException("Artsariiv not found");
    }

    internal override long GetLogOffset(EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData)
    {
        // artsarriv starts invulnerable
        var artsariiv = FindTargetArtsariiv(agentData);
        return GetLogOffsetByInvulnStart(logData, combatData, artsariiv, Determined762);
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        base.CheckSuccess(combatData, agentData, logData, playerAgents);
        // reward or death worked
        if (logData.Success)
        {
            return;
        }
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Artsariiv)) ?? throw new MissingKeyActorsException("Artsariiv not found");
        if (combatData.GetEffectEvents().Count > 0)
        {
            // TBC: this looks promising so far
            if (combatData.TryGetEffectEventsByGUID(EffectGUIDs.ArtsariivDeadExplosion, out var effects))
            {
                logData.SetSuccess(true, effects.Last().Time);
            }
            else
            {
                logData.SetSuccess(false, target.LastAware);
            }
            return;
        }
        // Legacy
        SetSuccessByBuffCount(combatData, logData, playerAgents, target, Determined762, 4);
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IsInstance)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IsInstance)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        long castDuration;
        (long start, long end) lifespan;

        switch (target.ID)
        {
            case (int)TargetID.Artsariiv:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        case Obliterate:
                            castDuration = 3160;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            replay.Decorations.AddWithGrowing(new CircleDecoration(1300, lifespan, Colors.Orange, 0.2, new AgentConnector(target)), lifespan.end);
                            (float, float)[][] positions = [
                                // positions taken from effects
                                [(9286.88f, 2512.43f), (11432.0f, 2529.76f), (11422.7f, 401.501f), (9284.73f, 392.916f)],
                                [(10941.61f, 2044.3567f), (10934.861f, 889.46716f), (9772.5205f, 880.9314f), (9780.549f, 2030.362f)],
                                [(10116.815f, 1701.9971f), (10104.783f, 1213.3477f), (10602.564f, 1221.8499f), (10607.577f, 1713.7196f)],
                                [(10281.519f, 1390.1648f), (10429.899f, 1537.8489f), (10425.812f, 1398.6493f), (10295.681f, 1527.335f)],
                            ];
                            uint[] radius = [400, 290, 180, 70];
                            long nextInvul = log.CombatData.GetBuffApplyDataByIDByDst(Determined762, target.AgentItem).OfType<BuffApplyEvent>().FirstOrDefault(x => x.Time >= cast.Time)?.Time ?? log.LogData.LogEnd;
                            for (int i = 0; i < 4; i++)
                            {
                                long start = lifespan.end + 560 * i;
                                long end = start + 2450;
                                if (start >= nextInvul)
                                {
                                    break;
                                }
                                foreach ((float x, float y) in positions[i])
                                {
                                    var position = new PositionConnector(new(x, y, 0));
                                    replay.Decorations.AddWithGrowing(new CircleDecoration(radius[i], (start, end), Colors.Orange, 0.2, position), end);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                break;
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IsInstance)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

        // Beaming Smile
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ArtsariivBeamingSmileIndicator, out var beamIndicators))
        {
            foreach (EffectEvent effect in beamIndicators)
            {
                int start = (int)effect.Time;
                int end = start + 2640;
                AddBeamingSmileDecoration(effect, (start, end), Colors.Orange, 0.2, environmentDecorations);
            }
        }
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ArtsariivBeamingSmile, out var beams))
        {
            foreach (EffectEvent effect in beams)
            {
                int start = (int)effect.Time;
                int end = start + 300;
                AddBeamingSmileDecoration(effect, (start, end), Colors.Red, 0.2, environmentDecorations);
            }
        }
    }

    private static void AddBeamingSmileDecoration(EffectEvent effect, (int, int) lifespan, Color color, double opacity, CombatReplayDecorationContainer environmentDecorations)
    {
        const int length = 2500;
        const int hitbox = 360;
        const int offset = 60;
        var rotation = new AngleConnector(effect.Rotation.Z);
        GeographicalConnector position = new PositionConnector(effect.Position).WithOffset(new(0.0f, length / 2.0f + offset, 0), true);
        environmentDecorations.Add(new RectangleDecoration(360, length + hitbox, lifespan, color, opacity, position).UsingRotationConnector(rotation));
    }

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
    {
        List<CastEvent> res = [];
        res.AddRange(ProfHelper.ComputeUnderBuffCastEvents(combatData, skillData, NovaLaunchSAK, NovaLaunchBuff));
        return res;
    }
}
