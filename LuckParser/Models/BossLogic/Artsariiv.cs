using LuckParser.Controllers;
using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class Artsariiv : BossLogic
    {
        public Artsariiv() : base()
        {
            mode = ParseMode.Fractal;
            mechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(38880, "Corporeal Reassignment", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Artsariiv, "symbol:'square',color:'rgb(0,128,0)',", "Corporeal Reassignment",0),
            new Mechanic(38977, "Vault", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'triangle-down-open',color:'rgb(255,200,0)',", "Vault (Add)",0),
            new Mechanic(39925, "Slam", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,140,0)',", "Vault (Arts)",0),
            new Mechanic(39469, "Teleport Lunge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'star-triangle-down-open',color:'rgb(255,140,0)',", "Triple Jump",0),
            new Mechanic(39442, "Blinding Radiance", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'square-open',color:'rgb(255,0,0)',", "Eye (Fear)",0),
            new Mechanic(39470, "Obliterate", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "Oneshot Circle",0),
            new Mechanic(39557, "Mib Ring", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'square',color:'rgb(140,0,140)',", "Purple Goo",0),
            new Mechanic(39398, "Mib Ring", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'square',color:'rgb(140,0,140)',", "Purple Goo",0),
            new Mechanic(39035, "Astral Surge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle-open',color:'rgb(255,200,0)',", "1000 Circles",0),
            new Mechanic(39029, "Red Marble", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "Red Marble",0),
            new Mechanic(39863, "Red Marble", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "Red Marble",0),
            new Mechanic(39238, "Beaming Smile", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Artsariiv, "symbol:'circle',color:'rgb(255,0,0)',", "Triple Death Rays",0),
            });
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/4wmuc8B.png",
                            Tuple.Create(914, 914),
                            Tuple.Create(8991, 112, 11731, 2812),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
        }
    
        public override string getReplayIcon()
        {
            return "https://wiki.guildwars2.com/images/b/b4/Artsariiv.jpg";
        }
    }
}
