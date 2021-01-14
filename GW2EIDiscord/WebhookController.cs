using System;
using System.Collections.Generic;
using Discord;
using Discord.Webhook;

namespace GW2EIDiscord
{
    public class WebhookController
    {
        private string _webhookURL { get; }
        private Embed _embed { get; }
        private string _message { get; }

        private bool _webhookURLValid => _webhookURL != null && _webhookURL.Length > 0;

        public WebhookController(string webhookURL, Embed embed)
        {
            _webhookURL = webhookURL;
            _embed = embed;
        }
        public WebhookController(string webhookURL, string message)
        {
            _webhookURL = webhookURL;
            _message = message;
        }

        public string SendMessage()
        {
            if (_webhookURLValid)
            {
                try
                {
                    var client = new DiscordWebhookClient(_webhookURL);
                    try
                    {
                        if (_embed == null)
                        {
                            _ = client.SendMessageAsync(text: _message).Result;
                        }
                        else
                        {
                            _ = client.SendMessageAsync(embeds: new[] { _embed }).Result;
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
