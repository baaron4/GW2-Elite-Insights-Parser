namespace GW2EIEvtcParser.EIData
{
    public class Consumable
    {
        public Buff Buff { get; }
        public long Time { get; }
        public int Duration { get; }
        public int Stack { get; internal set; }

        public Consumable(Buff item, long time, int duration)
        {
            Buff = item;
            Time = time;
            Duration = duration;
            Stack = 1;
        }
    }
}
