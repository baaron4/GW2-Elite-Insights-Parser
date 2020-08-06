namespace GW2EIDiscord
{
    public class WebhookSettings
    {
        public bool SendEmbedToWebhook { get; }
        public string WebhookURL { get; }
        public bool SendSimpleWebhookMessage { get; }

        public WebhookSettings(bool sendEmbedToWebhook, string webhookURL, bool simpleWebhookMessage)
        {
            SendEmbedToWebhook = sendEmbedToWebhook;
            WebhookURL = webhookURL;
            SendSimpleWebhookMessage = simpleWebhookMessage;
        }
        public WebhookSettings(bool sendEmbedToWebhook, string webhookURL) : this(sendEmbedToWebhook, webhookURL, false)
        {
        }
    }
}
