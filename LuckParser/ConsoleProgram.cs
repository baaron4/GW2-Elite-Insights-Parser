using LuckParser.Controllers;
using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuckParser
{
    public class ConsoleProgram
    {
        public ConsoleProgram(IEnumerable<string> logFiles)
        {
            if (Properties.Settings.Default.ParseOneAtATime)
            {
                foreach (string file in logFiles)
                {
                    ParseLog(file);
                }
            }
            else
            {
                List<Task> tasks = new List<Task>();

                foreach (string file in logFiles)
                {
                    tasks.Add(Task.Factory.StartNew(ParseLog, file));
                }

                Task.WaitAll(tasks.ToArray());
            }
        }

        private void ParseLog(object logFile)
        {
            UploadController up_controller = null;
            System.Globalization.CultureInfo before = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture =
                    new System.Globalization.CultureInfo("en-US");
            GridRow row = new GridRow(logFile as string, "")
            {
                BgWorker = new System.ComponentModel.BackgroundWorker()
                {
                    WorkerReportsProgress = true
                }
            };
            row.Metadata.FromConsole = true;

            FileInfo fInfo = new FileInfo(row.Location);
            if (!fInfo.Exists)
            {
                throw new CancellationException(row, new FileNotFoundException("File does not exist", fInfo.FullName));
            }
            //Upload Process
            Task<string> DREITask = null;
            Task<string> DRRHTask = null;
            Task<string> RaidarTask = null;
            string[] uploadresult = new string[3] { "", "", "" };
            try
            {
                SettingsContainer settings = new SettingsContainer(Properties.Settings.Default);
                Parser control = new Parser(settings);

                if (GeneralHelper.IsSupportedFormat(fInfo.Name))
                {
                    //Process evtc here
                    ParsedLog log = control.ParseLog(row, fInfo.FullName);
                    Console.Write("Log Parsed\n");
                    bool uploadAuthorized = !Properties.Settings.Default.SkipFailedTries || (Properties.Settings.Default.SkipFailedTries && log.FightData.Success);
                    if (Properties.Settings.Default.UploadToDPSReports && uploadAuthorized)
                    {
                        Console.Write("Uploading to DPSReports using EI\n");
                        if (up_controller == null)
                        {
                            up_controller = new UploadController();
                        }
                        DREITask = Task.Factory.StartNew(() => up_controller.UploadDPSReportsEI(fInfo));
                        if (DREITask != null)
                        {
                            while (!DREITask.IsCompleted)
                            {
                                System.Threading.Thread.Sleep(100);
                            }
                            uploadresult[0] = DREITask.Result;
                        }
                        else
                        {
                            uploadresult[0] = "Failed to Define Upload Task";
                        }
                    }
                    if (Properties.Settings.Default.UploadToDPSReportsRH && uploadAuthorized)
                    {
                        Console.Write("Uploading to DPSReports using RH\n");
                        if (up_controller == null)
                        {
                            up_controller = new UploadController();
                        }
                        DRRHTask = Task.Factory.StartNew(() => up_controller.UploadDPSReportsRH(fInfo));
                        if (DRRHTask != null)
                        {
                            while (!DRRHTask.IsCompleted)
                            {
                                System.Threading.Thread.Sleep(100);
                            }
                            uploadresult[1] = DRRHTask.Result;
                        }
                        else
                        {
                            uploadresult[1] = "Failed to Define Upload Task";
                        }
                    }
                    if (Properties.Settings.Default.UploadToRaidar && uploadAuthorized)
                    {
                        Console.Write("Uploading to Raidar\n");
                        if (up_controller == null)
                        {
                            up_controller = new UploadController();
                        }
                        RaidarTask = Task.Factory.StartNew(() => up_controller.UploadRaidar(fInfo));
                        if (RaidarTask != null)
                        {
                            while (!RaidarTask.IsCompleted)
                            {
                                System.Threading.Thread.Sleep(100);
                            }
                            uploadresult[2] = RaidarTask.Result;
                        }
                        else
                        {
                            uploadresult[2] = "Failed to Define Upload Task";
                        }
                    }
                    if (Properties.Settings.Default.SkipFailedTries)
                    {
                        if (!log.FightData.Success)
                        {
                            throw new SkipException();
                        }
                    }
                    //Creating File
                    //save location
                    DirectoryInfo saveDirectory;
                    if (Properties.Settings.Default.SaveAtOut || Properties.Settings.Default.OutLocation == null)
                    {
                        //Default save directory
                        saveDirectory = fInfo.Directory;
                    }
                    else
                    {
                        //Customised save directory
                        saveDirectory = new DirectoryInfo(Properties.Settings.Default.OutLocation);
                    }

                    if (saveDirectory == null)
                    {
                        throw new CancellationException(row, new InvalidDataException("Save Directory not found"));
                    }
                    
                    string result = log.FightData.Success ? "kill" : "fail";
                    string encounterLengthTerm = Properties.Settings.Default.AddDuration ? "_" + (log.FightData.FightDuration / 1000).ToString() + "s" : "";
                    string PoVClassTerm = Properties.Settings.Default.AddPoVProf ? "_" + log.PlayerList.Find(x => x.AgentItem.Name.Split(':')[0] == log.LogData.PoV.Split(':')[0]).Prof.ToLower() : "";

                    StatisticsCalculator statisticsCalculator = new StatisticsCalculator(settings);
                    StatisticsCalculator.Switches switches = new StatisticsCalculator.Switches();
                    if (Properties.Settings.Default.SaveOutHTML)
                    {
                        LegacyHTMLBuilder.UpdateStatisticSwitches(switches);
                    }
                    if (Properties.Settings.Default.SaveOutCSV)
                    {
                        CSVBuilder.UpdateStatisticSwitches(switches);
                    }
                    if (Properties.Settings.Default.SaveOutJSON || Properties.Settings.Default.SaveOutXML)
                    {
                        RawFormatBuilder.UpdateStatisticSwitches(switches);
                    }
                    Statistics statistics = statisticsCalculator.CalculateStatistics(log, switches);
                    Console.Write("Statistics Computed\n");

                    string fName = fInfo.Name.Split('.')[0];
                    fName = $"{fName}{PoVClassTerm}_{log.FightData.Logic.Extension}{encounterLengthTerm}_{result}";
                    if (Properties.Settings.Default.SaveOutHTML)
                    {
                        string outputFile = Path.Combine(
                        saveDirectory.FullName,
                        $"{fName}.html"
                        );
                        using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                if (!Properties.Settings.Default.LegacyHtmlMode)
                                {
                                    var builder = new HTMLBuilder(log, settings, statistics, uploadresult);
                                    builder.CreateHTML(sw, saveDirectory.FullName);
                                } else
                                {
                                    var builder = new LegacyHTMLBuilder(log, settings, statistics, uploadresult);
                                    builder.CreateHTML(sw);
                                }
                            }
                        }
                    }
                    if (Properties.Settings.Default.SaveOutCSV)
                    {
                        string outputFile = Path.Combine(
                        saveDirectory.FullName,
                        $"{fName}.csv"
                        );
                        using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding(1252)))
                            {
                                var builder = new CSVBuilder(sw, ",",log, settings, statistics,uploadresult);
                                builder.CreateCSV();
                            }
                        }
                    }

                    if (Properties.Settings.Default.SaveOutJSON)
                    {
                        string outputFile = Path.Combine(
                            saveDirectory.FullName,
                            $"{fName}.json"
                        );
                        using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                            {
                                var builder = new RawFormatBuilder(sw, log, settings, statistics, uploadresult);
                                builder.CreateJSON();
                            }
                        }
                    }

                    if (Properties.Settings.Default.SaveOutXML)
                    {
                        string outputFile = Path.Combine(
                            saveDirectory.FullName,
                            $"{fName}.xml"
                        );
                        using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                            {
                                var builder = new RawFormatBuilder(sw, log, settings, statistics, uploadresult);
                                builder.CreateXML();
                            }
                        }
                    }

                    Console.Write("Generation Done\n");
                }
                else
                {
                    Console.Error.Write("Not EVTC");
                    throw new CancellationException(row, new InvalidDataException("Not EVTC"));
                }
            }
            catch (SkipException s)
            {
                Console.Error.Write(s.Message);
            }
            catch (Exception ex) when (!System.Diagnostics.Debugger.IsAttached)
            {
                Console.Error.Write(ex.Message);
                throw new CancellationException(row, ex);
            } 
            finally
            {
                Thread.CurrentThread.CurrentCulture = before;
            }
            
        }
    }
}
