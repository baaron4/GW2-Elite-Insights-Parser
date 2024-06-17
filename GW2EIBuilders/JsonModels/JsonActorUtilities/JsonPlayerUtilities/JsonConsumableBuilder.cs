using GW2EIEvtcParser.EIData;
using GW2EIJSON;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonPlayerUtilities
{
    /// <summary>
    /// Class representing consumables
    /// </summary>
    internal static class JsonConsumableBuilder
    {
        public static JsonConsumable BuildJsonConsumable(Consumable food)
        {
            var jsonConsumable = new JsonConsumable();
            jsonConsumable.Stack = food.Stack;
            jsonConsumable.Duration = food.Duration;
            jsonConsumable.Time = food.Time;
            jsonConsumable.Id = food.Buff.ID;
            return jsonConsumable;
        }
    }
}
