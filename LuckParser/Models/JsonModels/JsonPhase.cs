using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonPhase
    {
        public long Start;
        public long End;
        public string Name;
        public List<int> Targets;
        public List<int> SubPhases;
    }
}
