using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Sabir : TheKeyOfAhdashim
    {
        public Sabir(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                new SkillOnPlayerMechanic(56202, "Dire Drafts", new MechanicPlotlySetting(Symbols.Circle,Colors.Orange), "B.Tornado", "Hit by big tornado", "Big Tornado Hit", 500, (de, log) => de.HasDowned || de.HasKilled),
                new SkillOnPlayerMechanic(56643, "Unbridled Tempest", new MechanicPlotlySetting(Symbols.Hexagon,Colors.Pink), "Shockwave", "Hit by Shockwave", "Shockwave Hit", 0, (de, log) => de.HasDowned || de.HasKilled),
                new SkillOnPlayerMechanic(56372, "Fury of the Storm", new MechanicPlotlySetting(Symbols.Circle,Colors.Purple), "Arena AoE", "Hit by Arena wide AoE", "Arena AoE hit", 0, (de, log) => de.HasDowned || de.HasKilled ),
                new HitOnPlayerMechanic(56055, "Dynamic Deterrent", new MechanicPlotlySetting(Symbols.YUpOpen,Colors.Pink), "Pushed", "Pushed by rotating breakbar", "Pushed", 0, (de, log) => !de.To.HasBuff(log, SkillIDs.Stability, de.Time - ParserHelper.ServerDelayConstant)),
                new EnemyCastStartMechanic(56349, "Regenerative Breakbar", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.Magenta), "Reg.Breakbar","Regenerating Breakbar", "Regenerative Breakbar", 0),
                new EnemyBuffRemoveMechanic(IonShield, "Regenerative Breakbar Broken", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.DarkTeal), "Reg.Breakbar Brkn", "Regenerative Breakbar Broken", "Regenerative Breakbar Broken", 2000),
                new EnemyBuffApplyMechanic(RepulsionField, "Rotating Breakbar", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Magenta), "Rot.Breakbar","Rotating Breakbar", "Rotating Breakbar", 0),
                new EnemyBuffRemoveMechanic(RepulsionField, "Rotating Breakbar Broken", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "Rot.Breakbar Brkn","Rotating Breakbar Broken", "Rotating Breakbar Broken", 0),
            });
            // rotating cc 56403
            // interesting stuff 56372 (big AoE?)
            Extension = "sabir";
            Icon = "https://wiki.guildwars2.com/images/f/fc/Mini_Air_Djinn.png";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>()
            {
                ArcDPSEnums.TrashID.ParalyzingWisp,
                ArcDPSEnums.TrashID.VoltaicWisp,
                ArcDPSEnums.TrashID.SmallKillerTornado,
                ArcDPSEnums.TrashID.SmallJumpyTornado,
                ArcDPSEnums.TrashID.BigKillerTornado
            };
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(56523, 56523, InstantCastFinder.DefaultICD), // Bolt Break
            };
        }

        internal override List<AbstractHealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, SkillData skillData)
        {
            NegateDamageAgainstBarrier(combatData, Targets.Select(x => x.AgentItem).ToList());
            return new List<AbstractHealthDamageEvent>();
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Sabir);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Sabir not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            IReadOnlyList<AbstractCastEvent> cls = mainTarget.GetCastEvents(log, 0, log.FightData.FightEnd);
            var wallopingWinds = cls.Where(x => x.SkillId == 56094).ToList();
            long start = 0, end = 0;
            for (int i = 0; i < wallopingWinds.Count; i++)
            {
                AbstractCastEvent wallopinbWind = wallopingWinds[i];
                end = wallopinbWind.Time;
                var phase = new PhaseData(start, end, "Phase " + (i + 1));
                phase.AddTarget(mainTarget);
                phases.Add(phase);
                AbstractCastEvent nextAttack = cls.FirstOrDefault(x => x.Time >= wallopinbWind.EndTime && (x.SkillId == 56620 || x.SkillId == 56629 || x.SkillId == 56307));
                if (nextAttack == null)
                {
                    break;
                }
                start = nextAttack.Time;
                if (i == wallopingWinds.Count - 1)
                {
                    phase = new PhaseData(start, log.FightData.FightEnd, "Phase " + (i + 2));
                    phase.AddTarget(mainTarget);
                    phases.Add(phase);
                }
            }

            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/wesgoc6.png",
                            (1000, 910),
                            (-14122, 142, -9199, 4640)/*,
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970)*/);
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            List<AbstractBuffEvent> boltBreaks = GetFilteredList(log.CombatData, BoltBreak, p, true, true);
            int boltBreakStart = 0;
            foreach (AbstractBuffEvent c in boltBreaks)
            {
                if (c is BuffApplyEvent)
                {
                    boltBreakStart = (int)c.Time;
                }
                else
                {
                    int boltBreakEnd = (int)c.Time;
                    int radius = 180;
                    replay.Decorations.Add(new CircleDecoration(true, 0, radius, (boltBreakStart, boltBreakEnd), "rgba(255, 150, 0, 0.3)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, boltBreakEnd, radius, (boltBreakStart, boltBreakEnd), "rgba(255, 150, 0, 0.3)", new AgentConnector(p)));
                }
            }
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            int crStart = (int)replay.TimeOffsets.start;
            int crEnd = (int)replay.TimeOffsets.end;
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, 0, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Sabir:
                    List<AbstractBuffEvent> repulsionFields = GetFilteredList(log.CombatData, RepulsionField, target, true, true);
                    int repulsionFieldStart = 0;
                    foreach (AbstractBuffEvent c in repulsionFields)
                    {
                        if (c is BuffApplyEvent)
                        {
                            repulsionFieldStart = (int)c.Time;
                        }
                        else
                        {
                            int repulsionFieldEnd = (int)c.Time;
                            replay.Decorations.Add(new CircleDecoration(true, 0, 120, (repulsionFieldStart, repulsionFieldEnd), "rgba(80, 0, 255, 0.3)", new AgentConnector(target)));
                        }
                    }
                    List<AbstractBuffEvent> ionShields = GetFilteredList(log.CombatData, IonShield, target, true, true);
                    int ionShieldStart = 0;
                    foreach (AbstractBuffEvent c in ionShields)
                    {
                        if (c is BuffApplyEvent)
                        {
                            ionShieldStart = (int)c.Time;
                        }
                        else
                        {
                            int ionShieldEnd = (int)c.Time;
                            replay.Decorations.Add(new CircleDecoration(true, 0, 120, (ionShieldStart, ionShieldEnd), "rgba(0, 80, 255, 0.3)", new AgentConnector(target)));
                        }
                    }
                    //
                    var furyOfTheStorm = cls.Where(x => x.SkillId == 56372).ToList();
                    foreach (AbstractCastEvent c in furyOfTheStorm)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, (int)c.EndTime, 1200, ((int)c.Time, (int)c.EndTime), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    //
                    var unbridledTempest = cls.Where(x => x.SkillId == 56643).ToList();
                    foreach (AbstractCastEvent c in unbridledTempest)
                    {
                        int start = (int)c.Time;
                        int delay = 3000; // casttime 0 from skill def
                        int duration = 5000;
                        int radius = 1200;
                        Point3D targetPosition = replay.PolledPositions.LastOrDefault(x => x.Time <= start + 1000);
                        if (targetPosition != null)
                        {
                            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start, start + delay), "rgba(255, 100, 0, 0.2)", new PositionConnector(targetPosition)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, radius, (start + delay - 10, start + delay + 100), "rgba(255, 100, 0, 0.5)", new PositionConnector(targetPosition)));
                            replay.Decorations.Add(new CircleDecoration(false, start + duration, radius, (start + delay, start + duration), "rgba(255, 150, 0, 0.7)", new PositionConnector(targetPosition)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.BigKillerTornado:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 480, (crStart, crEnd), "rgba(255, 150, 0, 0.4)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.SmallKillerTornado:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 120, (crStart, crEnd), "rgba(255, 150, 0, 0.4)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.SmallJumpyTornado:
                case (int)ArcDPSEnums.TrashID.ParalyzingWisp:
                case (int)ArcDPSEnums.TrashID.VoltaicWisp:
                    break;
                default:
                    break;

            }
        }
        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // Find target
            AgentItem target = agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.Sabir).FirstOrDefault();
            if (target == null)
            {
                throw new MissingKeyActorsException("Sabir not found");
            }
            CombatItem enterCombat = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat && x.SrcMatchesAgent(target));
            return enterCombat != null ? enterCombat.Time : fightData.LogStart;
        }

        internal override FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Sabir);
            if (target == null)
            {
                throw new MissingKeyActorsException("Sabir not found");
            }
            return (target.GetHealth(combatData) > 32e6) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
