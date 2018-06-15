using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LuckParser
{
    public class GridRow
    {
        public const string STATUS_PARSE = "Parse";
        public const string STATUS_CANCEL = "Cancel";
        public const string STATUS_OPEN = "Open";

        public string Location { get; set; }
        public string Status { get; set; }
        public string ButtonState { get; set; }
        public BackgroundWorker BgWorker { get; set; }

        public GridRow(string location, string status)
        {
            Location = location;
            Status = status;
            ButtonState = "Parse";
        }
    }
}
