using System;

namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericDecorationRenderableDescription : AbstractRenderableDescription
    {

        protected GenericDecorationRenderableDescription(GenericDecoration decoration)
        {
            Start = decoration.Lifespan.start;
            End = decoration.Lifespan.end;
            if (End <= Start)
            {
                // such things should be filtered way before coming here
                throw new InvalidOperationException("Decorations can not have a negative or zero lifespan");
            }
        }
    }
}
