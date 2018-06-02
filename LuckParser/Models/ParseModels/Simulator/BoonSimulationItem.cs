using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class BoonSimulationItem
    {

        public BoonSimulationItem()
        {
        }

        public abstract long getDuration();


        public abstract long getStart();

        public abstract ushort getSrc();

        public abstract long getEnd();

        public abstract int getStack();
    }
}
