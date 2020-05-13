using System;
using System.Collections.Generic;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{
    public class FinalBuffs
    {
        public double Uptime { get; set; }
        public double Presence { get; set; }

        protected FinalBuffs()
        {

        }

        public FinalBuffs(Buff buff, BuffDistribution buffDistribution, Dictionary<long, long> buffPresence, long phaseDuration)
        {
            if (buff.Stack == BuffStack.Duration)
            {
                Uptime = Math.Round(100.0 * buffDistribution.GetUptime(buff.ID) / phaseDuration, GeneralHelper.BuffDigit);
            }
            else if (buff.Stack == BuffStack.Intensity)
            {
                Uptime = Math.Round((double)buffDistribution.GetUptime(buff.ID) / phaseDuration, GeneralHelper.BuffDigit);
                if (buffPresence.TryGetValue(buff.ID, out long presenceValueBoon))
                {
                    Presence = Math.Round(100.0 * presenceValueBoon / phaseDuration, GeneralHelper.BuffDigit);
                }
            }
        }

    }

}
