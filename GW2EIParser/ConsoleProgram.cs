using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using GW2EIParser.Exceptions;

namespace GW2EIParser
{
    internal class ConsoleProgram
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
            var operation = new ConsoleOperationController(ProgramHelper.ParserName, ProgramHelper.ParserVersion, logFile, "Ready to parse");
            try
            {
                ProgramHelper.DoWork(operation);
                operation.FinalizeStatus("Parsing Successful - ");
            }
            catch (EncompassException ex)
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
