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
            public string ShortName;
            public bool Enemy;
            public int TargetIndex;
            public long Skill;
        }

        public long Time;
        public string Player;
        public string Description;
        public JsonExtraMechanic ExtraData;

    }
}
