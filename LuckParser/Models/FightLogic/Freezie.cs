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
        }
    }
}
