using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public abstract class AbstractBoon
    {

        // Fields
        protected List<int> boon_stack = new List<int>();
        protected int capacity;

        // Constructor
        public AbstractBoon(int capacity)
        {
            this.capacity = capacity;
        }

        // Abstract Methods
        public abstract int getStackValue();

        public abstract void update(int time_passed);

        public abstract void addStacksBetween(List<int> boon_stacks, int time_between);

        // Public Methods
        public void add(int boon_duration)
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