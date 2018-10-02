using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class MursaatOverseer : RaidLogic
    {
        public MursaatOverseer(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            new Mechanic(37677, "Soldier's Aura", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle-open',color:'rgb(255,0,0)'", "Jade","Jade Soldier's Aura hit", "Jade Aura",0),
            new Mechanic(37788, "Jade Explosion", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle',color:'rgb(255,0,0)'", "JExpl","Jade Soldier's Death Explosion", "Jade Explosion",0),
            //new Mechanic(37779, "Claim", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, "symbol:'square',color:'rgb(255,200,0)'", "Claim",0), //Buff remove only
            //new Mechanic(37697, "Dispel", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle',color:'rgb(255,200,0)'", "Dispel",0), //Buff remove only
            //new Mechanic(37813, "Protect", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle',color:'rgb(0,255,255)'", "Protect",0), //Buff remove only
            new Mechanic(757, "Invulnerability", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle-open',color:'rgb(0,255,255)'", "Prtct","Protected by the Protect Shield","Protect Shield",0,(condition=> condition.CombatItem.Value == 1000)),
            new Mechanic(38155, "Mursaat Overseer's Shield", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.MursaatOverseer, "symbol:'circle-open',color:'rgb(255,200,0)'", "Shield","Jade Soldier Shield", "Soldier Shield",0),
            new Mechanic(38155, "Mursaat Overseer's Shield", Mechanic.MechType.EnemyBoonStrip, ParseEnum.BossIDS.MursaatOverseer, "symbol:'square-open',color:'rgb(255,200,0)'", "Dspl","Dispelled Jade Soldier Shield", "Dispel",0),
            //new Mechanic(38184, "Enemy Tile", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.MursaatOverseer, "symbol:'square-open',color:'rgb(255,200,0)'", "Floor","Enemy Tile damage", "Tile dmg",0) //Fixed damage (3500), not trackable
            });
            Extension = "mo";
            IconUrl = "https://wiki.guildwars2.com/images/c/c8/Mini_Mursaat_Overseer.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/lT1FW2r.png",
                            Tuple.Create(889, 889),
                            Tuple.Create(1360, 2701, 3911, 5258),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                Jade
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch(mob.ID)
            {
                case (ushort)Jade:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }


        public override void ComputeAdditionalBossData(Boss boss, ParsedLog log)
        {
            // TODO: needs doughnuts (wave) and facing information (sword)
            CombatReplay replay = boss.CombatReplay;
            List<CastLog> cls = boss.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (boss.ID)
            {
                case (ushort)ParseEnum.BossIDS.MursaatOverseer:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override int IsCM(ParsedLog log)
        {
            Boss target = Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.MursaatOverseer);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.Health > 25e6) ? 1 : 0;
        }
    }
}
