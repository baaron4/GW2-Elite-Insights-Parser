using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders;

internal class BuffSourceFinder20190305 : BuffSourceFinder20181211
{
    public BuffSourceFinder20190305(HashSet<long> boonIDs) : base(boonIDs)
    {
        DurationToIDs = new Dictionary<long, HashSet<long>>
        {
            {3000, [TrueNatureDragon, SignetOfInspirationSkill, SandSquall] }, // SoI, Treated TN, SandSquall
            {2000, [TrueNatureDragon] }, // TN
        };
    }
}
