namespace GW2EIEvtcParser.EIData;

internal abstract class FormDecoration : AttachedDecoration
{
    public abstract class FormDecorationMetadata : AttachedDecorationMetadata
    {
        public readonly string Color;

        protected FormDecorationMetadata(string color)
        {
            Color = color;
        }
    }
    public abstract class FormDecorationRenderingData : AttachedDecorationRenderingData
    {
        public bool Filled { get; private set; } = true;
        public long GrowingEnd { get; private set; }
        public bool GrowingReverse { get; private set; }
        protected FormDecorationRenderingData((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
        {
        }

        public virtual void UsingFilled(bool filled)
        {
            Filled = filled;
        }

        public virtual void UsingGrowingEnd(long growingEnd, bool reverse = false)
        {
            GrowingEnd = growingEnd <= Lifespan.start ? Lifespan.start : growingEnd;
            GrowingReverse = reverse;
        }
    }
    private new FormDecorationMetadata DecorationMetadata => (FormDecorationMetadata)base.DecorationMetadata;
    private new FormDecorationRenderingData DecorationRenderingData => (FormDecorationRenderingData)base.DecorationRenderingData;

    public bool Filled => DecorationRenderingData.Filled;
    public string Color => DecorationMetadata.Color;
    public long GrowingEnd => DecorationRenderingData.GrowingEnd;
    public bool GrowingReverse => DecorationRenderingData.GrowingReverse;

    protected FormDecoration(FormDecorationMetadata metadata, FormDecorationRenderingData renderingData) : base(metadata, renderingData)
    {
    }

    public virtual FormDecoration UsingFilled(bool filled)
    {
        DecorationRenderingData.UsingFilled(filled);
        return this;
    }

    public virtual FormDecoration UsingGrowingEnd(long growingEnd, bool reverse = false)
    {
        DecorationRenderingData.UsingGrowingEnd(growingEnd, reverse);
        return this;
    }

    public abstract FormDecoration Copy(string? color = null);

    public abstract FormDecoration GetBorderDecoration(string? borderColor = null);
    public FormDecoration GetBorderDecoration(Color borderColor, double opacity)
    {
        return GetBorderDecoration(borderColor.WithAlpha(opacity).ToString(true));
    }

}
