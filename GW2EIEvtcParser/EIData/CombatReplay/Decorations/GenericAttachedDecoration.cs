using System;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class GenericAttachedDecoration : GenericDecoration
    {
        internal abstract class GenericAttachedDecorationMetadata : GenericDecorationMetadata
        {
        }
        internal abstract class GenericAttachedDecorationRenderingData : GenericDecorationRenderingData
        {
            public GeographicalConnector ConnectedTo { get; }
            public RotationConnector RotationConnectedTo { get; protected set; }
            public SkillModeDescriptor SkillMode { get; protected set; }
            protected GenericAttachedDecorationRenderingData((long, long) lifespan, GeographicalConnector connector) : base(lifespan)
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
        private new GenericAttachedDecorationRenderingData DecorationRenderingData => (GenericAttachedDecorationRenderingData)base.DecorationRenderingData;

        public GeographicalConnector ConnectedTo => DecorationRenderingData.ConnectedTo;
        public RotationConnector RotationConnectedTo => DecorationRenderingData.RotationConnectedTo;
        public SkillModeDescriptor SkillMode => DecorationRenderingData.SkillMode;

        internal GenericAttachedDecoration(GenericAttachedDecorationMetadata metadata, GenericAttachedDecorationRenderingData renderingData) : base(metadata, renderingData)
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

        public GenericAttachedDecoration UsingRotationConnector(RotationConnector rotationConnectedTo)
        {
            DecorationRenderingData.UsingRotationConnector(rotationConnectedTo);
            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="skill">Skill information</param>
        /// <returns></returns>
        public GenericAttachedDecoration UsingSkillMode(SkillModeDescriptor skill)
        {
            DecorationRenderingData.UsingSkillMode(skill);
            return this;
        }
    }
}
