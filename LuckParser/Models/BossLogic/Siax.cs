using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;

namespace LuckParser.Models
{
    public class Siax : FractalLogic
    {
        public Siax()
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(37477, "Vile Spit", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Siax, "symbol:'circle',color:'rgb(150,150,0)',", "Spit","Vile Spit (green goo)", "Poison Spit",0),
            new Mechanic(37488, "Tail Lash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Siax, "symbol:'triangle',color:'rgb(255,200,0)',", "Tail","Tail Lash (half circle Knockback)", "Tail Lash",0),
            });
        }

        public override CombatReplayMap GetCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/UzaQHW9.png",
                            Tuple.Create(476, 548),
                            Tuple.Create(663, -4127, 3515, -997),
                            Tuple.Create(-6144, -6144, 9216, 9216),
                            Tuple.Create(11804, 4414, 12444, 5054));
        }

        public override List<ParseEnum.ThrashIDS> GetAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO: needs facing information for hadouken
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Hallucination
                    };
            return ids;
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/5C60cQb.png";
        }
    }
}
