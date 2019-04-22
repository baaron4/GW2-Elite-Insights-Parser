using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class BrokenKing : RaidLogic
    {
        // TODO - add CR icons and some mechanics
        public BrokenKing(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            MechanicList.AddRange( new List<Mechanic>
            {
            new SkillOnPlayerMechanic(48066, "King's Wrath", new MechanicPlotlySetting("triangle-left","rgb(0,100,255)"), "Cone Hit","King's Wrath (Auto Attack Cone Part)", "Cone Auto Attack",0),
            new SkillOnPlayerMechanic(47531, "Numbing Breach", new MechanicPlotlySetting("asterisk-open","rgb(0,100,255)"), "Cracks","Numbing Breach (Ice Cracks in the Ground)", "Cracks",0),
            new PlayerBoonApplyMechanic(47776, "Frozen Wind", new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "Green","Frozen Wind (Stood in Green)", "Green Stack",0),
            }
            );
            Extension = "brokenking";
            IconUrl = "https://wiki.guildwars2.com/images/3/37/Mini_Broken_King.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/JRPskkX.png",
                            (999, 890),
                            (2497, 5388, 7302, 9668),
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508));
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
            CombatReplay replay = p.CombatReplay;
            List<CombatItem> green = log.CombatData.GetBoonData(47776).Where(x => x.DstInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.None).ToList();
            foreach (CombatItem c in green)
            {
                int duration = 45000;
                CombatItem removedBuff = log.CombatData.GetBoonData(47776).FirstOrDefault(x => x.SrcInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.All && x.Time > c.Time && x.Time < c.Time + duration);
                int start = (int)(log.FightData.ToFightSpace(c.Time));
                int end = start + duration;
                if (removedBuff != null)
                {
                    end = (int)(log.FightData.ToFightSpace(removedBuff.Time));
                }
                replay.Actors.Add(new CircleActor(true, 0, 100, (start, end), "rgba(100, 200, 255, 0.25)", new AgentConnector(p)));
            }
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            CombatReplay replay = target.CombatReplay;
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.BrokenKing:
                    List<CastLog> Cone = cls.Where(x => x.SkillId == 48066).ToList();
                    foreach (CastLog c in Cone)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        int range = 450;
                        int angle = 100;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start+1000);
                        if (facing == null)
                        {
                            continue;
                        }
                            replay.Actors.Add(new PieActor(true, 0, range, facing, angle, (start, end), "rgba(0,100,255,0.2)", new AgentConnector(target)));
                            replay.Actors.Add(new PieActor(true, 0, range, facing, angle, (start+1900, end), "rgba(0,100,255,0.3)", new AgentConnector(target)));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }

        }


        public override void SetSuccess(ParsedLog log)
        {
            SetSuccessByDeath(log, TriggerID);
        }

        public override string GetFightName() {
            return "Statue of Ice";
        }
    }
}
