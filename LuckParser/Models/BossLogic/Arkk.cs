using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class Arkk : FractalLogic
    {
        public Arkk() : base()
        {
            mechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(39685, "Horizon Strike 1", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(50,0,0)',", "Horizon Strike 1",0),
            new Mechanic(39001, "Horizon Strike 2", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(100,0,0)',", "Horizon Strike 2",0),
            new Mechanic(39787, "Diffractive Edge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'star',color:'rgb(255,200,0)',", "5 Cones",0),
            new Mechanic(39728, "Solar Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(255,0,0)',", "Red Ball AoE",0),
            new Mechanic(39711, "Focused Rage", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'triangle-left',color:'rgb(140,0,140)',", "Cone Knockback",0),
            new Mechanic(39691, "Solar Discharge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle-open',color:'rgb(255,0,0)',", "Shockwave",0),
            new Mechanic(38982, "Starburst Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle-open',color:'rgb(255,140,0)',", "Float Ring",0),
            new Mechanic(39297, "Horizon Strike Normal", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(150,0,0)',", "Horizon Strike Normal",0),
            new Mechanic(38844, "Overhead Smash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(200,0,0)',", "Overhead Smash",0),
            new Mechanic(38880, "Corporeal Reassignment", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'square',color:'rgb(0,128,0)',", "Corporeal Reassignment",0),
            new Mechanic(39442, "Blinding Radiance", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'square-open',color:'rgb(255,0,0)',", "Eye (Fear)",0),
            //new Mechanic(39823, "Solar Blast", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-diamond',color:'rgb(255,0,0)',", "Eye",0),
            new Mechanic(39849, "Explode", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(255,200,0)',", "Bloom Explosion",0),
            new Mechanic(39558, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Bloom Fixate",0),
            new Mechanic(39928, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Bloom Fixate",0),
            new Mechanic(39131, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Bloom Fixate",0),
            new Mechanic(38985, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Bloom Fixate",0),
            //Breakbar (39645) is cast with is_activation=5 (reset) when failed (not broken).
            });
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/BIybWJe.png",
                            Tuple.Create(914, 914),
                            Tuple.Create(-19231, -18137, -16591, -15677),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
        }
        
        public override string getReplayIcon()
        {
            return "https://i.imgur.com/u6vv8cW.png";
        }
    }
}
