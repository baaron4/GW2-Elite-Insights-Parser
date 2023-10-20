namespace GW2EIEvtcParser.EIData
{
    internal abstract class FormDecoration : GenericAttachedDecoration
    {

        public bool Filled { get; }
        public string Color { get; }
        public int GrowingEnd { get; }

        protected FormDecoration(bool fill, int growing, (int start, int end) lifespan, string color, Connector connector) : base(lifespan, connector)
        {
            Color = color;
            Filled = fill;
            GrowingEnd = growing;
        }

    }
}
