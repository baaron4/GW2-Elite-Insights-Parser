using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class MAMA : FractalLogic
    {
        public MAMA() : base()
        {
            mechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(37408, "Blastwave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle',color:'rgb(255,200,0)',", "KB",0), //Blastwave (Spinning Knockback), KB Spin
            new Mechanic(37103, "Blastwave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle',color:'rgb(255,200,0)',", "KB",0), //Blastwave (Spinning Knockback), KB Spin
            new Mechanic(37391, "Tantrum", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'star-diamond-open',color:'rgb(0,255,0)',", "Tntrm",4000), //Tantrum (Many rapid front spins), Spinning Tantrum
            new Mechanic(37577, "Leap", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle',color:'rgb(255,0,0)',", "Jmp",0), //Leap (<33% only), Leap
            new Mechanic(37437, "Shoot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(200,150,0)',", "Shoot",0), //Shoot, Shoot
            new Mechanic(37185, "Explosive Impact", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'diamond',color:'rgb(255,200,0)',", "ExplImp",0), // Explosive Impact, Explosive Impact
            new Mechanic(37085, "Sweeping Strikes", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'square-open',color:'rgb(255,0,0)',", "Swp",200), //Double Swing (between Jump and Slams), Sweeping Strikes
            new Mechanic(37217, "Nightmare Miasma", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(0,255,255)',", "Goo",0), //Nightmare Miasma (Poison Puddle), Poison Goo
            new Mechanic(37180, "Grenade Barrage", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(255,0,0)',", "Balls",0), //Grenade Barrage (many projectiles in all directions), Ball Barrage
            });
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/lFGNKuf.png",
                            Tuple.Create(664, 407),
                            Tuple.Create(1653, 4555, 5733, 7195),
                            Tuple.Create(-6144, -6144, 9216, 9216),
                            Tuple.Create(11804, 4414, 12444, 5054));
        }

        public override string getReplayIcon()
        {
            return "https://i.imgur.com/1h7HOII.png";
        }
    }
}
