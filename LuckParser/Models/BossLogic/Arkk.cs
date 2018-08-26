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
            new Mechanic(39685, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(50,0,0)',", "HS",0), // Horizon Strike (turning pizza slices), Horizon Strike
            new Mechanic(39001, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(50,0,0)',", "HS",0), // Horizon Strike 2(turning pizza slices), Horizon Strike 2
            new Mechanic(39787, "Diffractive Edge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'star',color:'rgb(255,200,0)',", "5Cone",0), // Diffractive Edge (5 Cone Knockback), Five Cones
            new Mechanic(39755, "Diffractive Edge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'star',color:'rgb(255,200,0)',", "5Cone",0), // Diffractive Edge (5 Cone Knockback), Five Cones
            new Mechanic(39728, "Solar Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(255,0,0)',", "Ball",0), // Stood in Red Overhead Ball Field, Red Ball Aoe
            new Mechanic(39711, "Focused Rage", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'triangle-left',color:'rgb(140,0,140)',", "Cone KB",0), // Knockback in Cone with overhead crosshair, Knockback Cone
            new Mechanic(39691, "Solar Discharge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle-open',color:'rgb(255,0,0)',", "Wave",0), // Knockback shockwave after Overhead Balls, Shockwave
            new Mechanic(38982, "Starburst Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle-open',color:'rgb(255,140,0)',", "Ring",0), //Expanding/Retracting Lifting Ring, Float Ring
            new Mechanic(39523, "Starburst Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle-open',color:'rgb(255,140,0)',", "Ring",0), //Expanding/Retracting Lifting Ring, Float Ring
            new Mechanic(39297, "Horizon Strike Normal", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(150,0,0)',", "HS norm",0), //Horizon Strike (normal), Horizon Strike (normal)//shouldn't even appear in CM?
            new Mechanic(38844, "Overhead Smash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(200,0,0)',", "Smash",0), //Overhead Smash, Overhead Smash
            new Mechanic(38880, "Corporeal Reassignment", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'square',color:'rgb(0,128,0)',", "Skull",0), //Exploding Skull mechanic application, Corporeal Reassignment
            //new Mechanic(39442, "Blinding Radiance", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Arkk, "symbol:'square-open',color:'rgb(255,0,0)',", "B.Eye",0), //Hit by the Overhead Eye Fear, Eye (Fear)
            new Mechanic(39823, "Solar Blast", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'star-diamond',color:'rgb(255,0,0)',", "SlBl",0),
            new Mechanic(39849, "Explode", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(255,200,0)',", "Blm.Exp",0), //Hit by Solar Bloom explosion, Bloom Explosion
            new Mechanic(39558, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix",0), //Fixated by Solar Bloom, Bloom Fixate
            new Mechanic(39928, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix",0), //Fixated by Solar Bloom, Bloom Fixate
            new Mechanic(39131, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix",0), //Fixated by Solar Bloom, Bloom Fixate
            new Mechanic(38985, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix",0), //Fixated by Solar Bloom, Bloom Fixate
            //new Mechanic(39237, "Cosmic Energy", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Arkk, "symbol:'diamond-open',color:'rgb(0,0,0)',", "CE",0),
            //new Mechanic(38858, "Temporal Realignment", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'square-open',color:'rgb(0,0,0)',", "Green",0), //Exploding Skull mechanic application, Corporeal Reassignment
            new Mechanic(39268, "Cosmic Meteor", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'square-open',color:'rgb(0,0,180)',", "Green",0), //Exploding Skull mechanic application, Corporeal Reassignment
            new Mechanic(791, "Fear", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'square-open',color:'rgb(255,0,0)',", "Eye",0,delegate(long value){return value == 3000;}), //Hit by the Overhead Eye Fear, Eye (Fear) //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new Mechanic(39645, "Breakbar Start", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Arkk, "symbol:'diamond-wide',color:'rgb(0,200,100)',", "Breakbar",0),
            new Mechanic(39645, "Breakbar End", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Arkk, "symbol:'diamond-wide',color:'rgb(255,0,0)',", "CC.Fail",0,delegate(long value){return value > 9668;}), //Summon Fragment (Failed CC), CC Fail
            new Mechanic(38941, "Small  Ball", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(255,200,0)',", "SmBll",0), //Hit by Solar Bloom explosion, Bloom Explosion
            new Mechanic(34748, "Overhead Smash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'diamond-open',color:'rgb(100,200,0)',", "A.Smsh",0), //Hit by Solar Bloom explosion, Bloom Explosion
            new Mechanic(39674, "Rolling Chaos", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'diamond-open',color:'rgb(100,100,100)',", "RlMrbl",0), //Hit by Solar Bloom explosion, Bloom Explosion
            new Mechanic(39638, "Mist Bomb", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Arkk, "symbol:'diamond-open',color:'rgb(100,0,100)',", "MB",0), //Hit by Solar Bloom explosion, Bloom Explosion
            new Mechanic(39298, "Solar Stomp", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'diamond-open',color:'rgb(180,0,100)',", "Stmp",0), //Hit by Solar Bloom explosion, Bloom Explosion
            new Mechanic(39078, "Plunge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'diamond-open',color:'rgb(180,0,100)',", "Plg",0), //Hit by Solar Bloom explosion, Bloom Explosion
            new Mechanic(39021, "Cosmic Streaks", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Arkk, "symbol:'diamond-open',color:'rgb(180,0,100)',", "DDR.Bm",0), //Hit by Solar Bloom explosion, Bloom Explosion
            new Mechanic(35940, "Whirling Devastation", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'square-open',color:'rgb(180,0,100)',", "Whrl",0), //Hit by Solar Bloom explosion, Bloom Explosion
            //new Mechanic(35611, "Prey", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'diamond-open',color:'rgb(0,100,100)',", "Prey",0), //Hit by Solar Bloom explosion, Bloom Explosion
            //new Mechanic(35681, "Dice", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'diamond-open',color:'rgb(0,201,100)',", "Dice",0), //Hit by Solar Bloom explosion, Bloom Explosion
            new Mechanic(35761, "Pull Charge", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Arkk, "symbol:'diamond-open',color:'rgb(0,255,100)',", "Pull",0), //Hit by Solar Bloom explosion, Bloom Explosion
            new Mechanic(35761, "Pull Charge", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Arkk, "symbol:'diamond',color:'rgb(255,255,100)',", "Pll.Fail",0,delegate(long value){return value > 3200; }), //Hit by Solar Bloom explosion, Bloom Explosion
            new Mechanic(35452, "Spinning Cut", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'diamond-open',color:'rgb(0,255,255)',", "Daze",0), //Hit by Solar Bloom explosion, Bloom Explosion
            // 38858, "Temporal Realignment"
            //new Mechanic(39111, ",Diaphanous Shielding", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Arkk, "symbol:'square-open',color:'rgb(0,180,0)',", "Shld",0), //Hit by the Overhead Eye Fear, Eye (Fear)

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
