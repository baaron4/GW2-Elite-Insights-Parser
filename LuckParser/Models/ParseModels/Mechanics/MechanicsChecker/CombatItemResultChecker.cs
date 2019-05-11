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
        private readonly ParseEnum.Result _value;

        public CombatItemResultChecker(ParseEnum.Result value)
        {
            _value = value;
        }

        public override bool Keep(CombatItem item, ParsedLog log)
        {
            ParseEnum.Result val = item.ResultEnum;
            return val == _value;
        }
    }
}
