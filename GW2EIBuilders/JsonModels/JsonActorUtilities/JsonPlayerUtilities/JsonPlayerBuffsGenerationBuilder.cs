using GW2EIEvtcParser.EIData;
using static GW2EIJSON.JsonPlayerBuffsGeneration;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonPlayerUtilities
{
    /// <summary>
    /// Class representing buffs generation by player actors
    /// </summary>
    internal static class JsonPlayerBuffsGenerationBuilder
    {
        public static JsonBuffsGenerationData BuildJsonBuffsGenerationData(FinalActorBuffs stats)
        {
            var jsonBuffsGenerationData = new JsonBuffsGenerationData
            {
                Generation = stats.Generation,
                Overstack = stats.Overstack,
                Wasted = stats.Wasted,
                UnknownExtended = stats.UnknownExtended,
                Extended = stats.Extended,
                ByExtension = stats.ByExtension
            };
            return jsonBuffsGenerationData;
        }

    }
}
