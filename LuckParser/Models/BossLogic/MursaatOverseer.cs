using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class MursaatOverseer : BossLogic
    {
        public MursaatOverseer() : base()
        {
            mode = ParseMode.Raid;
            mechanicList.AddRange(new List<Mechanic>()
            {
            new Mechanic(37677, "Soldier's Aura", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle-open',color:'rgb(255,0,0)',", "Jade Aura",0),
            new Mechanic(37788, "Jade Explosion", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle',color:'rgb(255,0,0)',", "Jade Explosion",0),
            new Mechanic(37779, "Claim", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, "symbol:'square',color:'rgb(255,200,0)',", "Claim",0),
            new Mechanic(37697, "Dispel", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle',color:'rgb(255,200,0)',", "Dispel",0),
            new Mechanic(37813, "Protect", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle',color:'rgb(0,255,255)',", "Protect",0),
            new Mechanic(38155, "Mursaat Overseer's Shield", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle-open',color:'rgb(0,255,255)',", "Protect (Active)",0),
            new Mechanic(38184, "Enemy Tile", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MursaatOverseer, "symbol:'square-open',color:'rgb(255,200,0)',", "Tile Dmg",0)
            });
        }

        public override CombatReplayMap GetCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/lT1FW2r.png",
                            Tuple.Create(889, 889),
                            Tuple.Create(1360, 2701, 3911, 5258),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
        }      

        public override List<ParseEnum.ThrashIDS> GetAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Jade
                    };     
            return ids;
        }

        public override int IsCM(List<CombatItem> clist, int health)
        {
            return (health > 25e6) ? 1 : 0;
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/5LNiw4Y.png";
        }
    }
}
