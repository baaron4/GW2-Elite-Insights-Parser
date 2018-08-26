using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class Siax : FractalLogic
    {
        public Siax() : base()
        {
            mechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(37477, "Vile Spit", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Siax, "symbol:'circle',color:'rgb(70,150,0)',", "Spit",0), //Vile Spit (green goo), Poison Spit
            new Mechanic(37488, "Tail Lash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Siax, "symbol:'triangle-left',color:'rgb(255,200,0)',", "Tail",0), //Tail Lash (half circle Knockback), Tail Lash
            new Mechanic(16911, "Nightmare Hallucination", Mechanic.MechType.Spawn, ParseEnum.BossIDS.Siax, "symbol:'star-open',color:'rgb(100,100,100)',", "NgtmHlc",0), //Tail Lash (half circle Knockback), Tail Lash
            new Mechanic(37303, "Cascade of Torment", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Siax, "symbol:'circle-open',color:'rgb(0,111,111)',", "Rings", 0), //Cascade of Torment (Alternating Rings), Rings
            new Mechanic(36984, "Cascade of Torment", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Siax, "symbol:'circle-open',color:'rgb(0,111,111)',", "Rings", 0), //Cascade of Torment (Alternating Rings), Rings
            //new Mechanic(37078, "Caustic Barrage", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Siax, "symbol:'circle-open',color:'rgb(0,0,111)',", "CB1", 0), //Cascade of Torment (Alternating Rings), Rings
            //new Mechanic(37497, "Caustic Barrage", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Siax, "symbol:'circle-open',color:'rgb(111,111,111)',", "CB2", 0), //Cascade of Torment (Alternating Rings), Rings
            new Mechanic(37320, "Caustic Explosion", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Siax, "symbol:'diamond-tall',color:'rgb(0,200,100)',", "Phase", 0), //Cascade of Torment (Alternating Rings), Rings
            new Mechanic(37320, "Caustic Explosion", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Siax, "symbol:'diamond-tall',color:'rgb(255,0,0)',", "Ph.Fail", 0, delegate(long value){return value >=20649;}), //Cascade of Torment (Alternating Rings), Rings
            new Mechanic(36929, "Caustic Explosion", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Siax, "symbol:'diamond-wide',color:'rgb(0,200,100)',", "CC", 0), //Cascade of Torment (Alternating Rings), Rings
            new Mechanic(36929, "Caustic Explosion", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Siax, "symbol:'diamond-wide',color:'rgb(255,0,0)',", "CC.Fail", 0, delegate(long value){return value >=15232;}), //Cascade of Torment (Alternating Rings), Rings
            new Mechanic(36894, "Volatile Expulsion", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Siax, "symbol:'circle-open',color:'rgb(0,255,111)',", "VE", 0), //Cascade of Torment (Alternating Rings), Rings
            new Mechanic(36998, "Fixated", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Siax, "symbol:'star-open',color:'rgb(200,0,200)',", "Fix", 0), //Cascade of Torment (Alternating Rings), Rings
            });
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/UzaQHW9.png",
                            Tuple.Create(476, 548),
                            Tuple.Create(663, -4127, 3515, -997),
                            Tuple.Create(-6144, -6144, 9216, 9216),
                            Tuple.Create(11804, 4414, 12444, 5054));
        }

        public override List<ParseEnum.ThrashIDS> getAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO: needs facing information for hadouken
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Hallucination
                    };
            return ids;
        }

        public override string getReplayIcon()
        {
            return "https://i.imgur.com/5C60cQb.png";
        }
    }
}
