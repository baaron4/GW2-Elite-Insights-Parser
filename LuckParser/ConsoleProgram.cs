using LuckParser.Builders;
using LuckParser.Controllers;
using LuckParser.Exceptions;
using LuckParser.Models;
using LuckParser.Parser;
using LuckParser.Setting;
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
            UploadController up_controller = new UploadController();
            System.Globalization.CultureInfo before = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture =
                    new System.Globalization.CultureInfo("en-US");
            GridRow row = new GridRow(logFile as string, "Ready to parse")
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
                ParsingController control = new ParsingController();

                if (!GeneralHelper.HasFormat())
                {
                    throw new CancellationException(row, new InvalidDataException("No output format has been selected"));
                }

                if (GeneralHelper.IsSupportedFormat(fInfo.Name))
                {
                    //Process evtc here
                    ParsedLog log = control.ParseLog(row, fInfo.FullName);
                    string[] uploadresult = up_controller.UploadOperation(row, fInfo);
                    //Creating File
                    GeneralHelper.GenerateFiles(log, row, uploadresult, fInfo);
                }
                else
                {
                    row.BgWorker.UpdateProgress(row, "Not EVTC", 100);
                    Console.Error.Write("Not EVTC");
                    throw new CancellationException(row, new InvalidDataException("Not EVTC"));
                }
            }
            catch (SkipException s)
            {
                Console.Error.Write(s.Message);
                throw new CancellationException(row, s);
            }
            catch (TooShortException t)
            {
                Console.Error.Write(t.Message);
                throw new CancellationException(row, t);
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
