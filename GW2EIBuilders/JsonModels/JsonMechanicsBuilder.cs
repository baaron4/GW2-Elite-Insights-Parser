using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIJSON;
using static GW2EIJSON.JsonMechanics;

namespace GW2EIBuilders.JsonModels;

/// <summary>
/// Class corresponding to mechanics
/// </summary>
internal static class JsonMechanicsBuilder
{

    public static JsonMechanic BuildJsonMechanic(MechanicEvent ml)
    {
        var jsMech = new JsonMechanic
        {
            Time = ml.Time,
            Actor = ml.Actor.Character,
            Instid = ml.Actor.InstID,
            Id = ml.Actor.ID,
            Weight = ml.GetWeight(),
        };
        return jsMech;
    }

    public static JsonMechanics BuildJsonMechanics(Mechanic mech, List<JsonMechanic> data)
    {
        var jsMechs = new JsonMechanics
        {
            Name = mech.ShortName,
            FullName = mech.FullName,
            Description = mech.Description,
            InternalCooldown = mech.InternalCooldown > 0 ? mech.InternalCooldown : null,
            MechanicsData = data
        };
        switch (mech.Severity & Mechanic.MechanicSeverity.SeverityMask)
        {
            case Mechanic.MechanicSeverity.Sev0:
                jsMechs.Severity = "Sev0";
                break;
            case Mechanic.MechanicSeverity.Sev1:
                jsMechs.Severity = "Sev1";
                break;
            case Mechanic.MechanicSeverity.Sev2:
                jsMechs.Severity = "Sev2";
                break;
            case Mechanic.MechanicSeverity.Sev3:
                jsMechs.Severity = "Sev3";
                break;
            case Mechanic.MechanicSeverity.Sev4:
                jsMechs.Severity = "Sev4";
                break;
        }
        return jsMechs;
    }

    internal static List<JsonMechanics> GetJsonMechanicsList(ParsedEvtcLog log, MechanicData mechanicData, IReadOnlyCollection<Mechanic> presentMechanics)
    {
        var dict = new Dictionary<Mechanic, List<JsonMechanic>>(presentMechanics.Count);
        foreach (Mechanic mech in presentMechanics)
        {
            var jsonMechanics = new List<JsonMechanic>();
            foreach (MechanicEvent ml in mechanicData.GetMechanicLogs(log, mech, log.LogData.LogStart, log.LogData.LogEnd))
            {
                jsonMechanics.Add(BuildJsonMechanic(ml));
            }
            dict[mech] = jsonMechanics;
        }
        var mechanics = new List<JsonMechanics>(dict.Count);
        foreach (KeyValuePair<Mechanic, List<JsonMechanic>> pair in dict)
        {
            mechanics.Add(BuildJsonMechanics(pair.Key, pair.Value));
        }
        return mechanics;
    }

}
