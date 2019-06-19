using LuckParser.Exceptions;
using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static LuckParser.Parser.ParseEnum.TrashIDS;
using LuckParser.Controllers;
using System.IO;
using System.Threading;
using LuckParser.Builders;

namespace LuckParser
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

        private readonly static HashSet<string> _compressedFiles = new HashSet<string>()
        {
            ".zevtc",
            ".evtc.zip",
        };

        private readonly static HashSet<string> _tmpFiles = new HashSet<string>()
        {
            ".tmp.zip"
        };

        private readonly static HashSet<string> _supportedFiles = new HashSet<string>(_compressedFiles)
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
            UploadController up_controller = new UploadController();
            FileInfo fInfo = new FileInfo(row.Location);
            try
            {
                if (!fInfo.Exists)
                {
                    throw new FileNotFoundException("File " + fInfo.FullName + " does not exist");
                }
                ParsingController control = new ParsingController();

                if (!HasFormat())
                {
                    throw new InvalidDataException("No output format has been selected");
                }

                if (IsSupportedFormat(fInfo.Name))
                {
                    //Process evtc here
                    ParsedLog log = control.ParseLog(row, fInfo.FullName);
                    string[] uploadresult = up_controller.UploadOperation(row, fInfo);
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
                    var builder = new HTMLBuilder(log, uploadresult);
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
                using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8))
                {
                    var builder = new RawFormatBuilder(log, uploadresult);
                    builder.CreateJSON(sw);
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
                using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var sw = new StreamWriter(fs, GeneralHelper.NoBOMEncodingUTF8))
                {
                    var builder = new RawFormatBuilder(log, uploadresult);
                    builder.CreateXML(sw);
                }
            }
            rowData.BgWorker.ThrowIfCanceled(rowData);
            rowData.BgWorker.UpdateProgress(rowData, $"100% - Complete_{log.FightData.Logic.Extension}_{result}", 100);
        }

    }
}
