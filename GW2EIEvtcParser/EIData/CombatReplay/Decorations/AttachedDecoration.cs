namespace GW2EIEvtcParser.EIData;

internal abstract class AttachedDecoration : Decoration
{
    public abstract class AttachedDecorationMetadata : _DecorationMetadata
    {
    }
    public abstract class AttachedDecorationRenderingData : _DecorationRenderingData
    {
        public readonly GeographicalConnector ConnectedTo;
        public RotationConnector? RotationConnectedTo { get; protected set; }
        public SkillModeDescriptor? SkillMode { get; protected set; }
        protected AttachedDecorationRenderingData((long, long) lifespan, GeographicalConnector connector) : base(lifespan)
        {
            ConnectedTo = connector;
        }

        public virtual void UsingRotationConnector(RotationConnector? rotationConnectedTo)
        {
            RotationConnectedTo = rotationConnectedTo;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="skill">Skill information</param>
        /// <returns></returns>
        public virtual void UsingSkillMode(SkillModeDescriptor? skill)
        {
            SkillMode = skill;
        }
    }
    private new AttachedDecorationRenderingData DecorationRenderingData => (AttachedDecorationRenderingData)base.DecorationRenderingData;

    public GeographicalConnector ConnectedTo => DecorationRenderingData.ConnectedTo;
    public RotationConnector? RotationConnectedTo => DecorationRenderingData.RotationConnectedTo;
    public SkillModeDescriptor? SkillMode => DecorationRenderingData.SkillMode;

    protected AttachedDecoration(AttachedDecorationMetadata metadata, AttachedDecorationRenderingData renderingData) : base(metadata, renderingData)
    {
    }

    /// <summary>Creates a new line towards the other decoration</summary>
    public LineDecoration LineTo(AttachedDecoration other, string color)
    {
        long start = Math.Max(Lifespan.start, other.Lifespan.start);
        long end = Math.Min(Lifespan.end, other.Lifespan.end);
        return new LineDecoration((start, end), color, ConnectedTo, other.ConnectedTo);
    }

    public LineDecoration LineTo(AttachedDecoration other, Color color, double opacity)
    {
        return LineTo(other, color.WithAlpha(opacity).ToString(true));
    }

    public AttachedDecoration UsingRotationConnector(RotationConnector? rotationConnectedTo)
    {
        DecorationRenderingData.UsingRotationConnector(rotationConnectedTo);
        return this;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="skill">Skill information</param>
    /// <returns></returns>
    public AttachedDecoration UsingSkillMode(SkillModeDescriptor? skill)
    {
        DecorationRenderingData.UsingSkillMode(skill);
        return this;
    }
}
