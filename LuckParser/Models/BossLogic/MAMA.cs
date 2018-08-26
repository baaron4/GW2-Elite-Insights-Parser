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
            new Mechanic(37391, "Tantrum", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'star-diamond-open',color:'rgb(0,255,0)',", "Tntrm",700), //Tantrum (Double hit or Slams), Dual Spin/Slams
            new Mechanic(37577, "Leap", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle',color:'rgb(255,0,0)',", "Jmp",0), //Leap (<33% only), Leap
            new Mechanic(37437, "Shoot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(200,150,0)',", "Shoot",0), //Toxic Shoot (Green Bullets), Toxic Shoot
            new Mechanic(37185, "Explosive Impact", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'diamond',color:'rgb(255,200,0)',", "ExplImp",0), // Explosive Impact (Knight Jump), Knight Jump
            new Mechanic(37085, "Sweeping Strikes", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'square-open',color:'rgb(255,0,0)',", "Swp",200), //Swings (Many rapid front spins), Sweeping Strikes
            new Mechanic(37217, "Nightmare Miasma", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(0,255,255)',", "Goo",700), //Nightmare Miasma (Poison Puddle), Poison Goo
            new Mechanic(37180, "Grenade Barrage", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(255,0,0)',", "Brg",0), //Grenade Barrage (many projectiles in all directions), Ball Barrage
            new Mechanic(37173, "Red Ball Shot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(255,0,0)',", "Bll",0), //Grenade Barrage (many projectiles in all directions), Ball Barrage
            new Mechanic(36903, "Extraction", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(255,0,0)',", "Pll",0), //Extraction (Knight Pull Circle), Knight Pull
            new Mechanic(36887, "Homing Grenades", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(255,0,0)',", "HmGrnds",0), //Homing Grenades, Homing Grenades
            //new Mechanic(31371, "Homing ?", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(255,111,0)',", "Hm?",0), //Homing Grenades, Homing Grenades
            //new Mechanic(37450, "???", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'diamond-open',color:'rgb(255,0,0)',", "??",0), //Grenade Barrage (many projectiles in all directions), Ball Barrage
            //new Mechanic(38134, "??", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MAMA, "symbol:'diamond-open',color:'rgb(255,255,0)',", "??B",0), //Grenade Barrage (many projectiles in all directions), Ball Barrage
            //new Mechanic(36865, "Arkk's Shield", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(0,255,255)',", "Shld",250), //Arkk's Shield (stood in bubble), Shield //PoV Only
            new Mechanic(37030, "Toxic Sickness", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(0,255,111)',", "Vmt", 0), //Vomit, Vomit //PoV Only
            new Mechanic(37303, "Cascade of Torment", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(0,111,111)',", "Rings", 0), //Cascade of Torment (Alternating Rings), Rings
            new Mechanic(36984, "Cascade of Torment", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(0,111,111)',", "Rings", 0), //Cascade of Torment (Alternating Rings), Rings
            new Mechanic(37315, "Knight's Daze", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(0,111,111)',", "K.Daze", 0), //Knight's Daze, Daze
            
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
