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
        public Dictionary<string, JsonDamageDist>[] TotalDamageDist;
        public Dictionary<string, JsonDamageDist>[][] TargetDamageDist;
        public Dictionary<string, List<JsonSkill>> Rotation;
    }
}
