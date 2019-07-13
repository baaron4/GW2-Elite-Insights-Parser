using System.Collections.Generic;

namespace LuckParser.Logic
{
    public class UnknownFightLogic : FightLogic
    {
        public UnknownFightLogic(ushort triggerID) : base(triggerID)
        {
            Extension = "boss";
            IconUrl = "https://wiki.guildwars2.com/images/d/d2/Guild_emblem_004.png";
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>();
        }
    }
}
