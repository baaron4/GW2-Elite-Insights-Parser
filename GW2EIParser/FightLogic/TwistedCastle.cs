using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.Logic
{
    public class TwistedCastle : RaidLogic
    {
        public TwistedCastle(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            }
            );
            Extension = "twstcstl";
            Targetless = true;
            Icon = "https://wiki.guildwars2.com/images/b/b5/Mini_McLeod_the_Silent.png";
        }

        /*protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/RZbs21b.png",
                            (1099, 1114),
                            (-5467, 8069, -2282, 11297),
                            (-12288, -27648, 12288, 27648),
                            (1920, 12160, 2944, 14464));
        }*/

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            AgentItem dummyAgent = agentData.AddCustomAgent(combatData.First().LogTime, combatData.Last().LogTime, AgentItem.AgentType.NPC, "Twisted Castle", "", TriggerID);
            ComputeFightTargets(agentData, combatData);
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
               ParseEnum.TrashIDS.HauntingStatue
            };
        }

        public override void ComputeMobCombatReplayActors(Mob mob, ParsedLog log, CombatReplay replay)
        {
            switch (mob.ID)
            {
                case (ushort)ParseEnum.TrashIDS.HauntingStatue:
                    var lifespan = ((int)replay.TimeOffsets.start, (int)replay.TimeOffsets.end);
                    if (replay.Rotations.Any())
                    {
                        replay.Decorations.Add(new FacingDecoration(lifespan, new AgentConnector(mob), replay.PolledRotations));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override string GetFightName()
        {
            return "Twisted Castle";
        }
    }
}
