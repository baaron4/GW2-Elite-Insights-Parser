using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class CombatItemResultChecker : MechanicChecker
    {
        private readonly ParseEnum.PhysicalResult _value;

        public CombatItemResultChecker(ParseEnum.PhysicalResult value)
        {
            _value = value;
        }

        public override bool Keep(CombatItem item, ParsedLog log)
        {
            ParseEnum.PhysicalResult val = ParseEnum.GetPhysicalResult(item.Result);
            return val == _value;
        }
    }
}
