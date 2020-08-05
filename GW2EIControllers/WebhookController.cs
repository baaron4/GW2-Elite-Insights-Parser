using System;
using Discord;
using Discord.Webhook;

namespace GW2EIControllers
{
    public static class WebhookController
    {
        public static void SendMessage(OperationTracer operation, Embed embed, string[] uploadresult, WebhookSettings settings)
        {
            if (settings.WebhookURL != null && settings.WebhookURL.Length > 0)
            {
                if (!uploadresult[0].Contains("https"))
                {
                    operation.UpdateProgressWithCancellationCheck("Nothing to send to Webhook");
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
                        operation.UpdateProgressWithCancellationCheck("Sent Embed");
                    }
                    finally
                    {
                        client.Dispose();
                    }
                }
                catch (Exception e)
                {
                    operation.UpdateProgressWithCancellationCheck("Couldn't send embed: " + e.Message);
                }
            }
            
        }

    }

}
