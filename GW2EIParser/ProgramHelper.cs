using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using GW2EIParser.Builders;
using GW2EIParser.Controllers;
using GW2EIParser.Exceptions;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser
{
    public static class ProgramHelper
    {
        /// <summary>
        /// Reports a status update for a log, updating the background worker and the related row with the new status
        /// </summary>
        /// <param name="bg"></param>
        /// <param name="row"></param>
        /// <param name="status"></param>
        /// <param name="percent"></param>
        public static void UpdateProgress(this BackgroundWorker bg, GridRow row, string status, int percent)
        {
            row.Status = status;
            bg.ReportProgress(percent, row);
            Console.WriteLine($"{row.Location}: {status}" + Environment.NewLine);
        }

        private static bool HasFormat()
        {
            return Properties.Settings.Default.SaveOutCSV || Properties.Settings.Default.SaveOutHTML || Properties.Settings.Default.SaveOutXML || Properties.Settings.Default.SaveOutJSON;
        }

        /// <summary>
        /// Throws a <see cref="CancellationException"/> if the background worker has been cancelled
        /// </summary>
        /// <param name="bg"></param>
        /// <param name="row"></param>
        /// <param name="cancelStatus"></param>
        public static void ThrowIfCanceled(this BackgroundWorker bg, GridRow row, string cancelStatus = "Canceled")
        {
            if (bg.CancellationPending)
            {
                row.Status = cancelStatus;
                throw new CancellationException(row);

            }
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

        public static void DoWork(GridRow row)
        {
            System.Globalization.CultureInfo before = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture =
                    new System.Globalization.CultureInfo("en-US");
            var fInfo = new FileInfo(row.Location);
            try
            {
                if (!fInfo.Exists)
                {
                    throw new FileNotFoundException("File " + fInfo.FullName + " does not exist");
                }
                var control = new ParsingController(Properties.Settings.Default.Anonymous, Properties.Settings.Default.SkipFailedTries, Properties.Settings.Default.ParsePhases);

                if (!HasFormat())
                {
                    throw new InvalidDataException("No output format has been selected");
                }

                if (IsSupportedFormat(fInfo.Name))
                {
                    //Process evtc here
                    ParsedLog log = control.ParseLog(row, fInfo.FullName);
                    string[] uploadresult = UploadController.UploadOperation(row, fInfo);
                    //Creating File
                    GenerateFiles(log, row, uploadresult, fInfo);
                }
                else
                {
                    row.BgWorker.UpdateProgress(row, "Not EVTC", 100);
                    throw new InvalidDataException("Not EVTC");
                }
            }
            catch (Exception ex)
            {
                throw new CancellationException(row, ex);
            }
            finally
            {
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

        private static void GenerateFiles(ParsedLog log, GridRow rowData, string[] uploadresult, FileInfo fInfo)
        {
            rowData.BgWorker.ThrowIfCanceled(rowData);
            rowData.BgWorker.UpdateProgress(rowData, "50% - Creating File(s)...", 50);
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
                throw new InvalidDataException("Save Directory not found");
            }

            string result = log.FightData.Success ? "kill" : "fail";
            string encounterLengthTerm = Properties.Settings.Default.AddDuration ? "_" + (log.FightData.FightDuration / 1000).ToString() + "s" : "";
            string PoVClassTerm = Properties.Settings.Default.AddPoVProf ? "_" + log.LogData.PoV.Prof.ToLower() : "";
            string fName = fInfo.Name.Split('.')[0];
            fName = $"{fName}{PoVClassTerm}_{log.FightData.Logic.Extension}{encounterLengthTerm}_{result}";

            rowData.BgWorker.ThrowIfCanceled(rowData);
            if (Properties.Settings.Default.SaveOutHTML)
            {
                string outputFile = Path.Combine(
                saveDirectory.FullName,
                $"{fName}.html"
                );
                rowData.LogLocation = outputFile;
                using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    var builder = new HTMLBuilder(log, uploadresult, Properties.Settings.Default.ParseCombatReplay, Properties.Settings.Default.LightTheme, Properties.Settings.Default.HtmlExternalScripts);
                    builder.CreateHTML(sw, saveDirectory.FullName);
                }
            }
            rowData.BgWorker.ThrowIfCanceled(rowData);
            if (Properties.Settings.Default.SaveOutCSV)
            {
                string outputFile = Path.Combine(
                    saveDirectory.FullName,
                    $"{fName}.csv"
                );
                string splitString = "";
                if (rowData.LogLocation != null) { splitString = ","; }
                rowData.LogLocation += splitString + outputFile;
                using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs, Encoding.GetEncoding(1252)))
                {
                    var builder = new CSVBuilder(sw, ",", log, uploadresult);
                    builder.CreateCSV();
                }
            }
            rowData.BgWorker.ThrowIfCanceled(rowData);
            if (Properties.Settings.Default.SaveOutJSON)
            {
                string outputFile = Path.Combine(
                    saveDirectory.FullName,
                    $"{fName}.json"
                );
                string splitString = "";
                if (rowData.LogLocation != null) { splitString = ","; }
                rowData.LogLocation += splitString + saveDirectory.FullName;
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
                    var builder = new RawFormatBuilder(log, uploadresult);
                    builder.CreateJSON(sw, Properties.Settings.Default.IndentJSON);
                }
                if (str is MemoryStream msr)
                {
                    CompressFile(outputFile, msr);
                }
            }
            rowData.BgWorker.ThrowIfCanceled(rowData);
            if (Properties.Settings.Default.SaveOutXML)
            {
                string outputFile = Path.Combine(
                    saveDirectory.FullName,
                    $"{fName}.xml"
                );
                string splitString = "";
                if (rowData.LogLocation != null) { splitString = ","; }
                rowData.LogLocation += splitString + saveDirectory.FullName;
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
                    var builder = new RawFormatBuilder(log, uploadresult);
                    builder.CreateXML(sw, Properties.Settings.Default.IndentXML);
                }
                if (str is MemoryStream msr)
                {
                    CompressFile(outputFile, msr);
                }
            }
            rowData.BgWorker.ThrowIfCanceled(rowData);
            rowData.BgWorker.UpdateProgress(rowData, $"100% - Complete_{log.FightData.Logic.Extension}_{result}", 100);
        }

    }
}
