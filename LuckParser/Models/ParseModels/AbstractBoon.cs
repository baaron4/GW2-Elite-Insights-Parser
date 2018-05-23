using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractBoon
    {

        // Fields
        protected List<long> boon_stack = new List<long>();
        protected int capacity;

        // Constructor
        public AbstractBoon(int capacity)
        {
            this.capacity = capacity;
        }

        // Abstract Methods
        public abstract long getStackValue();

        public abstract void update(long time_passed);

        public abstract void addStacksBetween(List<long> boon_stacks, long time_between);

        // Public Methods
        public void add(long boon_duration)
        {
            // Find empty slot
            if (!isFull())
            {
                boon_stack.Add(boon_duration);
                sort();
            }
            // Replace lowest value
            else
            {
                int index = boon_stack.Count() - 1;
                if (boon_stack[index] < boon_duration)
                {
                    boon_stack[index] = boon_duration;
                    sort();
                }
            }
        }
         
        // Protected Methods
        protected bool isFull()
        {
            return boon_stack.Count() >= capacity;
        }

        protected void sort()
        {
            // Collections.sort(boon_stack, Collections.reverseOrder());
            boon_stack = boon_stack.OrderByDescending(x=>x).ToList();
        }
    }
}