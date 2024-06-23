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
        internal abstract class VariableFormDecoration : VariableGenericAttachedDecoration
        {
            public bool Filled { get; private set; } = true;
            public int GrowingEnd { get; private set; }
            public bool GrowingReverse { get; private set; }
            protected VariableFormDecoration((long, long) lifespan, GeographicalConnector connector) : base(lifespan, connector)
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
        private new VariableFormDecoration VariableDecoration => (VariableFormDecoration)base.VariableDecoration;

        public bool Filled => VariableDecoration.Filled;
        public string Color => DecorationMetadata.Color;
        public int GrowingEnd => VariableDecoration.GrowingEnd;
        public bool GrowingReverse => VariableDecoration.GrowingReverse;

        internal FormDecoration(FormDecorationMetadata metadata, VariableFormDecoration variable) : base(metadata, variable)
        {
        }

        public virtual FormDecoration UsingFilled(bool filled)
        {
            VariableDecoration.UsingFilled(filled);
            return this;
        }

        public virtual FormDecoration UsingGrowingEnd(long growingEnd, bool reverse = false)
        {
            VariableDecoration.UsingGrowingEnd(growingEnd, reverse);
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
