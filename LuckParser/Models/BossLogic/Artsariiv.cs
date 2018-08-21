using LuckParser.Models.ParseModels;
using System;

namespace LuckParser.Models
{
    public class Artsariiv : FractalLogic
    {
        public Artsariiv()
        {           
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(38880, "Corporeal Reassignment", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Artsariiv, "symbol:'square',color:'rgb(0,128,0)',", "Skull",0), //Exploding Skull mechanic application,Corporeal Reassignment
            new Mechanic(38977, "Vault", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'triangle-down-open',color:'rgb(255,200,0)',", "Vlt",0), //Vault from Big Adds, Vault (Add)
            new Mechanic(39925, "Slam", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,140,0)',", "Slam",0), //Slam (Vault) from Boss, Vault (Arts)
            new Mechanic(39469, "Teleport Lunge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'star-triangle-down-open',color:'rgb(255,140,0)',", "Jmp",0), //Triple Jump Mid->Edge, Triple Jump
            new Mechanic(39442, "Blinding Radiance", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'square-open',color:'rgb(255,0,0)',", "Eye",0), //Hit by the Overhead Eye Fear, Eye (Fear)
            new Mechanic(39470, "Obliterate", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "1Sht",0), //After Phase Expanding Oneshot, Oneshot Circle
            new Mechanic(39557, "Mib Ring", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'square',color:'rgb(140,0,140)',", "Goo",0), //Purple Goo below Boss(es), Purple Goo
            new Mechanic(39398, "Mib Ring", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'square',color:'rgb(140,0,140)',", "Goo",0), //Purple Goo below Boss(es), Purple Goo
            new Mechanic(39035, "Astral Surge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle-open',color:'rgb(255,200,0)',", "Flr",0), //Different sized spiraling circles, 1000 Circles
            new Mechanic(39029, "Red Marble", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "Marble",0), // Red KD Marble after Jump, Red Marble
            new Mechanic(39863, "Red Marble", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "Marble",0), // Red KD Marble after Jump, Red Marble
            new Mechanic(39238, "Beaming Smile", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "DRay",0), // Three Death Rays before jump, Triple Death Ray
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
