using GW2EIEvtcParser.EIData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonPlayerUtilities;

/// <summary>
/// Class representing consumables
/// </summary>
internal static class JsonConsumableBuilder
{
    public static JsonConsumable BuildJsonConsumable(Consumable food)
    {
        var jsonConsumable = new JsonConsumable
        {
            Stack = food.Stack,
            Duration = food.Duration,
            Time = food.Time,
            Id = food.Buff.ID
        };
        return jsonConsumable;
    }
}
