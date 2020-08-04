using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GW2EIUtils.Exceptions;
using GW2EIUtils;

namespace GW2EIParser
{
    public class ConsoleProgram
    {
        public ConsoleProgram(IEnumerable<string> logFiles)
        {
            if (Properties.Settings.Default.ParseMultipleLogs)
            {
                Parallel.ForEach(logFiles, file => ParseLog(file));
            }
            else
            {
                foreach (string file in logFiles)
                {
                    ParseLog(file);
                }
            }
        }

        private static void ParseLog(string logFile)
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
