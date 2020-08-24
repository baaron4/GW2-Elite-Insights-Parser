using System;
using System.Collections.Generic;
using Discord.Webhook;

namespace GW2EIDiscord
{
    public static class WebhookController
    {
        public static void SendMessage(List<string> traces, string dpsReportPermalink, WebhookSettings settings)
        {
            if (settings.WebhookURL != null && settings.WebhookURL.Length > 0)
            {
                if (!dpsReportPermalink.Contains("https"))
                {
                    traces.Add("Nothing to send to Webhook");
                    return;
                }
                try
                {
                    var client = new DiscordWebhookClient(settings.WebhookURL);
                    try
                    {
                        if (settings.Embed == null)
                        {
                            _ = client.SendMessageAsync(text: dpsReportPermalink).Result;
                        } 
                        else
                        {
                            _ = client.SendMessageAsync(embeds: new[] { settings.Embed }).Result;
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
