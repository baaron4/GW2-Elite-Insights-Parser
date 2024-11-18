namespace GW2EIBuilders;

public class HTMLSettings
{

    public readonly bool HTMLLightTheme;

    public readonly bool ExternalHTMLScripts;

    public readonly string? ExternalHtmlScriptsPath;

    public readonly string? ExternalHtmlScriptsCdn;
    public readonly bool CompressJson;

    public HTMLSettings(bool htmlLightTheme, bool externalHTMLScripts)
    {
        HTMLLightTheme = htmlLightTheme;
        ExternalHTMLScripts = externalHTMLScripts;
    }

    public HTMLSettings(bool htmlLightTheme, bool externalHTMLScripts, string externalHTMLScriptsPath, string externalHTMLScriptsCdn) : this(htmlLightTheme, externalHTMLScripts)
    {
        ExternalHtmlScriptsPath = externalHTMLScriptsPath;
        ExternalHtmlScriptsCdn = externalHTMLScriptsCdn;
    }


    public HTMLSettings(bool htmlLightTheme, bool externalHTMLScripts, bool compressJson) : this(htmlLightTheme, externalHTMLScripts)
    {
        CompressJson = compressJson;
    }

    public HTMLSettings(bool htmlLightTheme, bool externalHTMLScripts, string externalHTMLScriptsPath, string externalHTMLScriptsCdn, bool compressJson) : this(htmlLightTheme, externalHTMLScripts, externalHTMLScriptsPath, externalHTMLScriptsCdn)
    {
        CompressJson = compressJson;
    }
}
