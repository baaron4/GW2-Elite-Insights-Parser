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
                        HTMLBuilderNew.UpdateStatisticSwitches(switches);
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
                                HTMLBuilderNew builder = new HTMLBuilderNew(log, settings, statistics);
                                builder.CreateHTML(sw, saveDirectory.FullName);
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
                                CSVBuilder builder = new CSVBuilder(sw, ",",log, settings, statistics);
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
