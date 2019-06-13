using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;
using System.Drawing;

namespace LuckParser.Models.Logic
{
    public class PeerlessQadim : RaidLogic
    {
        public PeerlessQadim(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            });
            Extension = "prlqadim";
            IconUrl = "https://wiki.guildwars2.com/images/d/d2/Guild_emblem_004.png";
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>()
            {
                Pylon1,
                Pylon2,
                EntropicDistortion
            };
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("",
                            (800, 800),
                            (-21504, -21504, 24576, 24576),
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970));
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.PeerlessQadim);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.GetHealth(combatData) > 48e6) ? 1 : 0;
        }
    }
}
