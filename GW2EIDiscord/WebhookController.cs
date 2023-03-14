using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;

[assembly: System.CLSCompliant(false)]
namespace GW2EIDiscord
{
    public static class WebhookController
    {
        private static bool IsWebhookURLValid(string webhookURL)
        {
            return webhookURL != null && webhookURL.Length > 0;
        }

        public static void DeleteMessage(string webhookURL, ulong id, out string message)
        {
            if (IsWebhookURLValid(webhookURL))
            {
                try
                {
                    var client = new DiscordWebhookClient(webhookURL);
                    try
                    {
                        client.DeleteMessageAsync(id).Wait();
                    }
                    finally
                    {
                        client.Dispose();
                    }
                    message = "Deleted message";
                    return;
                }
                catch (Exception e)
                {
                    message = "Couldn't delete message: " + e.Message;
                    return;
                }
            }
            message = "Webhook url invalid";
        }

        public static ulong SendMessage(string webhookURL, Embed embed, out string message)
        {
            if (IsWebhookURLValid(webhookURL))
            {
                try
                {
                    var client = new DiscordWebhookClient(webhookURL);
                    ulong id;
                    try
                    {
                        id = client.SendMessageAsync(embeds: new[] { embed }).Result;
                    }
                    finally
                    {
                        client.Dispose();
                    }
                    message = "Sent Embed";
                    return id;
                }
                catch (Exception e)
                {
                    message = "Couldn't send embed: " + e.Message;
                    return 0;
                }
            }
            message = "Webhook url invalid";
            return 0;
        }

        public static ulong SendMessage(string webhookURL, string embed, out string message)
        {
            if (IsWebhookURLValid(webhookURL))
            {
                try
                {
                    var client = new DiscordWebhookClient(webhookURL);
                    ulong id;
                    try
                    {
                        id = client.SendMessageAsync(text: embed).Result;
                    }
                    finally
                    {
                        client.Dispose();
                    }
                    message = "Sent Embed";
                    return id;
                }
                catch (Exception e)
                {
                    message = "Couldn't send embed: " + e.Message;
                    return 0;
                }
            }
            message = "Webhook url invalid";
            return 0;
        }

    }

}
