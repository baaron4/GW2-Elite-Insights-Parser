using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class DarkMaze : RaidLogic
    {
        // TODO - add CR icons and some mechanics
        public DarkMaze(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange( new List<Mechanic>
            {

            }
            );
            Extension = "eyes";
            IconUrl = "https://wiki.guildwars2.com/images/thumb/a/a7/Eye_of_Fate.jpg/188px-Eye_of_Fate.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/3muXEM7.png",
                            (1052, 1301),
                            (11664, -2108, 16724, 4152),
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508));
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                // skeletons - red dot
                // minotaur - special icon
            };
        }


        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.EyeOfFate,
                (ushort)ParseEnum.TargetIDS.EyeOfJudgement
            };
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                (ushort)ParseEnum.TargetIDS.EyeOfFate,
                (ushort)ParseEnum.TargetIDS.EyeOfJudgement
            };
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target eye1 = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.EyeOfFate);
            Target eye2 = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.EyeOfJudgement);
            if (eye2 == null || eye1 == null)
            {
                throw new InvalidOperationException("Eyes not found");
            }
            phases[0].Targets.Add(eye2);
            phases[0].Targets.Add(eye1);
            return phases;
        }

        public override void SetSuccess(ParsedLog log)
        {

            Target eye1 = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.EyeOfFate);
            Target eye2 = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.EyeOfJudgement);
            if (eye2 == null || eye1 == null)
            {
                throw new InvalidOperationException("Eyes not found");
            }
            if (eye1.HealthOverTime.Count == 0 || eye2.HealthOverTime.Count == 0)
            {
                return;
            }
            long lastEye1Hp = eye1.HealthOverTime.LastOrDefault().hp;
            long lastEye2Hp = eye2.HealthOverTime.LastOrDefault().hp;
            if (lastEye1Hp == 0 && lastEye2Hp == 0)
            {
                log.FightData.Success = true;
                int lastIEye1;
                for (lastIEye1 = eye1.HealthOverTime.Count - 1; lastIEye1 >= 0; lastIEye1--)
                {
                    if (eye1.HealthOverTime[lastIEye1].hp > 0)
                    {
                        lastIEye1++;
                        break;
                    }
                }
                int lastIEye2;
                for (lastIEye2 = eye2.HealthOverTime.Count - 1; lastIEye2 >= 0; lastIEye2--)
                {
                    if (eye2.HealthOverTime[lastIEye2].hp > 0)
                    {
                        lastIEye2++;
                        break;
                    }
                }
                log.FightData.FightEnd = Math.Max(eye1.HealthOverTime[lastIEye1].logTime, eye2.HealthOverTime[lastIEye2].logTime);
            }
        }

        public override string GetFightName() {
            return "Statue of Darkness";
        }
    }
}
