using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace GW2EIUtils
{

    public class OperationTracer
    {
        /// <summary>
        /// Status of the parse operation
        /// </summary>
        public string Status { get; protected set; }

        protected List<string> StatusList { get; }

        public OperationTracer(string status)
        {
            Status = status;
            StatusList = new List<string>();
        }

        protected virtual void ThrowIfCanceled()
        {

        }

        public void WriteLogMessages(StreamWriter sw)
        {
            foreach (string str in StatusList)
            {
                sw.WriteLine(str);
            }
        }

        public virtual void UpdateProgressWithCancellationCheck(string status)
        {
            UpdateProgress(status);
            ThrowIfCanceled();
        }
        public void UpdateProgress(string status)
        {
            StatusList.Add(status);
        }

        public virtual void FinalizeStatus(string prefix)
        {
            Status = StatusList.LastOrDefault() ?? "";
            Console.WriteLine(prefix + $"{Status}" + Environment.NewLine);
        }
    }
}
