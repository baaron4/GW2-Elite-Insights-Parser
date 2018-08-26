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
            mechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(37154, "Lunge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'triangle-right-open',color:'rgb(50,150,0)',", "Chrg",0), // Lunge (KB charge over arena), Charge
            new Mechanic(37278, "Upswing", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'circle',color:'rgb(255,100,0)',", "Smsh1",0), // High damage Jump hit, First Smash 
            new Mechanic(36927, "Lunge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'triangle-right-open',color:'rgb(50,150,0)',", "Chrg",0), // Lunge (KB charge over arena), Charge
            new Mechanic(36962, "Upswing", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'circle',color:'rgb(255,200,0)',", "Smsh2",50), // Torment Explosion from Illusions after jump, Torment Smash
            new Mechanic(37466, "Nightmare Miasma", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'circle-open',color:'rgb(140,0,140)',", "Goo",0), //Nightmare Miasma (Goo), Miasma
            new Mechanic(36944, "Nightmare Miasma", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'circle-open',color:'rgb(140,0,140)',", "Goo",0), //Nightmare Miasma (Goo), Miasma
            new Mechanic(37096, "Caustic Explosion", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-wide',color:'rgb(0,200,100)',", "CC", 0), //Cascade of Torment (Alternating Rings), Rings
            new Mechanic(37096, "Caustic Explosion", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-wide',color:'rgb(255,0,0)',", "CC.Fail", 0, delegate(long value){return value >=15260;}), //Cascade of Torment (Alternating Rings), Rings
            new Mechanic(37096, "Caustic Explosion", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'bowtie',color:'rgb(255,200,0)',", "CC.KB", 0), //Knockback hourglass during CC, CC KB
            new Mechanic(37523, "Nightmare Devastation", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Ensolyss, "symbol:'circle',color:'rgb(0,0,255)',", "Bbbl",0), //Nightmare Miasma (Goo), Miasma
            new Mechanic(37050, "Nightmare Devastation", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Ensolyss, "symbol:'circle',color:'rgb(0,0,255)',", "Bbbl",0), //Nightmare Devastation (bubble attack), Bubble
            new Mechanic(37151, "Nightmare Miasma", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'circle-open',color:'rgb(0,0,255)',", "Goo",0), //Nightmare Miasma (Goo), Miasma
            //new Mechanic(36985, "Nightmare Bomb", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-open',color:'rgb(0,100,255)',", "NB",0), //Nightmare Miasma (Goo), Miasma
            //new Mechanic(37352, "Nightmare Bomb", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-open',color:'rgb(0,180,255)',", "NB2",0), //Nightmare Miasma (Goo), Miasma
            //new Mechanic(37013, "Serpent Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-open',color:'rgb(100,0,255)',", "SS1",0), //Nightmare Miasma (Goo), Miasma
            //new Mechanic(37082, "Serpent Strike", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-open',color:'rgb(180,0,255)',", "SS2",0), //Nightmare Miasma (Goo), Miasma
            //new Mechanic(37316, "Nightmare Bullet", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-open',color:'rgb(0,255,255)',", "Bllt",0), //Nightmare Miasma (Goo), Miasma
            //new Mechanic(37432, "Nightmare Bullet", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-open',color:'rgb(0,255,255)',", "Bllt2",0), //Nightmare Miasma (Goo), Miasma
            //new Mechanic(37461, "Nightmare Bullet", Mechanic.MechType.HitOnEnemy, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-open',color:'rgb(0,255,255)',", "Bllt3",0), //Nightmare Miasma (Goo), Miasma
            //new Mechanic(37473, "Nightmare Bullet", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-open',color:'rgb(0,255,255)',", "Bllt4",0), //Nightmare Miasma (Goo), Miasma
            //new Mechanic(37539, "Nightmare Bullet", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-open',color:'rgb(0,255,255)',", "Bllt5",0), //Nightmare Miasma (Goo), Miasma
            new Mechanic(37003, "Tail Lash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'triangle-left',color:'rgb(255,200,0)',", "Tail",0), //Tail Lash (half circle Knockback), Tail Lash
            new Mechanic(37434, "Rampage", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-open',color:'rgb(255,200,0)',", "Rmpg",0), //Tail Lash (half circle Knockback), Tail Lash
            new Mechanic(37045, "Caustic Grasp", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-open',color:'rgb(0,200,0)',", "Pull",0), //Caustic Grasp (Arena Wide Pull), Pull
            new Mechanic(36926, "Tormenting Blast", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-open',color:'rgb(255,200,25)',", "Qrtr",0), //Tormenting Blast (Two Quatrer Circle attacks), Quarter circle
            new Mechanic(36958, "Caustic Barrage", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-open',color:'rgb(0,200,0)',", "CB",0), //Tail Lash (half circle Knockback), Tail Lash
            new Mechanic(37101, "Nightmare Discharge", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Ensolyss, "symbol:'diamond-open',color:'rgb(0,0,150)',", "ND",0), //Tail Lash (half circle Knockback), Tail Lash
            }); 
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/kjelZ4t.png",
                            Tuple.Create(366, 366),
                            Tuple.Create(252, 1, 2892, 2881),
                            Tuple.Create(-6144, -6144, 9216, 9216),
                            Tuple.Create(11804, 4414, 12444, 5054));
        }
  
        public override string getReplayIcon()
        {
            return "https://i.imgur.com/GUTNuyP.png";
        }
    }
}
