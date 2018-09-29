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
            public int[] TargetIds;
            public bool DrawStart;
            public bool DrawEnd;
            public bool DrawArea;
        }

        public long Start;
        public long End;
        public string Name;
        public JsonExtraPhase ExtraData;
    }
}
