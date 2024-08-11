namespace GW2EIEvtcParser.EIData
{
    internal abstract class FormDecoration : GenericAttachedDecoration
    {
        internal abstract class FormDecorationMetadata : GenericAttachedDecorationMetadata
        {
            public string Color { get; }

            protected FormDecorationMetadata(string color)
            {
                Color = color;
            }
        }
        internal abstract class FormDecorationRenderingData : GenericAttachedDecorationRenderingData
        {
            public bool Filled { get; private set; } = true;
            public int GrowingEnd { get; private set; }
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
                GrowingEnd = growingEnd <= Lifespan.start ? Lifespan.start : (int)growingEnd;
                GrowingReverse = reverse;
            }
        }
        private new FormDecorationMetadata DecorationMetadata => (FormDecorationMetadata)base.DecorationMetadata;
        private new FormDecorationRenderingData DecorationRenderingData => (FormDecorationRenderingData)base.DecorationRenderingData;

        public bool Filled => DecorationRenderingData.Filled;
        public string Color => DecorationMetadata.Color;
        public int GrowingEnd => DecorationRenderingData.GrowingEnd;
        public bool GrowingReverse => DecorationRenderingData.GrowingReverse;

        internal FormDecoration(FormDecorationMetadata metadata, FormDecorationRenderingData renderingData) : base(metadata, renderingData)
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

        public abstract FormDecoration Copy(string color = null);

        public abstract FormDecoration GetBorderDecoration(string borderColor = null);
        public FormDecoration GetBorderDecoration(Color borderColor, double opacity)
        {
            return GetBorderDecoration(borderColor.WithAlpha(opacity).ToString(true));
        }

    }
}
