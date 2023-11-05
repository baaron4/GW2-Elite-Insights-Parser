using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders
{
    internal class BuffSourceFinder20210511 : BuffSourceFinder20191001
    {
        public BuffSourceFinder20210511(HashSet<long> boonIds) : base(boonIds)
        {
        }

        // Spec specific checks
        protected override int CouldBeEssenceOfSpeed(AgentItem dst, long buffID, long time, long extension, ParsedEvtcLog log)
        {
            BuffInfoEvent buffDescription = log.CombatData.GetBuffInfoEvent(buffID);
            if (buffDescription != null && buffDescription.DurationCap == 0)
            {
                return base.CouldBeEssenceOfSpeed(dst, buffID, time, extension, log);
            }
            if (dst.Spec == ParserHelper.Spec.Soulbeast && extension <= EssenceOfSpeed + ParserHelper.BuffSimulatorStackActiveDelayConstant)
            {
                if (GetIDs(log, buffID, extension).Any())
                {
                    // uncertain, needs to check more
                    return 0;
                }
                if (extension <= ImbuedMelodies + ParserHelper.BuffSimulatorStackActiveDelayConstant && log.FriendliesListBySpec.ContainsKey(ParserHelper.Spec.Tempest))
                {
                    // uncertain, needs to check more
                    return 0;
                }
                if (extension <= ImperialImpactExtension + ParserHelper.BuffSimulatorStackActiveDelayConstant && log.FriendliesListBySpec.ContainsKey(ParserHelper.Spec.Vindicator))
                {
                    // uncertain, needs to check more
                    return 0;
                }
                return 1;
            }
            return -1;
        }

        protected override HashSet<long> GetIDs(ParsedEvtcLog log, long buffID, long extension)
        {
            BuffInfoEvent buffDescription = log.CombatData.GetBuffInfoEvent(buffID);
            if (buffDescription != null && buffDescription.DurationCap == 0)
            {
                return base.GetIDs(log, buffID, extension);
            }
            var res = new HashSet<long>();
            foreach (KeyValuePair<long, HashSet<long>> pair in DurationToIDs)
            {
                if (extension <= pair.Key + ParserHelper.BuffSimulatorStackActiveDelayConstant)
                {
                    res.UnionWith(pair.Value);
                }
            }
            return res;
        }
    }
}
