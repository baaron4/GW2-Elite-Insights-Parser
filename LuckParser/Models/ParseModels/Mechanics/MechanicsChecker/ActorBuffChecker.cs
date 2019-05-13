using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class ActorBuffChecker : MechanicChecker
    {
        private readonly long _boonId;
        private readonly bool _has;
        private readonly bool _dst;

        public ActorBuffChecker(long boonId, bool has, bool dst)
        {
            _boonId = boonId;
            _has = has;
            _dst = dst;
        }

        public override bool Keep(CombatItem item, ParsedLog log)
        {
            AbstractActor actor = log.FindActor(item.Time, _dst ? item.DstInstid : item.SrcInstid);
            if (actor.GetBoonGraphs(log).TryGetValue(_boonId, out BoonsGraphModel bgm))
            {
                bool has = bgm.IsPresent(log.FightData.ToFightSpace(item.Time), 10);
                return has == _has;
            } 
            else
            {
                if (_has)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
