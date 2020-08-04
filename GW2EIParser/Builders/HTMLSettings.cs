using System;
using System.Configuration;
using System.IO;

namespace GW2EIParser.Builders
{
    public class HTMLSettings
    {

        public bool HTMLLightTheme { get; }

        public bool ExternalHTMLScripts { get; }

        public HTMLSettings(bool htmlLightTheme, bool externalHTMLScripts)
        {
            HTMLLightTheme = htmlLightTheme;
            ExternalHTMLScripts = externalHTMLScripts;
        }
    }
}
