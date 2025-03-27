using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class KeepConstruct : StrongholdOfTheFaithful
{
    public KeepConstruct(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([    
            new PlayerDstBuffApplyMechanic([StatueFixated1, StatueFixated2], new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "Fixate", "Fixated by Statue","Fixated", 0),
            new PlayerDstHitMechanic(HailOfFury, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Debris", "Hail of Fury (Falling Debris)","Debris", 0),
            new EnemyDstBuffApplyMechanic(Compromised, new MechanicPlotlySetting(Symbols.Hexagon,Colors.Blue), "Rift#", "Compromised (Pushed Orb through Rifts)","Compromised", 0),
            new MechanicGroup([
                new EnemyDstBuffApplyMechanic(MagicBlast, new MechanicPlotlySetting(Symbols.Star,Colors.Teal), "M.B.# 33%", "Magic Blast (Orbs eaten by KC) at 33%","Magic Blast 33%", 0)
                    .UsingChecker( (de, log) => {
                        var phases = log.FightData.GetPhases(log).Where(x => x.Name.Contains('%')).ToList();
                        if (phases.Count < 2)
                        {
                            // no 33% magic blast
                            return false;
                        }
                        return de.Time >= phases[1].End;
                    }
                ),
                new EnemyDstBuffApplyMechanic(MagicBlast, new MechanicPlotlySetting(Symbols.Star,Colors.DarkTeal), "M.B.# 66%", "Magic Blast (Orbs eaten by KC) at 66%","Magic Blast 66%", 0)
                    .UsingChecker((de, log) => {
                        var phases = log.FightData.GetPhases(log).Where(x => x.Name.Contains('%')).ToList();
                        if (phases.Count < 1)
                        {
                            // no 66% magic blast
                            return false;
                        }
                        bool condition = de.Time >= phases[0].End;
                        if (phases.Count > 1)
                        {
                            // must be before 66%-33% phase if it exists
                            condition = condition && de.Time <= phases[1].Start;
                        }
                        return condition;
                    }
                ),
            ]),
            new SpawnMechanic((int) TargetID.InsidiousProjection, new MechanicPlotlySetting(Symbols.Bowtie,Colors.Red), "Merge", "Insidious Projection spawn (2 Statue merge)","Merged Statues", 0),
            new PlayerDstHitMechanic([PhantasmalBlades2,PhantasmalBlades3, PhantasmalBlades1], new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.Magenta), "Pizza", "Phantasmal Blades (rotating Attack)","Phantasmal Blades", 0),
            new PlayerDstHitMechanic(TowerDrop, new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Jump", "Tower Drop (KC Jump)","Tower Drop", 0),
            new PlayerDstBuffApplyMechanic(XerasFury, new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Bomb", "Xera's Fury (Large Bombs) application","Bombs", 0),
            new MechanicGroup([
                new PlayerDstHitMechanic(WhiteOrb, new MechanicPlotlySetting(Symbols.Circle,Colors.White), "GW.Orb", "Good White Orb","Good White Orb", 0)
                    .UsingChecker((de,log) => de.To.HasBuff(log, RadiantAttunementOrb, de.Time)),
                new PlayerDstHitMechanic(RedOrb, new MechanicPlotlySetting(Symbols.Circle,Colors.DarkRed), "GR.Orb", "Good Red Orb","Good Red Orb", 0)
                    .UsingChecker((de,log) => de.To.HasBuff(log, CrimsonAttunementOrb, de.Time)),
                new PlayerDstHitMechanic(WhiteOrb, new MechanicPlotlySetting(Symbols.Circle,Colors.Grey), "BW.Orb", "Bad White Orb","Bad White Orb", 0)
                    .UsingChecker((de,log) => !de.To.HasBuff(log, RadiantAttunementOrb, de.Time)),
                new PlayerDstHitMechanic(RedOrb, new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "BR.Orb", "Bad Red Orb","Bad Red Orb", 0)
                    .UsingChecker((de,log) => !de.To.HasBuff(log, CrimsonAttunementOrb, de.Time)),
            ]),
            new PlayerSrcAllHitsMechanic(new MechanicPlotlySetting(Symbols.StarOpen,Colors.LightOrange), "Core Hit","Core was Hit by Player", "Core Hit",1000)
                .UsingChecker((de, log) => de.To.IsSpecies(TargetID.KeepConstructCore) && de is DirectHealthDamageEvent)
        ]));
        Extension = "kc";
        Icon = EncounterIconKeepConstruct;
        EncounterCategoryInformation.InSubCategoryOrder = 1;
        EncounterID |= 0x000002;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayKeepConstruct,
                        (987, 1000),
                        (-5467, 8069, -2282, 11297)/*,
                        (-12288, -27648, 12288, 27648),
                        (1920, 12160, 2944, 14464)*/);
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(ConstructAura, ConstructAura),
        ];
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        long start = 0;
        long end = 0;
        long fightEnd = log.FightData.FightEnd;
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.KeepConstruct)) ?? throw new MissingKeyActorsException("Keep Construct not found");
        phases[0].AddTarget(mainTarget);
        if (!requirePhases)
        {
            return phases;
        }
        // Main phases 35025
        var kcPhaseInvuls = GetFilteredList(log.CombatData, XerasBoon, mainTarget, true, true);
        foreach (BuffEvent c in kcPhaseInvuls)
        {
            if (c is BuffApplyEvent)
            {
                end = c.Time;
                phases.Add(new PhaseData(start, end));
            }
            else
            {
                start = c.Time;
            }
        }
        if (fightEnd - start > PhaseTimeLimit && start >= phases.Last().End)
        {
            phases.Add(new PhaseData(start, fightEnd));
            start = fightEnd;
        }
        for (int i = 1; i < phases.Count; i++)
        {
            phases[i].Name = "Phase " + i;
            phases[i].AddTarget(mainTarget);
        }
        // add burn phases
        int offset = phases.Count;
        IReadOnlyList<BuffEvent> orbItems = log.CombatData.GetBuffDataByIDByDst(Compromised, mainTarget.AgentItem);
        // Get number of orbs and filter the list
        start = 0;
        int orbCount = 0;
        var segments = new List<Segment>();
        foreach (BuffEvent c in orbItems)
        {
            if (c is BuffApplyEvent)
            {
                if (start == 0)
                {
                    start = c.Time;
                }
                orbCount++;
            }
            else if (start != 0)
            {
                segments.Add(new Segment(start, Math.Min(c.Time, fightEnd), orbCount));
                orbCount = 0;
                start = 0;
            }
        }
        int burnCount = 1;
        foreach (Segment seg in segments)
        {
            var phase = new PhaseData(seg.Start, seg.End, "Burn " + burnCount++ + " (" + seg.Value + " orbs)");
            phase.AddTarget(mainTarget);
            phases.Add(phase);
        }
        phases.Sort((x, y) => x.Start.CompareTo(y.Start));
        // pre burn phases
        int preBurnCount = 1;
        var preBurnPhase = new List<PhaseData>();
        var kcInvuls = GetFilteredList(log.CombatData, Determined762, mainTarget, true, true);
        foreach (BuffEvent invul in kcInvuls)
        {
            if (invul is BuffApplyEvent)
            {
                end = invul.Time;
                PhaseData? prevPhase = phases.LastOrDefault(x => x.Start <= end || x.End <= end);
                if (prevPhase != null)
                {
                    start = (prevPhase.End >= end ? prevPhase.Start : prevPhase.End) + 1;
                    if (end - start > 1000)
                    {
                        var phase = new PhaseData(start, end, "Pre-Burn " + preBurnCount++);
                        phase.AddTarget(mainTarget);
                        preBurnPhase.Add(phase);
                    }
                }
            }
        }
        phases.AddRange(preBurnPhase);
        phases.Sort((x, y) => x.Start.CompareTo(y.Start));
        // add leftover phases
        PhaseData? cur = null;
        int leftOverCount = 1;
        var leftOverPhases = new List<PhaseData>();
        for (int i = 0; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            if (phase.Name.Contains('%'))
            {
                cur = phase;
            }
            else if (phase.Name.Contains("orbs"))
            {
                if (cur != null)
                {
                    if (cur.End >= phase.End + 5000 && (i == phases.Count - 1 || phases[i + 1].Name.Contains('%')))
                    {
                        var leftOverPhase = new PhaseData(phase.End, cur.End, "Leftover " + leftOverCount++);
                        leftOverPhase.AddTarget(mainTarget);
                        leftOverPhases.Add(leftOverPhase);
                    }
                }
            }
        }
        phases.AddRange(leftOverPhases);
        return phases;
    }

    protected override ReadOnlySpan<TargetID> GetTargetsIDs()
    {
        return
        [
            TargetID.KeepConstruct,
            TargetID.Jessica,
            TargetID.Olson,
            TargetID.Engul,
            TargetID.Faerla,
            TargetID.Caulle,
            TargetID.Henley,
            TargetID.Galletta,
            TargetID.Ianim,
        ];
    }
    protected override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID.KeepConstruct, 0 },
            {TargetID.Jessica, 1 },
            {TargetID.Olson, 1 },
            {TargetID.Engul, 1 },
            {TargetID.Faerla, 1 },
            {TargetID.Caulle, 1 },
            {TargetID.Henley, 1 },
            {TargetID.Galletta, 1 },
            {TargetID.Ianim, 1 },
        };
    }

    protected override List<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.KeepConstructCore,
            TargetID.GreenPhantasm,
            TargetID.InsidiousProjection,
            TargetID.UnstableLeyRift,
            TargetID.RadiantPhantasm,
            TargetID.CrimsonPhantasm,
            TargetID.RetrieverProjection
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        var countDict = new Dictionary<int, int>();
        var bigPhantasmIDs = new HashSet<TargetID>
        {
            TargetID.Jessica,
            TargetID.Olson,
            TargetID.Engul,
            TargetID.Faerla,
            TargetID.Caulle,
            TargetID.Henley,
            TargetID.Galletta,
            TargetID.Ianim,
        };
        foreach (SingleActor target in Targets)
        {
            if (target.IsAnySpecies(bigPhantasmIDs))
            {
                if (countDict.TryGetValue(target.ID, out int count))
                {
                    target.OverrideName(target.Character + " " + (++count));
                }
                else
                {
                    count = 1;
                    target.OverrideName(target.Character + " " + count);
                }
                countDict[target.ID] = count;
            }
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        var cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
        int start = (int)replay.TimeOffsets.start;
        int end = (int)replay.TimeOffsets.end;
        switch (target.ID)
        {
            case (int)TargetID.KeepConstruct:

                var kcOrbCollect = target.GetBuffStatus(log, XerasBoon, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
                foreach (Segment seg in kcOrbCollect)
                {
                    replay.Decorations.Add(new OverheadProgressBarDecoration(CombatReplayOverheadProgressBarMajorSizeInPixel, (seg.Start, seg.End), Colors.Red, 0.6, Colors.Black, 0.2, [(seg.Start, 0), (seg.End, 100)], new AgentConnector(target))
                        .UsingRotationConnector(new AngleConnector(180)));
                }
                
                var towerDrop = cls.Where(x => x.SkillId == TowerDrop);
                foreach (CastEvent c in towerDrop)
                {
                    start = (int)c.Time;
                    end = (int)c.EndTime;
                    int skillCast = end - 1000;
                    if (target.TryGetCurrentInterpolatedPosition(log, end, out var position))
                    {
                        replay.Decorations.AddWithFilledWithGrowing(new CircleDecoration(400, (start, skillCast), Colors.LightOrange, 0.5, new PositionConnector(position)).UsingFilled(false), true, skillCast);
                    }
                }
               
                var blades1 = cls.Where(x => x.SkillId == PhantasmalBlades1);
                var blades2 = cls.Where(x => x.SkillId == PhantasmalBlades2);
                var blades3 = cls.Where(x => x.SkillId == PhantasmalBlades3);
                int bladeDelay = 150;
                int duration = 1000;
                float bladeOpeningAngle = 360 * 3 / 32;
                uint bladeRadius = 1600;
                foreach (CastEvent c in blades1)
                {
                    int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((c.ActualDuration - 1150) / 1000.0), 9));
                    start = (int)c.Time + bladeDelay;
                    if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing))
                    {
                        var connector = new AgentConnector(target);
                        replay.Decorations.Add(new CircleDecoration(200, (start, start + (ticks + 1) * 1000), Colors.Red, 0.4, connector));
                        float initialAngle = facing.GetRoundedZRotationDeg();
                        replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start, start + 2 * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(initialAngle))); // First blade lasts twice as long
                        for (int i = 1; i < ticks; i++)
                        {
                            float angle = initialAngle + i * 360 / 8;
                            replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(angle))); // First blade lasts longer
                        }
                    }
                }

                foreach (CastEvent c in blades2)
                {
                    int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((c.ActualDuration - 1150) / 1000.0), 9));
                    start = (int)c.Time + bladeDelay;
                    if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing))
                    {
                        var connector = new AgentConnector(target);
                        replay.Decorations.Add(new CircleDecoration(200, (start, start + (ticks + 1) * 1000), Colors.Red, 0.4, connector));
                        float initialAngle1 = facing.GetRoundedZRotationDeg();
                        replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start, start + 2 * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(initialAngle1))); // First blade lasts twice as long
                        float initialAngle2 = RadianToDegreeF(Math.Atan2(-facing.Y, -facing.X));
                        replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start, start + 2 * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(initialAngle2))); // First blade lasts twice as long
                        for (int i = 1; i < ticks; i++)
                        {
                            float angle1 = initialAngle1 + i * 360 / 8;
                            replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(angle1))); // First blade lasts longer
                            float angle2 = initialAngle2 + i * 360 / 8;
                            replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(angle2))); // First blade lasts longer
                        }
                    }
                }

                foreach (CastEvent c in blades3)
                {
                    int ticks = (int)Math.Max(0, Math.Min(Math.Ceiling((c.ActualDuration - 1150) / 1000.0), 9));
                    start = (int)c.Time + bladeDelay;
                    if (target.TryGetCurrentFacingDirection(log, start + 1000, out var facing))
                    {
                        var connector = new AgentConnector(target);
                        replay.Decorations.Add(new CircleDecoration(200, (start, start + (ticks + 1) * 1000), Colors.Red, 0.4, connector));
                        float initialAngle1 = RadianToDegreeF(Math.Atan2(-facing.Y, -facing.X));
                        replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start, start + 2 * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(initialAngle1))); // First blade lasts twice as long
                        float initialAngle2 = initialAngle1 + 120;
                        replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start, start + 2 * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(initialAngle2))); // First blade lasts twice as long
                        float initialAngle3 = initialAngle1 - 120;
                        replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start, start + 2 * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(initialAngle3))); // First blade lasts twice as long
                        for (int i = 1; i < ticks; i++)
                        {
                            float angle1 = initialAngle1 + i * 360 / 8;
                            replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(angle1))); // First blade lasts longer
                            float angle2 = initialAngle2 + i * 360 / 8;
                            replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(angle2))); // First blade lasts longer
                            float angle3 = initialAngle3 + i * 360 / 8;
                            replay.Decorations.Add(new PieDecoration(bladeRadius, bladeOpeningAngle, (start + 1000 + i * duration, start + 1000 + (i + 1) * duration), Colors.Magenta, 0.5, connector).UsingRotationConnector(new AngleConnector(angle3))); // First blade lasts longer
                        }
                    }
                }
                break;
            case (int)TargetID.KeepConstructCore:
                break;
            case (int)TargetID.Jessica:
            case (int)TargetID.Olson:
            case (int)TargetID.Engul:
            case (int)TargetID.Faerla:
            case (int)TargetID.Caulle:
            case (int)TargetID.Henley:
            case (int)TargetID.Galletta:
            case (int)TargetID.Ianim:
                replay.Decorations.Add(new CircleDecoration(600, (start, end), Colors.Red, 0.5, new AgentConnector(target)).UsingFilled(false));
                replay.Decorations.Add(new CircleDecoration(400, (start, end), Colors.LightBlue, 0.5, new AgentConnector(target)));
                if (replay.PolledPositions.Count > 0)
                {
                    replay.Decorations.AddWithGrowing(new CircleDecoration(300, (start - 5000, start), Colors.Orange, 0.3, new PositionConnector(replay.PolledPositions[0].XYZ)), start);
                }
                break;
            case (int)TargetID.GreenPhantasm:
                int lifetime = 8000;
                replay.Decorations.AddWithGrowing(new CircleDecoration(210, (start, start + lifetime), Colors.Green, 0.2, new AgentConnector(target)), start + lifetime);
                break;
            case (int)TargetID.RetrieverProjection:
            case (int)TargetID.InsidiousProjection:
            case (int)TargetID.UnstableLeyRift:
            case (int)TargetID.RadiantPhantasm:
            case (int)TargetID.CrimsonPhantasm:
                break;
            default:
                break;
        }

    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        return combatData.GetSkills().Contains(34958) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        // Bombs
        var xeraFury = p.GetBuffStatus(log, XerasFury, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        foreach (Segment seg in xeraFury)
        {
            replay.Decorations.AddWithGrowing(new CircleDecoration(550, seg, Colors.Orange, 0.2, new AgentConnector(p)), seg.End);

        }
        // Fixated Statue tether to Player
        var fixatedStatue = GetFilteredList(log.CombatData, [ StatueFixated1, StatueFixated2 ], p, true, true);
        replay.Decorations.AddTether(fixatedStatue, Colors.Magenta, 0.5);
        // Fixation Overhead
        IEnumerable<Segment> fixations = p.GetBuffStatus(log, [ StatueFixated1, StatueFixated2 ], log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(fixations, p, ParserIcons.FixationPurpleOverhead);
        // Attunements Overhead
        IEnumerable<Segment> crimsonAttunements = p.GetBuffStatus(log, [ CrimsonAttunementPhantasm, CrimsonAttunementOrb ], log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(crimsonAttunements, p, ParserIcons.CrimsonAttunementOverhead);
        IEnumerable<Segment> radiantAttunements = p.GetBuffStatus(log, [ RadiantAttunementPhantasm, RadiantAttunementOrb ], log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(radiantAttunements, p, ParserIcons.RadiantAttunementOverhead);
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        if (log.FightData.Success && log.FightData.IsCM)
        {
            int hasHitKc = 0;
            foreach (Player p in log.PlayerList)
            {
                if (p.GetDamageEvents(Targets.FirstOrDefault(x => x.IsSpecies(TargetID.KeepConstruct)), log, log.FightData.FightStart, log.FightData.FightEnd).Any())
                {
                    hasHitKc++;
                }
            }
            if (hasHitKc == log.PlayerList.Count)
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityDownDownDowned], 1));
            }
        }
    }
}
