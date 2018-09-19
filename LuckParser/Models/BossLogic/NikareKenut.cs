using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class NikareKenut : RaidLogic
    {
        public NikareKenut()
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            });
            Extension = "nk";
            IconUrl = "";
        }

        public override CombatReplayMap GetCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/RMBeXhd.png",
                            Tuple.Create(5760, 7538),
                            Tuple.Create(10896, -2448, 18096, 8352),
                            Tuple.Create(-21504, -21504, 24576, 24576),
                            Tuple.Create(13440, 14336, 15360, 16256));
        }
        
        public override List<ParseEnum.TrashIDS> GetAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            List<ParseEnum.TrashIDS> ids = new List<ParseEnum.TrashIDS>();
            return ids;
        }

        public override void GetAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {

        }

        public override int IsCM(List<CombatItem> clist, int health)
        {
            return 0;
        }

        public override string GetReplayIcon()
        {
            return "";
        }
    }
}
