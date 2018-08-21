using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class Ensolyss : FractalLogic
    {
        public Ensolyss() : base()
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(37154, "Lunge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'circle',color:'rgb(50,150,0)',", "Lunge",0),
            new Mechanic(37278, "First Smash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'circle',color:'rgb(100,150,0)',", "First Smash",0),
            new Mechanic(36962, "Torment Smash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'circle',color:'rgb(150,150,0)',", "Torment Smash",0)
            });
        }

        public override CombatReplayMap GetCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/kjelZ4t.png",
                            Tuple.Create(366, 366),
                            Tuple.Create(252, 1, 2892, 2881),
                            Tuple.Create(-6144, -6144, 9216, 9216),
                            Tuple.Create(11804, 4414, 12444, 5054));
        }
  
        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/GUTNuyP.png";
        }
    }
}
