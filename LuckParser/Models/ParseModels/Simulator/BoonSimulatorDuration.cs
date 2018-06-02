using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class BoonSimulatorDuration : BoonSimulator
    {

        private List<BoonSimulationItemDuration> simulation = new List<BoonSimulationItemDuration>();
        // Constructor
        public BoonSimulatorDuration(int capacity) : base(capacity)
        {
            //super(capacity);
        }

        // Public Methods


        public override List<BoonSimulationItem> getSimulationResult()
        {
            return new List<BoonSimulationItem>(simulation);
        }

        public override void simulate(List<LogBoon> logs)
        {
            long t_curr = 0;
            long t_prev = 0;
            foreach (LogBoon log in logs)
            {
                t_curr = log.getTime();
                update(t_curr - t_prev);
                add(log.getValue(), log.getSrcInstid(), t_curr);
                t_prev = t_curr;
            }
            boon_stack.Clear();
        }

        public override void update(long time_passed)
        {
            if (boon_stack.Count() > 0)
            {
                BoonSimulationItem toAdd = new BoonSimulationItemDuration(boon_stack[0]);
                if (simulation.Count > 0)
                {
                    BoonSimulationItemDuration last = simulation.Last();
                    if (last.getEnd() > toAdd.getStart())
                    {
                        last.setEnd(toAdd.getStart());
                    }
                }
                simulation.Add(new BoonSimulationItemDuration(boon_stack[0]));
                boon_stack[0] = new BoonStackItem(boon_stack[0], time_passed, -time_passed);
                long diff = time_passed - Math.Abs(Math.Min(boon_stack[0].boon_duration, 0));
                for (int i = 1; i < boon_stack.Count(); i++)
                {
                    boon_stack[i] = new BoonStackItem(boon_stack[i], diff, 0);
                }
                if (boon_stack[0].boon_duration <= 0)
                {
                    // Spend leftover time
                    long leftover = Math.Abs(boon_stack[0].boon_duration);
                    boon_stack.RemoveAt(0);
                    update(leftover);
                }

            }
        }
        public override void trim(long fight_duration)
        {
            for (int i = simulation.Count - 1; i >= 0; i--)
            {
                BoonSimulationItemDuration data = simulation[i];
                if (data.getEnd() > fight_duration)
                {
                    data.setEnd(fight_duration);
                } else
                {
                    break;
                }
            }
        }

    }
}