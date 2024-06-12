namespace GW2EIEvtcParser.EIData
{
    internal abstract class BackgroundDecoration : GenericDecoration
    {
        internal abstract class ConstantBackgroundDecoration : ConstantGenericDecoration
        {
        }
        internal abstract class VariableBackgroundDecoration : VariableGenericDecoration
        {
            protected VariableBackgroundDecoration((long, long) lifespan) : base(lifespan)
            {
            }
        }

        public BackgroundDecoration() : base()
        {
        }
    }
}
