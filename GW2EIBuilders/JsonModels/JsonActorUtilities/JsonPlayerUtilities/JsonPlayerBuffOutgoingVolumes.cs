using System.Collections.Generic;
using GW2EIEvtcParser.EIData;
using Newtonsoft.Json;
using static GW2EIJSON.JsonPlayerBuffOutgoingVolumes;

namespace GW2EIBuilders.JsonModels.JsonActorUtilities.JsonPlayerUtilities
{
    /// <summary>
    /// Class representing buffs generation by player actors
    /// </summary>
    internal static class JsonPlayerBuffOutgoingVolumesBuilder
    {
        public static JsonBuffOutgoingVolumesData BuildJsonBuffsOutgoingVolumesData(FinalActorBuffVolumes stats)
        {
            var jsonBuffsGenerationData = new JsonBuffOutgoingVolumesData
            {
                Outgoing = stats.Outgoing,
                OutgoingByExtension = stats.OutgoingByExtension
            };
            return jsonBuffsGenerationData;
        }

    }
}
