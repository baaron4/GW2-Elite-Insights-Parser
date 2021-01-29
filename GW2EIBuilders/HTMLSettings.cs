namespace GW2EIBuilders
{
    public class HTMLSettings
    {

        public bool HTMLLightTheme { get; }

        public bool ExternalHTMLScripts { get; }

        public string ExternalHtmlScriptsPath { get; set; }

        public string ExternalHtmlScriptsCdn { get; set; }

        public HTMLSettings(bool htmlLightTheme, bool externalHTMLScripts, string externalHTMLScriptsPath, string externalHTMLScriptsCdn)
        {
            HTMLLightTheme = htmlLightTheme;
            ExternalHTMLScripts = externalHTMLScripts;
            ExternalHtmlScriptsPath = externalHTMLScriptsPath;
            ExternalHtmlScriptsCdn = externalHTMLScriptsCdn;
        }
    }
}
