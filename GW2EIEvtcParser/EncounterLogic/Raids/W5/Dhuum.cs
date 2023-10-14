using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Dhuum : HallOfChains
    {
        private bool _isBugged;
        private int _greenStart;

        public Dhuum(int triggerID) : base(triggerID)
        {
            _isBugged = false;
            _greenStart = 0;
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(HatefulEphemera, "Hateful Ephemera", new MechanicPlotlySetting(Symbols.Square,Colors.LightOrange), "Golem","Hateful Ephemera (Golem AoE dmg)", "Golem Dmg",0),
            new PlayerDstHitMechanic(ArcingAfflictionHit, "Arcing Affliction", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Bomb dmg","Arcing Affliction (Bomb) hit", "Bomb dmg",0),
            new PlayerDstBuffApplyMechanic(ArcingAffliction, "Arcing Affliction", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Bomb","Arcing Affliction (Bomb) application", "Bomb",0),
            new PlayerDstBuffRemoveMechanic(ArcingAffliction, "Arcing Affliction", new MechanicPlotlySetting(Symbols.Diamond,Colors.Red), "Bomb Trig","Arcing Affliction (Bomb) manualy triggered", "Bomb Triggered",0).UsingChecker((br, log) =>
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
                if (br.To.HasBuff(log, MortalCoilDhuum, br.Time))
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
            //new Mechanic(ResidualAffliction, "Residual Affliction", ParseEnum.BossIDS.Dhuum, new MechanicPlotlySetting(Symbols.StarDiamond,Colors.Yellow), "Bomb",0), //not needed, imho, applied at the same time as Arcing Affliction
            new PlayerSrcPlayerDstBuffApplyMechanic(DhuumShacklesBuff, "Soul Shackle", new MechanicPlotlySetting(Symbols.Diamond,Colors.Teal), "Shackles","Soul Shackle (Tether) application", "Shackles",10000),//  //also used for removal.
            new PlayerDstHitMechanic(DhuumShacklesHit, "Soul Shackle", new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Teal), "Shackles dmg","Soul Shackle (Tether) dmg ticks", "Shackles Dmg",0).UsingChecker((de,log) => de.HealthDamage > 0),
            new PlayerDstHitMechanic(ConeSlash, "Slash", new MechanicPlotlySetting(Symbols.TriangleUp,Colors.DarkGreen), "Cone","Boon ripping Cone Attack", "Cone",0),
            new PlayerDstHitMechanic(Cull, "Cull", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Teal), "Crack","Cull (Fearing Fissures)", "Cracks",0),
            new PlayerDstHitMechanic(PutridBomb, "Putrid Bomb", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "Mark","Necro Marks during Scythe attack", "Necro Marks",0),
            new PlayerDstHitMechanic(CataclysmicCycle, "Cataclysmic Cycle", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Suck dmg","Damage when sucked to close to middle", "Suck dmg",0),
            new PlayerDstHitMechanic(DeathMark, "Death Mark", new MechanicPlotlySetting(Symbols.Hexagon,Colors.LightOrange), "Dip","Lesser Death Mark hit (Dip into ground)", "Dip AoE",0),
            new PlayerDstHitMechanic(GreaterDeathMark, "Greater Death Mark", new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "KB dmg","Knockback damage during Greater Deathmark (mid port)", "Knockback dmg",0),
          //  new Mechanic(MortalCoilDhuum, "Mortal Coil", ParseEnum.BossIDS.Dhuum, new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "Green Orbs",
            new PlayerDstBuffApplyMechanic(FracturedSpirit, "Fractured Spirit", new MechanicPlotlySetting(Symbols.Square,Colors.Green), "Orb CD","Applied when taking green", "Green port",0),
            //new SkillOnPlayerMechanic(EndersEchoDamage , "Echo's Damage", new MechanicPlotlySetting(Symbols.Square,Color.Red), "Echo","Damaged by Ender's Echo (pick up)", "Ender's Echo",5000),
            new PlayerDstBuffApplyMechanic(EchosPickup, "Echo's Pick up", new MechanicPlotlySetting(Symbols.Square,Colors.Red), "Echo PU","Picked up by Ender's Echo", "Ender's Pick up", 3000),
            new PlayerDstBuffApplyMechanic(SourcePureOblivionBuff, "Pure Oblivion", new MechanicPlotlySetting(Symbols.HexagonOpen, Colors.Black), "10%", "Lifted by Pure Oblivion", "Pure Oblivion (10%)", 0),
            new PlayerDstBuffRemoveMechanic(EchosPickup, "Freed from Echo", new MechanicPlotlySetting(Symbols.Square,Colors.Blue), "F Echo","Freed from Ender's Echo", "Freed from Echo", 0).UsingChecker( (br,log) => !log.CombatData.GetDeadEvents(br.To).Where(x => Math.Abs(x.Time - br.Time) <= 150).Any())
            });
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
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(DeathlyAura, DeathlyAura),
            };
        }

        private static void ComputeFightPhases(List<PhaseData> phases, IReadOnlyList<AbstractCastEvent> castLogs, long fightDuration, long start)
        {
            AbstractCastEvent shield = castLogs.FirstOrDefault(x => x.SkillId == MajorSoulSplit);
            // Dhuum brought down to 10%
            if (shield != null)
            {
                long end = shield.Time;
                phases.Add(new PhaseData(start, end, "Dhuum Fight"));
                AbstractCastEvent firstDamageable = castLogs.FirstOrDefault(x => x.SkillId == DhuumVulnerableLast10Percent && x.Time >= end);
                // ritual started
                if (firstDamageable != null)
                {
                    phases.Add(new PhaseData(end, firstDamageable.Time, "Shielded Dhuum")
                    {
                        CanBeSubPhase = false
                    });
                    phases.Add(new PhaseData(firstDamageable.Time, fightDuration, "Ritual"));
                } else
                {
                    phases.Add(new PhaseData(end, fightDuration, "Shielded Dhuum")
                    {
                        CanBeSubPhase = false
                    });
                }
            }
        }

        private static List<PhaseData> GetInBetweenSoulSplits(ParsedEvtcLog log, AbstractSingleActor dhuum, long mainStart, long mainEnd, bool hasRitual)
        {
            IReadOnlyList<AbstractCastEvent> cls = dhuum.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            var cataCycles = cls.Where(x => x.SkillId == CataclysmicCycle).ToList();
            var gDeathmarks = cls.Where(x => x.SkillId == GreaterDeathMark).ToList();
            if (gDeathmarks.Count < cataCycles.Count)
            {
                // anomaly, don't do sub phases
                return new List<PhaseData>();
            }
            var phases = new List<PhaseData>();
            long start = mainStart;
            long end = 0;
            int i = 0;
            foreach (AbstractCastEvent cataCycle in cataCycles)
            {
                AbstractCastEvent gDeathmark = gDeathmarks[i];
                end = Math.Min(gDeathmark.Time, mainEnd);
                long soulsplitEnd = Math.Min(cataCycle.EndTime, mainEnd);
                ++i;
                phases.Add(new PhaseData(start, end, "Pre-Soulsplit " + i));
                phases.Add(new PhaseData(end, soulsplitEnd, "Soulsplit " + i)
                {
                    CanBeSubPhase = false
                }); ;
                start = cataCycle.EndTime;
            }
            phases.Add(new PhaseData(start, mainEnd, hasRitual ? "Pre-Ritual" : "Pre-Wipe"));
            return phases;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            long fightDuration = log.FightData.FightEnd;
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor dhuum = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Dhuum));
            if (dhuum == null)
            {
                throw new MissingKeyActorsException("Dhuum not found");
            }
            phases[0].AddTarget(dhuum);
            if (!requirePhases)
            {
                return phases;
            }
            // Sometimes the pre event is not in the evtc
            IReadOnlyList<AbstractCastEvent> castLogs = dhuum.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            IReadOnlyList<AbstractCastEvent> dhuumCast = dhuum.GetCastEvents(log, log.FightData.FightStart, 20000);
            if (dhuumCast.Any(x => !(x is InstantCastEvent) && x.SkillId != WeaponDraw && x.SkillId != WeaponStow))
            {
                // full fight does not contain the pre event
                ComputeFightPhases(phases, castLogs, fightDuration, 0);
                _isBugged = true;
            }
            else
            {
                // full fight contains the pre event
                AbstractBuffEvent invulDhuum = log.CombatData.GetBuffData(Determined762).FirstOrDefault(x => x is BuffRemoveManualEvent && x.To == dhuum.AgentItem && x.Time > 115000);
                // pre event done
                if (invulDhuum != null)
                {
                    long end = invulDhuum.Time;
                    phases.Add(new PhaseData(0, end, "Pre Event"));
                    phases.Add(new PhaseData(end, fightDuration, "Main Fight") { CanBeSubPhase = false });
                    ComputeFightPhases(phases, castLogs, fightDuration, end);
                }
            }
            bool hasRitual = phases.Last().Name == "Ritual";
            // present if not bugged and pre-event done
            PhaseData mainFight = phases.Find(x => x.Name == "Main Fight");
            // if present, Dhuum was at least at 10%
            PhaseData dhuumFight = phases.Find(x => x.Name == "Dhuum Fight");
            if (mainFight != null)
            {
                mainFight.CanBeSubPhase = dhuumFight == null;
                // from pre event end to 10% or fight end if 10% not achieved
                phases.AddRange(GetInBetweenSoulSplits(log, dhuum, mainFight.Start, dhuumFight != null ? dhuumFight.End : mainFight.End, hasRitual));
            }
            else if (_isBugged)
            {
                // from start to 10% or fight end if 10% not achieved
                phases.AddRange(GetInBetweenSoulSplits(log, dhuum, 0, dhuumFight != null ? dhuumFight.End : fightDuration, hasRitual));
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].AddTarget(dhuum);
            }
            return phases;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Dhuum,
                (int)ArcDPSEnums.TrashID.UnderworldReaper,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Echo,
                ArcDPSEnums.TrashID.Enforcer,
                ArcDPSEnums.TrashID.Messenger,
                ArcDPSEnums.TrashID.Deathling,
                ArcDPSEnums.TrashID.DhuumDesmina
            };
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            long startToUse = GetGenericFightOffset(fightData);
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogStartNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                AgentItem messenger = agentData.GetNPCsByID(ArcDPSEnums.TrashID.Messenger).MinBy(x => x.FirstAware);
                if (messenger != null)
                {
                    startToUse = messenger.FirstAware;
                }
            }
            return startToUse;
        }

        private static readonly Dictionary<Point3D, int> ReapersToGreen = new Dictionary<Point3D, int>
        {
            { new Point3D(16897, 1225, -6215), 0 },
            { new Point3D(16853, 65, -6215), 1 },
            { new Point3D(15935, -614, -6215), 2 },
            { new Point3D(14830, -294, -6215), 3 },
            { new Point3D(14408, 764, -6215), 4 },
            { new Point3D(14929, 1762, -6215), 5 },
            { new Point3D(16062, 1991, -6215), 6 },
        };

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            // TODO: correct position
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Dhuum:
                    var deathmark = cls.Where(x => x.SkillId == DeathMark).ToList();
                    AbstractCastEvent majorSplit = cls.FirstOrDefault(x => x.SkillId == MajorSoulSplit);
                    foreach (AbstractCastEvent c in deathmark)
                    {
                        start = (int)c.Time;
                        int zoneActive = start + 1550;
                        int zoneDeadly = zoneActive + 6000; //point where the zone becomes impossible to walk through unscathed
                        int zoneEnd = zoneActive + 120000;
                        int radius = 450;
                        if (majorSplit != null)
                        {
                            zoneEnd = Math.Min(zoneEnd, (int)majorSplit.Time);
                            zoneDeadly = Math.Min(zoneDeadly, (int)majorSplit.Time);
                        }
                        int spellCenterDistance = 200; //hitbox radius
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 3000);
                        Point3D targetPosition = replay.PolledPositions.LastOrDefault(x => x.Time <= start + 3000);
                        if (facing != null && targetPosition != null)
                        {
                            var position = new Point3D(targetPosition.X + (facing.X * spellCenterDistance), targetPosition.Y + (facing.Y * spellCenterDistance), targetPosition.Z);
                            replay.Decorations.Add(new CircleDecoration(true, zoneActive, radius, (start, zoneActive), "rgba(200, 255, 100, 0.5)", new PositionConnector(position)));
                            replay.Decorations.Add(new CircleDecoration(false, 0, radius, (start, zoneActive), "rgba(200, 255, 100, 0.5)", new PositionConnector(position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (zoneActive, zoneDeadly), "rgba(200, 255, 100, 0.5)", new PositionConnector(position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (zoneDeadly, zoneEnd), "rgba(255, 100, 0, 0.5)", new PositionConnector(position)));

                        }
                    }
                    var cataCycle = cls.Where(x => x.SkillId == CataclysmicCycle).ToList();
                    foreach (AbstractCastEvent c in cataCycle)
                    {
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        replay.Decorations.Add(new CircleDecoration(true, end, 300, (start, end), "rgba(255, 150, 0, 0.7)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, 300, (start, end), "rgba(255, 150, 0, 0.5)", new AgentConnector(target)));
                    }
                    var slash = cls.Where(x => x.SkillId == ConeSlash).ToList();
                    foreach (AbstractCastEvent c in slash)
                    {
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        replay.Decorations.Add(new PieDecoration(false, 0, 850, 60, (start, end), "rgba(255, 150, 0, 0.5)", new AgentConnector(target)).UsingRotationConnector(new AngleConnector(facing)));
                    }

                    if (majorSplit != null)
                    {
                        start = (int)majorSplit.Time;
                        end = (int)log.FightData.FightEnd;
                        replay.Decorations.Add(new CircleDecoration(true, 0, 320, (start, end), "rgba(0, 180, 255, 0.2)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.DhuumDesmina:
                    break;
                case (int)ArcDPSEnums.TrashID.Echo:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 120, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.Enforcer:
                    break;
                case (int)ArcDPSEnums.TrashID.Messenger:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(255, 125, 0, 0.5)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.Deathling:
                    break;
                case (int)ArcDPSEnums.TrashID.UnderworldReaper:
                    var stealths = target.GetBuffStatus(log, Stealth, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                    foreach (Segment seg in stealths)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, seg, "rgba(80, 80, 80, 0.3)", new AgentConnector(target)));
                    }
                    if (!_isBugged)
                    {
                        if (_greenStart == 0)
                        {
                            AbstractBuffEvent greenTaken = log.CombatData.GetBuffData(FracturedSpirit).Where(x => x is BuffApplyEvent).FirstOrDefault();
                            if (greenTaken != null)
                            {
                                _greenStart = (int)greenTaken.Time - 5000;
                            }
                            else
                            {
                                _greenStart = 30600;
                            }
                        }
                        Point3D pos = replay.Positions.FirstOrDefault();
                        if (replay.Positions.Count > 1)
                        {
                            replay.Trim(replay.Positions.LastOrDefault().Time, replay.TimeOffsets.end);
                        }
                        if (pos == null)
                        {
                            break;
                        }
                        int reaper = -1;
                        foreach (KeyValuePair<Point3D, int> pair in ReapersToGreen)
                        {
                            if (pair.Key.DistanceToPoint(pos) < 10)
                            {
                                reaper = pair.Value;
                                break;
                            }
                        }
                        if (reaper == -1)
                        {
                            break;
                        }
                        int multiplier = 210000;
                        int gStart = _greenStart + reaper * 30000;
                        var greens = new List<int>() {
                            gStart,
                            gStart + multiplier,
                            gStart + 2 * multiplier
                        };
                        foreach (int gstart in greens)
                        {
                            int gend = gstart + 5000;
                            replay.Decorations.Add(new CircleDecoration(true, 0, 240, (gstart, gend), "rgba(0, 255, 0, 0.2)", new AgentConnector(target)));
                            replay.Decorations.Add(new CircleDecoration(true, gend, 240, (gstart, gend), "rgba(0, 255, 0, 0.2)", new AgentConnector(target)));
                        }
                    }
                    break;
                default:
                    break;
            }

        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            // spirit transform
            var spiritTransform = log.CombatData.GetBuffData(FracturedSpirit).Where(x => x.To == p.AgentItem && x is BuffApplyEvent).ToList();
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Dhuum));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Dhuum not found");
            }
            foreach (AbstractBuffEvent c in spiritTransform)
            {
                int duration = 15000;
                HealthUpdateEvent hpUpdate = log.CombatData.GetHealthUpdateEvents(mainTarget.AgentItem).FirstOrDefault(x => x.Time > c.Time);
                if (hpUpdate != null && hpUpdate.HPPercent < 10.50)
                {
                    duration = 30000;
                }
                AbstractBuffEvent removedBuff = log.CombatData.GetBuffRemoveAllData(MortalCoilDhuum).FirstOrDefault(x => x.To == p.AgentItem && x.Time > c.Time && x.Time < c.Time + duration);
                int start = (int)c.Time;
                int end = start + duration;
                if (removedBuff != null)
                {
                    end = (int)removedBuff.Time;
                }
                replay.Decorations.Add(new CircleDecoration(true, 0, 100, (start, end), "rgba(0, 50, 200, 0.3)", new AgentConnector(p)));
                replay.Decorations.Add(new CircleDecoration(true, start + duration, 100, (start, end), "rgba(0, 50, 200, 0.5)", new AgentConnector(p)));
            }
            // bomb
            var bombDhuum = p.GetBuffStatus(log, ArcingAffliction, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            foreach (Segment seg in bombDhuum)
            {
                replay.Decorations.Add(new CircleDecoration(true, 0, 100, seg, "rgba(80, 180, 0, 0.3)", new AgentConnector(p)));
                replay.Decorations.Add(new CircleDecoration(true, (int)seg.Start + 13000, 100, seg, "rgba(80, 180, 0, 0.5)", new AgentConnector(p)));
                replay.AddOverheadIcon(seg, p, ParserIcons.BombTimerFullOverhead);
            }
            // shackles connection
            List<AbstractBuffEvent> shackles = GetFilteredList(log.CombatData, new long[] { DhuumShacklesBuff, DhuumShacklesBuff2 }, p, true, true);
            replay.AddTether(shackles, "rgba(0, 255, 255, 0.5)");

            // shackles damage (identical to the connection for now, not yet properly distinguishable from the pure connection, further investigation needed due to inconsistent behavior (triggering too early, not triggering the damaging skill though)
            // shackles start with buff 47335 applied from one player to the other, this is switched over to buff 48591 after mostly 2 seconds, sometimes later. This is switched to 48042 usually 4 seconds after initial application and the damaging skill 47164 starts to deal damage from that point on.
            // Before that point, 47164 is only logged when evaded/blocked, but doesn't deal damage. Further investigation needed.
            List<AbstractBuffEvent> shacklesDmg = GetFilteredList(log.CombatData, DhuumDamagingShacklesBuff, p, true, true);
            replay.AddTether(shacklesDmg, "rgba(255, 200, 0, 0.5)");
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Dhuum));
            if (target == null)
            {
                throw new MissingKeyActorsException("Dhuum not found");
            }
            return (target.GetHealth(combatData) > 35e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        }
    }
}
