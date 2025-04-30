using System.Diagnostics;
using System.IO.Compression;
using System.Text;
using Discord;
using GW2EIBuilders;
using GW2EIDiscord;
using GW2EIWingman;
using GW2EIDPSReport;
using GW2EIDPSReport.DPSReportJsons;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using GW2EIGW2API;
using GW2EIParserCommons.Exceptions;
using Tracing;

[assembly: CLSCompliant(false)]
namespace GW2EIParserCommons;

public sealed class ProgramHelper : IDisposable
{

    public ProgramHelper(Version parserVersion, ProgramSettings settings)
    {
        ParserVersion = parserVersion;
        Settings = settings;
    }

    public void ApplySettings(ProgramSettings settings)
    {
        Settings = settings;
    }

    public static IReadOnlyList<string> SupportedFormats => SupportedFileFormats.SupportedFormats;

    public static bool IsSupportedFormat(string path)
    {
        return SupportedFileFormats.IsSupportedFormat(path);
    }

    public static bool IsCompressedFormat(string path)
    {
        return SupportedFileFormats.IsCompressedFormat(path);
    }

    public static bool IsTemporaryCompressedFormat(string path)
    {
        return SupportedFileFormats.IsTemporaryCompressedFormat(path);
    }

    public static bool IsTemporaryFormat(string path)
    {
        return SupportedFileFormats.IsTemporaryFormat(path);
    }

    internal readonly static HTMLAssets htmlAssets = new();

    public ProgramSettings Settings { get; private set; }
    private readonly Version ParserVersion;

    private static readonly UTF8Encoding NoBOMEncodingUTF8 = new(false);

    public static readonly string SkillAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SkillList.json";
    public static readonly string SpecAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/SpecList.json";
    public static readonly string TraitAPICacheLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Content/TraitList.json";
    public static readonly string EILogPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Logs/";

    public static readonly GW2APIController APIController = new(SkillAPICacheLocation, SpecAPICacheLocation, TraitAPICacheLocation);

    private CancellationTokenSource? RunningMemoryCheck = null;

    public void Dispose()
    {
        if (RunningMemoryCheck != null)
        {
            RunningMemoryCheck.Cancel();
            RunningMemoryCheck.Dispose();
            RunningMemoryCheck = null;
        }
    }

    public int GetMaxParallelRunning()
    {
        return Settings.GetMaxParallelRunning();
    }

    public bool HasFormat()
    {
        return Settings.HasFormat();
    }

    public bool ParseMultipleLogs()
    {
        return Settings.DoParseMultipleLogs();
    }
    public void ExecuteMemoryCheckTask()
    {
        if (Settings.MemoryLimit == 0 && RunningMemoryCheck != null)
        {
            RunningMemoryCheck.Cancel();
            RunningMemoryCheck.Dispose();
            RunningMemoryCheck = null;
        }

        if (Settings.MemoryLimit == 0 || RunningMemoryCheck != null)
        {
            return;
        }

        RunningMemoryCheck = new CancellationTokenSource();// Prepare task
        Task.Run(async () =>
        {
            using var proc = Process.GetCurrentProcess();

            while (true)
            {
                await Task.Delay(500).ConfigureAwait(false);
                //NOTE(Rennorb): cannot wait for GC here because this is just a task (not a thread) and we would potentially be blocking other things from happening.
                proc.Refresh();
                if (proc.PrivateMemorySize64 > Math.Max(Settings.MemoryLimit, 100) * 1024L * 1024L)
                {
                    Environment.Exit(2);
                }
            }

        }, RunningMemoryCheck.Token);
    }

    public EmbedBuilder GetEmbedBuilder()
    {
        var builder = new EmbedBuilder();
        builder.WithAuthor("Elite Insights " + ParserVersion.ToString(), "https://github.com/baaron4/GW2-Elite-Insights-Parser/blob/master/GW2EIParserCommons/Content/ArenaNet/LI.png?raw=true", "https://github.com/baaron4/GW2-Elite-Insights-Parser");
        return builder;
    }

