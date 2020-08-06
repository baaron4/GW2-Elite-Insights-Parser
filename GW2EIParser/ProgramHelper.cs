using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIBuilders;
using GW2EIParser.Exceptions;
using Discord;
using System.Linq;
using GW2EIDPSReport;
using GW2EIDiscord;
using System.Windows.Forms;

namespace GW2EIParser
{
    internal static class ProgramHelper
    {
        internal static HTMLAssets htmlAssets { get; set; }

        internal static Version ParserVersion { get; } = new Version(Application.ProductVersion);
        internal static string ParserName { get; } = "Elite Insights";

        private static Embed GetEmbed(ParsedEvtcLog log, string[] uploadresult)
        {
            var builder = new EmbedBuilder();
            builder.WithThumbnailUrl(log.FightData.Logic.Icon);
            //
            builder.AddField("Encounter Duration", log.FightData.DurationString);
            //
            if (log.Statistics.PresentFractalInstabilities.Any())
            {
                builder.AddField("Instabilities", String.Join("\n", log.Statistics.PresentFractalInstabilities.Select(x => x.Name)));
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
            builder.AddField("Game Data", "ARC: " + log.LogData.ArcVersion + " | " + "GW2 Build: " + log.LogData.GW2Build);
            //
            builder.WithTitle(log.FightData.GetFightName(log));
            builder.WithTimestamp(DateTime.Now);
            builder.WithAuthor(log.ParserName + " " + log.ParserVersion.ToString(4), "https://github.com/baaron4/GW2-Elite-Insights-Parser/blob/master/GW2EIParser/Content/LI.png?raw=true", "https://github.com/baaron4/GW2-Elite-Insights-Parser");
            builder.WithFooter(log.LogData.LogStartStd + " / " + log.LogData.LogEndStd);
            builder.WithColor(log.FightData.Success ? Color.Green : Color.Red);
            if (uploadresult[0].Length > 0)
            {
                builder.WithUrl(uploadresult[0]);
            }
            return builder.Build();
        }

        internal static Exception GetFinalException(this Exception ex)
        {
            Exception final = ex;
            while (final.InnerException != null)
            {
                final = final.InnerException;
            }
            return final;
        }

        private static bool HasFormat()
        {
            return Properties.Settings.Default.SaveOutCSV || Properties.Settings.Default.SaveOutHTML || Properties.Settings.Default.SaveOutXML || Properties.Settings.Default.SaveOutJSON;
        }

        public static void DoWork(OperationController operation)
        {
            System.Globalization.CultureInfo before = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture =
                    new System.Globalization.CultureInfo("en-US");
            try
            {
                var fInfo = new FileInfo(operation.Location);

                var parser = new EvtcParser(new EvtcParserSettings(Properties.Settings.Default.Anonymous, Properties.Settings.Default.SkipFailedTries, Properties.Settings.Default.ParsePhases, Properties.Settings.Default.ParseCombatReplay, Properties.Settings.Default.ComputeDamageModifiers));

                if (!HasFormat())
                {
                    throw new InvalidDataException("No output format has been selected");
                }
                //Process evtc here
                ParsedEvtcLog log = parser.ParseLog(operation, fInfo);
                var externalTraces = new List<string>();
                string[] uploadresult = UploadController.UploadOperation(externalTraces, fInfo, new UploadSettings(Properties.Settings.Default.UploadToDPSReports, Properties.Settings.Default.UploadToDPSReportsRH));

                WebhookController.SendMessage(externalTraces, GetEmbed(log, uploadresult), uploadresult, new WebhookSettings(Properties.Settings.Default.SendEmbedToWebhook && Properties.Settings.Default.UploadToDPSReports && !Properties.Settings.Default.ParseMultipleLogs, Properties.Settings.Default.WebhookURL, Properties.Settings.Default.SendSimpleMessageToWebhook));
                foreach (string trace in externalTraces)
                {
                    operation.UpdateProgress(trace);
                }
                //Creating File
                GenerateFiles(log, operation, uploadresult, fInfo);
            }
            catch (Exception ex)
            {
                throw new EncompassException(ex);
            }
            finally
            {
                GC.Collect();
                Thread.CurrentThread.CurrentCulture = before;
            }
        }

        private static void CompressFile(string file, MemoryStream str, OperationController operation)
        {
            // Create the compressed file.
            byte[] data = str.ToArray();
            string outputFile = file + ".gz";
            using (FileStream outFile =
                        File.Create(outputFile))
            {
                using (var Compress =
                    new GZipStream(outFile,
                    CompressionMode.Compress))
                {
                    // Copy the source file into 
                    // the compression stream.
                    Compress.Write(data, 0, data.Length);
                }
            }
            operation.GeneratedFiles.Add(outputFile);
        }

        private static DirectoryInfo GetSaveDirectory(FileInfo fInfo)
        {
            //save location
            DirectoryInfo saveDirectory;
            if (Properties.Settings.Default.SaveAtOut || Properties.Settings.Default.OutLocation == null)
            {
                //Default save directory
                saveDirectory = fInfo.Directory;
                if (!saveDirectory.Exists)
                {
                    throw new InvalidOperationException("Save directory does not exist");
                }
            }
            else
            {
                if (!Directory.Exists(Properties.Settings.Default.OutLocation))
                {
                    throw new InvalidOperationException("Save directory does not exist");
                }
                saveDirectory = new DirectoryInfo(Properties.Settings.Default.OutLocation);
            }
            return saveDirectory;
        }

        public static void GenerateLogFile(OperationController operation)
        {
            if (Properties.Settings.Default.SaveOutTrace)
            {
                var fInfo = new FileInfo(operation.Location);

                string fName = fInfo.Name.Split('.')[0];
                if (!fInfo.Exists)
                {
                    fInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory);
                }

                DirectoryInfo saveDirectory = GetSaveDirectory(fInfo);

                string outputFile = Path.Combine(
                saveDirectory.FullName,
                $"{fName}.log"
                );
                operation.GeneratedFiles.Add(outputFile);
                operation.PathsToOpen.Add(saveDirectory.FullName);
                using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    operation.WriteLogMessages(sw);
                }
            }
        }

