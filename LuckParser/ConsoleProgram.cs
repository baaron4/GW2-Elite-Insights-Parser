using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LuckParser.Exceptions;

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
                var tasks = new List<Task>();

                foreach (string file in logFiles)
                {
                    tasks.Add(Task.Factory.StartNew(ParseLog, file));
                }

                Task.WaitAll(tasks.ToArray());
            }
        }

        private void ParseLog(object logFile)
        {
            var row = new GridRow(logFile as string, "Ready to parse")
            {
                BgWorker = new System.ComponentModel.BackgroundWorker()
                {
                    WorkerReportsProgress = true
                }
            };
            try
            {
                ProgramHelper.DoWork(row);
            }
            catch (CancellationException ex)
            {
                Console.WriteLine(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            catch (Exception)
            {
                Console.WriteLine("Something terrible has happened");
            }

        }
    }
}
