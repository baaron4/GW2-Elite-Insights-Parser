using System;
using System.Collections.Generic;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{
    public class FinalNPCBuffs
    {
        public double Uptime { get; set; }
        public double Presence { get; set; }

        public FinalNPCBuffs(Buff buff, BuffDistribution buffDistribution, Dictionary<long, long> buffPresence, long phaseDuration)
        {
            if (buff.Type == BuffType.Duration)
            {
                Uptime = Math.Round(100.0 * buffDistribution.GetUptime(buff.ID) / phaseDuration, GeneralHelper.BoonDigit);
            }
            else if (buff.Type == BuffType.Intensity)
            {
                Uptime = Math.Round((double)buffDistribution.GetUptime(buff.ID) / phaseDuration, GeneralHelper.BoonDigit);
                if (buffPresence.TryGetValue(buff.ID, out long presenceValueBoon))
                {
                    Presence = Math.Round(100.0 * presenceValueBoon / phaseDuration, GeneralHelper.BoonDigit);
                }
            }
        }

    }

}
