using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData.BuffSourceFinders;

internal class BuffSourceFinder20181211 : BuffSourceFinder
{

    public BuffSourceFinder20181211(HashSet<long> boonIDs) : base(boonIDs)
    {
        ExtensionIDS =
        [
            SignetOfInspirationSkill,
            TrueNatureDragon,
            SandSquall
        ];
        DurationToIDs = new Dictionary<long, HashSet<long>>
        {
            {5000, [SignetOfInspirationSkill] }, // SoI
            {3000, [TrueNatureDragon] }, // Treated TN
            {2000, [TrueNatureDragon, SandSquall] }, // TN, SandSquall
        };
        EssenceOfSpeed = 2000;
        ImbuedMelodies = 2000;
    }

}
