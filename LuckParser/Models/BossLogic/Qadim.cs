using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class Qadim : RaidLogic
    {
        public Qadim(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(52242, "Shattering Impact", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle',color:'rgb(255,200,0)',", "Stun","Shattering Impact (Stunning flame bolt)", "Flame Bolt Stun",0),
            new Mechanic(52814, "Flame Wave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle-open',color:'rgb(255,0,0)',", "Shwv","Flame Wave (Knockback)", "Shockwave",0),
            new Mechanic(52814, "Flame Wave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle-open',color:'rgb(255,0,0)',", "KB","Flame Wave (Knockback Frontal Beam)", "KB Frontal Beam",0),
            new Mechanic(52520, "Elemental Breath", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'triangle-left',color:'rgb(255,0,0)',", "H.Brth","Elemental Breath (Hydra Breath)", "Hydra Breath",0),
            new Mechanic(53013, "Fireball", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle-open',color:'rgb(255,150,0)',size:10,", "H.Fb","Fireball (Hydra)", "Hydra Fireball",0),
            new Mechanic(52941, "Fiery Meteor", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle-open',color:'rgb(255,150,0)',", "H.Mtr","Fiery Meteor (Hydra)", "Hydra Meteor",0),
            new Mechanic(53051, "Teleport", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle',color:'rgb(150,0,200)',", "H.KB","Teleport Knockback (Hydra)", "Hydra TP KB",0),
            new Mechanic(52310, "Big Hit", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle',color:'rgb(255,0,0)',", "Mace","Big Hit (Mace Shockwave)", "Mace Shockwave",0),
            new Mechanic(52955, "Lava Pool Drop", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'square-open',color:'rgb(255,0,0)',", "LvPl","Lava Pool drop (on short platform spikes)", "Lava Pool Drop",0),
            //new Mechanic(51943, "Breakbar Start", Mechanic.MechType.EnemyCastStart, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(0,160,150)',", "Breakbar","Start Breakbar", "CC",0),
            //new Mechanic(51943, "Breakbar End", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(255,0,0)',", "CC.Fail","Breakbar (Failed CC)", "CC Fail",0,(condition => condition.CombatItem.Value > 8000)),
            //new Mechanic(51943, "Breakbar End", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Qadim, "symbol:'diamond-tall',color:'rgb(0,160,0)',", "CCed","Breakbar broken", "CCed",0,(condition => condition.CombatItem.Value < 8000)),

            });
            CanCombatReplay = false;
            Extension = "qadim";
            IconUrl = "https://wiki.guildwars2.com/images/f/f2/Mini_Qadim.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/vtVubK8.png",
                            Tuple.Create(3241, 2814),
                            Tuple.Create(-10886, -12019, -3950, -5995),
                            Tuple.Create(-21504,-21504,24576,24576),
                            Tuple.Create(13440,14336,15360,16256));
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.BossIDS.Qadim,
                (ushort)AncientInvokedHydra,
                (ushort)WyvernMatriarch,
                (ushort)WyvernPatriarch,
                (ushort)ApocalypseBringer,
            };
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>()
            {
                LavaElemental1,
                LavaElemental2,
                IcebornHydra,
                GreaterMagmaElemental1,
                GreaterMagmaElemental2,
                FireElemental,
                FireImp,
                PyreGuardian,
                ReaperofFlesh,
                IceElemental,
                Zommoros
            };
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)LavaElemental1:
                case (ushort)LavaElemental2:
                case (ushort)IcebornHydra:
                case (ushort)GreaterMagmaElemental1:
                case (ushort)GreaterMagmaElemental2:
                case (ushort)FireElemental:
                case (ushort)FireImp:
                case (ushort)PyreGuardian:
                case (ushort)ReaperofFlesh:
                case (ushort)IceElemental:
                case (ushort)Zommoros:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalBossData(Boss boss, ParsedLog log)
        {
            switch (boss.ID)
            {
                case (ushort)ParseEnum.BossIDS.Qadim:
                    break;
                case (ushort)AncientInvokedHydra:
                case (ushort)WyvernMatriarch:
                case (ushort)WyvernPatriarch:
                case (ushort)ApocalypseBringer:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {

        }

        public override int IsCM(ParsedLog log)
        {
            return 0; //Check via Hydra HP or (>27e6)
        }
        
    }
}
