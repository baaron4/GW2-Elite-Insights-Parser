namespace LuckParser.Models.ParseModels
{
    public class PhaseData
    {
        public long start;
        public long end;

        public PhaseData(long start, long end)
        {
            this.start = start;
            this.end = end;
        }

        public long getDuration(string format)
        {
            switch (format)
            {
                case "m":
                    return (end - start) / 60000;
                case "s":
                    return (end - start) / 1000;
                case "ms":
                default:
                    return (end - start) / 1000;
            }

        }

        public bool inInterval(long time, long offset = 0)
        {
            return start <= time - offset && time - offset <= end;
        }
    }
}
