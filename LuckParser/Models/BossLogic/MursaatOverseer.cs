using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;

namespace LuckParser.Models
{
    public class MursaatOverseer : BossLogic
    {
        public MursaatOverseer()
        {
            Mode = ParseMode.Raid;
            MechanicList.AddRange(new List<Mechanic>()
            {
            new Mechanic(37677, "Soldier's Aura", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle-open',color:'rgb(255,0,0)',", "Jade",0), // Jade Soldier's Aura hit, Jade Aura
            new Mechanic(37788, "Jade Explosion", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle',color:'rgb(255,0,0)',", "JExpl",0), // Jade Soldier's Death Explosion, Jade Explosion
            new Mechanic(37779, "Claim", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, "symbol:'square',color:'rgb(255,200,0)',", "Claim",0), //Claim, Claim
            new Mechanic(37697, "Dispel", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle',color:'rgb(255,200,0)',", "Dspl",0), //Dispel, Dispel
            new Mechanic(37813, "Protect", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle',color:'rgb(0,255,255)',", "Prtct",0), //Protect, Protect
            new Mechanic(38155, "Mursaat Overseer's Shield", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle-open',color:'rgb(0,255,255)',", "P.Shield",0), // Mursaat Overseer's Shield, Protect (active)
            new Mechanic(38184, "Enemy Tile", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MursaatOverseer, "symbol:'square-open',color:'rgb(255,200,0)',", "Floor",0) //Enemey Tile damage, Tile dmg
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
