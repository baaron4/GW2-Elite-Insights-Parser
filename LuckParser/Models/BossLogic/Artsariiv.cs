using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;

namespace LuckParser.Models
{
    public class Artsariiv : FractalLogic
    {
        public Artsariiv()
        {           
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(38880, "Corporeal Reassignment", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Artsariiv, "symbol:'square',color:'rgb(0,128,0)',", "Skull","Exploding Skull mechanic application","Corporeal Reassignment",0),
            new Mechanic(38977, "Vault", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'triangle-down-open',color:'rgb(255,200,0)',", "Vlt","Vault from Big Adds", "Vault (Add)",0),
            new Mechanic(39925, "Slam", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,140,0)',", "Slam","Slam (Vault) from Boss", "Vault (Arts)",0),
            new Mechanic(39469, "Teleport Lunge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'star-triangle-down-open',color:'rgb(255,140,0)',", "Jmp","Triple Jump Mid->Edge", "Triple Jump",0),
            new Mechanic(39442, "Blinding Radiance", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'square-open',color:'rgb(255,0,0)',", "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)",0),
            new Mechanic(39470, "Obliterate", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "1Sht","After Phase Expanding Oneshot", "Oneshot Circle",0),
            new Mechanic(39557, "Mib Ring", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'square',color:'rgb(140,0,140)',", "Goo","Purple Goo below Boss(es)", "Purple Goo",0),
            new Mechanic(39398, "Mib Ring", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'square',color:'rgb(140,0,140)',", "Goo","Purple Goo below Boss(es)", "Purple Goo",0),
            new Mechanic(39035, "Astral Surge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle-open',color:'rgb(255,200,0)',", "Flr","Different sized spiraling circles", "1000 Circles",0),
            new Mechanic(39029, "Red Marble", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "Marble","Red KD Marble after Jump", "Red Marble",0),
            new Mechanic(39863, "Red Marble", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "Marble","Red KD Marble after Jump", "Red Marble",0),
            new Mechanic(39238, "Beaming Smile", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "DRay","Three Death Rays before jump", "Triple Death Ray",0), 
            });
        }

        public override CombatReplayMap GetCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/4wmuc8B.png",
                            Tuple.Create(914, 914),
                            Tuple.Create(8991, 112, 11731, 2812),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
        }
    
        public override string GetReplayIcon()
        {
            return "https://wiki.guildwars2.com/images/b/b4/Artsariiv.jpg";
        }
    }
}
