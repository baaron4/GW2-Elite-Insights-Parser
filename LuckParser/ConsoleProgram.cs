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
        public ConsoleProgram(string[] args)
        {
            if (Properties.Settings.Default.ParseOneAtATime)
            {
                foreach (string file in args)
                {
                    ParseLog(file);
                }
            }
            else
            {
                List<Task> tasks = new List<Task>();

                foreach (string file in args)
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
            if (Properties.Settings.Default.UploadToDPSReports)
            {
               
                if (up_controller == null)
                {
                    up_controller = new UploadController();
                }
                DREITask = Task.Factory.StartNew(() => up_controller.UploadDPSReportsEI(fInfo));

            }
            if (Properties.Settings.Default.UploadToDPSReportsRH)
            {
              
                if (up_controller == null)
                {
                    up_controller = new UploadController();
                }
                DRRHTask = Task.Factory.StartNew(() => up_controller.UploadDPSReportsRH(fInfo));

            }
            if (Properties.Settings.Default.UploadToRaidar)
            {
               
                if (up_controller == null)
                {
                    up_controller = new UploadController();
                }
                RaidarTask = Task.Factory.StartNew(() => up_controller.UploadRaidar(fInfo));

            }
            try
            {
                Parser control = new Parser();

                if (fInfo.Extension.Equals(".evtc", StringComparison.OrdinalIgnoreCase) ||
                    fInfo.Name.EndsWith(".evtc.zip", StringComparison.OrdinalIgnoreCase))
                {
                    //Process evtc here
                    control.ParseLog(row, fInfo.FullName);
                    ParsedLog log = control.GetParsedLog();
                    Console.Write("Log Parsed");
                    //Creating File
                    //Wait for Upload
                    if (Properties.Settings.Default.UploadToDPSReports)
                    {
                       
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
                    if (Properties.Settings.Default.UploadToDPSReportsRH)
                    {
                        
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
                    if (Properties.Settings.Default.UploadToRaidar)
                    {
                        
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

                    string bossid = control.GetBossData().GetID().ToString();
                    string result = "fail";

                    if (control.GetLogData().GetBosskill())
                    {
                        result = "kill";
                    }

                    SettingsContainer settings = new SettingsContainer(Properties.Settings.Default);
                    StatisticsCalculator statisticsCalculator = new StatisticsCalculator(settings);
                    StatisticsCalculator.Switches switches = new StatisticsCalculator.Switches();
                    if (Properties.Settings.Default.SaveOutHTML)
                    {
                        HTMLBuilder.UpdateStatisticSwitches(switches);
                    }
                    if (Properties.Settings.Default.SaveOutCSV)
                    {
                        CSVBuilder.UpdateStatisticSwitches(switches);
                    }
                    Statistics statistics = statisticsCalculator.CalculateStatistics(log, switches);
                    Console.Write("Statistics Computed");

                    string fName = fInfo.Name.Split('.')[0];
                    if (Properties.Settings.Default.SaveOutHTML)
                    {
                        string outputFile = Path.Combine(
                        saveDirectory.FullName,
                        $"{fName}_{HTMLHelper.GetLink(bossid + "-ext")}_{result}.html"
                        );
                        using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs))
                            {
                                HTMLBuilder builder = new HTMLBuilder(log, settings, statistics,uploadresult);
                                builder.CreateHTML(sw);
                            }
                        }
                    }
                    if (Properties.Settings.Default.SaveOutCSV)
                    {
                        string outputFile = Path.Combine(
                        saveDirectory.FullName,
                        $"{fName}_{HTMLHelper.GetLink(bossid + "-ext")}_{result}.csv"
                        );
                        using (FileStream fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding(1252)))
                            {
                                CSVBuilder builder = new CSVBuilder(sw, ",",log, settings, statistics,uploadresult);
                                builder.CreateCSV();
                            }
                        }
                    }
                    Console.Write("Generation Done");

                }
                else
                {
                    Console.Error.Write("Not EVTC");
                    throw new CancellationException(row, new InvalidDataException("Not EVTC"));
                }
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
