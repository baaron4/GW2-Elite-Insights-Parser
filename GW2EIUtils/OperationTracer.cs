using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GW2EIUtils
{

    public class OperationTracer
    {

        protected List<string> StatusList { get; }

        public OperationTracer()
        {
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
    }
}
