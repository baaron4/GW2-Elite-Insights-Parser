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
            public bool Auto;
            public int TimeSaved;
            public int TimeWasted;
            public bool UnderQuickness;
            public long Skill;
        }

        public string Name;
        public int Time;
        public int Duration;
        public JsonExtraSkill ExtraData;
    }
}
