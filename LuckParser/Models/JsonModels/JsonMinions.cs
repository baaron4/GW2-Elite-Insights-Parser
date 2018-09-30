using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class JsonMinions
    {

        public string Name;
        public Dictionary<long, JsonDamageDist>[] TotalDamageDist;
        public Dictionary<long, JsonDamageDist>[][] TargetDamageDist;
        public Dictionary<long, List<JsonSkill>> Rotation;
    }
}
