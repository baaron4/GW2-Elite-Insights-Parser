using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class PeerlessQadim : TheKeyOfAhdashim
    {
        public PeerlessQadim(int triggerID) : base(triggerID)
        {
            
            MechanicList.AddRange(new List<Mechanic>()
            {
                new HitOnPlayerMechanic(56541, "Pylon Debris Field", new MechanicPlotlySetting(Symbols.CircleOpenDot,Colors.Orange), "P.Magma", "Hit by Pylon Magma", "Pylon Magma", 0),
                new HitOnPlayerMechanic(56020, "Energized Affliction", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Green), "E.Aff", "Energized Affliction", "Energized Affliction", 0),
                new HitOnPlayerMechanic(56134, "Force of Retaliation", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Black), "Pushed", "Pushed by Shockwave", "Shockwave Push", 1000, (de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
                new HitOnPlayerMechanic(56093, "Exponential Repercussion", new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Magenta), "P.KB", "Pushed by Pylon Knockback", "Pylon Knockback", 1000),
                new HitOnPlayerMechanic(56254, "Exponential Repercussion", new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.DarkPurple), "Dome.KB", "Pushed by Dome Shield Knockback", "Dome Knockback", 1000),
                new HitOnPlayerMechanic(56441, "Force of Havoc", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Purple), "P.Rect", "Hit by Purple Rectangle", "Purple Rectangle", 0),
                new HitOnPlayerMechanic(56145, "Chaos Called", new MechanicPlotlySetting(Symbols.CircleXOpen,Colors.Purple), "Pattern.H", "Hit by Energy on Pattern", "Pattern Energy Hit", 0),
                new HitOnPlayerMechanic(56527, "Rain of Chaos", new MechanicPlotlySetting(Symbols.StarSquare,Colors.Purple), "Lght.H", "Hit by Expanding Lightning", "Lightning Hit", 0),
                new HitOnPlayerMechanic(56656, "Brandstorm Lightning", new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Yellow), "S.Lght.H", "Hit by Small Lightning", "Small Lightning Hit", 0),
                new HitOnPlayerMechanic(56180, "Residual Impact", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Orange), "Magma.F", "Hit by Magma Field", "Magma Field", 500),
                new HitOnPlayerMechanic(56378, "Residual Impact", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Orange,10), "S.Magma.F", "Hit by Small Magma Field", "Small Magma Field", 500),
                new HitOnPlayerMechanic(BatteringBlitz, "Battering Blitz", new MechanicPlotlySetting(Symbols.Bowtie,Colors.Orange), "Rush.H", "Hit by Qadim Rush", "Qadim Rush", 500),
                new HitOnPlayerMechanic(56332, "Caustic Chaos", new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Red), "A.Prj.H", "Hit by Aimed Projectile", "Aimed Projectile", 0),
                new HitByEnemyMechanic(56598, "Shower of Chaos", new MechanicPlotlySetting(Symbols.Circle,Colors.Black), "Orb.D", "Pylon Orb not caught", "Shower of Chaos", 1000),
                new HitByEnemyMechanic(56316, "Eclipsed Backlash", new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "Entropic.Expl", "Entropic Distortion exploded", "Eclipsed Backlash", 1000),
                new PlayerBuffApplyMechanic(FixatedQadimThePeerless, "Fixated", new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "Fixated", "Fixated", "Fixated", 0),
                new PlayerBuffApplyMechanic(CriticalMass, "Critical Mass", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Orb caught", "Collected a Pylon Orb", "Critical Mass", 0),
                new HitOnPlayerMechanic(56543, "Caustic Chaos", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Red), "A.Prj.E", "Hit by Aimed Projectile Explosion", "Aimed Projectile Explosion", 0),
                new PlayerBuffApplyMechanic(SappingSurge, "Sapping Surge", new MechanicPlotlySetting(Symbols.YDownOpen,Colors.Red), "B.Tether", "25% damage reduction", "Bad Tether", 0),
            });
            Extension = "prlqadim";
            Icon = "https://wiki.guildwars2.com/images/8/8b/Mini_Qadim_the_Peerless.png";
            EncounterCategoryInformation.InSubCategoryOrder = 1;
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>()
            {
                ArcDPSEnums.TrashID.Pylon1,
                ArcDPSEnums.TrashID.Pylon2,
                ArcDPSEnums.TrashID.EntropicDistortion,
                ArcDPSEnums.TrashID.BigKillerTornado,
                ArcDPSEnums.TrashID.EnergyOrb,
            };
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(56038, 56038, InstantCastFinder.DefaultICD), // Unbearable Power
            };
        }
        internal override List<AbstractBuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            IReadOnlyList<AbstractBuffEvent> sappingSurges = combatData.GetBuffData(SappingSurge);
            var sappingSurgeByDst = sappingSurges.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<AgentItem, List<AbstractBuffEvent>> pair in sappingSurgeByDst.Where(x => x.Value.Exists(y => y is BuffRemoveSingleEvent)))
            {
                var sglRemovals = pair.Value.Where(x => x is BuffRemoveSingleEvent).ToList();
                foreach (AbstractBuffEvent sglRemoval in sglRemovals)
                {
                    AbstractBuffEvent ba = pair.Value.LastOrDefault(x => x is BuffApplyEvent && Math.Abs(x.Time - sglRemoval.Time) < 5);
                    if (ba != null)
                    {
                        res.Add(new BuffRemoveAllEvent(sglRemoval.CreditedBy, pair.Key, ba.Time - 1, int.MaxValue, ba.BuffSkill, 0, int.MaxValue));
                        res.Add(new BuffRemoveManualEvent(sglRemoval.CreditedBy, pair.Key, ba.Time - 1, int.MaxValue, ba.BuffSkill));
                    }
                }
            }
            return res;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.PeerlessQadim);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Peerless Qadim not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            var phaseStarts = new List<long>();
            var phaseEnds = new List<long>();
            //
            var magmaDrops = log.CombatData.GetBuffData(MagmaDrop).Where(x => x is BuffApplyEvent).ToList();
            foreach (AbstractBuffEvent magmaDrop in magmaDrops)
            {
                if (phaseEnds.Count > 0)
                {
                    if (Math.Abs(phaseEnds.Last() - magmaDrop.Time) > 1000)
                    {
                        phaseEnds.Add(magmaDrop.Time);
                    }
                }
                else
                {
                    phaseEnds.Add(magmaDrop.Time);
                }
            }
            IReadOnlyList<AnimatedCastEvent> pushes = log.CombatData.GetAnimatedCastData(ForceOfRetaliationCast);
            if (pushes.Count > 0)
            {
                AbstractCastEvent push = pushes[0];
                phaseStarts.Add(push.Time);
                foreach (long magmaDrop in phaseEnds)
                {
                    push = pushes.FirstOrDefault(x => x.Time >= magmaDrop);
                    if (push == null)
                    {
                        break;
                    }
                    phaseStarts.Add(push.Time);
                }
            }
            // rush to pylon
            phaseEnds.AddRange(log.CombatData.GetAnimatedCastData(BatteringBlitz).Select(x => x.Time).ToList());
            phaseEnds.Add(log.FightData.FightEnd);
            // tp to middle after pylon destruction
            phaseStarts.AddRange(log.CombatData.GetAnimatedCastData(PeerlessQadimTPCenter).Select(x => x.EndTime));
            // There should be at least as many starts as ends, otherwise skip phases
            if (phaseEnds.Count < phaseStarts.Count)
            {
                return phases;
            }
            for (int i = 0; i < phaseStarts.Count; i++)
            {
                var phase = new PhaseData(phaseStarts[i], phaseEnds[i], "Phase " + (i + 1));
                phase.AddTarget(mainTarget);
                phases.Add(phase);
            }
            // intermission phase never finished, add a "dummy" log end
            if (phaseEnds.Count - 1 == phaseStarts.Count)
            {
                phaseStarts.Add(log.FightData.FightEnd);
            }
            // There should be as many ends as starts, otherwise anomaly, skip intermission phases
            if (phaseEnds.Count != phaseStarts.Count)
            {
                return phases;
            }
            string[] intermissionNames = { "Magma Drop 1", "Magma Drop 2", "North Pylon", "SouthWest Pylon", "SouthEast Pylon" };
            bool skipNames = intermissionNames.Length < phaseEnds.Count - 1;
            for (int i = 0; i < phaseEnds.Count - 1; i++)
            {
                var phase = new PhaseData(phaseEnds[i], Math.Min(phaseStarts[i + 1], log.FightData.FightEnd), skipNames ? "Intermission " + (i + 1) : intermissionNames[i]);
                phase.AddTarget(mainTarget);
                phases.Add(phase);
            }
            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/Q5R4R6q.png",
                            (1000, 1000),
                            (-968, 7480, 4226, 12676)/*,
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970)*/);
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, 0, log.FightData.FightEnd);
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.PeerlessQadim:
                    var cataCycle = cls.Where(x => x.SkillId == 56329).ToList();
                    var forceOfHavoc = cls.Where(x => x.SkillId == 56017).ToList();
                    var forceOfRetal = cls.Where(x => x.SkillId == ForceOfRetaliationCast).ToList();
                    var etherStrikes = cls.Where(x => x.SkillId == 56012 || x.SkillId == 56653).ToList();
                    var causticChaos = cls.Where(x => x.SkillId == 56332).ToList();
                    var expoReperc = cls.Where(x => x.SkillId == 56223).ToList();
                    foreach (AbstractCastEvent c in cataCycle)
                    {
                        int magmaRadius = 850;
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        Point3D pylonPosition = replay.PolledPositions.LastOrDefault(x => x.Time <= end);
                        replay.Decorations.Add(new CircleDecoration(true, 0, magmaRadius, (start, end), "rgba(255, 50, 50, 0.15)", new PositionConnector(pylonPosition)));
                        replay.Decorations.Add(new CircleDecoration(true, end, magmaRadius, (start, end), "rgba(255, 50, 50, 0.25)", new PositionConnector(pylonPosition)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, magmaRadius, (end, (int)log.FightData.FightEnd), "rgba(255, 50, 0, 0.5)", new PositionConnector(pylonPosition)));
                    }
                    foreach (AbstractCastEvent c in forceOfHavoc)
                    {
                        int roadLength = 2400;
                        int roadWidth = 360;
                        int hitboxOffset = 200;
                        int subdivisions = 100;
                        int rollOutTime = 3250;
                        start = (int)c.Time;
                        int preCastTime = 1500;
                        int duration = 22500;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        Point3D position = replay.Positions.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null && position != null)
                        {
                            int direction = (int)(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI);
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, roadLength, roadWidth, direction, roadLength / 2 + 200, (start, start + preCastTime), "rgba(255, 0, 0, 0.1)", new PositionConnector(position)));
                            for (int i = 0; i < subdivisions; i++)
                            {
                                replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, roadLength/subdivisions, roadWidth, direction, (int)((i + 0.5) * roadLength / subdivisions + hitboxOffset), (start + preCastTime + i * (rollOutTime / subdivisions), start + preCastTime + i * (rollOutTime / subdivisions) + duration), "rgba(143, 0, 179, 0.6)", new PositionConnector(position)));
                            }
                        }
                    }
                    foreach (AbstractCastEvent c in forceOfRetal)
                    {
                        int radius = 650;
                        double radiusIncrement = 433.3;
                        int preCastTime = 1800;
                        int timeBetweenCascades = 200;
                        int cascades = 5;
                        start = (int)c.Time + 1400;
                        Point3D position = replay.Positions.LastOrDefault(x => x.Time <= start + 1000);
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start, start + preCastTime), "rgba(255, 220, 50, 0.15)", new PositionConnector(position)));
                        replay.Decorations.Add(new CircleDecoration(true, start + preCastTime, radius, (start, start + preCastTime), "rgba(255, 220, 50, 0.25)", new PositionConnector(position)));
                        for (int i = 0; i < cascades; i++)
                        {
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, radius + (int)(radiusIncrement * i), radius + (int)(radiusIncrement * (i + 1)), (start + preCastTime + timeBetweenCascades * i, start + preCastTime + timeBetweenCascades * (i + 1)), "rgba(30, 30, 30, 0.5)", new PositionConnector(position)));
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, radius + (int)(radiusIncrement * i), radius + (int)(radiusIncrement * (i + 1)), (start + preCastTime + timeBetweenCascades * (i + 1), start + preCastTime + timeBetweenCascades * (i + 2)), "rgba(50, 20, 50, 0.25)", new PositionConnector(position)));
                        }
                    }
                    foreach (AbstractCastEvent c in etherStrikes)
                    {
                        int coneRadius = 2600;
                        int coneAngle = 60;
                        start = (int)c.Time;
                        end = start + 250;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 300);
                        replay.Decorations.Add(new PieDecoration(false, 0, coneRadius, facing, coneAngle, (start, end), "rgba(255, 100, 0, 0.30)", new AgentConnector(target)));
                        replay.Decorations.Add(new PieDecoration(true, 0, coneRadius, facing, coneAngle, (start, end), "rgba(255, 100, 0, 0.1)", new AgentConnector(target)));
                    }
                    foreach (AbstractCastEvent c in causticChaos)
                    {
                        double acceleration = c.Acceleration;
                        double ratio = 1.0;
                        if (acceleration > 0)
                        {
                            ratio = acceleration * 0.5 + 1;
                        }
                        else
                        {
                            ratio = acceleration * 0.6 + 1;
                        }
                        int chaosLength = 2600;
                        int chaosWidth = 100;
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        int aimTime = (int)((double)c.ExpectedDuration*ratio);
                        replay.Decorations.Add(new FacingDecoration((0, end), new AgentConnector(target), replay.PolledRotations));
                        replay.Decorations.Add(new FacingRectangleDecoration((start, end), new AgentConnector(target), replay.PolledRotations, chaosLength, chaosWidth, chaosLength / 2, "rgba(255,100,0,0.3)"));
                        if (end > start + aimTime)
                        {
                            replay.Decorations.Add(new FacingRectangleDecoration((start + aimTime,end), new AgentConnector(target), replay.PolledRotations, chaosLength, chaosWidth, chaosLength / 2, "rgba(100,100,100,0.7)"));
                        }
                    }
                    foreach (AbstractCastEvent c in expoReperc)
                    {
                        int radius = 650;
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        Point3D position = replay.Positions.LastOrDefault(x => x.Time <= start + 1000);
                        replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start, end), "rgba(255, 220, 0, 0.15)", new PositionConnector(position)));
                        replay.Decorations.Add(new CircleDecoration(true, end, radius, (start, end), "rgba(255, 220, 50, 0.25)", new PositionConnector(position)));

                        foreach (NPC pylon in TrashMobs.Where(x => x.ID == 21962))
                        {
                            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start, end), "rgba(255, 220, 0, 0.15)", new AgentConnector(pylon)));
                            replay.Decorations.Add(new CircleDecoration(true, end, radius, (start, end), "rgba(255, 220, 50, 0.25)", new AgentConnector(pylon)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.EntropicDistortion:
                    //sapping surge, red tether
                    List<AbstractBuffEvent> sappingSurge = GetFilteredList(log.CombatData, SappingSurge, target, true, true);
                    int surgeStart = 0;
                    AbstractSingleActor source = null;
                    foreach (AbstractBuffEvent c in sappingSurge)
                    {
                        if (c is BuffApplyEvent)
                        {
                            AbstractSingleActor qadim = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.PeerlessQadim);
                            surgeStart = (int)c.Time;
                            source = (AbstractSingleActor)log.PlayerList.FirstOrDefault(x => x.AgentItem == c.CreditedBy) ?? qadim;
                        }
                        else
                        {
                            int surgeEnd = (int)c.Time;
                            if (source != null)
                            {
                                replay.Decorations.Add(new LineDecoration(0, (surgeStart, surgeEnd), "rgba(255, 0, 0, 0.3)", new AgentConnector(target), new AgentConnector(source)));
                            }
                        }
                    }
                    Point3D firstEntropicPosition = replay.PolledPositions.FirstOrDefault();
                    if (firstEntropicPosition != null)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 300, (start - 5000, start), "rgba(255, 0, 0, 0.4)", new PositionConnector(firstEntropicPosition)));
                        replay.Decorations.Add(new CircleDecoration(true, start, 300, (start - 5000, start), "rgba(255, 0, 0, 0.4)", new PositionConnector(firstEntropicPosition)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.BigKillerTornado:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 450, (start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.Pylon1:
                    break;
                case (int)ArcDPSEnums.TrashID.Pylon2:
                    break;
                case (int)ArcDPSEnums.TrashID.EnergyOrb:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start, end), "rgba(0, 255, 0, 0.3)", new AgentConnector(target)));
                    break;
                default:
                    break;
            }

        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            // fixated
            List<AbstractBuffEvent> fixated = GetFilteredList(log.CombatData, FixatedQadimThePeerless, p, true, true);
            int fixatedStart = 0;
            foreach (AbstractBuffEvent c in fixated)
            {
                if (c is BuffApplyEvent)
                {
                    fixatedStart = Math.Max((int)c.Time, 0);
                }
                else
                {
                    int fixatedEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(true, 0, 120, (fixatedStart, fixatedEnd), "rgba(255, 80, 255, 0.3)", new AgentConnector(p)));
                }
            }
            // Chaos Corrosion
            List<AbstractBuffEvent> chaosCorrosion = GetFilteredList(log.CombatData, ChaosCorrosion, p, true, true);
            int corrosionStart = 0;
            foreach (AbstractBuffEvent c in chaosCorrosion)
            {
                if (c is BuffApplyEvent)
                {
                    corrosionStart = (int)c.Time;
                }
                else
                {
                    int corrosionEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(true, 0, 100, (corrosionStart, corrosionEnd), "rgba(80, 80, 80, 0.3)", new AgentConnector(p)));
                }
            }
            // Critical Mass, debuff while carrying an orb
            List<AbstractBuffEvent> criticalMass = GetFilteredList(log.CombatData, CriticalMass, p, true, true);
            int criticalMassStart = 0;
            foreach (AbstractBuffEvent c in criticalMass)
            {
                if (c is BuffApplyEvent)
                {
                    criticalMassStart = (int)c.Time;
                }
                else
                {
                    int criticalMassEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(false, 0, 200, (criticalMassStart, criticalMassEnd), "rgba(255, 0, 0, 0.3)", new AgentConnector(p)));
                }
            }
            // Magma drop
            List<AbstractBuffEvent> magmaDrop = GetFilteredList(log.CombatData, MagmaDrop, p, true, true);
            int magmaDropStart = 0;
            int magmaRadius = 420;
            int magmaOffset = 4000;
            string[] magmaColors = { "255, 215, 0", "255, 130, 50" };
            int magmaColor = 0;
            foreach (AbstractBuffEvent c in magmaDrop)
            {
                if (c is BuffApplyEvent)
                {
                    magmaDropStart = (int)c.Time;
                }
                else
                {
                    int magmaDropEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(true, 0, magmaRadius, (magmaDropStart, magmaDropEnd), "rgba(255, 50, 0, 0.15)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, magmaDropEnd, magmaRadius, (magmaDropStart, magmaDropEnd), "rgba(255, 50, 0, 0.25)", new AgentConnector(p)));
                    Point3D magmaNextPos = replay.PolledPositions.FirstOrDefault(x => x.Time >= magmaDropEnd);
                    Point3D magmaPrevPos = replay.PolledPositions.LastOrDefault(x => x.Time <= magmaDropEnd);
                    if (magmaNextPos != null || magmaPrevPos != null)
                    {
                        string colorToUse = magmaColors[magmaColor];
                        magmaColor = (magmaColor + 1) % 2;
                        replay.Decorations.Add(new CircleDecoration(true, 0, magmaRadius, (magmaDropEnd, magmaDropEnd + magmaOffset), "rgba("+ colorToUse + ", 0.15)", new InterpolatedPositionConnector(magmaPrevPos, magmaNextPos, magmaDropEnd)));
                        replay.Decorations.Add(new CircleDecoration(true, magmaDropEnd + magmaOffset, magmaRadius, (magmaDropEnd, magmaDropEnd + magmaOffset), "rgba(" + colorToUse + ", 0.25)", new InterpolatedPositionConnector(magmaPrevPos, magmaNextPos, magmaDropEnd)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, magmaRadius, (magmaDropEnd + magmaOffset, (int)log.FightData.FightEnd), "rgba(" + colorToUse + ", 0.5)", new InterpolatedPositionConnector(magmaPrevPos, magmaNextPos, magmaDropEnd)));
                    }
                }

            }
            //sapping surge, bad red tether
            List<AbstractBuffEvent> sappingSurge = GetFilteredList(log.CombatData, SappingSurge, p, true, true);
            int surgeStart = 0;
            AbstractSingleActor source = null;
            foreach (AbstractBuffEvent c in sappingSurge)
            {
                if (c is BuffApplyEvent)
                {
                    AbstractSingleActor qadim = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.PeerlessQadim);
                    surgeStart = (int)c.Time;
                    source = (AbstractSingleActor)log.PlayerList.FirstOrDefault(x => x.AgentItem == c.CreditedBy) ?? qadim;
                }
                else
                {
                    int surgeEnd = (int)c.Time;
                    if (source != null)
                    {
                        replay.Decorations.Add(new LineDecoration(0, (surgeStart, surgeEnd), "rgba(255, 0, 0, 0.4)", new AgentConnector(p), new AgentConnector(source)));
                    }
                }
            }
            // kinetic abundance, good (blue) tether
            List<AbstractBuffEvent> kineticAbundance = GetFilteredList(log.CombatData, KineticAbundance, p, true, true);
            int kinStart = 0;
            AbstractSingleActor kinSource = null;
            foreach (AbstractBuffEvent c in kineticAbundance)
            {
                if (c is BuffApplyEvent)
                {
                    kinStart = (int)c.Time;
                    //kinSource = log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By);
                    kinSource = (AbstractSingleActor)log.PlayerList.FirstOrDefault(x => x.AgentItem == c.CreditedBy) ?? TrashMobs.FirstOrDefault(x => x.AgentItem == c.CreditedBy);
                }
                else
                {
                    int kinEnd = (int)c.Time;
                    if (kinSource != null)
                    {
                        replay.Decorations.Add(new LineDecoration(0, (kinStart, kinEnd), "rgba(0, 0, 255, 0.4)", new AgentConnector(p), new AgentConnector(kinSource)));
                    }
                }
            }
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.PeerlessQadim);
            if (target == null)
            {
                throw new MissingKeyActorsException("Peerless Qadim not found");
            }
            return (target.GetHealth(combatData) > 48e6) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
