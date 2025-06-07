using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Dhuum : HallOfChains
{
    private bool _hasPrevent;
    private int _greenStart;

    public Dhuum(int triggerID) : base(triggerID)
    {
        _hasPrevent = true;
        _greenStart = 0;
        MechanicList.Add(new MechanicGroup([
            new PlayerDstHitMechanic(HatefulEphemera, new MechanicPlotlySetting(Symbols.Square,Colors.LightOrange), "Golem", "Hateful Ephemera (Golem AoE dmg)","Golem Dmg", 0),
            new MechanicGroup([
                new PlayerDstHitMechanic(ArcingAfflictionHit, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Bomb dmg", "Arcing Affliction (Bomb) hit","Bomb dmg", 0),
                new PlayerDstBuffApplyMechanic(ArcingAffliction, new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Bomb", "Arcing Affliction (Bomb) application","Bomb", 0),
                new PlayerDstBuffRemoveMechanic(ArcingAffliction, new MechanicPlotlySetting(Symbols.Diamond,Colors.Red), "Bomb Trig","Arcing Affliction (Bomb) manualy triggered", "Bomb Triggered",0).UsingChecker((br, log) =>
                {
                    // Removal duration check
                    if (br.RemovedDuration < 50)
                    {
                        return false;
                    }
                    // Greater Death mark check
                    if (log.CombatData.GetDamageData(GreaterDeathMark).Any(x => Math.Abs(x.Time - br.Time) < 100 && x.To == br.To)) {
                        return false;
                    }
                    // Spirit transformation check
                    if (br.To.HasBuff(log, MortalCoilDhuum, br.Time, ServerDelayConstant))
                    {
                        return false;
                    }
                    // Death check
                    if (log.CombatData.GetDeadEvents(br.To).Any(x => Math.Abs(x.Time - br.Time) < 100))
                    {
                        return false;
                    }
                    return true;
                 }),
            ]),
            new MechanicGroup([
                new PlayerSrcPlayerDstBuffApplyMechanic(DhuumShacklesBuff, new MechanicPlotlySetting(Symbols.Diamond,Colors.Teal), "Shackles","Soul Shackle (Tether) application", "Shackles",10000),//  //also used for removal.
                new PlayerDstHitMechanic(DhuumShacklesHit, new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Teal), "Shackles dmg", "Soul Shackle (Tether) dmg ticks","Shackles Dmg", 0)
                    .UsingChecker((de,log) => de.HealthDamage > 0),
            ]),
            new PlayerDstBuffApplyMechanic(Superspeed, new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Grey), "SupSpeed.Orb", "Gained Superspeed from Desmina (Walked over orb)", "Took Superspeed orb", 0)
                .UsingChecker((bae, log) => bae.CreditedBy.IsSpecies(TargetID.DhuumDesmina)),
            new PlayerDstHitMechanic(ConeSlash, new MechanicPlotlySetting(Symbols.TriangleUp,Colors.DarkGreen), "Cone", "Boon ripping Cone Attack","Cone", 0),
            new PlayerDstHitMechanic(CullDamage, new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.Teal), "Crack", "Cull (Fearing Fissures)","Cracks", 0),
            new PlayerDstHitMechanic(PutridBomb, new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "Mark", "Necro Marks during Scythe attack","Necro Marks", 0),
            new PlayerDstHitMechanic(CataclysmicCycle, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Suck dmg", "Damage when sucked to close to middle","Suck dmg", 0),
            new MechanicGroup([
                new PlayerDstHitMechanic(DeathMark, new MechanicPlotlySetting(Symbols.Hexagon,Colors.LightOrange), "Dip", "Lesser Death Mark hit (Dip into ground)","Dip AoE", 0),
                new PlayerDstHitMechanic(GreaterDeathMark, new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "KB dmg", "Knockback damage during Greater Deathmark (mid port)","Knockback dmg", 0),
            ]),
            new PlayerDstHitMechanic(RendingSwipe, new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.LightOrange), "Enf.Swipe", "Hit by Dhuum's Enforcer Rending Swipe", "Rending Swipe Hit", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(FracturedSpirit, new MechanicPlotlySetting(Symbols.Square,Colors.Green), "Orb CD", "Applied when taking green","Green port", 0),
                new PlayerDstBuffApplyMechanic(SourcePureOblivionBuff, new MechanicPlotlySetting(Symbols.HexagonOpen, Colors.Black), "10%", "Lifted by Pure Oblivion", "Pure Oblivion (10%)", 0),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(EchosPickup, new MechanicPlotlySetting(Symbols.Square,Colors.Red), "Echo PU", "Picked up by Ender's Echo","Ender's Pick up", 3000),
                new PlayerDstBuffRemoveMechanic(EchosPickup, new MechanicPlotlySetting(Symbols.Square,Colors.Blue), "F Echo","Freed from Ender's Echo", "Freed from Echo", 0)
                    .UsingChecker( (br,log) => !log.CombatData.GetDeadEvents(br.To).Where(x => Math.Abs(x.Time - br.Time) <= 150).Any()),
            ]),
            new PlayerSrcBuffApplyMechanic(DhuumsMessengerFixationBuff, new MechanicPlotlySetting(Symbols.CircleOpenDot, Colors.Brown), "Mess Fix", "Fixated by Messenger", "Messenger Fixation", 10)
                .UsingChecker((bae, log) =>
                {
                    // Additional buff applications can happen, filtering them out
                    BuffEvent? firstAggroEvent = log.CombatData.GetBuffDataByIDByDst(DhuumsMessengerFixationBuff, bae.To).FirstOrDefault();
                    if (firstAggroEvent != null && bae.Time > firstAggroEvent.Time + ServerDelayConstant && bae.Initial)
                    {
                        return false;
                    }
                    return true;
                }
            ),
        ]));
        Extension = "dhuum";
        Icon = EncounterIconDhuum;
        EncounterCategoryInformation.InSubCategoryOrder = 3;
        EncounterID |= 0x000006;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayDhuum,
                        (1000, 899),
                        (13524, -1334, 18039, 2735)/*,
                        (-21504, -12288, 24576, 12288),
                        (19072, 15484, 20992, 16508)*/);
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(DeathlyAura, DeathlyAura),
            new BuffLossCastFinder(ExpelEnergySAK, ArcingAffliction).UsingChecker((brae, combatData, agentData, skillData) =>
            {
                bool state = true;
                // Buff loss caused by the Greather Death Mark
                if (combatData.GetDamageData(GreaterDeathMark).Any(x => Math.Abs(x.Time - brae.Time) < 100 && x.To == brae.To))
                {
                    state = false;
                }
                // Buff loss at the time of the 10% starting
                if (combatData.GetBuffDataByIDByDst(SourcePureOblivionBuff, brae.To).Any(x => Math.Abs(x.Time - brae.Time) < 100))
                {
                    state = false;
                }
                return state;
            }),
        ];
    }

    //TODO(Rennorb) @perf
    private static void ComputeFightPhases(List<PhaseData> phases, SingleActor dhuum, IEnumerable<CastEvent> castLogs, ParsedEvtcLog log, long fightDuration, long start, PhaseData mainFightPhase)
    {
        CastEvent? shield = castLogs.FirstOrDefault(x => x.SkillId == MajorSoulSplit);
        // Dhuum brought down to 10%
        if (shield != null)
        {
            long end = shield.Time;
            var dhuumFight = new PhaseData(start, end, "Dhuum Fight");
            dhuumFight.AddTarget(dhuum, log);
            dhuumFight.AddParentPhase(mainFightPhase);
            phases.Add(dhuumFight);
            CastEvent? firstDamageable = castLogs.FirstOrDefault(x => x.SkillId == DhuumVulnerableLast10Percent && x.Time >= end);
            // ritual started
            if (firstDamageable != null)
            {
                var shieldPhase = new PhaseData(end, firstDamageable.Time, "Shielded Dhuum");
                shieldPhase.AddParentPhase(mainFightPhase);
                shieldPhase.AddTarget(dhuum, log);
                phases.Add(shieldPhase);
                var ritualPhase = new PhaseData(firstDamageable.Time, fightDuration, "Ritual");
                ritualPhase.AddTarget(dhuum, log);
                ritualPhase.AddParentPhase(mainFightPhase);
                phases.Add(ritualPhase);
            }
            else
            {
                var shieldPhase = new PhaseData(end, fightDuration, "Shielded Dhuum");
                shieldPhase.AddParentPhase(mainFightPhase);
                shieldPhase.AddTarget(dhuum, log);
                phases.Add(shieldPhase);
            }
        }
    }

    private static List<PhaseData> GetInBetweenSoulSplits(ParsedEvtcLog log, SingleActor dhuum, IEnumerable<SingleActor> enforcers, long mainStart, long mainEnd, bool hasRitual, PhaseData parentPhase)
    {
        var cls = dhuum.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
        var cataCycles = cls.Where(x => x.SkillId == CataclysmicCycle);
        var gDeathmarks = cls.Where(x => x.SkillId == GreaterDeathMark).ToList();
        if (gDeathmarks.Count < cataCycles.Count())
        {
            // anomaly, don't do sub phases
            return [];
        }
        var phases = new List<PhaseData>();
        long start = mainStart;
        long end = 0;
        int i = 0;
        foreach (CastEvent cataCycle in cataCycles)
        {
            CastEvent gDeathmark = gDeathmarks[i];
            end = Math.Min(gDeathmark.Time, mainEnd);
            long soulsplitEnd = Math.Min(cataCycle.EndTime, mainEnd);
            ++i;

            var preSoulSplit = new PhaseData(start, end, "Pre-Soulsplit " + i).WithParentPhase(parentPhase);
            preSoulSplit.AddTarget(dhuum, log);
            preSoulSplit.AddTargets(enforcers, log, PhaseData.TargetPriority.NonBlocking);
            phases.Add(preSoulSplit);

            var soulSplit = new PhaseData(end, soulsplitEnd, "Soulsplit " + i).WithParentPhase(parentPhase);
            soulSplit.AddTarget(dhuum, log);
            phases.Add(soulSplit);
            start = cataCycle.EndTime;
        }
        var final = new PhaseData(start, mainEnd, hasRitual ? "Pre-Ritual" : "Pre-Wipe").WithParentPhase(parentPhase);
        final.AddTarget(dhuum, log);
        phases.Add(final);
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        long fightDuration = log.FightData.FightEnd;
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor dhuum = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Dhuum)) ?? throw new MissingKeyActorsException("Dhuum not found");
        phases[0].AddTarget(dhuum, log);
        if (!requirePhases)
        {
            return phases;
        }
        var enforcers = Targets.Where(x => x.IsSpecies(TargetID.Enforcer));
        // Sometimes the pre event is not in the evtc
        var castLogs = dhuum.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
        // present if not bugged and pre-event done
        PhaseData? mainFight = null;
        if (!_hasPrevent)
        {
            // full fight does not contain the pre event
            ComputeFightPhases(phases, dhuum, castLogs, log, fightDuration, 0, phases[0]);
        }
        else
        {
            // full fight contains the pre event
            BuffEvent? invulDhuum = log.CombatData.GetBuffDataByIDByDst(Determined762, dhuum.AgentItem).FirstOrDefault(x => x is BuffRemoveAllEvent && x.Time > 115000);
            // pre event done
            if (invulDhuum != null)
            {
                long end = invulDhuum.Time;
                var preEvent = new PhaseData(0, end, "Pre Event").WithParentPhase(phases[0]);
                preEvent.AddTarget(dhuum, log);
                preEvent.AddTargets(enforcers, log, PhaseData.TargetPriority.NonBlocking);
                phases.Add(preEvent);

                mainFight = new PhaseData(end, fightDuration, "Main Fight");
                mainFight.AddTarget(dhuum, log);
                phases.Add(mainFight.WithParentPhase(phases[0]));
                ComputeFightPhases(phases, dhuum, castLogs, log, fightDuration, end, mainFight);
            }
        }
        bool hasRitual = phases.Last().Name == "Ritual";
        // if present, Dhuum was at least at 10%
        PhaseData? dhuumFight = phases.Find(x => x.Name == "Dhuum Fight");
        if (mainFight != null)
        {
            var parentPhase = dhuumFight ?? mainFight;
            // from pre event end to 10% or fight end if 10% not achieved
            phases.AddRange(GetInBetweenSoulSplits(log, dhuum, enforcers, mainFight.Start, parentPhase.End, hasRitual, parentPhase));
        }
        else if (!_hasPrevent)
        {
            var parentPhase = dhuumFight ?? phases[0];
            // from start to 10% or fight end if 10% not achieved
            phases.AddRange(GetInBetweenSoulSplits(log, dhuum, enforcers, 0, parentPhase.End, hasRitual, parentPhase));
        }
        return phases;
    }

    protected override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.Dhuum,
            TargetID.Echo,
            TargetID.Enforcer,
            TargetID.UnderworldReaper,
        ];
    }

    protected override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.Messenger,
            TargetID.Deathling,
            TargetID.DhuumDesmina
        ];
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        long startToUse = GetGenericFightOffset(fightData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            AgentItem messenger = agentData.GetNPCsByID(TargetID.Messenger).MinBy(x => x.FirstAware);
            if (messenger != null)
            {
                startToUse = messenger.FirstAware;
            }
        }
        return startToUse;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (!agentData.TryGetFirstAgentItem(TargetID.Dhuum, out var dhuum))
        {
            throw new MissingKeyActorsException("Dhuum not found");
        }
        _hasPrevent = !combatData.Any(x => x.SrcMatchesAgent(dhuum) && x.EndCasting() && (x.SkillID != WeaponStow && x.SkillID != WeaponDraw) && x.Time >= 0 && x.Time <= 40000);

        // Player Souls - Filter out souls without master
        var yourSoul = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 14940 && x.IsStateChange == StateChange.MaxHealthUpdate)
            .Select(x => agentData.GetAgent(x.SrcAgent, x.Time))
            .Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxHeight == 120 && x.HitboxWidth == 100);
        var dhuumPlayerToSoulTrackBuffApplications = combatData.Where(x => x.IsBuffApply() && x.SkillID == DhuumPlayerToSoulTrackBuff)
            .Select(x => (agentData.GetAgent(x.SrcAgent, x.Time), agentData.GetAgent(x.DstAgent, x.Time)))
            .Where(x => x.Item1.IsPlayer)
            .GroupBy(x => x.Item2)
            .ToDictionary(x => x.Key, x => x.Select(y => y.Item1).FirstOrDefault());
        foreach (AgentItem soul in yourSoul)
        {
            if (dhuumPlayerToSoulTrackBuffApplications.TryGetValue(soul, out var firstApplier) && firstApplier != null)
            {
                soul.OverrideType(AgentItem.AgentType.NPC, agentData);
                soul.OverrideID(TargetID.YourSoul, agentData);
                if (soul.GetFinalMaster() != firstApplier)
                {
                    soul.SetMaster(firstApplier);
                }
            }
        }

        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);

        // Adding counting number to the Enforcers
        int i = 1;
        foreach (var enforcer in Targets.Where(x => x.IsSpecies(TargetID.Enforcer)))
        {
            enforcer.OverrideName(enforcer.Character + " " + i);
            i++;
        }
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        // We expect pre event in all logs
        if (!_hasPrevent)
        {
            return FightData.EncounterStartStatus.NoPreEvent;
        }
        else
        {
            return base.GetEncounterStartStatus(combatData, agentData, fightData);
        }
    }

    private static readonly List<(Vector3 Position, int Index)> ReapersToGreen = new()
    {
        { (new(16897, 1225, -6215), 0) },
        { (new(16853, 65, -6215), 1) },
        { (new(15935, -614, -6215), 2) },
        { (new(14830, -294, -6215), 3) },
        { (new(14408, 764, -6215), 4) },
        { (new(14929, 1762, -6215), 5) },
        { (new(16062, 1991, -6215), 6) },
    };

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        (long start, long end) lifespan = (replay.TimeOffsets.start, replay.TimeOffsets.end);

        switch (target.ID)
        {
            case (int)TargetID.Dhuum:
            {
                var casts = target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).ToList();

                foreach (CastEvent cast in casts)
                {
                    switch (cast.SkillId)
                    {
                        // Cataclysmic Cycle - Suction during Major Soul Split
                        case CataclysmicCycle:
                            lifespan = (cast.Time, cast.EndTime);
                            replay.Decorations.Add(new CircleDecoration(300, lifespan, Colors.LightOrange, 0.5, new AgentConnector(target)));
                            replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, lifespan, Colors.Orange, 0.9, Colors.Black, 0.2, [(lifespan.start, 0), (lifespan.end, 100)], new AgentConnector(target))
                            .UsingRotationConnector(new AngleConnector(180)));
                            break;
                        // Cone Slash
                        case ConeSlash:
                            // Using new effects method for logs that contain them
                            if (!log.CombatData.HasEffectData)
                            {
                                lifespan = (cast.Time, cast.EndTime);
                                // Get Dhuum's rotation with 200 ms delay and a 200ms forward time window.
                                if (target.TryGetCurrentFacingDirection(log, lifespan.start + 200, out var facing, 200))
                                {
                                    replay.Decorations.Add(new PieDecoration(850, 60, lifespan, Colors.LightOrange, 0.5, new AgentConnector(target)).UsingRotationConnector(new AngleConnector(facing)));
                                }
                            }
                            else
                            {
                                if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.DhuumConeSlash, out var coneSlashes))
                                {
                                    foreach (EffectEvent effect in coneSlashes)
                                    {
                                        int castDuration = 3250;
                                        int expectedEndCastTime = (int)effect.Time + castDuration;

                                        // Find if Dhuum has stolen quickness
                                        double actualDuration = ComputeCastTimeWithQuickness(log, target, effect.Time, castDuration);

                                        // Dhuum can interrupt his own cast with other skills and the effect duration logged of 10000 isn't correct.
                                        lifespan = effect.ComputeDynamicLifespan(log, castDuration);
                                        (long, long) supposedLifespan = (effect.Time, effect.Time + castDuration);

                                        // If Dhuum has stolen quickness, find the minimum cast duration
                                        if (actualDuration > 0)
                                        {
                                            supposedLifespan.Item2 = effect.Time + Math.Min(castDuration, (long)Math.Ceiling(actualDuration));
                                        }

                                        var position = new PositionConnector(effect.Position);
                                        var rotation = new AngleConnector(effect.Rotation.Z + 90);

                                        var coneDec = (PieDecoration)new PieDecoration(850, 60, lifespan, Colors.Orange, 0.2, position).UsingRotationConnector(rotation);
                                        var coneGrowing = (PieDecoration)new PieDecoration(850, 60, lifespan, Colors.Orange, 0.2, position).UsingGrowingEnd(supposedLifespan.Item2).UsingRotationConnector(rotation);
                                        replay.Decorations.Add(coneDec);
                                        replay.Decorations.Add(coneGrowing);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                var deathmark = casts.Where(x => x.SkillId == DeathMark);
                CastEvent? majorSplit = casts.FirstOrDefault(x => x.SkillId == MajorSoulSplit);
                // Using new effects method for logs that contain them
                if (!log.CombatData.HasEffectData)
                {
                    foreach (CastEvent cast in deathmark)
                    {
                        long start = cast.Time;
                        long defaultCastDuration = 1550;
                        long castDuration = 0;

                        // Compute cast time of the Death Mark with Quickness
                        double computedDuration = ComputeCastTimeWithQuickness(log, target, start, defaultCastDuration);
                        if (computedDuration > 0)
                        {
                            castDuration = Math.Min(defaultCastDuration, (long)Math.Ceiling(computedDuration));
                        }

                        long zoneActive = start + castDuration; // When the Death Mark hits (Soul Split and spawns the AoE)
                        long zoneDeadly = zoneActive + 6000; // Point where the zone becomes impossible to walk through unscathed
                        long zoneEnd = zoneActive + 120000; // End of the AoE
                        uint radius = 450;

                        if (majorSplit != null)
                        {
                            zoneEnd = Math.Min(zoneEnd, majorSplit.Time);
                            zoneDeadly = Math.Min(zoneDeadly, majorSplit.Time);
                        }
                        int spellCenterDistance = 200; //hitbox radius
                        if (target.TryGetCurrentFacingDirection(log, start + castDuration, out var facing)
                            && target.TryGetCurrentPosition(log, start + castDuration, out var targetPosition))
                        {
                            var position = new Vector3(
                                targetPosition.X + (facing.X * spellCenterDistance),
                                targetPosition.Y + (facing.Y * spellCenterDistance),
                                targetPosition.Z
                            );
                            var positionConnector = new PositionConnector(position);

                            (long, long) lifespanWarning = (start, zoneActive);
                            (long, long) lifespanActivation = (zoneActive, zoneDeadly);
                            (long, long) lifespanDeadly = (zoneDeadly, zoneEnd);

                            // Warning
                            var circleOrange = new CircleDecoration(radius, lifespanWarning, Colors.Orange, 0.2, positionConnector);
                            var circleRed = new CircleDecoration(radius, lifespanWarning, Colors.Red, 0.4, positionConnector);
                            replay.Decorations.Add(circleOrange);
                            replay.Decorations.Add(circleRed.UsingGrowingEnd(lifespanWarning.Item2));

                            // Activation
                            var greenCircle = new CircleDecoration(radius, lifespanActivation, "rgba(200, 255, 100, 0.5)", positionConnector);
                            replay.Decorations.Add(greenCircle);
                            replay.Decorations.Add(greenCircle.Copy().UsingGrowingEnd(lifespanActivation.Item2));

                            // Deadly
                            var redCircle = new CircleDecoration(radius, lifespanDeadly, Colors.Red, 0.4, positionConnector);
                            replay.Decorations.Add(redCircle);
                        }
                    }
                }
                if (majorSplit != null)
                {
                    lifespan = (majorSplit.Time, log.FightData.FightEnd);
                    replay.Decorations.Add(new CircleDecoration(320, lifespan, "rgba(0, 180, 255, 0.2)", new AgentConnector(target)));
                }

                // Scythe Swing - AoEs
                var scytheSwing = casts.Where(x => x.SkillId == ScytheSwing).ToList();
                for (int i = 0; i < scytheSwing.Count; i++)
                {
                    var nextSwing = i < scytheSwing.Count - 1 ? scytheSwing[i + 1].Time : log.FightData.FightEnd;

                    // AoE Indicator
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.DhuumScytheSwingIndicator, out var scytheSwingIndicators))
                    {
                        uint radius = 45;
                        uint radiusIncrease = 5;
                        foreach (EffectEvent indicator in scytheSwingIndicators.Where(x => x.Time >= scytheSwing[i].Time && x.Time < nextSwing))
                        {
                            // Computing lifespan through secondary effect and position.
                            lifespan = indicator.ComputeLifespanWithSecondaryEffectAndPosition(log, EffectGUIDs.DhuumScytheSwingDamage);
                            var circle = new CircleDecoration(radius, lifespan, Colors.Orange, 0.2, new PositionConnector(indicator.Position));
                            replay.Decorations.Add(circle);
                            radius += radiusIncrease;
                        }
                    }

                    // Brief damage indicator
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.DhuumScytheSwingDamage, out var scytheSwingDamage))
                    {
                        uint radius = 45;
                        uint radiusIncrease = 5;
                        foreach (EffectEvent damage in scytheSwingDamage.Where(x => x.Time >= scytheSwing[i].Time && x.Time < nextSwing))
                        {
                            // The effect has 0 duration, setting it to 250
                            lifespan = (damage.Time, damage.Time + 250);
                            var circle = new CircleDecoration(radius, lifespan, "rgba(97, 104, 51, 0.5)", new PositionConnector(damage.Position));
                            replay.Decorations.Add(circle);
                            radius += radiusIncrease;
                        }
                    }
                }
            }

            // Collection Orbs
            var orbs = log.CombatData.GetMissileEventsBySkillIDs([DhuumEnforcerOrb, DhuumMessengerOrb, DhuumSpiderOrb, DhuumCollectableSmallOrb]);
            foreach (MissileEvent orb in orbs)
            {
                uint radius = 0;
                Color color = Colors.Grey;

                switch (orb.SkillID)
                {
                    case DhuumEnforcerOrb:
                        radius = 50;
                        color = Colors.LightRed;
                        break;
                    case DhuumMessengerOrb:
                        radius = 35;
                        color = Colors.Purple;
                        break;
                    case DhuumSpiderOrb:
                        radius = 20;
                        color = Colors.Pink;
                        break;
                    case DhuumCollectableSmallOrb:
                        radius = 10;
                        color = Colors.Grey;
                        break;
                    default:
                        break;
                }
                replay.Decorations.AddNonHomingMissile(log, orb, color, 0.5, radius);
            }
            break;
            case (int)TargetID.DhuumDesmina:
                break;
            case (int)TargetID.Echo:
                replay.Decorations.Add(new CircleDecoration(120, lifespan, Colors.Red, 0.5, new AgentConnector(target)));
                break;
            case (int)TargetID.Enforcer:
            {
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillId)
                    {
                        case RendingSwipe:
                            long castDuration = 667;
                            lifespan = (cast.Time, cast.Time + castDuration);
                            if (target.TryGetCurrentFacingDirection(log, cast.Time, out var facing, 200))
                            {
                                var agentConnector = new AgentConnector(target);
                                var rotationConnector = new AngleConnector(facing);
                                var cone = (PieDecoration)new PieDecoration(40, 90, lifespan, Colors.Orange, 0.2, agentConnector).UsingRotationConnector(rotationConnector);
                                replay.Decorations.AddWithFilledWithGrowing(cone, true, lifespan.end);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            break;
            case (int)TargetID.Messenger:
                replay.Decorations.Add(new CircleDecoration(180, lifespan, Colors.Orange, 0.5, new AgentConnector(target)));
                // Fixation tether to player
                var fixations = GetFilteredList(log.CombatData, DhuumsMessengerFixationBuff, target, true, true);
                replay.Decorations.AddTether(fixations, Colors.Red, 0.4);
                break;
            case (int)TargetID.Deathling:
                break;
            case (int)TargetID.UnderworldReaper:
                var stealths = target.GetBuffStatus(log, Stealth, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
                replay.Decorations.AddOverheadIcons(stealths, target, BuffImages.Stealth);
                var underworldReaperHPs = target.GetHealthUpdates(log);
                replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMinorSizeInPixel, lifespan, Colors.Green, 0.6, Colors.Black, 0.2, underworldReaperHPs.Select(x => (x.Start, x.Value)).ToList(), new AgentConnector(target))
                    .UsingInterpolationMethod(Connector.InterpolationMethod.Step)
                    .UsingRotationConnector(new AngleConnector(180)));
                if (_hasPrevent)
                {
                    if (_greenStart == 0)
                    {
                        BuffEvent? greenTaken = log.CombatData.GetBuffData(FracturedSpirit).Where(x => x is BuffApplyEvent).FirstOrDefault();
                        if (greenTaken != null)
                        {
                            _greenStart = (int)greenTaken.Time - 5000;
                        }
                        else
                        {
                            _greenStart = 30600;
                        }
                    }

                    ParametricPoint3D pos;
                    if (replay.Positions.Count > 0)
                    {
                        pos = replay.Positions[0];

                        if (pos.XYZ.X < 14000)
                        {
                            // Outside reaper
                            replay.Trim(target.FirstAware, target.FirstAware + CombatReplayPollingRate);
                        }
                        else
                        {
                            if (replay.Positions.Count > 1)
                            {
                                replay.Trim(replay.Positions.LastOrDefault().Time, replay.TimeOffsets.end);
                            }
                        }
                    }
                    else
                    {
                        break;
                    }

                    int reaperIndex = -1;
                    foreach (var reaper in ReapersToGreen)
                    {
                        if ((reaper.Position - pos.XYZ).Length() < 10)
                        {
                            reaperIndex = reaper.Index;
                            break;
                        }
                    }

                    if (reaperIndex == -1)
                    {
                        break;
                    }

                    int multiplier = 210000;
                    int gStart = _greenStart + reaperIndex * 30000;
                    var greens = new List<int>() {
                        gStart,
                        gStart + multiplier,
                        gStart + 2 * multiplier
                    };

                    foreach (int gstart in greens)
                    {
                        int gend = gstart + 5000;
                        var greenCircle = new CircleDecoration(240, (gstart, gend), Colors.DarkGreen, 0.4, new AgentConnector(target));
                        replay.Decorations.AddWithGrowing(greenCircle, gend);
                    }
                }
                break;
            default:
                break;
        }

    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);

        (long start, long end) lifespan;

        // spirit transform
        var spiritTransform = log.CombatData.GetBuffApplyDataByIDByDst(FracturedSpirit, p.AgentItem);
        foreach (BuffEvent c in spiritTransform)
        {
            int duration = 15000;
            if (p.HasBuff(log, SourcePureOblivionBuff, c.Time + ServerDelayConstant))
            {
                duration = 30000;
            }
            BuffEvent? removedBuff = log.CombatData.GetBuffRemoveAllData(MortalCoilDhuum).FirstOrDefault(x => x.To == p.AgentItem && x.Time > c.Time && x.Time < c.Time + duration);
            lifespan = (c.Time, c.Time + duration);
            if (removedBuff != null)
            {
                lifespan.end = removedBuff.Time;
            }

            // Progress Bar
            replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMinorSizeInPixel, lifespan, Colors.CobaltBlue, 0.6, Colors.Black, 0.2, [(lifespan.start, 0), (lifespan.start + duration, 100)], new AgentConnector(p))
                .UsingRotationConnector(new AngleConnector(130)));

            // Overhead Icon
            replay.Decorations.AddRotatedOverheadIcon(new Segment(lifespan, 1), p, ParserIcons.GenericGreenArrowUp, 40f);
        }
        // bomb
        var bombDhuum = p.GetBuffStatus(log, ArcingAffliction, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        foreach (Segment seg in bombDhuum)
        {
            replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMinorSizeInPixel, seg.TimeSpan, Colors.Orange, 0.6, Colors.Black, 0.2, [(seg.Start, 0), (seg.Start + 13000, 100)], new AgentConnector(p))
                .UsingRotationConnector(new AngleConnector(-130)));
            replay.Decorations.AddRotatedOverheadIcon(seg, p, ParserIcons.BombTimerFullOverhead, -40f);
        }
        // shackles connection
        var shackles = GetFilteredList(log.CombatData, [DhuumShacklesBuff, DhuumShacklesBuff2], p, true, true);
        replay.Decorations.AddTether(shackles, Colors.Teal, 0.5);

        // shackles damage (identical to the connection for now, not yet properly distinguishable from the pure connection, further investigation needed due to inconsistent behavior (triggering too early, not triggering the damaging skill though)
        // shackles start with buff 47335 applied from one player to the other, this is switched over to buff 48591 after mostly 2 seconds, sometimes later. This is switched to 48042 usually 4 seconds after initial application and the damaging skill 47164 starts to deal damage from that point on.
        // Before that point, 47164 is only logged when evaded/blocked, but doesn't deal damage. Further investigation needed.
        var shacklesDmg = GetFilteredList(log.CombatData, DhuumDamagingShacklesBuff, p, true, true);
        replay.Decorations.AddTether(shacklesDmg, Colors.Yellow, 0.5);

        // Soul split
        var hastenedDemise = p.GetBuffStatus(log, HastenedDemise, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value == 1);
        var souls = log.AgentData.GetNPCsByID(TargetID.YourSoul).Where(x => x.GetFinalMaster() == p.AgentItem);
        foreach (AgentItem soul in souls)
        {
            Segment? curHastenedDemise = hastenedDemise.FirstOrNull((in Segment x) => x.Start >= soul.FirstAware - 100);
            if (curHastenedDemise != null && soul.TryGetCurrentPosition(log, soul.FirstAware, out var soulPosition, 1000))
            {
                AddSoulSplitDecorations(p, replay, soul, curHastenedDemise.Value, soulPosition);
            }
        }
        // show the death trigger even if we don't have the souls
        foreach (var seg in hastenedDemise)
        {
            long soulSplitDeathTime = seg.Start + 10000;
            replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMinorSizeInPixel, seg.TimeSpan, Colors.Red, 0.6, Colors.Black, 0.2, [(seg.Start, 0), (soulSplitDeathTime, 100)], new AgentConnector(p))
                .UsingRotationConnector(new AngleConnector(90)));
        }
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log);

        // Death Mark - First Warning (2 seconds)
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.DhuumDeathMarkFirstIndicator, out var deathMarkFirstIndicators))
        {
            foreach (EffectEvent effect in deathMarkFirstIndicators)
            {
                (long, long) lifespan = effect.ComputeLifespanWithSecondaryEffect(log, EffectGUIDs.DhuumDeathMarkSecondIndicator);
                var connector = new PositionConnector(effect.Position);
                var circleOrange = new CircleDecoration(450, lifespan, Colors.Orange, 0.2, connector);
                var circleRed = new CircleDecoration(450, lifespan, Colors.Red, 0.4, connector);
                EnvironmentDecorations.Add(circleOrange);
                EnvironmentDecorations.Add(circleRed.UsingGrowingEnd(lifespan.Item2));
            }
        }

        // Death Mark - Death Zone
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.DhuumDeathMarkDeathZone, out var deathMarkDeathZones))
        {
            foreach (EffectEvent effect in deathMarkDeathZones)
            {
                int warningDuration = 6000;
                uint radius = 450;
                (long, long) lifespan = effect.ComputeLifespan(log, 120000);
                (long, long) lifespanActivation = (lifespan.Item1, lifespan.Item1 + warningDuration);
                (long, long) lifespanDeadly = (lifespan.Item1 + warningDuration, lifespan.Item2);

                var connector = new PositionConnector(effect.Position);

                // Green indicator for the safe zone - Activation
                var greenCircle = new CircleDecoration(radius, lifespanActivation, "rgba(200, 255, 100, 0.5)", connector);
                EnvironmentDecorations.Add(greenCircle);
                EnvironmentDecorations.Add(greenCircle.Copy().UsingGrowingEnd(lifespanActivation.Item2));
                // Damage zone
                var redCircle = new CircleDecoration(radius, lifespanDeadly, Colors.Red, 0.4, connector);
                EnvironmentDecorations.Add(redCircle);
            }
        }

        // Cull - Circle orange AoE indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.DhuumCullAoEIndicator, out var cullingAoEs))
        {
            foreach (EffectEvent effect in cullingAoEs)
            {
                // Effect duration is 0, we get the effect start time of the cracks
                (long, long) lifespan = effect.ComputeLifespanWithSecondaryEffectAndPosition(log, EffectGUIDs.DhuumCullCracksIndicator);
                var connector = new PositionConnector(effect.Position);
                var greenCircle = new CircleDecoration(300, lifespan, Colors.Orange, 0.2, connector);
                EnvironmentDecorations.Add(greenCircle);
                EnvironmentDecorations.Add(greenCircle.Copy().UsingGrowingEnd(lifespan.Item2));
            }
        }

        // Cull - Black cracks spawning indicator
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.DhuumCullCracksIndicator, out var cullingCracksIndicators))
        {
            foreach (EffectEvent effect in cullingCracksIndicators)
            {
                (long, long) lifespan = (effect.Time, effect.Time + effect.Duration);
                var connector = (PositionConnector)new PositionConnector(effect.Position).WithOffset(new(230 / 2, 0, 0), true);
                var rotationConnector = new AngleConnector(effect.Rotation.Z - 90);
                var rectangle = (RectangleDecoration)new RectangleDecoration(220, 40, lifespan, Colors.Black, 0.3, connector).UsingRotationConnector(rotationConnector);
                EnvironmentDecorations.Add(rectangle);
            }
        }

        // Cull - Cracks explosion
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.DhuumCullCracksDamage, out var cullingCracksDamage))
        {
            foreach (EffectEvent effect in cullingCracksDamage)
            {
                // Effect duration is 0, using it as a wind-up to the hit by 500ms
                (long, long) lifespan = (effect.Time - 500, effect.Time);
                var connector = (PositionConnector)new PositionConnector(effect.Position).WithOffset(new(230 / 2, 0, 0), true);
                var rotationConnector = new AngleConnector(effect.Rotation.Z - 90);
                var rectangle = (RectangleDecoration)new RectangleDecoration(220, 40, lifespan, "rgba(173, 255, 225, 0.4)", connector).UsingRotationConnector(rotationConnector);
                EnvironmentDecorations.Add(rectangle);
                EnvironmentDecorations.Add(rectangle.Copy().UsingGrowingEnd(effect.Time));
            }
        }

        // Superspeed Orbs
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.DhuumSuperspeedOrb, out var superspeedOrbs))
        {
            foreach (EffectEvent effect in superspeedOrbs)
            {
                (long, long) lifespan = effect.ComputeDynamicLifespan(log, long.MaxValue);
                var position = new PositionConnector(effect.Position);
                var circle = (CircleDecoration)new CircleDecoration(50, lifespan, Colors.White, 0.5, position).UsingFilled(false);
                var centralDot = new CircleDecoration(20, lifespan, "rgba(203, 195, 227, 0.5)", position);
                EnvironmentDecorations.Add(circle);
                EnvironmentDecorations.Add(centralDot);
            }
        }
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Dhuum)) ?? throw new MissingKeyActorsException("Dhuum not found");
        return (target.GetHealth(combatData) > 35e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }

    /// <summary>
    /// Adds the Soul Split decorations.
    /// </summary>
    /// <param name="p">The player.</param>
    /// <param name="replay">The Combat Replay.</param>
    /// <param name="soul">The Soul to tether to the player.</param>
    /// <param name="hastenedDemise">Time frame during which the hastened demise buff is present.</param>
    /// <param name="soulPosition">The position of the Soul.</param>
    private static void AddSoulSplitDecorations(PlayerActor p, CombatReplay replay, AgentItem soul, in Segment hastenedDemise, in Vector3 soulPosition)
    {
        (long, long) soulLifespan = (soul.FirstAware, hastenedDemise.End);

        uint radius = (soul.HitboxWidth / 2);
        var soulConnector = new PositionConnector(soulPosition);
        var playerConnector = new AgentConnector(p);

        // Soul outer circle
        var hitbox = (CircleDecoration)new CircleDecoration(radius, radius - 25, soulLifespan, Colors.White, 0.8, soulConnector).UsingFilled(false);
        // Soul tether to player
        var line = new LineDecoration(soulLifespan, Colors.White, 0.8, soulConnector, playerConnector);
        // Soul icon
        var icon = new IconDecoration(ParserIcons.DhuumPlayerSoul, 16, 1, soulLifespan, soulConnector);

        replay.Decorations.Add(hitbox);
        replay.Decorations.Add(line);
        replay.Decorations.Add(icon);
    }
}
