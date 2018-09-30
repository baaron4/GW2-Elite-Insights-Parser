using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonMechanic
    {

        public class JsonExtraMechanic
        {
            public string SN;
            public int E;
            public int TI;
            public long S;
        }

        public long Time;
        public string Player;
        public JsonExtraMechanic ED;

    }
}
