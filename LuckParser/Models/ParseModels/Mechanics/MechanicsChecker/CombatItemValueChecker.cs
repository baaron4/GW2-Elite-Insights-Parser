using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class CombatItemValueChecker : MechanicChecker
    {
        private readonly ValueCompare _compare;
        private readonly long _value;

        public CombatItemValueChecker(long value, ValueCompare compare)
        {
            _value = value;
            _compare = compare;
        }

        public override bool Keep(CombatItem item, ParsedLog log)
        {
            long val = item.Value;
            switch(_compare)
            {
                case ValueCompare.EQ:
                    return val == _value;
                case ValueCompare.G:
                    return val < _value;
                case ValueCompare.GEQ:
                    return val <= _value;
                case ValueCompare.L:
                    return val > _value;
                case ValueCompare.LEQ:
                    return val >= _value;
                case ValueCompare.NEQ:
                    return val != _value;
            }
            return false;
        }
    }
}
