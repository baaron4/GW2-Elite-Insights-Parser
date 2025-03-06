using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLStats;

using DeathRecapItem = object[];
internal class DeathRecapDto
{
    public long Time { get; set; }
    public List<DeathRecapItem>? ToDown { get; set; } = null;
    public List<DeathRecapItem>? ToKill { get; set; } = null;

    private static List<DeathRecapItem> BuildDeathRecapItemList(List<DeathRecap.DeathRecapDamageItem> list)
    {
        var data = new List<DeathRecapItem>(list.Count);
        foreach (DeathRecap.DeathRecapDamageItem item in list)
        {
            data.Add(
            [
                    item.Time,
                    item.ID,
                    item.Damage,
                    item.SourceAgent,
                    item.IndirectDamage
            ]);
        }
        return data;
    }

    public static List<DeathRecapDto>? BuildDeathRecap(ParsedEvtcLog log, SingleActor actor)
    {
        IReadOnlyList<DeathRecap> recaps = actor.GetDeathRecaps(log);
        if (!recaps.Any())
        {
            return null;
        }

        var res = new List<DeathRecapDto>(recaps.Count);
        foreach (DeathRecap deathRecap in recaps)
        {
            var recap = new DeathRecapDto()
            {
                Time = deathRecap.DeathTime
            };
            res.Add(recap);
            if (deathRecap.ToKill != null)
            {
                recap.ToKill = BuildDeathRecapItemList(deathRecap.ToKill);
            }
            if (deathRecap.ToDown != null)
            {
                recap.ToDown = BuildDeathRecapItemList(deathRecap.ToDown);
            }

        }
        return res;
    }
}
