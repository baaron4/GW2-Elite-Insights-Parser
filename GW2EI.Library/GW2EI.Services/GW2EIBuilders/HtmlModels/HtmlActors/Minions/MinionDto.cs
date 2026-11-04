using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIBuilders.HtmlModels.LogDataDto;

namespace GW2EIBuilders.HtmlModels.HTMLActors;

internal class MinionDto
{
    public string? Name { get; set; }
    public string? Icon { get; set; }
    public MinionDetailsDto? Details;

    public List<MinionInstanceDto>? Instances;

    private static MinionDto BuildBasicMinionDto(Minions minions)
    {
        return new MinionDto()
        {
            Name = minions.Character,
            Icon = minions.GetIcon(),
        };
    }

    public static MinionDto BuildPlayerMinionDto(Minions minionGroup, ParsedEvtcLog log, UsedHtmlData used)
    {
        var dto = BuildBasicMinionDto(minionGroup);
        dto.Details = MinionDetailsDto.BuildFriendlyMinionsData(log, minionGroup, used);
        if (minionGroup.IsWhiteListedForSomeStats)
        {
            dto.Instances = new(minionGroup.MinionList.Count);
            foreach (var minion in minionGroup.MinionList)
            {
                dto.Instances.Add(new(minion, log, used));
            }
        }
        return dto;
    }



    public static MinionDto BuildTargetMinionDto(Minions minions, ParsedEvtcLog log, UsedHtmlData used)
    {
        var dto = BuildBasicMinionDto(minions);
        return dto;
    }
}
