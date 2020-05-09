using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GW2EIParser.Builders;
using GW2EIParser.Controllers;
using GW2EIParser.EIData;
using GW2EIParser.Exceptions;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser
{
    public static class ProgramHelper
    {
        private static bool HasFormat()
        {
            return Properties.Settings.Default.SaveOutCSV || Properties.Settings.Default.SaveOutHTML || Properties.Settings.Default.SaveOutXML || Properties.Settings.Default.SaveOutJSON;
        }

        private static readonly HashSet<string> _compressedFiles = new HashSet<string>()
        {
            ".zevtc",
            ".evtc.zip",
        };

        private static readonly HashSet<string> _tmpFiles = new HashSet<string>()
        {
            ".tmp.zip"
        };

        private static readonly HashSet<string> _supportedFiles = new HashSet<string>(_compressedFiles)
        {
            ".evtc"
        };

        public static bool IsCompressedFormat(string fileName)
        {
            foreach (string format in _compressedFiles)
            {
                if (fileName.EndsWith(format, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static List<string> GetSupportedFormats()
        {
            return new List<string>(_supportedFiles);
        }

        public static bool IsSupportedFormat(string fileName)
        {
            foreach (string format in _supportedFiles)
            {
                if (fileName.EndsWith(format, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsTemporaryFormat(string fileName)
        {
            foreach (string format in _tmpFiles)
            {
                if (fileName.EndsWith(format, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static void DoWork(OperationController operation)
        {
            System.Globalization.CultureInfo before = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture =
                    new System.Globalization.CultureInfo("en-US");
            var fInfo = new FileInfo(operation.Location);
            try
            {
                if (!fInfo.Exists)
                {
                    throw new FileNotFoundException("File " + fInfo.FullName + " does not exist");
                }
                var control = new ParsingController(new ParserSettings());

                if (!HasFormat())
                {
                    throw new InvalidDataException("No output format has been selected");
                }

                if (IsSupportedFormat(fInfo.Name))
                {
                    //Process evtc here
                    ParsedLog log = control.ParseLog(operation, fInfo.FullName);
                    string[] uploadresult = UploadController.UploadOperation(operation, fInfo);
                    //Creating File
                    GenerateFiles(log, operation, uploadresult, fInfo);
                }
                else
                {
                    throw new InvalidDataException("Not EVTC");
                }
            }
            catch (Exception ex)
            {
                throw new ExceptionEncompass(ex);
            }
            finally
            {
                GC.Collect();
                Thread.CurrentThread.CurrentCulture = before;
            }
        }

        private static void CompressFile(string file, MemoryStream str)
        {
            // Create the compressed file.
            byte[] data = str.ToArray();
            using (FileStream outFile =
                        File.Create(file + ".gz"))
            {
                using (var Compress =
                    new GZipStream(outFile,
                    CompressionMode.Compress))
                {
                    // Copy the source file into 
                    // the compression stream.
                    Compress.Write(data, 0, data.Length);
                }
            }
        }

        private static DirectoryInfo GetSaveDirectory(FileInfo fInfo)
        {
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
            return saveDirectory;
        }

        public static void GenerateLogFile(OperationController operation)
        {
            if (Properties.Settings.Default.SaveOutTrace)
            {
                var fInfo = new FileInfo(operation.Location);

                string fName = fInfo.Name.Split('.')[0];
                if (!fInfo.Exists)
                {
                    fInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory);
                }

                DirectoryInfo saveDirectory = GetSaveDirectory(fInfo);

                if (saveDirectory == null)
                {
                    return;
                }

                string outputFile = Path.Combine(
                saveDirectory.FullName,
                $"{fName}.log"
                );
                using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    operation.WriteLogMessages(sw);
                }
            }
        }

        private static void GenerateFiles(ParsedLog log, OperationController operation, string[] uploadresult, FileInfo fInfo)
        {
            operation.UpdateProgressWithCancellationCheck("Creating File(s)");

            DirectoryInfo saveDirectory = GetSaveDirectory(fInfo);

            if (saveDirectory == null)
            {
                throw new InvalidDataException("Save Directory not found");
            }

            string result = log.FightData.Success ? "kill" : "fail";
            string encounterLengthTerm = Properties.Settings.Default.AddDuration ? "_" + (log.FightData.FightEnd / 1000).ToString() + "s" : "";
            string PoVClassTerm = Properties.Settings.Default.AddPoVProf ? "_" + log.LogData.PoV.Prof.ToLower() : "";
            string fName = fInfo.Name.Split('.')[0];
            fName = $"{fName}{PoVClassTerm}_{log.FightData.Logic.Extension}{encounterLengthTerm}_{result}";

            // parallel stuff
            if (log.ParserSettings.MultiTasks)
            {
                operation.UpdateProgressWithCancellationCheck("Multi threading buff and damage mod computations");
                log.FightData.GetPhases(log);
                var actors = new List<AbstractSingleActor>(log.PlayerList);
                actors.AddRange(log.FightData.Logic.Targets);
                foreach (AbstractSingleActor actor in actors)
                {
                    // that part can't be //
                    actor.ComputeBuffMap(log);
                }
                Parallel.ForEach(actors, actor => actor.GetBuffGraphs(log));
                //
                Parallel.ForEach(log.PlayerList, player => player.GetDamageModifierStats(log, null));
                // once simulation is done, computing buff stats is thread safe
                Parallel.ForEach(log.PlayerList, player => player.GetBuffs(log, BuffEnum.Self));
            }
            if (Properties.Settings.Default.SaveOutHTML)
            {
                operation.UpdateProgressWithCancellationCheck("Creating HTML");
                string outputFile = Path.Combine(
                saveDirectory.FullName,
                $"{fName}.html"
                );
                operation.LogLocation = outputFile;
                using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    var builder = new HTMLBuilder(log, uploadresult, Properties.Settings.Default.LightTheme, Properties.Settings.Default.HtmlExternalScripts);
                    builder.CreateHTML(sw, saveDirectory.FullName);
                }
                operation.UpdateProgressWithCancellationCheck("HTML created");
            }
            if (Properties.Settings.Default.SaveOutCSV)
            {
                operation.UpdateProgressWithCancellationCheck("Creating CSV");
                string outputFile = Path.Combine(
                    saveDirectory.FullName,
                    $"{fName}.csv"
                );
                string splitString = "";
                if (operation.LogLocation != null) { splitString = ","; }
                operation.LogLocation += splitString + outputFile;
                using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs, Encoding.GetEncoding(1252)))
                {
                    var builder = new CSVBuilder(sw, ",", log, uploadresult);
                    builder.CreateCSV();
                }
                operation.UpdateProgressWithCancellationCheck("CSV created");
            }
            if (Properties.Settings.Default.SaveOutJSON || Properties.Settings.Default.SaveOutXML)
            {
                var builder = new RawFormatBuilder(log, uploadresult);
                if (Properties.Settings.Default.SaveOutJSON)
                {
                    operation.UpdateProgressWithCancellationCheck("Creating JSON");
                    string outputFile = Path.Combine(
                        saveDirectory.FullName,
                        $"{fName}.json"
                    );
                    string splitString = "";
                    if (operation.LogLocation != null) { splitString = ","; }
                    operation.LogLocation += splitString + saveDirectory.FullName;
                    Stream str;
                    if (Properties.Settings.Default.CompressRaw)
                    {
                        str = new MemoryStream();
                    }
                    else
                    {
                        str = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                    }
                    using (var sw = new StreamWriter(str, GeneralHelper.NoBOMEncodingUTF8))
                    {
                        builder.CreateJSON(sw, Properties.Settings.Default.IndentJSON);
                    }
                    if (str is MemoryStream msr)
                    {
                        CompressFile(outputFile, msr);
                        operation.UpdateProgressWithCancellationCheck("JSON compressed");
                    }
                    operation.UpdateProgressWithCancellationCheck("JSON created");
                }
                if (Properties.Settings.Default.SaveOutXML)
                {
                    operation.UpdateProgressWithCancellationCheck("Creating XML");
                    string outputFile = Path.Combine(
                        saveDirectory.FullName,
                        $"{fName}.xml"
                    );
                    string splitString = "";
                    if (operation.LogLocation != null) { splitString = ","; }
                    operation.LogLocation += splitString + saveDirectory.FullName;
                    Stream str;
                    if (Properties.Settings.Default.CompressRaw)
                    {
                        str = new MemoryStream();
                    }
                    else
                    {
                        str = new FileStream(outputFile, FileMode.Create, FileAccess.Write);
                    }
                    using (var sw = new StreamWriter(str, GeneralHelper.NoBOMEncodingUTF8))
                    {
                        builder.CreateXML(sw, Properties.Settings.Default.IndentXML);
                    }
                    if (str is MemoryStream msr)
                    {
                        CompressFile(outputFile, msr);
                        operation.UpdateProgressWithCancellationCheck("XML compressed");
                    }
                    operation.UpdateProgressWithCancellationCheck("XML created");
                }
            }
            operation.UpdateProgress($"Completed parsing for {result}ed {log.FightData.Logic.Extension}");
        }

    }
}
