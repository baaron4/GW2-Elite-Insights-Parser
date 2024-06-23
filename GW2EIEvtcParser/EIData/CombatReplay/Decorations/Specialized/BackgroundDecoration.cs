
namespace GW2EIEvtcParser.EIData
{
    internal abstract class BackgroundDecoration : GenericDecoration
    {
        internal abstract class BackgroundDecorationMetadata : GenericDecorationMetadata
        {
        }
        internal abstract class VariableBackgroundDecoration : VariableGenericDecoration
        {
            protected VariableBackgroundDecoration((long, long) lifespan) : base(lifespan)
            {
            }
        }
        internal BackgroundDecoration(BackgroundDecorationMetadata metadata, VariableBackgroundDecoration variable) : base(metadata, variable)
        {
        }

        public BackgroundDecoration() : base()
        {
        }
    }
}