        private static void GenerateFiles(ParsedEvtcLog log, OperationController operation, string[] uploadresult, FileInfo fInfo)
        {
            operation.UpdateProgressWithCancellationCheck("Creating File(s)");

            DirectoryInfo saveDirectory = GetSaveDirectory(fInfo);

            string result = log.FightData.Success ? "kill" : "fail";
            string encounterLengthTerm = Properties.Settings.Default.AddDuration ? "_" + (log.FightData.FightEnd / 1000).ToString() + "s" : "";
            string PoVClassTerm = Properties.Settings.Default.AddPoVProf ? "_" + log.LogData.PoV.Prof.ToLower() : "";
            string fName = fInfo.Name.Split('.')[0];
            fName = $"{fName}{PoVClassTerm}_{log.FightData.Logic.Extension}{encounterLengthTerm}_{result}";

            // parallel stuff
            if (Properties.Settings.Default.MultiThreaded)
            {
                log.FightData.GetPhases(log);
                operation.UpdateProgressWithCancellationCheck("Multi threading");
                var playersAndTargets = new List<AbstractSingleActor>(log.PlayerList);
                playersAndTargets.AddRange(log.FightData.Logic.Targets);
                foreach (AbstractSingleActor actor in playersAndTargets)
                {
                    // that part can't be //
                    actor.ComputeBuffMap(log);
                }
                if (log.CanCombatReplay)
                {
                    var playersAndTargetsAndMobs = new List<AbstractSingleActor>(log.FightData.Logic.TrashMobs);
                    playersAndTargetsAndMobs.AddRange(playersAndTargets);
                    // init all positions
                    Parallel.ForEach(playersAndTargetsAndMobs, actor => actor.GetCombatReplayPolledPositions(log));
                }
                else if (log.CombatData.HasMovementData)
                {
                    Parallel.ForEach(log.PlayerList, player => player.GetCombatReplayPolledPositions(log));
                }
                Parallel.ForEach(playersAndTargets, actor => actor.GetBuffGraphs(log));
                //
                Parallel.ForEach(log.PlayerList, player => player.GetDamageModifierStats(log, null));
                // once simulation is done, computing buff stats is thread safe
                Parallel.ForEach(log.PlayerList, player => player.GetBuffs(log, BuffEnum.Self));
                Parallel.ForEach(log.FightData.Logic.Targets, target => target.GetBuffs(log));
            }
            if (Properties.Settings.Default.SaveOutHTML)
            {
                operation.UpdateProgressWithCancellationCheck("Creating HTML");
                string outputFile = Path.Combine(
                saveDirectory.FullName,
                $"{fName}.html"
                );
                operation.GeneratedFiles.Add(outputFile);
                operation.PathsToOpen.Add(outputFile);
                using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    var builder = new HTMLBuilder(log, new HTMLSettings(Properties.Settings.Default.LightTheme, Properties.Settings.Default.HtmlExternalScripts), htmlAssets, uploadresult);
                    builder.CreateHTML(sw, saveDirectory.FullName);
                }
                operation.UpdateProgressWithCancellationCheck("HTML created");
            }
            if (Properties.Settings.Default.SaveOutCSV)
            {
                operation.UpdateProgressWithCancellationCheck("Creating CSV");
                string outputFile = Path.Combine(
                    saveDirectory.FullName,
                    $"{fName}.csv"
                );
                operation.GeneratedFiles.Add(outputFile);
                operation.PathsToOpen.Add(outputFile);
                using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs, Encoding.GetEncoding(1252)))
                {
                    var builder = new CSVBuilder(log, new CSVSettings(","), uploadresult);
                    builder.CreateCSV(sw);
                }
                operation.UpdateProgressWithCancellationCheck("CSV created");
            }
            if (Properties.Settings.Default.SaveOutJSON || Properties.Settings.Default.SaveOutXML)
            {
                var builder = new RawFormatBuilder(log, new RawFormatSettings(Properties.Settings.Default.RawTimelineArrays), uploadresult);
                if (Properties.Settings.Default.SaveOutJSON)
                {
                    operation.UpdateProgressWithCancellationCheck("Creating JSON");
                    string outputFile = Path.Combine(
                        saveDirectory.FullName,
                        $"{fName}.json"
                    );
                    operation.PathsToOpen.Add(saveDirectory.FullName);
                    Stream str;
                    if (Properties.Settings.Default.CompressRaw)
                    {
                        str = new MemoryStream();
                    }
                    else
                    {
                        str = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                    }
                    using (var sw = new StreamWriter(str, BuilderHelper.NoBOMEncodingUTF8))
                    {
                        builder.CreateJSON(sw, Properties.Settings.Default.IndentJSON);
                    }
                    if (str is MemoryStream msr)
                    {
                        CompressFile(outputFile, msr, operation);
                        operation.UpdateProgressWithCancellationCheck("JSON compressed");
                    }
                    else
                    {
                        operation.GeneratedFiles.Add(outputFile);
                    }
                    operation.UpdateProgressWithCancellationCheck("JSON created");
                }
                if (Properties.Settings.Default.SaveOutXML)
                {
                    operation.UpdateProgressWithCancellationCheck("Creating XML");
                    string outputFile = Path.Combine(
                        saveDirectory.FullName,
                        $"{fName}.xml"
                    );
                    operation.PathsToOpen.Add(saveDirectory.FullName);
                    Stream str;
                    if (Properties.Settings.Default.CompressRaw)
                    {
                        str = new MemoryStream();
                    }
                    else
                    {
                        str = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                    }
                    using (var sw = new StreamWriter(str, BuilderHelper.NoBOMEncodingUTF8))
                    {
                        builder.CreateXML(sw, Properties.Settings.Default.IndentXML);
                    }
                    if (str is MemoryStream msr)
                    {
                        CompressFile(outputFile, msr, operation);
                        operation.UpdateProgressWithCancellationCheck("XML compressed");
                    }
                    else
                    {
                        operation.GeneratedFiles.Add(outputFile);
                    }
                    operation.UpdateProgressWithCancellationCheck("XML created");
                }
            }
            operation.UpdateProgress($"Completed parsing for {result}ed {log.FightData.Logic.Extension}");
        }

    }
}
