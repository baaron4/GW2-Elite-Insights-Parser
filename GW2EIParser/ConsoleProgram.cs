using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GW2EIParser.Exceptions;

namespace GW2EIParser
{
    internal class ConsoleProgram
    {
        // https://stackoverflow.com/questions/11463734/split-a-list-into-smaller-lists-of-n-size
        private static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }

        public ConsoleProgram(IEnumerable<string> logFiles)
        {
            if (Properties.Settings.Default.ParseMultipleLogs)
            {
                var logList = new List<string>(logFiles);
                int splitSize = (int)Math.Ceiling((double)logList.Count / ProgramHelper.GetMaxParallelRunning());
                IEnumerable<List<string>> splitLogFiles = SplitList(logList, splitSize);
                Parallel.ForEach(splitLogFiles, files => {
                    foreach(string file in files)
                    {
                        ParseLog(file);
                    }
                });
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
            var operation = new ConsoleOperationController(ProgramHelper.ParserVersion, logFile, "Ready to parse");
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
                ProgramHelper.GenerateTraceFile(operation);
            }
        }
    }
}
