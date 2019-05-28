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
        public DarkMaze(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            MechanicList.AddRange( new List<Mechanic>
            {
            new PlayerBoonApplyMechanic(791, "Fear", new MechanicPlotlySetting("star-square","rgb(0,0,0)"), "Feared","Feared by Eye Teleport Skill", "Feared",0),
            new PlayerBoonApplyMechanic(48779, "Light Carrier", new MechanicPlotlySetting("circle-open","rgb(200,200,0)"), "Light Orb","Light Carrier (picked up a light orb)", "Picked up orb",0),
            new PlayerCastStartMechanic(47074, "Flare", new MechanicPlotlySetting("circle","rgb(0,255,0)"), "Detonate","Flare (detonate light orb to incapacitate eye)", "Detonate orb",0),
            new SkillOnPlayerMechanic(47518, "Piercing Shadow", new MechanicPlotlySetting("hexagram-open","rgb(0,0,255)"), "Spin","Piercing Shadow (damaging spin to all players in sight)", "Eye Spin",0),
            new SkillOnPlayerMechanic(48150, "Deep Abyss", new MechanicPlotlySetting("triangle-right-open","rgb(255,0,0)"), "Beam","Deep Abyss (ticking eye beam)", "Eye Beam",0),
            //47857 <- teleport + fear skill? 
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
                LightThieves,
                //MazeMinotaur,
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

        private void HPCheck(ParsedEvtcContainer evtcContainer)
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
            long margin1 = Math.Min(80, lastEye1Hp);
            long margin2 = Math.Min(80, lastEye2Hp);
            if (lastEye1Hp <= margin1 && lastEye2Hp <= margin2)
            {
                int lastIEye1;
                for (lastIEye1 = eye1.HealthOverTime.Count - 1; lastIEye1 >= 0; lastIEye1--)
                {
                    if (eye1.HealthOverTime[lastIEye1].hp > margin1)
                    {
                        lastIEye1++;
                        break;
                    }
                }
                int lastIEye2;
                for (lastIEye2 = eye2.HealthOverTime.Count - 1; lastIEye2 >= 0; lastIEye2--)
                {
                    if (eye2.HealthOverTime[lastIEye2].hp > margin2)
                    {
                        lastIEye2++;
                        break;
                    }
                }
                evtcContainer.FightData.SetSuccess(true, Math.Max(eye1.HealthOverTime[lastIEye1].logTime, eye2.HealthOverTime[lastIEye2].logTime));
            }
        }

        public override void CheckSuccess(ParsedEvtcContainer evtcContainer)
        {
            // First check using hp, best
            HPCheck(evtcContainer);
            // hp could be unreliable or missing, fall back (around 200 ms more)
            if (!evtcContainer.FightData.Success)
            {
                SetSuccessByDeath(evtcContainer, false, (ushort)ParseEnum.TargetIDS.EyeOfFate, (ushort)ParseEnum.TargetIDS.EyeOfJudgement);
            }
        }

        public override string GetFightName() {
            return "Statue of Darkness";
        }
    }
}