    private Embed BuildEmbed(ParsedEvtcLog log, string dpsReportPermalink)
    {
        EmbedBuilder builder = GetEmbedBuilder();
        builder.WithThumbnailUrl(log.FightData.Logic.Icon);
        //
        builder.AddField("Encounter Duration", log.FightData.DurationString);
        //
        if (log.FightData.Logic.GetInstanceBuffs(log).Any())
        {
            builder.AddField("Instance Buffs", string.Join("\n", log.FightData.Logic.GetInstanceBuffs(log).Select(x => (x.stack > 1 ? x.stack + " " : "") + x.buff.Name)));
        }
        //
        /*var playerByGroup = log.PlayerList.Where(x => !x.IsFakeActor).GroupBy(x => x.Group);
        var hasGroups = playerByGroup.Count > 1;
        foreach (var group in playerByGroup)
        {
            var groupField = new List<string>();
            foreach (Player p in group)
            {
                groupField.Add(p.Character + " - " + p.Prof);
            }
            builder.AddField(hasGroups ? "Group " + group.Key : "Party Composition", String.Join("\n", groupField));
        }*/
        //
        builder.AddField("Game Data", "ARC: " + log.LogData.ArcVersion + " | " + "GW2 Build: " + log.LogData.GW2Build);
        //
        builder.WithTitle(log.FightData.FightName);
        //builder.WithTimestamp(DateTime.Now);
        AgentItem? pov = log.LogData.PoV;
        if (pov != null)
        {
            SingleActor povActor = log.FindActor(pov);
            builder.WithFooter(povActor.Account + " - " + povActor.Spec.ToString() + "\n" + log.LogData.LogStartStd + " / " + log.LogData.LogEndStd, povActor.GetIcon());
        }
        builder.WithColor(log.FightData.Success ? Discord.Color.Green : Discord.Color.Red);
        if (dpsReportPermalink.Length > 0)
        {
            builder.WithUrl(dpsReportPermalink);
        }
        return builder.Build();
    }

    private string[] UploadOperation(FileInfo fInfo, ParsedEvtcLog originalLog, OperationController originalController)
    {
        //Upload Process
        string[] uploadresult = ["", ""];
        if (Settings.UploadToDPSReports)
        {
            originalController.UpdateProgressWithCancellationCheck("DPSReport: Uploading");
            DPSReportUploadObject? response = DPSReportController.UploadUsingEI(fInfo, str => originalController.UpdateProgress("DPSReport: " + str), Settings.DPSReportUserToken,
            originalLog.ParserSettings.AnonymousPlayers,
            originalLog.ParserSettings.DetailedWvWParse);
            uploadresult[0] = response != null ? response.Permalink : "Upload process failed";
            originalController.UpdateProgressWithCancellationCheck("DPSReport: " + uploadresult[0]);
            /*
            if (Properties.Settings.Default.UploadToWingman)
            {
                if (isWingmanCompatible)
                {
                    traces.Add("Uploading to Wingman using DPSReport url");
                    WingmanController.UploadToWingmanUsingImportLogQueued(uploadresult[0], traces, ParserVersion);
                }
                else
                {
                    traces.Add("Can not upload to Wingman using DPSReport url: unsupported log");
                }
            }
            */
        }
        if (Settings.UploadToWingman)
        {
            if (originalLog.ParserSettings.AnonymousPlayers)
            {
                originalController.UpdateProgressWithCancellationCheck("Wingman: players and accounts have been anonymized, log not supported");
            } 
            else
            {
                string accName = originalLog.LogData.PoV != null ? originalLog.LogData.PoVAccount : "-";

                if (WingmanController.CheckUploadPossible(fInfo, accName, originalLog.FightData.TriggerID, str => originalController.UpdateProgress("Wingman: " + str)))
                {
#if !DEBUG
                    try
                    {
                        var expectedSettings = new EvtcParserSettings(Settings.Anonymous,
                                                        Settings.SkipFailedTries,
                                                        true,
                                                        true,
                                                        true,
                                                        Settings.CustomTooShort,
                                                        Settings.DetailledWvW);
                        ParsedEvtcLog logToUse = originalLog;
                        if (originalLog.ParserSettings.ComputeDamageModifiers != expectedSettings.ComputeDamageModifiers ||
                            originalLog.ParserSettings.ParsePhases != expectedSettings.ParsePhases ||
                            originalLog.ParserSettings.ParseCombatReplay != expectedSettings.ParseCombatReplay)
                        {
                            // We need to create a parser that matches Wingman's expected settings
                            var parser = new EvtcParser(expectedSettings, APIController);
                            originalController.UpdateProgressWithCancellationCheck("Wingman: Setting mismatch, creating a new ParsedEvtcLog, this will extend total processing duration if file generation is also requested");
                            logToUse = parser.ParseLog(originalController, fInfo, out ParsingFailureReason? failureReason, !Settings.SingleThreaded)!;
                        }

                        byte[] jsonFile, htmlFile;
                        originalController.UpdateProgressWithCancellationCheck("Wingman: Creating JSON");
                        var uploadResult = new UploadResults();
                        {
                            var ms = new MemoryStream();
                            var builder = new RawFormatBuilder(logToUse, new RawFormatSettings(true), ParserVersion, uploadResult);

                            builder.CreateJSON(ms, false);

                            jsonFile = ms.ToArray();
                        }

                        originalController.UpdateProgressWithCancellationCheck("Wingman: Creating HTML");
                        {
                            var ms = new MemoryStream();
                            var sw = new StreamWriter(ms, NoBOMEncodingUTF8);
                            var builder = new HTMLBuilder(logToUse, new HTMLSettings(false, false, true), htmlAssets, ParserVersion, uploadResult);

                            builder.CreateHTML(sw, null);
                            sw.Close();
                            htmlFile = ms.ToArray();
                        }

                        if (logToUse != originalLog)
                        {
                            originalController.UpdateProgressWithCancellationCheck("Wingman: new ParsedEvtcLog processing completed");
                        }

                        originalController.UpdateProgressWithCancellationCheck("Wingman: Preparing upload");

                        string result = logToUse.FightData.Success ? "kill" : "fail";
                        WingmanController.UploadProcessed(fInfo, accName, jsonFile, htmlFile, $"_{logToUse.FightData.Logic.Extension}_{result}", str => originalController.UpdateProgress("Wingman: " + str), ParserVersion);
                    }
                    catch (Exception e)
                    {
                        originalController.UpdateProgressWithCancellationCheck("Wingman: Operation failed " + e.Message);
                    }
#endif
                }
                else
                {
                    originalController.UpdateProgressWithCancellationCheck("Wingman: Upload is not possible, unsupported log, log already uploaded or wingman is down");
                }
            }
            originalController.UpdateProgressWithCancellationCheck("Wingman: Operation completed");

        }
        return uploadresult;
    }

