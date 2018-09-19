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
            new Mechanic(51935, "Waterlogged", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.NikareKenut, "symbol:'circle-open',color:'rgb(0,140,255)',", "Wtlg","Waterlogged (stacking water debuff)", "Waterlogged",0),
            new Mechanic(52876, "Vapor Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.NikareKenut, "symbol:'triangle-left-open',color:'rgb(0,140,255)',", "Chrg","Vapor Rush (Triple Charge)", "Vapor Rush Charge",0),
            new Mechanic(52812, "Tidal Pool", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.NikareKenut, "symbol:'circle',color:'rgb(0,140,255)',", "Pool","Tidal Pool", "Tidal Pool",0),
            new Mechanic(51977, "Aquatic Barrage Start", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.NikareKenut, "symbol:'diamond-tall',color:'rgb(0,160,150)',", "CC","Breakbar", "Breakbar",0),
            new Mechanic(51977, "Aquatic Barrage End", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.NikareKenut, "symbol:'diamond-tall',color:'rgb(0,160,0)',", "CCed","Breakbar broken", "CCed",0),
            new Mechanic(53018, "Sea Swell", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.NikareKenut, "symbol:'circle',color:'rgb(255,100,255)',", "SSw","Sea Swell", "Sea Swell",0),
            new Mechanic(53130, "Geyser", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.NikareKenut, "symbol:'hexagon',color:'rgb(0,255,255)',", "Gysr","Geyser (Lifted up)", "Geyser (float)",0),
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
