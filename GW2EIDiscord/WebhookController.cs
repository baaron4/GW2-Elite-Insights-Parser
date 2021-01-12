using System;
using System.Collections.Generic;
using Discord;
using Discord.Webhook;

namespace GW2EIDiscord
{
    public class WebhookController
    {
        public string WebhookURL { get; }
        public Embed Embed { get; }
        public string Message { get; }

        internal bool WebhookURLValid => WebhookURL != null && WebhookURL.Length > 0;

        public WebhookController(string webhookURL, Embed embed)
        {
            WebhookURL = webhookURL;
            Embed = embed;
        }
        public WebhookController(string webhookURL, string message)
        {
            WebhookURL = webhookURL;
            Message = message;
        }

        public string SendMessage()
        {
            if (WebhookURLValid)
            {
                try
                {
                    var client = new DiscordWebhookClient(WebhookURL);
                    try
                    {
                        if (Embed == null)
                        {
                            _ = client.SendMessageAsync(text: Message).Result;
                        }
                        else
                        {
                            _ = client.SendMessageAsync(embeds: new[] { Embed }).Result;
                        }
                    }
                    finally
                    {
                        client.Dispose();
                    }
                    return "Sent Embed";
                }
                catch (Exception e)
                {
                    return "Couldn't send embed: " + e.Message;
                }
            }
            return "Webhook url invalid";
        }

    }

}
