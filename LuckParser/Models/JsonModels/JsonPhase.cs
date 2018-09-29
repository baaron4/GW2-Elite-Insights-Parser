using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonPhase
    {

        public class JsonExtraPhase
        {
            public int[] TI;
            public bool DS;
            public bool DE;
            public bool DA;
        }

        public long Start;
        public long End;
        public string Name;
        public JsonExtraPhase ED;
    }
}