    public void DoWork(OperationController operation)
    {
        System.Globalization.CultureInfo before = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture =
                new System.Globalization.CultureInfo("en-US");
        operation.Reset();
        try
        {
            operation.Start();
            var fInfo = new FileInfo(operation.InputFile);

            var parser = new EvtcParser(new EvtcParserSettings(Settings.Anonymous,
                                            Settings.SkipFailedTries,
                                            Settings.ParsePhases,
                                            Settings.ParseCombatReplay,
                                            Settings.ComputeDamageModifiers,
                                            Settings.CustomTooShort,
                                            Settings.DetailledWvW),
                                        APIController);

            //Process evtc here
            ParsedEvtcLog? log = parser.ParseLog(operation, fInfo, out var failureReason, !Settings.SingleThreaded && HasFormat());
            if (failureReason != null)
            {
                failureReason.Throw();
            }
            operation.BasicMetaData = new OperationController.OperationBasicMetaData(log!);
            string[] uploadStrings = UploadOperation(fInfo, log!, operation);
            if (uploadStrings[0].Contains("https"))
            {
                operation.DPSReportLink = uploadStrings[0];
                if (Settings.SendEmbedToWebhook)
                {
                    if (Settings.SendSimpleMessageToWebhook)
                    {
                        WebhookController.SendMessage(Settings.WebhookURL, uploadStrings[0], out string message);
                        operation.UpdateProgressWithCancellationCheck("Webhook: " + message);
                    }
                    else
                    {
                        WebhookController.SendMessage(Settings.WebhookURL, BuildEmbed(log!, uploadStrings[0]), out string message);
                        operation.UpdateProgressWithCancellationCheck("Webhook: " + message);
                    }
                }
            }
            //Creating File
            GenerateFiles(log!, operation, uploadStrings, fInfo);
        }
        catch (Exception ex)
        {
            throw new ProgramException(ex);
        }
        finally
        {
            operation.Stop();
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
            using var Compress =
                new GZipStream(outFile,
                CompressionMode.Compress);
            // Copy the source file into 
            // the compression stream.
            Compress.Write(data, 0, data.Length);
        }
        operation.AddFile(outputFile);
    }

    private DirectoryInfo GetSaveDirectory(FileInfo fInfo)
    {
        //save location
        DirectoryInfo? saveDirectory;
        if (Settings.SaveAtOut || Settings.OutLocation == null)
        {
            //Default save directory
            saveDirectory = fInfo.Directory;
            if (saveDirectory == null || !saveDirectory.Exists)
            {
                throw new InvalidOperationException("Save directory does not exist");
            }
        }
        else
        {
            if (!Directory.Exists(Settings.OutLocation))
            {
                throw new InvalidOperationException("Save directory does not exist");
            }
            saveDirectory = new DirectoryInfo(Settings.OutLocation);
        }
        return saveDirectory;
    }

