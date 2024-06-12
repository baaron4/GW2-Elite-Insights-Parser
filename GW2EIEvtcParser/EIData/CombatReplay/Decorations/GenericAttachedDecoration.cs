using System;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class GenericAttachedDecoration : GenericDecoration
    {
        internal abstract class ConstantGenericAttachedDecoration : ConstantGenericDecoration
        {
        }
        internal abstract class VariableGenericAttachedDecoration : VariableGenericDecoration
        {
            public GeographicalConnector ConnectedTo { get; }
            public RotationConnector RotationConnectedTo { get; protected set; }
            public SkillModeDescriptor SkillMode;
            protected VariableGenericAttachedDecoration((long, long) lifespan, GeographicalConnector connector) : base(lifespan)
            {
                ConnectedTo = connector;
            }

            public virtual void UsingRotationConnector(RotationConnector rotationConnectedTo)
            {
                RotationConnectedTo = rotationConnectedTo;
            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="skill">Skill information</param>
            /// <returns></returns>
            public virtual void UsingSkillMode(SkillModeDescriptor skill)
            {
                SkillMode = skill;
            }
        }
        private new VariableGenericAttachedDecoration VariableDecoration => (VariableGenericAttachedDecoration)base.VariableDecoration;

        public GeographicalConnector ConnectedTo => VariableDecoration.ConnectedTo;
        public RotationConnector RotationConnectedTo => VariableDecoration.RotationConnectedTo;
        public SkillModeDescriptor SkillMode => VariableDecoration.SkillMode;

        protected GenericAttachedDecoration() : base()
        {
        }

        /// <summary>Creates a new line towards the other decoration</summary>
        public LineDecoration LineTo(GenericAttachedDecoration other, string color)
        {
            int start = Math.Max(Lifespan.start, other.Lifespan.start);
            int end = Math.Min(Lifespan.end, other.Lifespan.end);
            return new LineDecoration((start, end), color, ConnectedTo, other.ConnectedTo);
        }

        public LineDecoration LineTo(GenericAttachedDecoration other, Color color, double opacity)
        {
            return LineTo(other, color.WithAlpha(opacity).ToString(true));
        }

        public virtual GenericAttachedDecoration UsingRotationConnector(RotationConnector rotationConnectedTo)
        {
            VariableDecoration.UsingRotationConnector(rotationConnectedTo);
            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="skill">Skill information</param>
        /// <returns></returns>
        public virtual GenericAttachedDecoration UsingSkillMode(SkillModeDescriptor skill)
        {
            VariableDecoration.UsingSkillMode(skill);
            return this;
        }
    }
}
