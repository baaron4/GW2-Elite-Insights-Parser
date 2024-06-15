
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
        internal BackgroundDecoration(ConstantBackgroundDecoration constant, VariableBackgroundDecoration variable) : base(constant, variable)
        {
        }

        public BackgroundDecoration() : base()
        {
        }
    }
}
