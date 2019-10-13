namespace GW2EIParser.EIData
{
    public abstract class FormDecoration : GenericDecoration
    {

        public bool Filled { get; }
        public string Color { get; }
        public int Growing { get; }

        protected FormDecoration(bool fill, int growing, (int start, int end) lifespan, string color, Connector connector) : base(lifespan, connector)
        {
            Color = color;
            Filled = fill;
            Growing = growing;
        }
        //
        protected class FormSerializable : GenericDecorationSerializable
        {
            public bool Fill { get; set; }
            public int Growing { get; set; }
            public string Color { get; set; }
        }

    }
}
