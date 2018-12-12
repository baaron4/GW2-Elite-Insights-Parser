using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonMinions
    {
        public string name;
        public Dictionary<string, JsonDamageDist>[] totalDamageDist;
        public Dictionary<string, JsonDamageDist>[][] targetDamageDist;
        public Dictionary<string, List<JsonSkill>> rotation;
    }
}
