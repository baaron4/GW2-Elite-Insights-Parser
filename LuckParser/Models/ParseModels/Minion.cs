using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class Minion : Actor
    {
        private ushort master_id;

        public Minion(ushort master, AgentItem agent) : base(agent)
        {
            master_id = master;
        }
        
    }
}
