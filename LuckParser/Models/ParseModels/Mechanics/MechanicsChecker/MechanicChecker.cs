using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public abstract class MechanicChecker
    {
        public enum ValueCompare { EQ, LEQ, L, GEQ, G, NEQ };
        public enum BoonCompare { GAIN, LOSS };

        public abstract bool Keep(CombatItem item, ParsedLog log);
    }
}
