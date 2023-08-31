using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class BanditTrio : SalvationPass
    {
        public BanditTrio(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            new PlayerDstBuffApplyMechanic(ShellShocked, "Shell-Shocked", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkGreen), "Launchd","Shell-Shocked (Launched from pad)", "Shell-Shocked",0),
            new PlayerDstHitMechanic(OverheadSmashBerg, "Overhead Smash", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Orange), "Smash","Overhead Smash (CC Attack Berg)", "CC Smash",0),
            new PlayerDstHitMechanic(HailOfBulletsZane, "Hail of Bullets", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Red), "Zane Cone","Hail of Bullets (Zane Cone Shot)", "Hail of Bullets",0),
            new PlayerDstHitMechanic(FieryVortexNarella, "Fiery Vortex", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Tornado","Fiery Vortex (Tornado)", "Tornado",250),
            });
            Extension = "trio";
            GenericFallBackMethod = FallBackMethod.None;
            Icon = EncounterIconBanditTrio;
            EncounterCategoryInformation.InSubCategoryOrder = 2;
            EncounterID |= 0x000002;
        }

        protected override List<int> GetSuccessCheckIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Narella
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Berg,
                (int)ArcDPSEnums.TargetID.Zane,
                (int)ArcDPSEnums.TargetID.Narella
            };
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayBanditTrio,
                            (1000, 913),
                            (-2900, -12251, 2561, -7265)/*,
                            (-12288, -27648, 12288, 27648),
                            (2688, 11906, 3712, 14210)*/);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                IReadOnlyList<AgentItem> prisoners = agentData.GetNPCsByID(ArcDPSEnums.TrashID.Prisoner2);
                var prisonerDeaths = new List<DeadEvent>();
                foreach (AgentItem prisoner in prisoners)
                {
                    prisonerDeaths.AddRange(combatData.GetDeadEvents(prisoner));
                }
                if (prisonerDeaths.Count == 0)
                {
                    AbstractSingleActor narella = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Narella));
                    if (narella == null)
                    {
                        throw new MissingKeyActorsException("Narella not found");
                    }
                    DeadEvent deadEvent = combatData.GetDeadEvents(narella.AgentItem).LastOrDefault();
                    if (deadEvent != null)
                    {
                        fightData.SetSuccess(true, deadEvent.Time);
                        return;
                    }
                    SetSuccessByCombatExit(GetSuccessCheckTargets(), combatData, fightData, playerAgents);
                }
            }
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            long startToUse = GetGenericFightOffset(fightData);
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogStartNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                startToUse = long.MaxValue;
                AgentItem berg = agentData.GetNPCsByID(ArcDPSEnums.TargetID.Berg).FirstOrDefault();
                if (berg == null)
                {
                    throw new MissingKeyActorsException("Berg not found");
                }
                startToUse = Math.Min(berg.FirstAware, startToUse);
                AgentItem zane = agentData.GetNPCsByID(ArcDPSEnums.TargetID.Zane).FirstOrDefault();
                if (zane == null)
                {
                    throw new MissingKeyActorsException("Zane not found");
                }
                startToUse = Math.Min(zane.FirstAware, startToUse);
                AgentItem narella = agentData.GetNPCsByID(ArcDPSEnums.TargetID.Narella).FirstOrDefault();
                if (narella == null)
                {
                    throw new MissingKeyActorsException("Narella not found");
                }
                startToUse = Math.Min(narella.FirstAware, startToUse);
            }
            return startToUse;
        }

        private static void SetPhasePerTarget(AbstractSingleActor target, List<PhaseData> phases, ParsedEvtcLog log)
        {
            EnterCombatEvent phaseStart = log.CombatData.GetEnterCombatEvents(target.AgentItem).LastOrDefault();
            if (phaseStart != null)
            {
                long start = phaseStart.Time;
                DeadEvent phaseEnd = log.CombatData.GetDeadEvents(target.AgentItem).LastOrDefault();
                long end = log.FightData.FightEnd;
                if (phaseEnd != null)
                {
                    end = phaseEnd.Time;
                }
                var phase = new PhaseData(start, Math.Min(end, log.FightData.FightEnd));
                phase.AddTarget(target);
                switch (target.ID)
                {
                    case (int)ArcDPSEnums.TargetID.Narella:
                        phase.Name = "Narella";
                        break;
                    case (int)ArcDPSEnums.TargetID.Berg:
                        phase.Name = "Berg";
                        break;
                    case (int)ArcDPSEnums.TargetID.Zane:
                        phase.Name = "Zane";
                        break;
                    default:
                        throw new MissingKeyActorsException("Unknown target in Bandit Trio");
                }
                phases.Add(phase);
            }
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.Berg,
                (int)ArcDPSEnums.TargetID.Zane,
                (int)ArcDPSEnums.TargetID.Narella
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor berg = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Berg));
            if (berg == null)
            {
                throw new MissingKeyActorsException("Berg not found");
            }
            AbstractSingleActor zane = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Zane));
            if (zane == null)
            {
                throw new MissingKeyActorsException("Zane not found");
            }
            AbstractSingleActor narella = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Narella));
            if (narella == null)
            {
                throw new MissingKeyActorsException("Narella not found");
            }
            phases[0].AddTargets(Targets);
            if (!requirePhases)
            {
                return phases;
            }
            foreach (AbstractSingleActor target in Targets)
            {
                SetPhasePerTarget(target, phases, log);
            }
            return phases;
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.BanditSaboteur,
                ArcDPSEnums.TrashID.Warg,
                ArcDPSEnums.TrashID.VeteranTorturedWarg,
                ArcDPSEnums.TrashID.BanditAssassin,
                ArcDPSEnums.TrashID.BanditAssassin2,
                ArcDPSEnums.TrashID.BanditSapperTrio,
                ArcDPSEnums.TrashID.BanditDeathsayer,
                ArcDPSEnums.TrashID.BanditDeathsayer2,
                ArcDPSEnums.TrashID.BanditBrawler,
                ArcDPSEnums.TrashID.BanditBrawler2,
                ArcDPSEnums.TrashID.BanditBattlemage,
                ArcDPSEnums.TrashID.BanditBattlemage2,
                ArcDPSEnums.TrashID.BanditCleric,
                ArcDPSEnums.TrashID.BanditCleric2,
                ArcDPSEnums.TrashID.BanditBombardier,
                ArcDPSEnums.TrashID.BanditSniper,
                ArcDPSEnums.TrashID.NarellaTornado,
                ArcDPSEnums.TrashID.OilSlick,
                ArcDPSEnums.TrashID.Prisoner1,
                ArcDPSEnums.TrashID.Prisoner2,
                ArcDPSEnums.TrashID.InsectSwarm,
            };
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Bandit Trio";
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Berg:
                    break;
                case (int)ArcDPSEnums.TargetID.Zane:
                    var bulletHail = cls.Where(x => x.SkillId == HailOfBulletsZane).ToList();
                    foreach (AbstractCastEvent c in bulletHail)
                    {
                        int start = (int)c.Time;
                        int firstConeStart = start;
                        int secondConeStart = start + 800;
                        int thirdConeStart = start + 1600;
                        int firstConeEnd = firstConeStart + 400;
                        int secondConeEnd = secondConeStart + 400;
                        int thirdConeEnd = thirdConeStart + 400;
                        int radius = 1500;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start);
                        if (facing != null)
                        {
                            var connector = new AgentConnector(target);
                            var rotationConnector = new AngleConnector(facing);
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, 28, (firstConeStart, firstConeEnd), "rgba(255,200,0,0.3)", connector).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, 54, (secondConeStart, secondConeEnd), "rgba(255,200,0,0.3)", connector).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, 81, (thirdConeStart, thirdConeEnd), "rgba(255,200,0,0.3)", connector).UsingRotationConnector(rotationConnector));
                        }
                    }
                    break;

                case (int)ArcDPSEnums.TargetID.Narella:
                    break;
                default:
                    break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            // Sapper bombs
            var sapperBombs = player.GetBuffStatus(log, SapperBombBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            foreach (Segment seg in sapperBombs)
            {
                replay.Decorations.Add(new CircleDecoration(false, 0, 180, seg, "rgba(200, 255, 100, 0.5)", new AgentConnector(player)));
                replay.Decorations.Add(new CircleDecoration(true, (int)seg.Start + 5000, 180, seg, "rgba(200, 255, 100, 0.5)", new AgentConnector(player)));
                replay.AddOverheadIcon(seg, player, ParserIcons.BombOverhead);
            }
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);
            IReadOnlyList<AbstractBuffEvent> environmentallyFriendly = log.CombatData.GetBuffData(EnvironmentallyFriendly);
            if (environmentallyFriendly.Any() && log.FightData.Success)
            {
                foreach (Player p in log.PlayerList)
                {
                    if (p.HasBuff(log, EnvironmentallyFriendly, log.FightData.FightEnd - ServerDelayConstant))
                    {
                        InstanceBuffs.Add((log.Buffs.BuffsByIds[EnvironmentallyFriendly], 1));
                        break;
                    }
                }
            }
        }
    }
}
