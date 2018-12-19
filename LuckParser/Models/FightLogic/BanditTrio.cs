using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class BanditTrio : RaidLogic
    {
        public BanditTrio(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                // TODO
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
            return new CombatReplayMap("https://i.imgur.com/eUZtON4.png",
                            Tuple.Create(2455, 2276),
                            Tuple.Create(5822, -3491, 9549, 2205),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(2688, 11906, 3712, 14210));
        }

        public void SetPhasePerTarget(Target target, List<PhaseData> phases, ParsedLog log)
        {
            long fightDuration = log.FightData.FightDuration;
            CombatItem phaseStart = log.GetStatesData(target.InstID, ParseEnum.StateChange.EnterCombat, target.FirstAware, target.LastAware).Where(x => x.SrcInstid == target.InstID).LastOrDefault();
            if (phaseStart != null)
            {
                long start = log.FightData.ToFightSpace(phaseStart.Time);
                CombatItem phaseEnd = log.GetStatesData(target.InstID, ParseEnum.StateChange.ChangeDead, target.FirstAware, target.LastAware).Where(x => x.SrcInstid == target.InstID).LastOrDefault();
                long end = fightDuration;
                if (phaseEnd != null)
                {
                    end = log.FightData.ToFightSpace(phaseEnd.Time);
                }
                PhaseData phase = new PhaseData(start, end);
                phase.Targets.Add(target);
                phases.Add(phase);
            }
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
            // TODO
            CombatReplay replay = target.CombatReplay;
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Berg:
                    break;
                case (ushort)ParseEnum.TargetIDS.Zane:
                    break;
                case (ushort)ParseEnum.TargetIDS.Narella:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }
    }
}
