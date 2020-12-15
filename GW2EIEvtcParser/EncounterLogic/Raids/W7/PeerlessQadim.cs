using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class PeerlessQadim : RaidLogic
    {
        public PeerlessQadim(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                new HitOnPlayerMechanic(56541, "Pylon Debris Field", new MechanicPlotlySetting("circle-open-dot","rgb(255,150,0)"), "P.Magma", "Hit by Pylon Magma", "Pylon Magma", 0),
                new HitOnPlayerMechanic(56020, "Energized Affliction", new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "E.Aff", "Energized Affliction", "Energized Affliction", 0),
                new HitOnPlayerMechanic(56134, "Force of Retaliation", new MechanicPlotlySetting("circle-open","rgb(0,0,0)"), "Pushed", "Pushed by Shockwave", "Shockwave Push", 0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
                new HitOnPlayerMechanic(56441, "Force of Havoc", new MechanicPlotlySetting("square-open","rgb(150,0,250)"), "P.Rect", "Hit by Purple Rectangle", "Purple Rectangle", 0),
                new HitOnPlayerMechanic(56145, "Chaos Called", new MechanicPlotlySetting("circle-x-open","rgb(150,0,250)"), "Pattern.H", "Hit by Energy on Pattern", "Pattern Energy Hit", 0),
                new HitOnPlayerMechanic(56527, "Rain of Chaos", new MechanicPlotlySetting("star-square","rgb(150,0,250)"), "Lght.H", "Hit by Expanding Lightning", "Lightning Hit", 0),
                new HitOnPlayerMechanic(56656, "Brandstorm Lightning", new MechanicPlotlySetting("triangle","rgb(150,0,250)"), "S.Lght.H", "Hit by Small Lightning", "Small Lightning Hit", 0),
                //new HitOnPlayerMechanic(56254, "Exponential Repercussion", new MechanicPlotlySetting("diamond-open","rgb(150,0,250)"), "Shield.H", "Hit by Energy Shield", "Shield Hit", 0),// to check
                new HitOnPlayerMechanic(56180, "Residual Impact", new MechanicPlotlySetting("circle-open","rgb(250,150,0)"), "Magma.F", "Hit by Magma Field", "Magma Field", 500),
                new HitOnPlayerMechanic(56378, "Residual Impact", new MechanicPlotlySetting("circle-open","rgb(250,150,0)",10), "S.Magma.F", "Hit by Small Magma Field", "Small Magma Field", 500),
                new HitOnPlayerMechanic(56616, "Battering Blitz", new MechanicPlotlySetting("bowtie","rgb(250,150,0)"), "Rush.H", "Hit by Qadim Rush", "Qadim Rush", 500),
                new HitOnPlayerMechanic(56332, "Caustic Chaos", new MechanicPlotlySetting("triangle-right","rgb(250,0,0)"), "A.Prj.H", "Hit by Aimed Projectile", "Aimed Projectile", 0),
                new HitByEnemyMechanic(56598, "Shower of Chaos", new MechanicPlotlySetting("circle","rgb(0,0,0)"), "Orb.D", "Pylon Orb not caught", "Shower of Chaos", 1000),
                new PlayerBuffApplyMechanic(56510, "Fixated", new MechanicPlotlySetting("star","rgb(255,0,255)"), "Fixated", "Fixated", "Fixated", 0),
                new HitOnPlayerMechanic(56543, "Caustic Chaos", new MechanicPlotlySetting("triangle-right-open","rgb(250,0,0)"), "A.Prj.E", "Hit by Aimed Projectile Explosion", "Aimed Projectile Explosion", 0),
                new PlayerBuffApplyMechanic(56118, "Sapping Surge", new MechanicPlotlySetting("y-down-open","rgb(250,0,50)"), "B.Tether", "25% damage reduction", "Bad Tether", 0),
            });
            Extension = "prlqadim";
            Icon = "https://wiki.guildwars2.com/images/8/8b/Mini_Qadim_the_Peerless.png";
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
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

        internal override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
        {
            var res = new List<AbstractBuffEvent>();
            if (buffsById.TryGetValue(56118, out List<AbstractBuffEvent> list))
            {
                var sappingSurgeByDst = list.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
                foreach (KeyValuePair<AgentItem, List<AbstractBuffEvent>> pair in sappingSurgeByDst.Where(x => x.Value.Exists(y => y is BuffRemoveSingleEvent)))
                {
                    var sglRemovals = pair.Value.Where(x => x is BuffRemoveSingleEvent).ToList();
                    foreach (AbstractBuffEvent sglRemoval in sglRemovals)
                    {
                        AbstractBuffEvent ba = pair.Value.LastOrDefault(x => x is BuffApplyEvent && Math.Abs(x.Time - sglRemoval.Time) < 5);
                        if (ba != null)
                        {
                            res.Add(new BuffRemoveAllEvent(sglRemoval.By, pair.Key, ba.Time - 1, int.MaxValue, ba.BuffSkill, 0, int.MaxValue));
                            res.Add(new BuffRemoveManualEvent(sglRemoval.By, pair.Key, ba.Time - 1, int.MaxValue, ba.BuffSkill));
                        }
                    }
                }
            }
            return res;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.PeerlessQadim);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Peerless Qadim not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            var phaseStarts = new List<long>();
            var phaseEnds = new List<long>();
            //
            var magmaDrops = log.CombatData.GetBuffData(56475).Where(x => x is BuffApplyEvent).ToList();
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
            List<AnimatedCastEvent> pushes = log.CombatData.GetAnimatedCastData(56405);
            if (pushes.Count > 0)
            {
                AbstractCastEvent push = pushes[0];
                phaseStarts.Add(push.EndTime);
                foreach (long magmaDrop in phaseEnds)
                {
                    push = pushes.FirstOrDefault(x => x.Time >= magmaDrop);
                    if (push == null)
                    {
                        break;
                    }
                    phaseStarts.Add(push.EndTime);
                }
            }
            // rush to pylon
            phaseEnds.AddRange(log.CombatData.GetAnimatedCastData(56616).Select(x => x.Time).ToList());
            phaseEnds.Add(log.FightData.FightEnd);
            // tp to middle after pylon destruction
            phaseStarts.AddRange(log.CombatData.GetAnimatedCastData(56375).Select(x => x.EndTime));
            if (phaseEnds.Count < phaseStarts.Count)
            {
                return phases;
            }
            for (int i = 0; i < phaseStarts.Count; i++)
            {
                var phase = new PhaseData(phaseStarts[i], phaseEnds[i], "Phase " + (i + 1));
                phase.Targets.Add(mainTarget);
                phases.Add(phase);
            }
            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/PgkZMYE.png",
                            (6822, 6822),
                            (-968, 7480, 4226, 12676),
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970));
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastEvents(log, 0, log.FightData.FightEnd);
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.PeerlessQadim:
                    var cataCycle = cls.Where(x => x.SkillId == 56329).ToList();

                    foreach (AbstractCastEvent c in cataCycle)
                    {
                        int magmaRadius = 850;
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        Point3D pylonPosition = replay.PolledPositions.LastOrDefault(x => x.Time <= end);
                        replay.Decorations.Add(new CircleDecoration(true, 0, magmaRadius, (start, end), "rgba(255, 220, 50, 0.15)", new PositionConnector(pylonPosition)));
                        replay.Decorations.Add(new CircleDecoration(true, end, magmaRadius, (start, end), "rgba(255, 220, 50, 0.25)", new PositionConnector(pylonPosition)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, magmaRadius, (end, (int)log.FightData.FightEnd), "rgba(255, 220, 0, 0.5)", new PositionConnector(pylonPosition)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.EntropicDistortion:
                    //sapping surge, red tether
                    List<AbstractBuffEvent> sappingSurge = GetFilteredList(log.CombatData, 56118, target, true);
                    int surgeStart = 0;
                    AbstractSingleActor source = null;
                    foreach (AbstractBuffEvent c in sappingSurge)
                    {
                        if (c is BuffApplyEvent)
                        {
                            NPC qadim = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.PeerlessQadim);
                            surgeStart = (int)c.Time;
                            source = (AbstractSingleActor)log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By) ?? qadim;
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
                    break;
                case (int)ArcDPSEnums.TrashID.BigKillerTornado:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 450, (start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.Pylon1:
                    break;
                case (int)ArcDPSEnums.TrashID.Pylon2:
                    break;
                case (int)ArcDPSEnums.TrashID.EnergyOrb:
                    break;
                default:
                    break;
            }

        }

        internal override void ComputePlayerCombatReplayActors(Player p, ParsedEvtcLog log, CombatReplay replay)
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
                    replay.Decorations.Add(new CircleDecoration(true, 0, 120, (fixatedStart, fixatedEnd), "rgba(255, 80, 255, 0.3)", new AgentConnector(p)));
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
                    replay.Decorations.Add(new CircleDecoration(true, 0, magmaRadius, (magmaDropStart, magmaDropEnd), "rgba(255, 150, 0, 0.15)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, magmaDropEnd, magmaRadius, (magmaDropStart, magmaDropEnd), "rgba(255, 150, 0, 0.25)", new AgentConnector(p)));
                    Point3D magmaNextPos = replay.PolledPositions.FirstOrDefault(x => x.Time >= magmaDropEnd);
                    Point3D magmaPrevPos = replay.PolledPositions.LastOrDefault(x => x.Time <= magmaDropEnd);
                    if (magmaNextPos != null || magmaPrevPos != null)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, magmaRadius, (magmaDropEnd, magmaDropEnd + magmaOffset), "rgba(255, 220, 50, 0.15)", new InterpolatedPositionConnector(magmaPrevPos, magmaNextPos, magmaDropEnd)));
                        replay.Decorations.Add(new CircleDecoration(true, magmaDropEnd + magmaOffset, magmaRadius, (magmaDropEnd, magmaDropEnd + magmaOffset), "rgba(255, 220, 50, 0.25)", new InterpolatedPositionConnector(magmaPrevPos, magmaNextPos, magmaDropEnd)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, magmaRadius, (magmaDropEnd + magmaOffset, (int)log.FightData.FightEnd), "rgba(255, 220, 50, 0.5)", new InterpolatedPositionConnector(magmaPrevPos, magmaNextPos, magmaDropEnd)));
                    }
                }

            }
            //sapping surge, bad red tether
            List<AbstractBuffEvent> sappingSurge = GetFilteredList(log.CombatData, 56118, p, true);
            int surgeStart = 0;
            AbstractSingleActor source = null;
            foreach (AbstractBuffEvent c in sappingSurge)
            {
                if (c is BuffApplyEvent)
                {
                    NPC qadim = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.PeerlessQadim);
                    surgeStart = (int)c.Time;
                    source = (AbstractSingleActor)log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By) ?? qadim;
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
            List<AbstractBuffEvent> kineticAbundance = GetFilteredList(log.CombatData, 56609, p, true);
            int kinStart = 0;
            AbstractSingleActor kinSource = null;
            foreach (AbstractBuffEvent c in kineticAbundance)
            {
                if (c is BuffApplyEvent)
                {
                    kinStart = (int)c.Time;
                    //kinSource = log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By);
                    kinSource = (AbstractSingleActor)log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By) ?? TrashMobs.FirstOrDefault(x => x.AgentItem == c.By);
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
            NPC target = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.PeerlessQadim);
            if (target == null)
            {
                throw new MissingKeyActorsException("Peerless Qadim not found");
            }
            return (target.GetHealth(combatData) > 48e6) ? FightData.CMStatus.CM : FightData.CMStatus.NoCM;
        }
    }
}
