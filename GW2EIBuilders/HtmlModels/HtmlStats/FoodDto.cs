using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels.HTMLStats;

internal class FoodDto
{
    public double Time { get; set; }
    public double Duration { get; set; }
    public long Id { get; set; }
    public int Stack { get; set; }
    public bool Dimished { get; set; }

    private FoodDto(Consumable consume)
    {
        Time = consume.Time / 1000.0;
        Duration = consume.Duration / 1000.0;
        Stack = consume.Stack;
        Id = consume.Buff.ID;
        Dimished = (consume.Buff.ID == 46587 || consume.Buff.ID == 46668);
    }

    public static List<FoodDto> BuildFoodData(ParsedEvtcLog log, SingleActor actor, Dictionary<long, Buff> usedBuffs)
    {
        var consumables = actor.GetConsumablesList(log, log.LogData.LogStart, log.LogData.LogEnd);
        var list = new List<FoodDto>(consumables.Count);

        foreach (var consumable in consumables)
        {
            usedBuffs[consumable.Buff.ID] = consumable.Buff;
            list.Add(new FoodDto(consumable));
        }

        return list;
    }
}
