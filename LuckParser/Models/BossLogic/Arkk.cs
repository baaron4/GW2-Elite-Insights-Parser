using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;

namespace LuckParser.Models
{
    public class Arkk : FractalLogic
    {
        public Arkk() : base()
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(39685, "Horizon Strike 1", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(50,0,0)',", "Horizon Strike 1",0),
            new Mechanic(39001, "Horizon Strike 2", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(100,0,0)',", "Horizon Strike 2",0),
            new Mechanic(39297, "Horizon Strike Normal", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(150,0,0)',", "Horizon Strike Normal",0),
            new Mechanic(38844, "Overhead Smash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Arkk, "symbol:'circle',color:'rgb(200,0,0)',", "Overhead Smash",0),
            new Mechanic(38880, "Corporeal Reassignment", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Arkk, "symbol:'square',color:'rgb(0,128,0)',", "Corporeal Reassignment",0)
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
