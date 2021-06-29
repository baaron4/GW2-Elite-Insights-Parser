namespace GW2EIBuilders
{
    public class HTMLSettings
    {

        public bool HTMLLightTheme { get; }

        public bool ExternalHTMLScripts { get; }

        public string ExternalHtmlScriptsPath { get; }

        public string ExternalHtmlScriptsCdn { get; }
        public bool CompressJson { get; }

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

        public HTMLSettings(bool htmlLightTheme, bool externalHTMLScripts, string externalHTMLScriptsPath, string externalHTMLScriptsCdn, bool compressJson) : this(htmlLightTheme, externalHTMLScripts, externalHTMLScriptsPath, externalHTMLScriptsCdn)
        {
            CompressJson = compressJson;
        }
    }
}
