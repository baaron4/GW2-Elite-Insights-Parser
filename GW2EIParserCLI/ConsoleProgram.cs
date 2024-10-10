using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GW2EIParserCommons;
using GW2EIParserCommons.Exceptions;

namespace GW2EIParser
{
    internal class ConsoleProgram
    {

        public ConsoleProgram(IEnumerable<string> logFiles, ProgramHelper programHelper)
        {
            if (programHelper.ParseMultipleLogs())
            {
                var splitLogFiles = new List<List<string>>();
                var sizeSortedLogFiles = new List<string>(logFiles);
                for (int i = 0; i < programHelper.GetMaxParallelRunning(); i++)
                {
                    splitLogFiles.Add(new List<string>());
                }
                sizeSortedLogFiles.Sort((x, y) =>
                {
                    var fInfoX = new FileInfo(x);
                    long xValue = fInfoX.Exists ? fInfoX.Length : 0;
                    var fInfoY = new FileInfo(y);
                    long yValue = fInfoY.Exists ? fInfoY.Length : 0;
                    return xValue.CompareTo(yValue);
                });
                int index = 0;
                foreach (string file in sizeSortedLogFiles)
                {
                    splitLogFiles[index].Add(file);
                    index = (index + 1) % programHelper.GetMaxParallelRunning();
                }
                Parallel.ForEach(splitLogFiles, files =>
                {
                    foreach (string file in files)
                    {
                        ParseLog(file, programHelper);
                    }
                });
            }
            else
            {
                foreach (string file in logFiles)
                {
                    ParseLog(file, programHelper);
                }
            }
        }

        private static void ParseLog(string logFile, ProgramHelper programHelper)
        {
            programHelper.ExecuteMemoryCheckTask();
            var operation = new ConsoleOperationController(logFile);
            try
            {
                programHelper.DoWork(operation);
                operation.FinalizeStatus("Parsing Successful - ");
            }
            catch (ProgramException ex)
            {
                operation.UpdateProgress("Program: " + ex.InnerException.Message);
                operation.FinalizeStatus("Parsing Failure - ");
            }
            catch (Exception)
            {
                operation.UpdateProgress("Program: something terrible has happened");
                operation.FinalizeStatus("Parsing Failure - ");
            }
            finally
            {
                programHelper.GenerateTraceFile(operation);
            }
            GC.Collect();
        }
    }
}
