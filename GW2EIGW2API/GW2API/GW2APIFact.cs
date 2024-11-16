
using System.Text.Json.Serialization;

namespace GW2EIGW2API.GW2API;

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(GW2APITraitedFact))]
public class GW2APIFact
{
    public string Text;
    public string Icon;
    public string Type;
    public string Target;
    public object Value;
    public string Status;
    public string Description;
    public int ApplyCount;
    public float Duration;
    public string FieldType;
    public string FinisherType;
    public float Percent;
    public int HitCount;
    public float DmgMultiplier;
    public int Distance;
    public GW2APIFact Prefix;
}

