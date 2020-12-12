using System.Collections.Generic;
using System.IO;

namespace GW2EIEvtcParser
{

    public abstract class ParserController
    {

        protected List<string> StatusList { get; }

        public ParserController()
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

        public virtual void Reset()
        {
            StatusList.Clear();
        }

        public virtual void UpdateProgressWithCancellationCheck(string status)
        {
            UpdateProgress(status);
            ThrowIfCanceled();
        }
        public virtual void UpdateProgress(string status)
        {
            StatusList.Add(status);
        }
    }
}
