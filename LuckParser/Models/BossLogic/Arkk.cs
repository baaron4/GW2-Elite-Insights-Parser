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
            new Mechanic(39685, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(255,140,0)',", "HS",0), // Horizon Strike (turning pizza slices), Horizon Strike
            new Mechanic(39001, "Horizon Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(255,140,0)',", "HS",0), // Horizon Strike (turning pizza slices), Horizon Strike 
            new Mechanic(39787, "Diffractive Edge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'star',color:'rgb(255,200,0)',", "5Cone",0), // Diffractive Edge (5 Cone Knockback), Five Cones
            new Mechanic(39755, "Diffractive Edge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'star',color:'rgb(255,200,0)',", "5Cone",0), // Diffractive Edge (5 Cone Knockback), Five Cones
            new Mechanic(39728, "Solar Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(128,0,0)',", "Ball",0), // Stood in Red Overhead Ball Field, Red Ball Aoe
            new Mechanic(39711, "Focused Rage", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'triangle-down',color:'rgb(255,100,0)',", "Cone KB",0), // Knockback in Cone with overhead crosshair, Knockback Cone
            new Mechanic(39691, "Solar Discharge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle-open',color:'rgb(255,0,0)',", "Wave",0), // Knockback shockwave after Overhead Balls, Shockwave
            new Mechanic(38982, "Starburst Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle-open',color:'rgb(255,140,0)',", "Ring",500), //Expanding/Retracting Lifting Ring, Float Ring
            new Mechanic(39523, "Starburst Cascade", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle-open',color:'rgb(255,140,0)',", "Ring",500), //Expanding/Retracting Lifting Ring, Float Ring
            new Mechanic(39297, "Horizon Strike Normal", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(80,0,0)',", "HS norm",0), //Horizon Strike (normal), Horizon Strike (normal)//shouldn't even appear in CM?
            new Mechanic(38844, "Overhead Smash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'triangle-left',color:'rgb(200,0,0)',", "Smash",0), //Overhead Smash, Overhead Smash
            new Mechanic(38880, "Corporeal Reassignment", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'diamond-tall',color:'rgb(255,0,0)',", "Skull",0), //Exploding Skull mechanic application, Corporeal Reassignment
            new Mechanic(39849, "Explode", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(255,200,0)',", "Blm.Exp",0), //Hit by Solar Bloom explosion, Bloom Explosion
            new Mechanic(39558, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix",0), //Fixated by Solar Bloom, Bloom Fixate
            new Mechanic(39928, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix",0), //Fixated by Solar Bloom, Bloom Fixate
            new Mechanic(39131, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix",0), //Fixated by Solar Bloom, Bloom Fixate
            new Mechanic(38985, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'star-open',color:'rgb(255,0,255)',", "Blm.Fix",0), //Fixated by Solar Bloom, Bloom Fixate
            new Mechanic(39268, "Cosmic Meteor", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'circle-open',color:'rgb(0,255,0)',", "Green",0), //Temporal Realignment (Green) application, Green
            new Mechanic(791, "Fear", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'square-open',color:'rgb(255,0,0)',", "Eye",0,(value => value == 3000)), //Hit by the Overhead Eye Fear, Eye (Fear) //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new Mechanic(39645, "Breakbar Start", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Arkk, "symbol:'diamond-wide',color:'rgb(0,160,150)',", "Breakbar",0), //Start Breakbar, CC
            new Mechanic(39645, "Breakbar End", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Arkk, "symbol:'diamond-wide',color:'rgb(255,0,0)',", "CC.Fail",0,(value => value > 9668)), //Breakbar (Failed CC), CC Fail
            new Mechanic(34748, "Overhead Smash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'triangle-left-open',color:'rgb(200,0,0)',", "A.Smsh",0), //Overhead Smash (Arcdiviner), Smash (Add)
            new Mechanic(39674, "Rolling Chaos", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(255,50,50)',", "RlMrbl",0), //Rolling Chaos (Arrow marble), KD Marble
            new Mechanic(39298, "Solar Stomp", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'triangle-up',color:'rgb(200,0,200)',", "Stmp",0), //Solar Stomp (Evading Stomp), Evading Jump
            new Mechanic(39021, "Cosmic Streaks", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Arkk, "symbol:'diamond-open',color:'rgb(255,0,100)',", "DDR.Bm",0), //Triple Death Ray Cast (last phase), Death Ray Cast
            new Mechanic(35940, "Whirling Devastation", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'star-diamond-open',color:'rgb(180,0,100)',", "Whrl",300), //Whirling Devastation (Gladiator Spin), Gladiator Spin
            new Mechanic(35761, "Pull Charge", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Arkk, "symbol:'bowtie',color:'rgb(0,160,150)',", "Pull",0), //Pull Charge (Gladiator Pull), Gladiator Pull
            new Mechanic(35761, "Pull Charge", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Arkk, "symbol:'bowtie',color:'rgb(255,0,0)',", "Pll.Fail",0,(value => value > 3200)), //Pull Charge CC failed, CC fail (Gladiator)
            new Mechanic(35452, "Spinning Cut", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'star-square-open',color:'rgb(200,140,255)',", "Daze",0), //Spinning Cut (3rd Gladiator Auto->Daze), Gladiator Daze
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
