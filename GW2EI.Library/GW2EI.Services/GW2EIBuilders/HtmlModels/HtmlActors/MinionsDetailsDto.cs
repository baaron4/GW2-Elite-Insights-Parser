namespace GW2EIBuilders.HtmlModels.HTMLActors;

internal class MinionsDetailsDto : ActorDetailsDto
{
    public List<MinionInstanceDetailsDto>? Instances { get; set; } = null;
}
