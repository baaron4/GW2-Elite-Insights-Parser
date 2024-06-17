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
            var jsonBuffsGenerationData = new JsonBuffsGenerationData();
            jsonBuffsGenerationData.Generation = stats.Generation;
            jsonBuffsGenerationData.Overstack = stats.Overstack;
            jsonBuffsGenerationData.Wasted = stats.Wasted;
            jsonBuffsGenerationData.UnknownExtended = stats.UnknownExtended;
            jsonBuffsGenerationData.Extended = stats.Extended;
            jsonBuffsGenerationData.ByExtension = stats.ByExtension;
            return jsonBuffsGenerationData;
        }

    }
}
