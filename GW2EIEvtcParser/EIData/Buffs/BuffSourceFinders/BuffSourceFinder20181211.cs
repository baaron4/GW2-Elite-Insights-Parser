using System.Collections.Generic;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders
{
    internal class BuffSourceFinder20181211 : BuffSourceFinder
    {

        public BuffSourceFinder20181211(HashSet<long> boonIds) : base(boonIds)
        {
            ExtensionIDS = new HashSet<long>()
            {
                SignetOfInspirationSkill,
                TrueNatureDragon,
                SandSquall
            };
            DurationToIDs = new Dictionary<long, HashSet<long>>
            {
                {5000, new HashSet<long> { SignetOfInspirationSkill } }, // SoI
                {3000, new HashSet<long> { TrueNatureDragon } }, // Treated TN
                {2000, new HashSet<long> { TrueNatureDragon, SandSquall } }, // TN, SandSquall
            };
            EssenceOfSpeed = 2000;
            ImbuedMelodies = 2000;
        }

    }
}
