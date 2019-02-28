using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class River : RaidLogic
    {
        public River(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange( new List<Mechanic>
            {

            }
            );
            Extension = "river";
            IconUrl = "https://wiki.guildwars2.com/images/thumb/7/7b/Gold_River_of_Souls_Trophy.jpg/220px-Gold_River_of_Souls_Trophy.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://wiki.guildwars2.com/images/e/e8/River_of_Souls_Mechanics.jpg",
                            (4625, 2209),
                            (-12250, -5115, 7900, 3697),
                            (-215040, -222880, 245760, 222880),
                            (19072, 15484, 20992, 16508));
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                Enervator,
                HollowedBomber,
                RiverOfSouls
            };
        }

        public override void ComputeAdditionalTrashMobData(Mob mob, ParsedLog log)
        {
            CombatReplay replay = mob.CombatReplay;
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (mob.ID)
            {
                case (ushort)HollowedBomber:
                    replay.Actors.Add(new CircleActor(false, 0, 260, (start, end), "rgba(0, 80, 255, 0.5)", new AgentConnector(mob)));
                    break;
                case (ushort)RiverOfSouls:
                    float prevX = replay.Positions[0].X;
                    float prevY = replay.Positions[0].Y;
                    foreach (Point3D pos in replay.Positions)
                    {
                        if (prevX != pos.X || prevY != pos.Y)
                        {
                            start = (int)pos.Time;
                            replay.Trim(start, end);
                            break;
                        }
                    }
                    int angle = 0;
                    Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                    if (facing != null)
                    {
                        angle = Point3D.GetRotationFromFacing(facing);
                    }
                    replay.Actors.Add(new RotatedRectangleActor(true, 0, 240, 660, angle, (start, end), "rgba(255,100,0,0.5)", new AgentConnector(mob)));
                    break;
                case (ushort)Enervator:
                    replay.Actors.Add(new CircleActor(true, 0, 200, (start, end), "rgba(0, 0, 255, 0.5)", new AgentConnector(mob)));
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }

        }

        public override string GetFightName() {
            return "River of Souls";
        }
    }
}
