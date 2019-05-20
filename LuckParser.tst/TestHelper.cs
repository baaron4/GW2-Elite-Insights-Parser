using LuckParser.Builders;
using LuckParser.Exceptions;
using LuckParser.Models.JsonModels;
using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.tst
{
    public class TestHelper
    {

        public static ParsedLog ParseLog(string location)
        {
            ParsingController parser = new ParsingController();

            GridRow row = new GridRow(location as string, "Ready to parse")
            {
                BgWorker = new System.ComponentModel.BackgroundWorker()
                {
                    WorkerReportsProgress = true
                }
            };
            row.Metadata.FromConsole = true;


            FileInfo fInfo = new FileInfo(row.Location);
            if (!fInfo.Exists)
            {
                throw new CancellationException(row, new FileNotFoundException("File does not exist", fInfo.FullName));
            }
            if (!GeneralHelper.IsSupportedFormat(fInfo.Name))
            {
                throw new CancellationException(row, new InvalidDataException("Not EVTC"));
            }

            return parser.ParseLog(row, fInfo.FullName);
        }

        public string JsonString(ParsedLog log)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            RawFormatBuilder builder = new RawFormatBuilder(sw, log, null);

            builder.CreateJSON();

            return sw.ToString();
        }

        public JsonLog JsonLog(ParsedLog log)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            RawFormatBuilder builder = new RawFormatBuilder(sw, log, null);

            return builder.CreateJsonLog();
        }
    }
}
