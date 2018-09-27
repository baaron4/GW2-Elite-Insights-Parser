using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;

namespace LuckParser.Models
{
    public class MAMA : FractalLogic
    {
        public MAMA(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(37408, "Blastwave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle',color:'rgb(255,0,0)',", "KB","Blastwave (Spinning Knockback)", "KB Spin",0),
            new Mechanic(37103, "Blastwave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle',color:'rgb(255,0,0)',", "KB","Blastwave (Spinning Knockback)", "KB Spin",0),
            new Mechanic(37391, "Tantrum", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'star-diamond-open',color:'rgb(0,255,0)',", "Tntrm","Tantrum (Double hit or Slams)", "Dual Spin/Slams",700),
            new Mechanic(37577, "Leap", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'triangle-down',color:'rgb(255,0,0)',", "Jmp","Leap (<33% only)", "Leap",0), 
            new Mechanic(37437, "Shoot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(130,180,0)',", "Shoot","Toxic Shoot (Green Bullets)", "Toxic Shoot",0),
            new Mechanic(37185, "Explosive Impact", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle',color:'rgb(255,200,0)',", "ExplImp","Explosive Impact (Knight Jump)", "Knight Jump",0),
            new Mechanic(37085, "Sweeping Strikes", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'asterisk-open',color:'rgb(255,0,0)',", "Swp","Swings (Many rapid front spins)", "Sweeping Strikes",200), 
            new Mechanic(37217, "Nightmare Miasma", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(255,0,255)',", "Goo","Nightmare Miasma (Poison Puddle)", "Poison Goo",700), 
            new Mechanic(37180, "Grenade Barrage", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(255,200,0)',", "Brg","Grenade Barrage (many projectiles in all directions)", "Ball Barrage",0), 
            new Mechanic(37173, "Red Ball Shot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(255,0,0)',", "Bll","Small Red Bullets", "Bullet",0), 
            new Mechanic(36903, "Extraction", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'bowtie',color:'rgb(255,140,0)',", "Pll","Extraction (Knight Pull Circle)", "Knight Pull",0),
            new Mechanic(36887, "Homing Grenades", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'star-triangle-down-open',color:'rgb(255,0,0)',", "HmGrnds","Homing Grenades", "Homing Grenades",0), 
            new Mechanic(37303, "Cascade of Torment", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(255,140,0)',", "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0), 
            new Mechanic(36984, "Cascade of Torment", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'circle-open',color:'rgb(255,140,0)',", "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0), 
            new Mechanic(37315, "Knight's Daze", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MAMA, "symbol:'square-open',color:'rgb(200,140,255)',", "K.Daze","Knight's Daze", "Daze", 0), 
            
            });
            Extension = "mama";
            IconUrl = "http://dulfy.net/wp-content/uploads/2016/11/gw2-nightmare-fractal-teaser.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/lFGNKuf.png",
                            Tuple.Create(664, 407),
                            Tuple.Create(1653, 4555, 5733, 7195),
                            Tuple.Create(-6144, -6144, 9216, 9216),
                            Tuple.Create(11804, 4414, 12444, 5054));
        }


        public override void ComputeAdditionalBossData(Boss boss, ParsedLog log)
        {
            CombatReplay replay = boss.CombatReplay;
            List<CastLog> cls = boss.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (boss.ID)
            {
                case (ushort)ParseEnum.BossIDS.MAMA:
                    replay.Icon = "https://i.imgur.com/1h7HOII.png";
                    break;
            }
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                ParseEnum.TrashIDS.GreenKnight,
                ParseEnum.TrashIDS.RedKnight,
                ParseEnum.TrashIDS.BlueKnight,
                ParseEnum.TrashIDS.TwistedHorror
            };
        }
    }
}
