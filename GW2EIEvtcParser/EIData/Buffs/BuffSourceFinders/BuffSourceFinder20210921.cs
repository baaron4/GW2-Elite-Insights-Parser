using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    internal class BuffSourceFinder20210921 : BuffSourceFinder20210511
    {
        private List<AbstractCastEvent> _vindicatorDodges = null;
        public BuffSourceFinder20210921(HashSet<long> boonIds) : base(boonIds)
        {
        }

        protected override List<AgentItem> CouldBeImperialImpact(long extension, long time, ParsedEvtcLog log)
        {
            if (_vindicatorDodges == null)
            {
                _vindicatorDodges = new List<AbstractCastEvent>();
                foreach (Player p in log.PlayerList)
                {
                    if (p.Spec == ParserHelper.Spec.Vindicator)
                    {
                        _vindicatorDodges.AddRange(p.GetIntersectingCastEvents(log, 0, log.FightData.FightEnd).Where(x => x.SkillId == SkillItem.DodgeId));
                    }
                }
                _vindicatorDodges = new List<AbstractCastEvent>(_vindicatorDodges.OrderBy(x => x.Time));
            }
            if (extension > 2000)
            {
                return new List<AgentItem>();
            }
            var candidates = _vindicatorDodges.Where(x => x.Time <= time && time <= x.EndTime + ParserHelper.ServerDelayConstant).ToList();
            return candidates.Select(x => x.Caster).ToList();
        }

    }
}
