using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Matthias : SalvationPass
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([


            new PlayerDstHitMechanic([OppressiveGazeHuman, OppressiveGazeAbomination], new MechanicPlotlySetting(Symbols.Hexagram,Colors.Red), "Hadouken", "Oppressive Gaze (Hadouken projectile)","Hadouken", 0),
            new MechanicGroup([
                new PlayerDstHitMechanic([BloodShardsHuman, BloodShardsAbomination], new MechanicPlotlySetting(Symbols.DiamondWideOpen,Colors.Magenta), "Shoot Shards", "Blood Shard projectiles during bubble","Rapid Fire", 0),
                new PlayerSrcHitMechanic([BloodShardsHuman, BloodShardsAbomination], new MechanicPlotlySetting(Symbols.DiamondWideOpen,Colors.Green), "Refl.Shards", "Blood Shard projectiles reflected during bubble","Reflected Rapid Fire", 0),
                new EnemySrcMissileMechanic([BloodShardsHuman, BloodShardsAbomination], new MechanicPlotlySetting(Symbols.DiamondWideOpen,Colors.Red), "N.Refl.Shards", "Blood Shard projectiles not reflected during bubble","Not reflected Rapid Fire", 0)
                    .UsingNotReflected(),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic([ShardsOfRageHuman, ShardsOfRageAbomination], new MechanicPlotlySetting(Symbols.StarDiamond,Colors.Red), "Jump Shards", "Shards of Rage (Jump)","Jump Shards", 1000),
                new PlayerSrcHitMechanic([ShardsOfRageHuman, ShardsOfRageAbomination], new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.Red), "Refl.Jump Shards", "Reflected Shards of Rage (Jump)","Reflected Jump Shards", 0)
                    .WithMinions(),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(FieryVortex, new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Yellow), "Tornado", "Fiery Vortex (Tornado)","Tornado", 250),
                new PlayerDstBuffApplyMechanic(Slow, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Blue), "Icy KD", "Knockdown by Icy Patch","Icy Patch KD", 0)
                    .UsingChecker((br,log) => br.AppliedDuration == 10000 && !br.To.HasBuff(log, Stability, br.Time - ServerDelayConstant)),
                new PlayerDstHitMechanic(Thunder, new MechanicPlotlySetting(Symbols.TriangleUpOpen,Colors.Teal), "Storm", "Thunder Storm hit (air phase)","Storm cloud", 0),
                new PlayerDstBuffRemoveMechanic(Unbalanced, new MechanicPlotlySetting(Symbols.Square,Colors.LightPurple), "KD","Unbalanced (triggered Storm phase Debuff)", "Knockdown",0)
                    .UsingChecker( (br,log) => br.RemovedDuration > 0 && !br.To.HasBuff(log, Stability, br.Time - ServerDelayConstant)),
                new PlayerDstHitMechanic(Surrender, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Black), "Spirit", "Surrender (hit by walking Spirit)","Spirit hit", 0)
            ]),
            new PlayerDstBuffApplyMechanic(UnstableBloodMagic, new MechanicPlotlySetting(Symbols.Diamond,Colors.Red), "Well", "Unstable Blood Magic application","Well", 0),
            new PlayerDstHitMechanic(WellOfTheProfane, new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Red), "Well dmg", "Unstable Blood Magic AoE hit","Stood in Well", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(Corruption1, new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Corruption", "Corruption Application","Corruption", 0),
                new PlayerDstHitMechanic(Corruption2, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Corr. dmg", "Hit by Corruption AoE","Corruption dmg", 0),
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
        EncounterCategoryInformation.InSubCategoryOrder = 2;
        EncounterID |= 0x000003;
        ChestID = ChestID.MatthiasChest;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayMatthias,
                        (880, 880),
                        (-7248, 4585, -4625, 7207)/*,
                        (-12288, -27648, 12288, 27648),
                        (2688, 11906, 3712, 14210)*/);
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);
        IReadOnlyList<BuffEvent> bloodstoneBisque = log.CombatData.GetBuffData(BloodstoneBisque);
        if (bloodstoneBisque.Any() && log.FightData.Success)
        {
            int playersWithBisque = 0;
            int expectedPlayersForSuccess = 0;
            long fightEnd = log.FightData.FightEnd - ServerDelayConstant;
            long fightStart = log.FightData.FightStart + ServerDelayConstant;
            foreach (Player p in log.PlayerList)
            {
                IReadOnlyDictionary<long, BuffGraph> graphs = p.GetBuffGraphs(log);
                if (graphs.TryGetValue(BloodstoneBisque, out var graph))
                {
                    if (!graph.Values.Any(x => x.Value == 0 && x.Intersects(fightStart, fightEnd)))
                    {
                        playersWithBisque++;
                    }
                }
                var (_, _, dcs, _) = p.GetStatus(log);
                if (!dcs.Any(x => x.ContainsPoint(fightEnd)))
                {
                    expectedPlayersForSuccess++;
                }
            }
            if (expectedPlayersForSuccess <= playersWithBisque)
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIds[BloodstoneBisque], 1));
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
    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        long fightEnd = log.FightData.FightEnd;
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Matthias)) ?? throw new MissingKeyActorsException("Matthias not found");
        phases[0].AddTarget(mainTarget, log);
        if (!requirePhases)
        {
            return phases;
        }
        // Special buff cast check
        BuffEvent? heatWave = log.CombatData.GetBuffData(HeatWaveMatthias).FirstOrDefault();
        if (heatWave != null)
        {
            phases.Add(new PhaseData(0, heatWave.Time));
            BuffEvent? downPour = log.CombatData.GetBuffData(DownpourMatthias).FirstOrDefault();
            if (downPour != null)
            {
                phases.Add(new PhaseData(heatWave.Time, downPour.Time));
                BuffEvent? abo = log.CombatData.GetBuffData(Unstable).FirstOrDefault();
                if (abo != null)
                {
                    phases.Add(new PhaseData(downPour.Time, abo.Time));
                    BuffEvent? invulRemove = log.CombatData.GetBuffDataByIDByDst(Invulnerability757, mainTarget.AgentItem).FirstOrDefault(x => x.Time >= abo.Time && x.Time <= abo.Time + 10000 && !(x is BuffApplyEvent));
                    if (invulRemove != null)
                    {
                        phases.Add(new PhaseData(invulRemove.Time, fightEnd));
                    }
                }
                else
                {
                    phases.Add(new PhaseData(downPour.Time, fightEnd));
                }
            }
            else
            {
                phases.Add(new PhaseData(heatWave.Time, fightEnd));
            }
        }
        else
        {
            phases.Add(new PhaseData(log.FightData.FightStart, fightEnd));
        }
        string[] namesMat = ["Ice Phase", "Fire Phase", "Storm Phase", "Abomination Phase"];
        for (int i = 1; i < phases.Count; i++)
        {
            phases[i].Name = namesMat[i - 1];
            phases[i].AddParentPhase(phases[0]);
            phases[i].AddTarget(mainTarget, log);
        }
        return phases;
    }

    internal static void FindSacrifices(FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        // has breakbar state into
        if (combatData.Any(x => x.IsStateChange == StateChange.BreakbarState))
        {
            var sacrificeList = combatData.Where(x => x.SkillID == MatthiasSacrifice && !x.IsExtension && (x.IsBuffRemove == BuffRemove.All || x.IsBuffApply()));
            var sacrificeStartList = sacrificeList.Where(x => x.IsBuffRemove == BuffRemove.None).ToList();
            var sacrificeEndList = sacrificeList.Where(x => x.IsBuffRemove == BuffRemove.All).ToList();
            var copies = new List<CombatItem>();
            for (int i = 0; i < sacrificeStartList.Count; i++)
            {
                //
                long sacrificeStartTime = sacrificeStartList[i].Time;
                long sacrificeEndTime = i < sacrificeEndList.Count ? sacrificeEndList[i].Time : fightData.FightEnd;
                //
                AgentItem? sacrifice = agentData.GetAgentByType(AgentItem.AgentType.Player).FirstOrDefault(x => x == agentData.GetAgent(sacrificeStartList[i].DstAgent, sacrificeStartList[i].Time));
                if (sacrifice == null)
                {
                    continue;
                }
                AgentItem sacrificeCrystal = agentData.AddCustomNPCAgent(sacrificeStartTime, sacrificeEndTime + 100, "Sacrificed " + (i + 1) + " " + sacrifice.Name.Split('\0')[0], sacrifice.Spec, TargetID.MatthiasSacrificeCrystal, false);
                foreach (CombatItem cbt in combatData)
                {
                    if (!sacrificeCrystal.InAwareTimes(cbt.Time))
                    {
                        continue;
                    }
                    bool skip = !(cbt.DstMatchesAgent(sacrifice, extensions) || cbt.SrcMatchesAgent(sacrifice, extensions));
                    if (skip)
                    {
                        continue;
                    }
                    // redirect damage events
                    if (cbt.IsDamage(extensions))
                    {
                        // only redirect incoming damage
                        if (cbt.DstMatchesAgent(sacrifice, extensions))
                        {
                            cbt.OverrideDstAgent(sacrificeCrystal);
                        }
                    }
                    // copy the rest
                    else
                    {
                        var copy = new CombatItem(cbt);
                        if (copy.DstMatchesAgent(sacrifice, extensions))
                        {
                            copy.OverrideDstAgent(sacrificeCrystal);
                        }
                        if (copy.SrcMatchesAgent(sacrifice, extensions))
                        {
                            copy.OverrideSrcAgent(sacrificeCrystal);
                        }
                        copies.Add(copy);
                    }
                }
            }
            if (copies.Count != 0)
            {
                combatData.AddRange(copies);
                combatData.SortByTime();
            }
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

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        FindSacrifices(fightData, agentData, combatData, extensions);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        ForceSacrificeHealth(Targets);
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.Matthias,
            TargetID.MatthiasSacrificeCrystal
        ];
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
        var shields = target.GetBuffStatus(log, buffID, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        foreach (var seg in shields)
        {
            replay.Decorations.Add(new CircleDecoration(250, seg, Colors.Magenta, 0.3, new AgentConnector(target)));
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        long castDuration;
        (long start, long end) lifespan = (replay.TimeOffsets.start, replay.TimeOffsets.end);

        switch (target.ID)
        {
            case (int)TargetID.Matthias:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
                {
                    switch (cast.SkillId)
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
                var oppressiveGaze = log.CombatData.GetMissileEventsBySkillIDs([OppressiveGazeHuman, OppressiveGazeAbomination]);
                replay.Decorations.AddNonHomingMissiles(log, oppressiveGaze, Colors.Red, 0.7, 65);


                var bloodShards = log.CombatData.GetMissileEventsBySkillIDs([BloodShardsHuman, BloodShardsAbomination]);
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
        base.ComputePlayerCombatReplayActors(p, log, replay);

        long growing;
        (long start, long end) lifespan;

        // Corruption
        var corruptedMatthias = p.GetBuffStatus(log, [Corruption1, Corruption2], log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
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
        var wellMatthias = p.GetBuffStatus(log, UnstableBloodMagic, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
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
        var sacrificeSelection = p.GetBuffStatus(log, MatthiasSacrificeSelection, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(sacrificeSelection, p, ParserIcons.RedArrowOverhead);

        // Sacrifice
        var sacrificeMatthias = p.GetBuffStatus(log, MatthiasSacrifice, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
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
        var unbalanced = p.GetBuffStatus(log, Unbalanced, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(unbalanced, p, ParserIcons.UnbalancedOverhead);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

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
