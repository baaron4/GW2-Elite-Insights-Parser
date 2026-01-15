using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class Matthias : SalvationPass
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([


            new PlayerDstHealthDamageHitMechanic([OppressiveGazeHuman, OppressiveGazeAbomination], new MechanicPlotlySetting(Symbols.Hexagram,Colors.Red), "Hadouken", "Oppressive Gaze (Hadouken projectile)","Hadouken", 0),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([BloodShardsHuman, BloodShardsAbomination], new MechanicPlotlySetting(Symbols.DiamondWideOpen,Colors.Magenta), "Shoot Shards", "Blood Shard projectiles during bubble","Rapid Fire", 0),
                new PlayerSrcHealthDamageHitMechanic([BloodShardsHuman, BloodShardsAbomination], new MechanicPlotlySetting(Symbols.DiamondWideOpen,Colors.Green), "Refl.Shards", "Blood Shard projectiles reflected during bubble","Reflected Rapid Fire", 0),
                new EnemySrcMissileMechanic([BloodShardsHuman, BloodShardsAbomination], new MechanicPlotlySetting(Symbols.DiamondWideOpen,Colors.Red), "N.Refl.Shards", "Blood Shard projectiles not reflected during bubble","Not reflected Rapid Fire", 0)
                    .UsingNotReflected(),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic([ShardsOfRageHuman, ShardsOfRageAbomination], new MechanicPlotlySetting(Symbols.StarDiamond,Colors.Red), "Jump Shards", "Shards of Rage (Jump)","Jump Shards", 1000),
                new PlayerSrcHealthDamageHitMechanic([ShardsOfRageHuman, ShardsOfRageAbomination], new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.Red), "Refl.Jump Shards", "Reflected Shards of Rage (Jump)","Reflected Jump Shards", 0)
                    .WithMinions(),
            ]),
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(FieryVortex, new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Yellow), "Tornado", "Fiery Vortex (Tornado Matthias)","Tornado (Matthias)", 250),
                new PlayerDstBuffApplyMechanic(Slow, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Blue), "Icy KD", "Knockdown by Icy Patch","Icy Patch KD", 0)
                    .UsingBuffChecker(Stability, false)
                    .UsingChecker((br,log) => br.AppliedDuration == 10000),
                new PlayerDstHealthDamageHitMechanic(Thunder, new MechanicPlotlySetting(Symbols.TriangleUpOpen,Colors.Teal), "Storm", "Thunder Storm hit (air phase)","Storm cloud", 0),
                new PlayerDstBuffRemoveMechanic(Unbalanced, new MechanicPlotlySetting(Symbols.Square,Colors.LightPurple), "KD","Unbalanced (triggered Storm phase Debuff)", "Knockdown",0)
                    .UsingBuffChecker(Stability, false)
                    .UsingChecker((br,log) => br.RemovedDuration > 0),
                new PlayerDstHealthDamageHitMechanic(Surrender, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Black), "Spirit", "Surrender (hit by walking Spirit)","Spirit hit", 0)
            ]),
            new PlayerDstBuffApplyMechanic(UnstableBloodMagic, new MechanicPlotlySetting(Symbols.Diamond,Colors.Red), "Well", "Unstable Blood Magic application","Well", 0),
            new PlayerDstHealthDamageHitMechanic(WellOfTheProfane, new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Red), "Well dmg", "Unstable Blood Magic AoE hit","Stood in Well", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(Corruption1, new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Corruption", "Corruption Application","Corruption", 0),
                new PlayerDstHealthDamageHitMechanic(Corruption2, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Corr. dmg", "Hit by Corruption AoE","Corruption dmg", 0),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(MatthiasSacrifice, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "Sacrifice", "Sacrifice (Breakbar)","Sacrifice", 0),
                new PlayerDstBuffRemoveMechanic(MatthiasSacrifice, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CC.End","Sacrifice (Breakbar) ended", "Sacrifice End",0)
                    .UsingChecker((br,log) => br.RemovedDuration > 25 && !log.CombatData.GetDeadEvents(br.To).Any(x => Math.Abs(br.Time - x.Time) < ServerDelayConstant)),
                new PlayerDstBuffRemoveMechanic(MatthiasSacrifice, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC.Fail","Sacrifice time ran out", "Sacrificed",0)
                    .UsingChecker( (br,log) => br.RemovedDuration <= 25 || log.CombatData.GetDeadEvents(br.To).Any(x => Math.Abs(br.Time - x.Time) < ServerDelayConstant)),
            ]),
            //new Mechanic(Unbalanced, "Unbalanced", Mechanic.MechType.PlayerOnPlayer, ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting(Symbols.Square,"rgb(0,140,0)"), "KD","Unbalanced (triggered Storm phase Debuff) only on successful interrupt", "Knockdown (interrupt)",0,(condition => condition.getCombatItem().Result == ParseEnum.Result.Interrupt)),
            //new Mechanic(Unbalanced, "Unbalanced", ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting(Symbols.Square,"rgb(0,140,0)"), "KD","Unbalanced (triggered Storm phase Debuff) only on successful interrupt", "Knockdown (interrupt)",0,(condition => condition.getDLog().GetResult() == ParseEnum.Result.Interrupt)),
            //new Mechanic(BloodFueled, "Blood Fueled", ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting(Symbols.Square,Color.Red), "Ate Reflects(good)",0),//human //Applied at the same time as Backflip Shards since it is the buff applied by them, can be omitted imho
            //new Mechanic(BloodFueledAbo, "Blood Fueled", ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting(Symbols.Square,Color.Red), "Ate Reflects(good)",0),//abom
            new MechanicGroup([
                new EnemyDstBuffApplyMechanic([BloodShield, BloodShieldAbo], new MechanicPlotlySetting(Symbols.Octagon,Colors.Red), "Bubble", "Blood Shield (protective bubble)","Bubble", 100)
                    .UsingChecker((ba, log) => !ba.To.HasBuff(log, BloodShield, ba.Time - 100) && !ba.To.HasBuff(log, BloodShieldAbo, ba.Time - 100)),
                new EnemyDstBuffRemoveMechanic([BloodShield, BloodShieldAbo], new MechanicPlotlySetting(Symbols.Octagon,Colors.Green), "Lost Bubble", "Lost Blood Shield (protective bubble)","Lost Bubble", 100),
                new PlayerSrcBuffRemoveSingleFromMechanic([BloodShield, BloodShieldAbo], new MechanicPlotlySetting(Symbols.Octagon,Colors.Blue), "Rmv.Sh.Stck","Removed Blood Shield (protective bubble) Stack", "Removed Bubble Stack"),
            ]),
            new PlayerDstBuffApplyMechanic(ZealousBenediction, new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Bombs", "Zealous Benediction (Expanding bombs)","Bomb",0),
        ]);
    public Matthias(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "matt";
        Icon = EncounterIconMatthias;
        LogCategoryInformation.InSubCategoryOrder = 2;
        LogID |= 0x000003;
        ChestID = ChestID.MatthiasChest;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations)
    {
        var crMap = new CombatReplayMap(
                        (880, 880),
                        (-7248, 4585, -4625, 7207));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayMatthias, crMap);
        return crMap;
    }

    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
        IReadOnlyList<BuffEvent> bloodstoneBisque = log.CombatData.GetBuffData(BloodstoneBisque);
        if (bloodstoneBisque.Any())
        {
            var encounterPhases = log.LogData.GetPhases(log).OfType<EncounterPhaseData>().Where(x => x.LogID == LogID);
            foreach (var encounterPhase in encounterPhases)
            {
                if (encounterPhase.Success)
                {
                    int playersWithBisque = 0;
                    int expectedPlayersForSuccess = 0;
                    long encounterEnd = encounterPhase.End - ServerDelayConstant;
                    long encounterStart = encounterPhase.Start + ServerDelayConstant;
                    foreach (Player p in log.PlayerList)
                    {
                        if (p.InAwareTimes(encounterPhase.Start, encounterPhase.End))
                        {
                            IReadOnlyDictionary<long, BuffGraph> graphs = p.GetBuffGraphs(log);
                            if (graphs.TryGetValue(BloodstoneBisque, out var graph))
                            {
                                if (!graph.Values.Any(x => x.Value == 0 && x.Intersects(encounterStart, encounterEnd)))
                                {
                                    playersWithBisque++;
                                }
                            }
                            var (_, _, dcs, _) = p.GetStatus(log);
                            if (!dcs.Any(x => x.ContainsPoint(encounterEnd)))
                            {
                                expectedPlayersForSuccess++;
                            }
                        }
                    }
                    if (expectedPlayersForSuccess <= playersWithBisque)
                    {
                        instanceBuffs.Add(new(log.Buffs.BuffsByIDs[BloodstoneBisque], 1, encounterPhase));
                    }
                }
            }
        }
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(SpontaneousCombustion, SpontaneousCombustion),
            new DamageCastFinder(SnowstormSkill, SnowstormSkill),
            new DamageCastFinder(DownpourSkill, DownpourSkill),
            new BuffLossCastFinder(UnstableBloodMagic, UnstableBloodMagic),
        ];
    }

    internal static List<PhaseData> ComputePhases(ParsedEvtcLog log, SingleActor matthias, IEnumerable<SingleActor> sacrifices, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var encounterStart = encounterPhase.Start;
        var encounterEnd = encounterPhase.End;
        var phases = new List<PhaseData>(4);
        // Special buff cast check
        BuffEvent? heatWave = log.CombatData.GetBuffData(HeatWaveMatthias).FirstOrDefault();
        if (heatWave != null)
        {
            phases.Add(new SubPhasePhaseData(encounterStart, heatWave.Time));
            BuffEvent? downPour = log.CombatData.GetBuffData(DownpourMatthias).FirstOrDefault();
            if (downPour != null)
            {
                phases.Add(new SubPhasePhaseData(heatWave.Time, downPour.Time));
                BuffEvent? abo = log.CombatData.GetBuffData(Unstable).FirstOrDefault();
                if (abo != null)
                {
                    phases.Add(new SubPhasePhaseData(downPour.Time, abo.Time));
                    BuffEvent? invulRemove = log.CombatData.GetBuffDataByIDByDst(Invulnerability757, matthias.AgentItem).FirstOrDefault(x => x.Time >= abo.Time && x.Time <= abo.Time + 10000 && !(x is BuffApplyEvent));
                    if (invulRemove != null)
                    {
                        phases.Add(new SubPhasePhaseData(invulRemove.Time, encounterEnd));
                    }
                }
                else
                {
                    phases.Add(new SubPhasePhaseData(downPour.Time, encounterEnd));
                }
            }
            else
            {
                phases.Add(new SubPhasePhaseData(heatWave.Time, encounterEnd));
            }
        }
        string[] namesMat = ["Ice Phase", "Fire Phase", "Storm Phase", "Abomination Phase"];
        for (int i = 0; i < phases.Count; i++)
        {
            phases[i].Name = namesMat[i];
            phases[i].AddParentPhase(encounterPhase);
            phases[i].AddTarget(matthias, log);
            phases[i].AddTargets(sacrifices, log, PhaseData.TargetPriority.NonBlocking);
        }
        return phases;
    }
    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Matthias)) ?? throw new MissingKeyActorsException("Matthias not found");
        var sacrifices = Targets.Where(x => x.IsSpecies(TargetID.MatthiasSacrificeCrystal));
        phases[0].AddTarget(mainTarget, log);
        phases[0].AddTargets(sacrifices, log, PhaseData.TargetPriority.NonBlocking);
        phases.AddRange(ComputePhases(log, mainTarget, sacrifices, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    internal static void FindSacrifices(LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var sacrificeList = combatData.Where(x => x.SkillID == MatthiasSacrifice && !x.IsExtension && (x.IsBuffRemove == BuffRemove.All || x.IsBuffApply()));
        var sacrificeStartList = sacrificeList.Where(x => x.IsBuffRemove == BuffRemove.None).ToList();
        var sacrificeEndList = sacrificeList.Where(x => x.IsBuffRemove == BuffRemove.All).ToList();
        var copies = new List<CombatItem>();
        for (int i = 0; i < sacrificeStartList.Count; i++)
        {
            //
            long sacrificeStartTime = sacrificeStartList[i].Time;
            long sacrificeEndTime = i < sacrificeEndList.Count ? sacrificeEndList[i].Time : logData.LogEnd;
            //
            AgentItem? sacrifice = agentData.GetAgentByType(AgentItem.AgentType.Player).FirstOrDefault(x => x.Is(agentData.GetAgent(sacrificeStartList[i].DstAgent, sacrificeStartList[i].Time)));
            if (sacrifice == null)
            {
                continue;
            }
            sacrifice = sacrifice.EnglobingAgentItem;
            AgentItem sacrificeCrystal = agentData.AddCustomNPCAgent(sacrificeStartTime, sacrificeEndTime + 100, "Sacrificed " + (i + 1) + " " + sacrifice.Name.Split('\0')[0], Spec.NPC, TargetID.MatthiasSacrificeCrystal, false);
            AgentManipulationHelper.RedirectDamageAndCopyRemainingFromSrcToDst(sacrificeCrystal, sacrifice, copies, combatData, extensions);
        }
        if (copies.Count != 0)
        {
            combatData.AddRange(copies);
            combatData.SortByTime();
        }
    }

    internal static void ForceSacrificeHealth(IReadOnlyList<SingleActor> targets)
    {
        foreach (SingleActor target in targets)
        {
            if (target.IsSpecies(TargetID.MatthiasSacrificeCrystal))
            {
                target.SetManualHealth(100000);
            }
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindSacrifices(logData, agentData, combatData, extensions);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
        ForceSacrificeHealth(Targets);
    }

    internal override IReadOnlyList<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.Matthias,
            TargetID.MatthiasSacrificeCrystal
        ];
    }

    protected override HashSet<int> IgnoreForAutoNumericalRenaming()
    {
        return [(int)TargetID.MatthiasSacrificeCrystal];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.Storm,
            TargetID.Spirit,
            TargetID.Spirit2,
            TargetID.IcePatch,
            TargetID.Tornado
        ];
    }

    private static void AddMatthiasBubbles(long buffID, NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        var shields = target.GetBuffStatus(log, buffID).Where(x => x.Value > 0);
        foreach (var seg in shields)
        {
            replay.Decorations.Add(new CircleDecoration(250, seg, Colors.Magenta, 0.3, new AgentConnector(target)));
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        long castDuration;
        (long start, long end) lifespan = (replay.TimeOffsets.start, replay.TimeOffsets.end);

        switch (target.ID)
        {
            case (int)TargetID.Matthias:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        // Shards of Rage - Jump with AoEs
                        case ShardsOfRageHuman:
                        case ShardsOfRageAbomination:
                            if (log.CombatData.HasEffectData)
                            {
                                // Generic indicator of casting
                                lifespan = (cast.Time, cast.EndTime);
                                replay.Decorations.AddWithFilledWithGrowing(new CircleDecoration(300, lifespan, Colors.Red, 0.5, new AgentConnector(target)).UsingFilled(false), true, lifespan.end);
                            }
                            break;
                        // Oppressive Gaze - Haduken Orb
                        case OppressiveGazeHuman:
                        case OppressiveGazeAbomination:
                            int preCastTime = 1000;
                            castDuration = 750;
                            lifespan = (cast.Time, cast.Time + preCastTime);
                            (long start, long end) lifespanHit = (lifespan.end, lifespan.end + castDuration);
                            uint width = 4000;
                            uint height = 130;
                            if (!log.CombatData.HasMissileData && target.TryGetCurrentFacingDirection(log, lifespan.start + 1000, out var facingOppressiveGaze))
                            {
                                var positionConnector = (AgentConnector)new AgentConnector(target).WithOffset(new(width / 2, 0, 0), true);
                                var rotationConnextor = new AngleConnector(facingOppressiveGaze);
                                replay.Decorations.Add(new RectangleDecoration(width, height, lifespan, Colors.Red, 0.1, positionConnector).UsingRotationConnector(rotationConnextor));
                                replay.Decorations.Add(new RectangleDecoration(width, height, lifespanHit, Colors.Red, 0.7, positionConnector).UsingRotationConnector(rotationConnextor));
                            }
                            break;
                        default:
                            break;
                    }
                }

                // Oppressive Gaze - Orb
                var oppressiveGaze = log.CombatData.GetMissileEventsBySrcBySkillIDs(target.AgentItem, [OppressiveGazeHuman, OppressiveGazeAbomination]);
                replay.Decorations.AddNonHomingMissiles(log, oppressiveGaze, Colors.Red, 0.7, 65);


                var bloodShards = log.CombatData.GetMissileEventsBySrcBySkillIDs(target.AgentItem, [BloodShardsHuman, BloodShardsAbomination]);
                replay.Decorations.AddNonHomingMissiles(log, bloodShards, Colors.Red, 0.7, 20);

                // Blood Shield - Invulnerability Bubble
                AddMatthiasBubbles(BloodShield, target, log, replay);
                AddMatthiasBubbles(BloodShieldAbo, target, log, replay);
                break;
            case (int)TargetID.Storm:
                replay.Decorations.Add(new CircleDecoration(260, lifespan, Colors.LightCobaltBlue, 0.5, new AgentConnector(target)).UsingFilled(false));
                break;
            case (int)TargetID.Spirit:
            case (int)TargetID.Spirit2:
                replay.Decorations.Add(new CircleDecoration(180, lifespan, Colors.LightOrange, 0.3, new AgentConnector(target)));
                break;
            case (int)TargetID.IcePatch:
                replay.Decorations.Add(new CircleDecoration(200, lifespan, Colors.Blue, 0.5, new AgentConnector(target)));
                break;
            case (int)TargetID.Tornado:
                replay.Decorations.Add(new CircleDecoration(90, lifespan, Colors.LightOrange, 0.3, new AgentConnector(target)));
                break;
            default:
                break;
        }

    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
        }

        long growing;
        (long start, long end) lifespan;

        // Corruption
        var corruptedMatthias = p.GetBuffStatus(log, [Corruption1, Corruption2]).Where(x => x.Value > 0);
        foreach (var seg in corruptedMatthias)
        {
            // Circle on player and overhead icon
            replay.Decorations.Add(new CircleDecoration(180, seg, Colors.LightOrange, 0.5, new AgentConnector(p)));
            replay.Decorations.AddOverheadIcon(seg, p, ParserIcons.CorruptionOverhead);

            // Progress bar on the well
            lifespan = (seg.End, seg.End + 100000);
            var playerPositions = p.GetCombatReplayNonPolledPositions(log);

            if (playerPositions.Count > 0)
            {
                // Get position when you lose the buff
                var positionsFiltered = playerPositions.Where(x => x.Time >= seg.End - ServerDelayConstant && x.Time <= seg.End + ArcDPSPollingRate);
                Vector3 foundPosition = new(0, 0, 0);
                bool found = false;

                // Search well by distance
                foreach (ParametricPoint3D playerPosition in positionsFiltered)
                {
                    if (found)
                    {
                        break;
                    }

                    foreach (Vector3 wellPosition in WellsPositions)
                    {
                        if ((wellPosition.XY() - playerPosition.XYZ.XY()).Length() < 200)
                        {
                            foundPosition = wellPosition;
                            found = true;
                            break;
                        }
                    }
                }

                // Other search method if first has failed
                if (!found)
                {
                    // Separated if in case TryGet also returns false
                    if (p.TryGetCurrentInterpolatedPosition(log, lifespan.start - ServerDelayConstant, out var position))
                    {
                        float curDistance = 99999;
                        int curIndex = 0;

                        for (int i = 0; i < WellsPositions.Count; i++)
                        {
                            float distance = Vector3.Distance(position, WellsPositions[i]);
                            if (distance < curDistance)
                            {
                                curDistance = distance;
                                curIndex = i;
                            }
                        }
                        Vector3 closestWell = WellsPositions[curIndex];
                        replay.Decorations.Add(new ProgressBarDecoration(240, 48, lifespan, Colors.Black, 0.6, Colors.Black, 0.2, [(lifespan.start, 0), (lifespan.end, 100)], new PositionConnector(closestWell)));
                    }
                }
                else
                {
                    replay.Decorations.Add(new ProgressBarDecoration(240, 48, lifespan, Colors.Black, 0.6, Colors.Black, 0.2, [(lifespan.start, 0), (lifespan.end, 100)], new PositionConnector(foundPosition)));
                }
            }
        }

        // Well of the Profane - Unstable Blood Magic SAK AoE
        var wellMatthias = p.GetBuffStatus(log, UnstableBloodMagic).Where(x => x.Value > 0);
        foreach (var seg in wellMatthias)
        {
            growing = seg.Start + 9000;

            replay.Decorations.AddWithFilledWithGrowing(new CircleDecoration(120, seg, Colors.DarkerLime, 0.5, new AgentConnector(p)).UsingFilled(false), true, growing);
            if (!log.CombatData.HasEffectData)
            {
                if (p.TryGetCurrentInterpolatedPosition(log, seg.End, out var position))
                {
                    lifespan = (seg.End, seg.End + 90000);
                    replay.Decorations.Add(new CircleDecoration(300, lifespan, Colors.Red, 0.4, new PositionConnector(position)));
                }
            }
            replay.Decorations.AddOverheadIcon(seg, p, ParserIcons.VolatilePoisonOverhead);
        }

        // Sacrifice Selection
        var sacrificeSelection = p.GetBuffStatus(log, MatthiasSacrificeSelection).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(sacrificeSelection, p, ParserIcons.RedArrowDownOverhead);

        // Sacrifice
        var sacrificeMatthias = p.GetBuffStatus(log, MatthiasSacrifice).Where(x => x.Value > 0);
        foreach (var seg in sacrificeMatthias)
        {
            replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, seg.TimeSpan, Colors.Red, 0.6, Colors.Black, 0.2, [(seg.Start, 0), (seg.Start + 10000, 100)], new AgentConnector(p))
                .UsingRotationConnector(new AngleConnector(90)));
        }

        // Zealous Benediction - AoE Bombs on players
        var zealousBenediction = log.CombatData.GetBuffDataByIDByDst(ZealousBenediction, p.AgentItem).Where(x => x is BuffApplyEvent);
        foreach (BuffEvent c in zealousBenediction)
        {
            lifespan = (c.Time, c.Time + 5000);
            replay.Decorations.AddWithGrowing(new CircleDecoration(180, lifespan, Colors.Orange, 0.2, new AgentConnector(p)), lifespan.end);
        }

        // Unbalanced
        var unbalanced = p.GetBuffStatus(log, Unbalanced).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(unbalanced, p, ParserIcons.UnbalancedOverhead);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }

        (long start, long end) lifespan;

        // Shards of Rage AoEs
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.MatthiasShardsOfRageAoEs, out var shards))
        {
            foreach (EffectEvent effect in shards)
            {
                lifespan = effect.ComputeLifespan(log, 3000);
                var circle = new CircleDecoration(120, lifespan, Colors.Red, 0.1, new PositionConnector(effect.Position)).UsingFilled(false);
                environmentDecorations.Add(circle);
            }
        }

        // Well of the Profane - Unstable Blood Magic SAK AoE
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.MatthiasWellOfTheProfane, out var wellsOfTheProfane))
        {
            foreach (var effect in wellsOfTheProfane)
            {
                lifespan = effect.ComputeLifespan(log, 90000);
                var circle = new CircleDecoration(300, lifespan, Colors.Red, 0.4, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
            }
        }
    }
    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
    }

    /// <summary>
    /// Coordinates of the wells.
    /// </summary>
    private static readonly List<Vector3> WellsPositions =
    [
        new Vector3(-6871.6543f, 6823.631f, -5180.869f), // North West
        new Vector3(-5011.504f, 6834.5103f, -5180.869f), // North East
        new Vector3(-6859.2817f, 4965.546f, -5180.869f), // South West
        new Vector3(-5002.0090f, 4976.911f, -5180.869f), // South East
    ];

}
