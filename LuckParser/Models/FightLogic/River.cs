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
            return new CombatReplayMap("https://i.imgur.com/YBtiFnH.png",
                            (4145, 1603),
                            (-12201, -4866, 7742, 2851),
                            (-21504, -12288, 24576, 12288),
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

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // The walls spawn at the start of the encounter, we fix it by overriding their first aware to the first velocity change event
            List<AgentItem> riverOfSouls = agentData.GetAgentByInstID((ushort)RiverOfSouls);
            bool sortCombatList = false;
            foreach (AgentItem riverOfSoul in riverOfSouls)
            {
                CombatItem firstMovement = combatData.FirstOrDefault(x => x.IsStateChange == ParseEnum.StateChange.Velocity && x.SrcInstid == riverOfSoul.InstID && x.Time <= riverOfSoul.LastAware);
                if (firstMovement != null)
                {
                    // update start
                    riverOfSoul.FirstAware = firstMovement.Time - 10;
                    foreach (CombatItem c in combatData)
                    {
                        if (c.SrcInstid == riverOfSoul.InstID && c.Time < riverOfSoul.LastAware && (c.IsStateChange == ParseEnum.StateChange.Position || c.IsStateChange == ParseEnum.StateChange.Rotation))
                        {
                            sortCombatList = true;
                            c.OverrideTime(riverOfSoul.FirstAware);
                        }
                    }
                }
                else
                {
                    // otherwise remove the agent from the pool
                    agentData.RemoveAgent(riverOfSoul);
                }
            }
            // make sure the list is still sorted by time after overrides
            if (sortCombatList)
            {
                combatData.Sort((x, y) => x.Time.CompareTo(y.Time));
            }
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
                    Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                    if (facing != null)
                    {
                        int angle = Point3D.GetRotationFromFacing(facing);
                        replay.Actors.Add(new RotatedRectangleActor(true, 0, 240, 660, angle, (start, end), "rgba(255,100,0,0.5)", new AgentConnector(mob)));
                    }
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
