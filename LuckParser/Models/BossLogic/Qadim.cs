using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

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
            new Mechanic(52520, "Elemental Breath", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'triangle-left',color:'rgb(255,0,0)',", "H.Brth","Elemental Breath (Hydra Breath)", "Hydra Breath",0),
            new Mechanic(53013, "Fireball", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle-open',color:'rgb(255,150,0)',size:10,", "H.Fb","Fireball (Hydra)", "Hydra Fireball",0),
            new Mechanic(52941, "Fiery Meteor", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle-open',color:'rgb(255,150,0)',", "H.Mtr","Fiery Meteor (Hydra)", "Hydra Meteor",0),
            new Mechanic(53051, "Teleport", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Qadim, "symbol:'circle',color:'rgb(150,0,200)',", "H.KB","Teleport Knockback (Hydra)", "Hydra TP KB",0),

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

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>();
        }

        public override void ComputeAdditionalBossData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            
        }

        public override void ComputeAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {

        }

        public override int IsCM(ParsedLog log)
        {
            return 0; //Check via Hydra HP or (>27e6)
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/IfoHTHT.png";
            // For legendary adds: https://imgur.com/YABLiBz https://imgur.com/0LGKCn2 https://imgur.com/kLKLSfv https://imgur.com/vjjNSpI
            // Zommoros: https://imgur.com/BxbsRCI
        }
    }
}
