using System;
using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    public class FinalBuffs
    {
        public double Uptime { get; internal set; }
        public double Presence { get; internal set; }

        protected FinalBuffs()
        {

        }

        internal FinalBuffs(Buff buff, BuffDistribution buffDistribution, Dictionary<long, long> buffPresence, long phaseDuration)
        {
            if (buff.Type == BuffType.Duration)
            {
                Uptime = Math.Round(100.0 * buffDistribution.GetUptime(buff.ID) / phaseDuration, ParserHelper.BuffDigit);
            }
            else if (buff.Type == BuffType.Intensity)
            {
                Uptime = Math.Round((double)buffDistribution.GetUptime(buff.ID) / phaseDuration, ParserHelper.BuffDigit);
                if (buffPresence.TryGetValue(buff.ID, out long presenceValueBoon))
                {
                    Presence = Math.Round(100.0 * presenceValueBoon / phaseDuration, ParserHelper.BuffDigit);
                }
            }
        }

    }

}
