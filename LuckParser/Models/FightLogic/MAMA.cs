using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class MAMA : FractalLogic
    {
        public MAMA(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(37408, "Blastwave", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(255,0,0)"), "KB","Blastwave (Spinning Knockback)", "KB Spin",0),
            new Mechanic(37103, "Blastwave", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(255,0,0)"), "KB","Blastwave (Spinning Knockback)", "KB Spin",0),
            new Mechanic(37391, "Tantrum", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("star-diamond-open","rgb(0,255,0)"), "Tantrum","Tantrum (Double hit or Slams)", "Dual Spin/Slams",700),
            new Mechanic(37577, "Leap", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-down","rgb(255,0,0)"), "Jump","Leap (<33% only)", "Leap",0), 
            new Mechanic(37437, "Shoot", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(130,180,0)"), "Shoot","Toxic Shoot (Green Bullets)", "Toxic Shoot",0),
            new Mechanic(37185, "Explosive Impact", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Knight Jump","Explosive Impact (Knight Jump)", "Knight Jump",0),
            new Mechanic(37085, "Sweeping Strikes", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("asterisk-open","rgb(255,0,0)"), "Sweep","Swings (Many rapid front spins)", "Sweeping Strikes",200), 
            new Mechanic(37217, "Nightmare Miasma", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,0,255)"), "Goo","Nightmare Miasma (Poison Puddle)", "Poison Goo",700), 
            new Mechanic(37180, "Grenade Barrage", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,200,0)"), "Barrage","Grenade Barrage (many projectiles in all directions)", "Ball Barrage",0), 
            new Mechanic(37173, "Red Ball Shot", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Ball","Small Red Bullets", "Bullet",0), 
            new Mechanic(36903, "Extraction", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("bowtie","rgb(255,140,0)"), "Pull","Extraction (Knight Pull Circle)", "Knight Pull",0),
            new Mechanic(36887, "Homing Grenades", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("star-triangle-down-open","rgb(255,0,0)"), "Grenades","Homing Grenades", "Homing Grenades",0), 
            new Mechanic(37303, "Cascade of Torment", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0), 
            new Mechanic(36984, "Cascade of Torment", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0), 
            new Mechanic(37315, "Knight's Daze", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("square-open","rgb(200,140,255)"), "Daze","Knight's Daze", "Daze", 0), 
            
            });
            Extension = "mama";
            IconUrl = "http://dulfy.net/wp-content/uploads/2016/11/gw2-nightmare-fractal-teaser.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/lFGNKuf.png",
                            (664, 407),
                            (1653, 4555, 5733, 7195),
                            (-6144, -6144, 9216, 9216),
                            (11804, 4414, 12444, 5054));
        }


        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            CombatReplay replay = target.CombatReplay;
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.MAMA:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                GreenKnight,
                RedKnight,
                BlueKnight,
                TwistedHorror
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)GreenKnight:
                case (ushort)RedKnight:
                case (ushort)BlueKnight:
                case (ushort)TwistedHorror:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
        }
    }
}
