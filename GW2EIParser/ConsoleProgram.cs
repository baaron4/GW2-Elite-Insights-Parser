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
                operation.FinalizeStatus("Parsing Successful - ");
            }
            catch (ExceptionEncompass ex)
            {
                operation.UpdateProgress(ex.GetFinalException().Message);
                operation.FinalizeStatus("Parsing Failure - ");
            }
            catch (Exception)
            {
                operation.UpdateProgress("Something terrible has happened");
                operation.FinalizeStatus("Parsing Failure - ");
            }
            finally
            {
                ProgramHelper.GenerateLogFile(operation);
            }
        }
    }
}
