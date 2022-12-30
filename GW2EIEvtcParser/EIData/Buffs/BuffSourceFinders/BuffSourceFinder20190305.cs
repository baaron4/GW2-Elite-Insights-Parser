using System.Collections.Generic;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders
{
    internal class BuffSourceFinder20190305 : BuffSourceFinder20181211
    {
        public BuffSourceFinder20190305(HashSet<long> boonIds) : base(boonIds)
        {
            DurationToIDs = new Dictionary<long, HashSet<long>>
            {
                {3000, new HashSet<long> { TrueNatureDragon , SignetOfInspirationSkill , SandSquall } }, // SoI, Treated TN, SandSquall
                {2000, new HashSet<long> { TrueNatureDragon } }, // TN
            };
        }
    }
}
