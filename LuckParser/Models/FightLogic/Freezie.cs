using LuckParser.Models.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.Logic
{
    class Freezie : RaidLogic
    {
        public Freezie(ushort triggerID) : base(triggerID)
        {
            CanCombatReplay = false;
            Extension = "freezie";
            IconUrl = "https://wiki.guildwars2.com/images/thumb/8/8b/Freezie.jpg/189px-Freezie.jpg";
        }
    }
}
