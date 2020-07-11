using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;
using Discord.Logging;
using Discord.Rest;
using System.Runtime.InteropServices.WindowsRuntime;

namespace GW2EIParser.Controllers
{
    public static class WebhookController
    {
        private static Embed GetEmbed(ParsedLog log, string[] uploadresult)
        {
            var builder = new EmbedBuilder();
            builder.WithThumbnailUrl(log.FightData.Logic.Icon);
            //
            builder.AddField("Encounter Duration", log.FightData.DurationString);
            //
            if (log.Statistics.PresentFractalInstabilities.Any())
            {
                builder.AddField("Instabilities", String.Join("\n",log.Statistics.PresentFractalInstabilities.Select(x => x.Name)));
            }
            //
            /*var playerByGroup = log.PlayerList.Where(x => !x.IsFakeActor).GroupBy(x => x.Group).ToDictionary(x => x.Key, x => x.ToList());
            var hasGroups = playerByGroup.Count > 1;
            foreach (KeyValuePair<int, List<Player>> pair in playerByGroup)
            {
                var groupField = new List<string>();
                foreach (Player p in pair.Value)
                {
                    groupField.Add(p.Character + " - " + p.Prof);
                }
                builder.AddField(hasGroups ? "Group " + pair.Key : "Party Composition", String.Join("\n", groupField));
            }*/
            //
            builder.AddField("Game Data", "ARC: " + log.LogData.BuildVersion + " | " + "GW2 Build: " + log.LogData.GW2Version);
            //
            builder.WithTitle(log.FightData.GetFightName(log));
            builder.WithTimestamp(DateTime.Now);
            builder.WithAuthor("Elite Insights " + Application.ProductVersion, "https://github.com/baaron4/GW2-Elite-Insights-Parser/blob/master/GW2EIParser/Content/LI.png?raw=true", "https://github.com/baaron4/GW2-Elite-Insights-Parser");
            builder.WithFooter(log.LogData.LogStartStd + " / " + log.LogData.LogEndStd);
            builder.WithColor(log.FightData.Success ? Color.Green : Color.Red);
            if (uploadresult[0].Length > 0)
            {
                builder.WithUrl(uploadresult[0]);
            }
            return builder.Build();
        }

        public static void SendMessage(ParsedLog log, string[] uploadresult)
        {
            if (Properties.Settings.Default.WebhookURL != null && Properties.Settings.Default.WebhookURL.Length > 0)
            {
                try
                {
                    var client = new DiscordWebhookClient(Properties.Settings.Default.WebhookURL);
                    try
                    {
                        _ = client.SendMessageAsync(embeds: new[] { GetEmbed(log, uploadresult) }).Result;
                        log.UpdateProgressWithCancellationCheck("Sent Embed");
                    }
                    finally
                    {
                        client.Dispose();
                    }
                }
                catch (Exception e)
                {
                    log.UpdateProgressWithCancellationCheck("Couldn't send embed: " + e.GetFinalException().Message);
                }
            }
            
        }

    }

}
