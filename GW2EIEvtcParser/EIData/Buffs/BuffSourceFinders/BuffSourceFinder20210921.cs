using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders
{
    internal class BuffSourceFinder20210921 : BuffSourceFinder20210511
    {
        private List<AbstractCastEvent> _vindicatorDodges = null;
        public BuffSourceFinder20210921(HashSet<long> boonIds) : base(boonIds)
        {
            ImperialImpactExtension = 2000;
        }

        protected override List<AgentItem> CouldBeImperialImpact(long buffID, long time, long extension, ParsedEvtcLog log)
        {
            if (extension > ImperialImpactExtension + ParserHelper.BuffSimulatorStackActiveDelayConstant)
            {
                return new List<AgentItem>();
            }
            if (_vindicatorDodges == null)
            {
                _vindicatorDodges = new List<AbstractCastEvent>();
                foreach (Player p in log.PlayerList)
                {
                    if (p.Spec == ParserHelper.Spec.Vindicator)
                    {
                        _vindicatorDodges.AddRange(p.GetIntersectingCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.SkillId == SkillIDs.ImperialImpactDodge));
                    }
                }
                _vindicatorDodges = new List<AbstractCastEvent>(_vindicatorDodges.OrderBy(x => x.Time));
            }
            BuffInfoEvent buffDescription = log.CombatData.GetBuffInfoEvent(buffID);
            if (buffDescription != null && buffDescription.DurationCap == 0)
            {
                if (Math.Abs(extension - ImperialImpactExtension) > ParserHelper.BuffSimulatorStackActiveDelayConstant)
                {
                    return new List<AgentItem>();
                }
            }
            var candidates = _vindicatorDodges.Where(x => x.Time <= time && time <= x.EndTime + ParserHelper.ServerDelayConstant).ToList();
            return candidates.Select(x => x.Caster).ToList();
        }

    }
}
