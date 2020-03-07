using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Parser.ParseEnum.TrashIDS;

namespace GW2EIParser.Logic
{
    public class BanditTrio : RaidLogic
    {
        public BanditTrio(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            new PlayerBuffApplyMechanic(34108, "Shell-Shocked", new MechanicPlotlySetting("circle-open","rgb(0,128,0)"), "Launchd","Shell-Shocked (Launched from pad)", "Shell-Shocked",0),
            new HitOnPlayerMechanic(34448, "Overhead Smash", new MechanicPlotlySetting("triangle-left","rgb(200,140,0)"), "Smash","Overhead Smash (CC Attack Berg)", "CC Smash",0),
            new HitOnPlayerMechanic(34383, "Hail of Bullets", new MechanicPlotlySetting("triangle-right-open","rgb(255,0,0)"), "Zane Cone","Hail of Bullets (Zane Cone Shot)", "Hail of Bullets",0),
            new HitOnPlayerMechanic(34344, "Fiery Vortex", new MechanicPlotlySetting("circle-open","rgb(255,200,0)"), "Tornado","Fiery Vortex (Tornado)", "Tornado",250),
            });
            Extension = "trio";
            GenericFallBackMethod = FallBackMethod.None;
            Icon = "https://i.imgur.com/UZZQUdf.png";
        }

        protected override List<int> GetSuccessCheckIds()
        {
            return new List<int>
            {
                (int)ParseEnum.TargetIDS.Narella
            };
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ParseEnum.TargetIDS.Berg,
                (int)ParseEnum.TargetIDS.Zane,
                (int)ParseEnum.TargetIDS.Narella
            };
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/cVuaOc5.png",
                            (2494, 2277),
                            (-2900, -12251, 2561, -7265),
                            (-12288, -27648, 12288, 27648),
                            (2688, 11906, 3712, 14210));
        }

        public override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                List<AgentItem> prisoners = agentData.GetNPCsByID((int)Prisoner2);
                var prisonerDeaths = new List<DeadEvent>();
                foreach (AgentItem prisoner in prisoners)
                {
                    prisonerDeaths.AddRange(combatData.GetDeadEvents(prisoner));
                }
                if (prisonerDeaths.Count == 0)
                {
                    SetSuccessByCombatExit(new HashSet<int>(GetSuccessCheckIds()), combatData, fightData, playerAgents);
                }
            }
        }

        public static void SetPhasePerTarget(NPC target, List<PhaseData> phases, ParsedLog log)
        {
            long fightDuration = log.FightData.FightEnd;
            EnterCombatEvent phaseStart = log.CombatData.GetEnterCombatEvents(target.AgentItem).LastOrDefault();
            if (phaseStart != null)
            {
                long start = phaseStart.Time;
                DeadEvent phaseEnd = log.CombatData.GetDeadEvents(target.AgentItem).LastOrDefault();
                long end = fightDuration;
                if (phaseEnd != null)
                {
                    end = phaseEnd.Time;
                }
                var phase = new PhaseData(start, Math.Min(end, log.FightData.FightEnd));
                phase.Targets.Add(target);
                phases.Add(phase);
            }
        }

        protected override HashSet<int> GetUniqueTargetIDs()
        {
            return new HashSet<int>
            {
                (int)ParseEnum.TargetIDS.Berg,
                (int)ParseEnum.TargetIDS.Zane,
                (int)ParseEnum.TargetIDS.Narella
            };
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC berg = Targets.Find(x => x.ID == (int)ParseEnum.TargetIDS.Berg);
            if (berg == null)
            {
                throw new InvalidOperationException("Error Encountered: Berg not found");
            }
            NPC zane = Targets.Find(x => x.ID == (int)ParseEnum.TargetIDS.Zane);
            if (zane == null)
            {
                throw new InvalidOperationException("Error Encountered: Zane not found");
            }
            NPC narella = Targets.Find(x => x.ID == (int)ParseEnum.TargetIDS.Narella);
            if (narella == null)
            {
                throw new InvalidOperationException("Error Encountered: Narella not found");
            }
            phases[0].Targets.AddRange(Targets);
            if (!requirePhases)
            {
                return phases;
            }
            foreach (NPC target in Targets)
            {
                SetPhasePerTarget(target, phases, log);
            }
            string[] phaseNames = { "Berg", "Zane", "Narella" };
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = phaseNames[i - 1];
            }
            return phases;
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                BanditSaboteur,
                Warg,
                CagedWarg,
                BanditAssassin,
                BanditSapperTrio ,
                BanditDeathsayer,
                BanditBrawler,
                BanditBattlemage,
                BanditCleric,
                BanditBombardier,
                BanditSniper,
                NarellaTornado,
                OilSlick,
                Prisoner1,
                Prisoner2
            };
        }

        public override string GetFightName()
        {
            return "Bandit Trio";
        }

        public override void ComputeNPCCombatReplayActors(NPC target, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ParseEnum.TargetIDS.Berg:
                    break;
                case (int)ParseEnum.TargetIDS.Zane:
                    var bulletHail = cls.Where(x => x.SkillId == 34383).ToList();
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
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, facing, 28, (firstConeStart, firstConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, facing, 54, (secondConeStart, secondConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                            replay.Decorations.Add(new PieDecoration(true, 0, radius, facing, 81, (thirdConeStart, thirdConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                        }
                    }
                    break;

                case (int)ParseEnum.TargetIDS.Narella:
                    break;
                default:
                    break;
            }
        }
    }
}
