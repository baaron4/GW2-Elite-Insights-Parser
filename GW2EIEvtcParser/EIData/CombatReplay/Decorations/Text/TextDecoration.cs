using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

internal class TextDecoration : Decoration
{
    public class TextDecorationMetadata : _DecorationMetadata
    {
        public readonly string Color;

        public readonly string? BackgroundColor; // For the future, in case we want to have a backplate for the text

        public TextDecorationMetadata(string color, string? backgroundColor = null)
        {
            Color = color;
            BackgroundColor = backgroundColor;
        }

        public override DecorationMetadataDescription GetCombatReplayMetadataDescription()
        {
            return new TextDecorationMetadataDescription(this);
        }

        public override string GetSignature()
        {
            return "Txt" + Color + BackgroundColor ?? "";
        }
    }
    public class TextDecorationRenderingData : _DecorationRenderingData
    {
        public readonly string Text;
        public bool Bold { get; private set; } = false;
        public string? FontType { get; private set; }
        public readonly uint FontSize;

        public readonly Connector ConnectedTo;

        private TextDecorationRenderingData((long, long) lifespan, string text, uint fontSize) : base(lifespan)
        {
            Text = text;
            FontSize = fontSize;
        }

        public TextDecorationRenderingData((long, long) lifespan, string text, uint fontSize, GeographicalConnector connector) : this(lifespan, text, fontSize)
        {
            ConnectedTo = connector;
        }

        public TextDecorationRenderingData((long, long) lifespan, string text, uint fontSize, ScreenSpaceConnector connector) : this(lifespan, text, fontSize)
        {
            ConnectedTo = connector;
        }

        public void UsingFontType(string fontType)
        {
            FontType = fontType;
        }

        public void UsingBold(bool bold)
        {
            Bold = bold;
        }

        public override DecorationRenderingDescription GetCombatReplayRenderingDescription(CombatReplayMap map, ParsedEvtcLog log, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature)
        {
            return new TextDecorationRenderingDescription(log, this, map, usedSkills, usedBuffs, metadataSignature);
        }
    }
    private new TextDecorationMetadata DecorationMetadata => (TextDecorationMetadata)base.DecorationMetadata;
    private new TextDecorationRenderingData DecorationRenderingData => (TextDecorationRenderingData)base.DecorationRenderingData;

    public string Color => DecorationMetadata.Color;

    protected TextDecoration(TextDecorationMetadata metadata, TextDecorationRenderingData renderingData) : base(metadata, renderingData)
    {
    }

    public TextDecoration((long, long) lifespan, string text, uint fontSize, string color, GeographicalConnector connector) : base(new TextDecorationMetadata(color), new TextDecorationRenderingData(lifespan, text, fontSize, connector))
    {

    }


    public TextDecoration((long, long) lifespan, string text, uint fontSize, Color color, double opacity, GeographicalConnector connector) : this(lifespan, text, fontSize, color.WithAlpha(opacity).ToString(true), connector)
    {

    }

    public TextDecoration((long, long) lifespan, string text, uint fontSize, string color, ScreenSpaceConnector connector) : base(new TextDecorationMetadata(color), new TextDecorationRenderingData(lifespan, text, fontSize, connector))
    {

    }
    public TextDecoration((long, long) lifespan, string text, uint fontSize, Color color, double opacity, ScreenSpaceConnector connector) : this(lifespan, text, fontSize, color.WithAlpha(opacity).ToString(true), connector)
    {

    }

    public TextDecoration UsingFontType(string fontType)
    {
        DecorationRenderingData.UsingFontType(fontType);
        return this;
    }

    public TextDecoration UsingBold(bool bold)
    {
        DecorationRenderingData.UsingBold(bold);
        return this;
    }

}
