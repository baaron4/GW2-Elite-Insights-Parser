using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonSkill
    {

        public class JsonExtraSkill
        {
            public int A;
            public int TS;
            public int UQ;
        }

        public int Time;
        public int Duration;
        public JsonExtraSkill ED;
    }
}
