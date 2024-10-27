using System.Collections.Generic;
using System.Text.Json.Serialization;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
[JsonDerivedType(typeof(MovingPlatformDecoration))]
[JsonDerivedType(typeof(IconDecoration))]
[JsonDerivedType(typeof(IconOverheadDecoration))]
[JsonDerivedType(typeof(LineDecoration))]
[JsonDerivedType(typeof(PieDecoration))]
[JsonDerivedType(typeof(CircleDecoration))]
[JsonDerivedType(typeof(RectangleDecoration))]
[JsonDerivedType(typeof(DoughnutDecoration))]
public abstract class GenericDecoration
{
    [JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
    [JsonDerivedType(typeof(MovingPlatformDecoration.MovingPlatformDecorationMetadata))]
    [JsonDerivedType(typeof(IconDecoration.IconDecorationMetadata))]
    [JsonDerivedType(typeof(IconOverheadDecoration.IconOverheadDecorationMetadata))]
    [JsonDerivedType(typeof(LineDecoration.LineDecorationMetadata))]
    [JsonDerivedType(typeof(PieDecoration.PieDecorationMetadata))]
    [JsonDerivedType(typeof(CircleDecoration.CircleDecorationMetadata))]
    [JsonDerivedType(typeof(RectangleDecoration.RectangleDecorationMetadata))]
    [JsonDerivedType(typeof(DoughnutDecoration.DoughnutDecorationMetadata))]
    internal abstract class GenericDecorationMetadata
    {

        public abstract string GetSignature();

        public abstract GenericDecorationMetadataDescription GetCombatReplayMetadataDescription();

    }

    [JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization)]
    [JsonDerivedType(typeof(MovingPlatformDecoration.MovingPlatformDecorationRenderingData))]
    [JsonDerivedType(typeof(IconDecoration.IconDecorationRenderingData))]
    [JsonDerivedType(typeof(IconOverheadDecoration.IconOverheadDecorationRenderingData))]
    [JsonDerivedType(typeof(LineDecoration.LineDecorationRenderingData))]
    [JsonDerivedType(typeof(PieDecoration.PieDecorationRenderingData))]
    [JsonDerivedType(typeof(CircleDecoration.CircleDecorationRenderingData))]
    [JsonDerivedType(typeof(RectangleDecoration.RectangleDecorationRenderingData))]
    [JsonDerivedType(typeof(DoughnutDecoration.DoughnutDecorationRenderingData))]
    internal abstract class GenericDecorationRenderingData
    {
        public (int start, int end) Lifespan { get; }

        protected GenericDecorationRenderingData((long start, long end) lifespan)
        {
            Lifespan = ((int)lifespan.start, (int)lifespan.end);
        }
        public abstract GenericDecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature);
    }

    internal GenericDecorationMetadata DecorationMetadata { get; }
    internal GenericDecorationRenderingData DecorationRenderingData { get; }

    public (int start, int end) Lifespan => DecorationRenderingData.Lifespan;
    internal GenericDecoration(GenericDecorationMetadata metaData, GenericDecorationRenderingData renderingData)
    {
        DecorationMetadata = metaData;
        DecorationRenderingData = renderingData;
    }
    protected GenericDecoration()
    {
    }
    //

}
