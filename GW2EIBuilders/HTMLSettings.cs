namespace GW2EIBuilders;

public class HTMLSettings
{

    public bool HTMLLightTheme { get; init; } = false;

    public bool ExternalHTMLScripts { get; init; } = false;

    public readonly string? ExternalHtmlScriptsPath;

    public readonly string? ExternalHtmlScriptsCdn;
    public bool CompressJson { get; init; } = true;

    public HTMLSettings() 
    { 
    }

    public HTMLSettings(string externalHTMLScriptsPath, string externalHTMLScriptsCdn)
    {
        ExternalHtmlScriptsPath = externalHTMLScriptsPath;
        ExternalHtmlScriptsCdn = externalHTMLScriptsCdn;
    }
}
