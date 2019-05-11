using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class ActorStateChecker : MechanicChecker
    {
        private readonly ParseEnum.StateChange _compare;
        private readonly bool _has;

        public ActorStateChecker(ParseEnum.StateChange compare, bool has)
        {
            _compare = compare;
            _has = has;
        }

        public override bool Keep(CombatItem item, ParsedLog log)
        {
            if (_has)
            {
                return log.CombatData.GetStatesData(item.SrcInstid, _compare, item.Time - 20, item.Time + 20).Count > 0;
            }
            else
            {
                return log.CombatData.GetStatesData(item.SrcInstid, _compare, item.Time - 20, item.Time + 20).Count == 0;
            }
        }
    }
}
