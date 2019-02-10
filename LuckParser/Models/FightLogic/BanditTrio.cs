using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class BanditTrio : RaidLogic
    {
        public BanditTrio(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            new Mechanic(34108, "Shell-Shocked", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("circle-open","rgb(0,128,0)"), "Launchd","Shell-Shocked (Launched from pad)", "Shell-Shocked",0),
            new Mechanic(34448, "Overhead Smash", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-left","rgb(200,140,0)"), "Smash","Overhead Smash (CC Attack Berg)", "CC Smash",0),
            new Mechanic(34383, "Hail of Bullets", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-right-open","rgb(255,0,0)"), "Zane Cone","Hail of Bullets (Zane Cone Shot)", "Hail of Bullets",0),
            new Mechanic(34344, "Fiery Vortex", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,200,0)"), "Tornado","Fiery Vortex (Tornado)", "Tornado",250),
            });
            Extension = "trio";
            IconUrl = "https://i.imgur.com/UZZQUdf.png";
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Berg,
                (ushort)ParseEnum.TargetIDS.Zane,
                (ushort)ParseEnum.TargetIDS.Narella
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

        public void SetPhasePerTarget(Target target, List<PhaseData> phases, ParsedLog log)
        {
            long fightDuration = log.FightData.FightDuration;
            CombatItem phaseStart = log.CombatData.GetStatesData(target.InstID, ParseEnum.StateChange.EnterCombat, target.FirstAware, target.LastAware).Where(x => x.SrcInstid == target.InstID).LastOrDefault();
            if (phaseStart != null)
            {
                long start = log.FightData.ToFightSpace(phaseStart.Time);
                CombatItem phaseEnd = log.CombatData.GetStatesData(target.InstID, ParseEnum.StateChange.ChangeDead, target.FirstAware, target.LastAware).Where(x => x.SrcInstid == target.InstID).LastOrDefault();
                long end = fightDuration;
                if (phaseEnd != null)
                {
                    end = log.FightData.ToFightSpace(phaseEnd.Time);
                }
                PhaseData phase = new PhaseData(start, Math.Min(end, log.FightData.FightDuration));
                phase.Targets.Add(target);
                phases.Add(phase);
            }
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                (ushort)ParseEnum.TargetIDS.Berg,
                (ushort)ParseEnum.TargetIDS.Zane,
                (ushort)ParseEnum.TargetIDS.Narella
            };
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target berg = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Berg);
            if (berg == null)
            {
                throw new InvalidOperationException("Berg not found");
            }
            Target zane = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Zane);
            if (zane == null)
            {
                throw new InvalidOperationException("Zane");
            }
            Target narella = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Narella);
            if (narella == null)
            {
                throw new InvalidOperationException("Narella");
            }
            phases[0].Targets.AddRange(Targets);
            if (!requirePhases)
            {
                return phases;
            }
            foreach (Target target in Targets)
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
                OilSlick
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            CombatReplay replay = mob.CombatReplay;
            List<CastLog> cls = mob.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (mob.ID)
            {
                case (ushort)BanditSaboteur:
                case (ushort)Warg:
                case (ushort)CagedWarg:
                case (ushort)BanditAssassin:
                case (ushort)BanditSapperTrio:
                case (ushort)BanditDeathsayer:
                case (ushort)BanditBrawler:
                case (ushort)BanditBattlemage:
                case (ushort)BanditCleric:
                case (ushort)BanditBombardier:
                case (ushort)BanditSniper:
                case (ushort)NarellaTornado:
                case (ushort)OilSlick:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }


        public override string GetFightName()
        {
            return "Bandit Trio";
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
            // TODO
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            CombatReplay replay = target.CombatReplay;
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Berg:
                    break;
                case (ushort)ParseEnum.TargetIDS.Zane:
                    List<CastLog> bulletHail = cls.Where(x => x.SkillId == 34383).ToList();
                    foreach (CastLog c in bulletHail)
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
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, 28, (firstConeStart, firstConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, 54, (secondConeStart, secondConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                            replay.Actors.Add(new PieActor(true, 0, radius, facing, 81, (thirdConeStart, thirdConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                        }
                    }
                    break;

                case (ushort)ParseEnum.TargetIDS.Narella:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }
    }
}