    public void GenerateTraceFile(OperationController operation)
    {
        if (Settings.SaveOutTrace)
        {
            var fInfo = new FileInfo(operation.InputFile);

            string fName = Path.GetFileNameWithoutExtension(fInfo.FullName);
            if (!fInfo.Exists)
            {
                fInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory);
            }

            DirectoryInfo saveDirectory = GetSaveDirectory(fInfo);

            string outputFile = Path.Combine(
            saveDirectory.FullName,
            $"{fName}.log"
            );
            operation.AddFile(outputFile);
            using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
            {
                operation.WriteLogMessages(sw);
            }
            operation.OutLocation = saveDirectory.FullName;
        }
    }

    private void GenerateFiles(ParsedEvtcLog log, OperationController operation, string[] uploadStrings, FileInfo fInfo)
    {
        using var _t = new AutoTrace("Generate files");
        operation.UpdateProgressWithCancellationCheck("Program: Creating File(s)");

        DirectoryInfo saveDirectory = GetSaveDirectory(fInfo);

        string result = log.FightData.Success ? "kill" : "fail";
        string encounterLengthTerm = Settings.AddDuration ? "_" + (log.FightData.FightDuration / 1000).ToString() + "s" : "";
        string PoVClassTerm = Settings.AddPoVProf && log.LogData.PoV != null ? "_" + log.LogData.PoV.Spec.ToString().ToLower(System.Globalization.CultureInfo.CurrentCulture) : "";
        string fName = Path.GetFileNameWithoutExtension(fInfo.FullName);
        fName = $"{fName}{PoVClassTerm}_{log.FightData.Logic.Extension}{encounterLengthTerm}_{result}";

        var uploadResults = new UploadResults(uploadStrings[0], uploadStrings[1]);
        operation.OutLocation = saveDirectory.FullName;
        if (Settings.SaveOutHTML)
        {
            using var _t1 = new AutoTrace("Generate HTML");
            operation.UpdateProgressWithCancellationCheck("Program: Creating HTML");
            string outputFile = Path.Combine(saveDirectory.FullName, $"{fName}.html");
            operation.AddOpenableFile(outputFile);
            using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
            {
                var builder = new HTMLBuilder(log,
                    new HTMLSettings(
                        Settings.LightTheme,
                        Settings.HtmlExternalScripts,
                        Settings.HtmlExternalScriptsPath,
                        Settings.HtmlExternalScriptsCdn,
                        Settings.HtmlCompressJson
                    ), htmlAssets, ParserVersion, uploadResults);
                builder.CreateHTML(sw, saveDirectory.FullName);
            }
            operation.UpdateProgressWithCancellationCheck("Program: HTML created");
        }
        if (Settings.SaveOutCSV)
        {
            using var _t1 = new AutoTrace("Generate CSV");
            operation.UpdateProgressWithCancellationCheck("Program: Creating CSV");
            string outputFile = Path.Combine(
                saveDirectory.FullName,
                $"{fName}.csv"
            );
            operation.AddOpenableFile(outputFile);
            var builder = new CSVBuilder(log, new CSVSettings(","), ParserVersion, uploadResults);
            using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            using (var sw = new StreamWriter(fs, Encoding.GetEncoding(1252)))
            {
                builder.CreateCSV(sw);
            }
            operation.UpdateProgressWithCancellationCheck("Program: CSV created");
        }
        if (Settings.SaveOutJSON || Settings.SaveOutXML)
        {
            var builder = new RawFormatBuilder(log, new RawFormatSettings(Settings.RawTimelineArrays), ParserVersion, uploadResults);
            if (Settings.SaveOutJSON)
            {
                using var _t1 = new AutoTrace("Generate JSON");
                operation.UpdateProgressWithCancellationCheck("Program: Creating JSON");
                string outputFile = Path.Combine(saveDirectory.FullName, $"{fName}.json");
                
                using Stream str = Settings.CompressRaw 
                    ? new MemoryStream()
                    : new FileStream(outputFile, FileMode.Create, FileAccess.Write);

                builder.CreateJSON(str, Settings.IndentJSON);

                if (str is MemoryStream msr)
                {
                    CompressFile(outputFile, msr, operation);
                    operation.UpdateProgressWithCancellationCheck("Program: JSON compressed");
                }
                else
                {
                    operation.AddFile(outputFile);
                }

                operation.UpdateProgressWithCancellationCheck("Program: JSON created");
            }
            if (Settings.SaveOutXML)
            {
                using var _t1 = new AutoTrace("Generate XML");
                operation.UpdateProgressWithCancellationCheck("Program: Creating XML");
                string outputFile = Path.Combine(
                    saveDirectory.FullName,
                    $"{fName}.xml"
                );
                Stream str;
                if (Settings.CompressRaw)
                {
                    str = new MemoryStream();
                }
                else
                {
                    str = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                }
                using (var sw = new StreamWriter(str, NoBOMEncodingUTF8))
                {
                    builder.CreateXML(sw, Settings.IndentXML);
                }
                if (str is MemoryStream msr)
                {
                    CompressFile(outputFile, msr, operation);
                    operation.UpdateProgressWithCancellationCheck("Program: XML compressed");
                }
                else
                {
                    operation.AddFile(outputFile);
                }
                operation.UpdateProgressWithCancellationCheck("Program: XML created");
            }
        }
        operation.UpdateProgressWithCancellationCheck($"Completed for {result}ed {log.FightData.Logic.Extension}");
    }
}
