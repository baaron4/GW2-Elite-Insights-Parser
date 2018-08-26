using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;

namespace LuckParser.Models
{
    public class Arkk : FractalLogic
    {
        public Arkk()
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(39685, "Horizon Strike 1", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(50,0,0)',", "HS","Horizon Strike (turning pizza slices)","Horizon Strike",0),
            new Mechanic(39001, "Horizon Strike 2", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(100,0,0)',", "HS2","Horizon Strike 2(turning pizza slices)","Horizon Strike 2",0),
            new Mechanic(39787, "Diffractive Edge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'star',color:'rgb(255,200,0)',", "5Cone","Diffractive Edge (5 Cone Knockback)","Five Cones",0),
            new Mechanic(39728, "Solar Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(255,0,0)',", "Ball","Stood in Red Overhead Ball Field","Red Ball Aoe",0), 
            new Mechanic(39711, "Focused Rage", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'triangle-left',color:'rgb(140,0,140)',", "Cone KB", "Knockback in Cone with overhead crosshair", "Knockback Cone",0),
            new Mechanic(39691, "Solar Discharge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle-open',color:'rgb(255,0,0)',", "Wave","Knockback shockwave after Overhead Balls", "Shockwave",0),
            new Mechanic(38982, "Starburst Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle-open',color:'rgb(255,140,0)',", "Ring","Expanding/Retracting Lifting Ring", "Float Ring",0),
            new Mechanic(39297, "Horizon Strike Normal", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(150,0,0)',", "HS norm","Horizon Strike (normal)", "Horizon Strike (normal)",0), ////shouldn't even appear in CM?
            new Mechanic(38844, "Overhead Smash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(200,0,0)',", "Smash",0),
            new Mechanic(38880, "Corporeal Reassignment", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'square',color:'rgb(0,128,0)',", "Skull","Exploding Skull mechanic application", "Corporeal Reassignment",0),
            new Mechanic(39442, "Blinding Radiance", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'square-open',color:'rgb(255,0,0)',", "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)",0),
            //new Mechanic(39823, "Solar Blast", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-diamond',color:'rgb(255,0,0)',", "Eye",0),
            new Mechanic(39849, "Explode", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(255,200,0)',", "Blm.Exp","Hit by Solar Bloom explosion", "Bloom Explosion",0), //
            new Mechanic(39558, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new Mechanic(39928, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new Mechanic(39131, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix","Fixated by Solar Bloom", "Bloom Fixate",0), 
            new Mechanic(38985, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix","Fixated by Solar Bloom", "Bloom Fixate",0), 
            //Breakbar (39645) is cast with is_activation=5 (reset) when failed (not broken).
            });
        }

        public override CombatReplayMap GetCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/BIybWJe.png",
                            Tuple.Create(914, 914),
                            Tuple.Create(-19231, -18137, -16591, -15677),
                            Tuple.Create(-24576, -24576, 24576, 24576),
                            Tuple.Create(11204, 4414, 13252, 6462));
        }
        
        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/u6vv8cW.png";
        }
    }
}
