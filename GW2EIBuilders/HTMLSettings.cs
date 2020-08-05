using System;

namespace GW2EIBuilders
{
    public class HTMLSettings : BuilderSettings
    {

        public bool HTMLLightTheme { get; }

        public bool ExternalHTMLScripts { get; }

        public HTMLSettings(string parserName, Version version, bool htmlLightTheme, bool externalHTMLScripts) : base(parserName, version)
        {
            HTMLLightTheme = htmlLightTheme;
            ExternalHTMLScripts = externalHTMLScripts;
        }
    }
}
