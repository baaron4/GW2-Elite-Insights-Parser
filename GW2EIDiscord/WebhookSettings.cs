using Discord;

namespace GW2EIDiscord
{
    public class WebhookSettings
    {
        public string WebhookURL { get; }
        public Embed Embed { get; }

        public WebhookSettings(string webhookURL, Embed embed)
        {
            WebhookURL = webhookURL;
            Embed = embed;
        }
        public WebhookSettings(string webhookURL) : this(webhookURL, null)
        {
        }
    }
}
