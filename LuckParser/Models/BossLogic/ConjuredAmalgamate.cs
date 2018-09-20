using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class ConjuredAmalgamate : RaidLogic
    {
        public ConjuredAmalgamate()
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(52173, "Pulverize", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.ConjuredAmalgamate, "symbol:'square',color:'rgb(255,140,0)',", "Plvrz","Pulverize", "Pulverize",0),
                
            });
            CanCombatReplay = false;
            Extension = "ca";
            IconUrl = "";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/Dp3SFq6.png",
                            Tuple.Create(2557, 4706),
                            Tuple.Create(-5664, 13752, -3264, 18552),
                            Tuple.Create(-21504, -21504, 24576, 24576),
                            Tuple.Create(13440, 14336, 15360, 16256));
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>();
        }

        public override void ComputeAdditionalBossData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
        }

        public override void ComputeAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {

        }

        public override int IsCM(ParsedLog log)
        {
            return 0;
        }

        public override string GetReplayIcon()
        {
            return "";
        }
    }
}
