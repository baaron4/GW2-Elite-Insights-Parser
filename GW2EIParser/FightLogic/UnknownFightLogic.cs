using System.Collections.Generic;

namespace GW2EIParser.Logic
{
    public class UnknownFightLogic : FightLogic
    {
        public UnknownFightLogic(ushort triggerID) : base(triggerID)
        {
            Extension = "boss";
            Icon = "https://wiki.guildwars2.com/images/d/d2/Guild_emblem_004.png";
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>();
        }
    }
}
