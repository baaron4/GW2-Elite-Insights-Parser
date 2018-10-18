using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class Connector
    {
        public abstract object GetConnectedTo(CombatReplayMap map);
    }
}
