using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLStats;

internal class DeathRecapDto
{
    public long Time { get; set; }
    public List<object[]>? ToDown { get; set; } = null;
    public List<object[]>? ToKill { get; set; } = null;

    private static List<object[]> BuildDeathRecapItemList(List<DeathRecap.DeathRecapDamageItem> list)
    {
        var data = new List<object[]>(list.Count);
        foreach (DeathRecap.DeathRecapDamageItem item in list)
        {
            data.Add(new object[]
            {
                    item.Time,
                    item.ID,
                    item.Damage,
                    item.Src,
                    item.IndirectDamage
            });
        }
        return data;
    }

    public static List<DeathRecapDto>? BuildDeathRecap(ParsedEvtcLog log, AbstractSingleActor actor)
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
