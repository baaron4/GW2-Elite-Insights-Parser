using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GW2EIParser.Exceptions;

namespace GW2EIParser
{
    public class ConsoleProgram
    {
        public ConsoleProgram(IEnumerable<string> logFiles)
        {
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Properties.Settings.Default.ParseMultipleLogs ? -1 : 1
            };
            Parallel.ForEach(logFiles, options, file => ParseLog(file));
        }

        private void ParseLog(string logFile)
        {
            var operation = new ConsoleOperationController(logFile, "Ready to parse");
            try
            {
                ProgramHelper.DoWork(operation);
            }
            catch (ExceptionEncompass ex)
            {
                Console.WriteLine(ex.GetFinalException().Message);
            }
            catch (Exception)
            {
                Console.WriteLine("Something terrible has happened");
            }

        }
    }
}
