using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class UnknownFightLogic : FractalLogic
    {
        public UnknownFightLogic(ushort triggerID) : base(triggerID)
        {
            Extension = "boss";
            IconUrl = "https://wiki.guildwars2.com/images/d/d2/Guild_emblem_004.png";
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
            
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>();
        }

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            
        }

        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            
        }
    }
}
