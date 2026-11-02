using GW2EIBuilders.HtmlModels.HTMLMetaData;
using GW2EIBuilders.HtmlModels.HTMLStats;

namespace GW2EIBuilders.HtmlModels.HTMLActors;

internal class MinionsDetailsDto
{
    public bool IsUniquePerTimeFrame { get; set; }

    public List<DamageDistributionDto>? DmgDistributions;
    public List<List<DamageDistributionDto>>? DmgDistributionsTargets;
    public List<DamageDistributionDto>? DmgDistributionsTaken;
    public List<List<SkillCastDto>>? Rotation;
    public List<MinionInstanceDetailsDto> Instances { get; set; } = [];
}
