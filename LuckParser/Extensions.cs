using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser
{
    public static class Extensions
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
            if (row.Metadata.FromConsole)
            {
                Console.WriteLine($"{row.Location}: {status}");
            }
        }

        /// <summary>
        /// Throws a <see cref="CancellationException"/> if the background worker has been cancelled
        /// </summary>
        /// <param name="bg"></param>
        /// <param name="row"></param>
        /// <param name="cancelStatus"></param>
        public static void ThrowIfCanceled(this BackgroundWorker bg, GridRow row, string cancelStatus)
        {
            if (bg.CancellationPending)
            {
                row.Status = cancelStatus;
                throw new CancellationException(row);
            }
        }
    }
}
