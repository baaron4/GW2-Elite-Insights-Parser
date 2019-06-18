using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;
using System.Drawing;

namespace LuckParser.Models.Logic
{
    public class PeerlessQadim : RaidLogic
    {
        public PeerlessQadim(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            });
            Extension = "prlqadim";
            IconUrl = "https://wiki.guildwars2.com/images/8/8b/Mini_Qadim_the_Peerless.png";
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>()
            {
                Pylon1,
                Pylon2,
                EntropicDistortion,
                BigKillerTornado
            };
        }

        public override List<AbstractBuffEvent> CreateCustomBuffEvents(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById)
        {
            List<AbstractBuffEvent> res = new List<AbstractBuffEvent>();
            if (buffsById.TryGetValue(56118, out var list))
            {
                Dictionary<AgentItem, List<AbstractBuffEvent>> sappingSurgeByDst = list.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
                foreach (var pair in sappingSurgeByDst.Where(x => x.Value.Exists(y => y is BuffRemoveSingleEvent)))
                {
                    List<AbstractBuffEvent> sglRemovals = pair.Value.Where(x => x is BuffRemoveSingleEvent).ToList();
                    foreach (AbstractBuffEvent sglRemoval in sglRemovals)
                    {
                        AbstractBuffEvent ba = pair.Value.LastOrDefault(x => x is BuffApplyEvent && Math.Abs(x.Time - sglRemoval.Time) < 5);
                        if (ba != null)
                        {
                            res.Add(new BuffRemoveAllEvent(sglRemoval.By, pair.Key, ba.Time - 1, int.MaxValue, ba.BuffSkill, 1, int.MaxValue));
                            res.Add(new BuffRemoveManualEvent(sglRemoval.By, pair.Key, ba.Time -1, int.MaxValue, ba.BuffSkill));
                        }
                    }
                }
            }
            return res;
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.PeerlessQadim);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            List<long> phaseStarts = new List<long>();
            List<long> phaseEnds = new List<long>();
            //
            List<AbstractBuffEvent> magmaDrops = log.CombatData.GetBoonData(56475).Where(x => x is BuffApplyEvent).ToList();
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
            List<AbstractCastEvent> pushes = log.CombatData.GetCastDataById(56405);
            if (pushes.Count > 0)
            {
                AbstractCastEvent push = pushes[0];
                phaseStarts.Add(push.Time + push.ActualDuration);
                foreach (long magmaDrop in phaseEnds)
                {
                    push = pushes.FirstOrDefault(x => x.Time >= magmaDrop);
                    if (push == null)
                    {
                        break;
                    }
                    phaseStarts.Add(push.Time + push.ActualDuration);
                }
            }
            // rush to pylon
            phaseEnds.AddRange(log.CombatData.GetCastDataById(56616).Select(x => x.Time).ToList());
            phaseEnds.Add(log.FightData.FightDuration);
            // tp to middle after pylon destruction
            phaseStarts.AddRange(log.CombatData.GetCastDataById(56375).Select(x => x.Time + x.ActualDuration));
            if (phaseEnds.Count < phaseStarts.Count)
            {
                return phases;
            }
            for (int i = 0; i < phaseStarts.Count; i++)
            {
                PhaseData phase = new PhaseData(phaseStarts[i], phaseEnds[i])
                {
                    Name = "Phase " + (i + 1)
                };
                phase.Targets.Add(mainTarget);
                phases.Add(phase);
            }
            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/PgkZMYE.png",
                            (6822, 6822),
                            (-968, 7480, 4226, 12676),
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970));
        }

        public override void ComputeTargetCombatReplayActors(Target target, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.PeerlessQadim:
                    List<AbstractCastEvent> cataCycle = cls.Where(x => x.SkillId == 56329).ToList();

                    foreach (AbstractCastEvent c in cataCycle)
                    {
                        int magmaRadius = 850;
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        Point3D pylonPosition = replay.PolledPositions.LastOrDefault(x => x.Time <= end);
                        replay.Actors.Add(new CircleActor(true, 0, magmaRadius, (start, end), "rgba(255, 220, 50, 0.15)", new PositionConnector(pylonPosition)));
                        replay.Actors.Add(new CircleActor(true, end, magmaRadius, (start, end), "rgba(255, 220, 50, 0.25)", new PositionConnector(pylonPosition)));
                        replay.Actors.Add(new CircleActor(true, 0, magmaRadius, (end, end + 10000000), "rgba(255, 220, 0, 0.5)", new PositionConnector(pylonPosition)));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }

        }

        public override void ComputeMobCombatReplayActors(Mob mob, ParsedLog log, CombatReplay replay)
        {
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (mob.ID)
            {
                case (ushort)EntropicDistortion:
                    //sapping surge, red tether
                    List<AbstractBuffEvent> sappingSurge = GetFilteredList(log.CombatData, 56118, mob, true);
                    int surgeStart = 0;
                    AbstractMasterActor source = null;
                    foreach (AbstractBuffEvent c in sappingSurge)
                    {
                        if (c is BuffApplyEvent)
                        {
                            Target qadim = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.PeerlessQadim);
                            surgeStart = (int)c.Time;
                            source = (AbstractMasterActor)log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By) ?? qadim;
                        }
                        else
                        {
                            int surgeEnd = (int)c.Time;
                            if (source != null)
                            {
                                replay.Actors.Add(new LineActor(0, (surgeStart, surgeEnd), "rgba(255, 0, 0, 0.3)", new AgentConnector(mob), new AgentConnector(source)));
                            }
                        }
                    }
                    break;
                case (ushort)BigKillerTornado:
                    replay.Actors.Add(new CircleActor(true, 0, 450, (start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(mob)));
                    break;
                case (ushort)Pylon1:
                    break;
                case (ushort)Pylon2:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");

            }
        }

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
            // fixated
            List<AbstractBuffEvent> fixated = GetFilteredList(log.CombatData, 56510, p, true);
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
                    replay.Actors.Add(new CircleActor(true, 0, 120, (fixatedStart, fixatedEnd), "rgba(255, 80, 255, 0.3)", new AgentConnector(p)));
                }
            }
            // Magma drop
            List<AbstractBuffEvent> magmaDrop = GetFilteredList(log.CombatData, 56475, p, true);
            int magmaDropStart = 0;
            int magmaRadius = 420;
            int magmaOffset = 4000;
            foreach (AbstractBuffEvent c in magmaDrop)
            {
                if (c is BuffApplyEvent)
                {
                    magmaDropStart = (int)c.Time;
                }
                else
                {
                    int magmaDropEnd = (int)c.Time;
                    replay.Actors.Add(new CircleActor(true, 0, magmaRadius, (magmaDropStart, magmaDropEnd), "rgba(255, 150, 0, 0.15)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleActor(true, magmaDropEnd, magmaRadius, (magmaDropStart, magmaDropEnd), "rgba(255, 150, 0, 0.25)", new AgentConnector(p)));
                    Point3D magmaNextPos = replay.PolledPositions.FirstOrDefault(x => x.Time >= magmaDropEnd);
                    Point3D magmaPrevPos = replay.PolledPositions.LastOrDefault(x => x.Time <= magmaDropEnd);
                    if (magmaNextPos != null || magmaPrevPos != null)
                    {
                        replay.Actors.Add(new CircleActor(true, 0, magmaRadius, (magmaDropEnd, magmaDropEnd + magmaOffset), "rgba(255, 220, 50, 0.15)", new InterpolatedPositionConnector(magmaPrevPos, magmaNextPos, magmaDropEnd)));
                        replay.Actors.Add(new CircleActor(true, magmaDropEnd + magmaOffset, magmaRadius, (magmaDropEnd, magmaDropEnd + magmaOffset), "rgba(255, 220, 50, 0.25)", new InterpolatedPositionConnector(magmaPrevPos, magmaNextPos, magmaDropEnd)));
                        replay.Actors.Add(new CircleActor(true, 0, magmaRadius, (magmaDropEnd+magmaOffset, magmaDropEnd + 10000000), "rgba(255, 220, 50, 0.5)", new InterpolatedPositionConnector(magmaPrevPos, magmaNextPos, magmaDropEnd)));
                    }
                }

            }
            //sapping surge, bad red tether
            List<AbstractBuffEvent> sappingSurge = GetFilteredList(log.CombatData, 56118, p, true);
            int surgeStart = 0;
            AbstractMasterActor source = null;
            foreach (AbstractBuffEvent c in sappingSurge)
            {
                if (c is BuffApplyEvent)
                {
                    Target qadim = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.PeerlessQadim);
                    surgeStart = (int)c.Time;
                    source = (AbstractMasterActor)log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By) ?? qadim;
                }
                else
                {
                    int surgeEnd = (int)c.Time;
                    if (source != null)
                    {
                        replay.Actors.Add(new LineActor(0, (surgeStart, surgeEnd), "rgba(255, 0, 0, 0.4)", new AgentConnector(p), new AgentConnector(source)));
                    }
                }
            }
            // kinetic abundance, good (blue) tether
            List<AbstractBuffEvent> kineticAbundance = GetFilteredList(log.CombatData, 56609, p, true);
            int kinStart = 0;
            AbstractMasterActor kinSource = null;
            foreach (AbstractBuffEvent c in kineticAbundance)
            {
                if (c is BuffApplyEvent)
                {
                    kinStart = (int)c.Time;
                    //kinSource = log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By);
                    kinSource = (AbstractMasterActor)log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By) ?? TrashMobs.FirstOrDefault(x => x.AgentItem == c.By);
                }
                else
                {
                    int kinEnd = (int)c.Time;
                    if (kinSource != null)
                    {
                        replay.Actors.Add(new LineActor(0, (kinStart, kinEnd), "rgba(0, 0, 255, 0.4)", new AgentConnector(p), new AgentConnector(kinSource)));
                    }
                }
            }
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.PeerlessQadim);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.GetHealth(combatData) > 48e6) ? 1 : 0;
        }
    }
}
