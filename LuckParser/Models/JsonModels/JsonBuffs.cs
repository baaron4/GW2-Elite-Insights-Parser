using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LuckParser.Models.JsonModels.JsonStatistics;

namespace LuckParser.Models.JsonModels
{
    public class JsonBuffs
    {
        public List<JsonBuffsData> Data;
        public List<int[]> States;
    }
}
