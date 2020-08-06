using System;
using System.Collections.Generic;
using Discord;
using Discord.Webhook;

namespace GW2EIDiscord
{
    public static class WebhookController
    {
        public static void SendMessage(List<string> traces, Embed embed, string[] uploadresult, WebhookSettings settings)
        {
            if (settings.WebhookURL != null && settings.WebhookURL.Length > 0)
            {
                if (!uploadresult[0].Contains("https"))
                {
                    traces.Add("Nothing to send to Webhook");
                    return;
                }
                try
                {
                    var client = new DiscordWebhookClient(settings.WebhookURL);
                    try
                    {
                        if (settings.SendSimpleWebhookMessage)
                        {
                            _ = client.SendMessageAsync(text: uploadresult[0]).Result;
                        } 
                        else
                        {
                            _ = client.SendMessageAsync(embeds: new[] { embed }).Result;
                        }
                        traces.Add("Sent Embed");
                    }
                    finally
                    {
                        client.Dispose();
                    }
                }
                catch (Exception e)
                {
                    traces.Add("Couldn't send embed: " + e.Message);
                }
            }
            
        }

    }

}
