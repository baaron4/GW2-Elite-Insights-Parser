using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIBuilders.HtmlModels.LogDataDto;

namespace GW2EIBuilders.HtmlModels.HTMLActors;

internal class MinionInstanceDto
{
    public long UniqueID { get; set; }
    public MinionInstanceDetailsDto Details;

    public long FirstAware;
    public long LastAware;

    public MinionInstanceDto(NPC minion, ParsedEvtcLog log, UsedHtmlData used)
    {
        UniqueID = minion.UniqueID;
        Details = new(log, minion, used);
        FirstAware = minion.FirstAware;
        LastAware = minion.LastAware;
    }
}
