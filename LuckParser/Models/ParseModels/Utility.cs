using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Utility
    {
        public static List<Point> mergeIntervals(List<Point> intervals)
        {

            if (intervals.Count() <= 1)
            {
                return intervals;
            }

            List<Point> merged = new List<Point>();
            int x = intervals[0].X;
            int y = intervals[0].Y;

            for (int i = 1; i < intervals.Count(); i++)
            {
                Point current = intervals[i];
                if (current.X <= y)
                {
                    y = Math.Max(current.Y, y);
                }
                else
                {
                    merged.Add(new Point(x, y));
                    x = current.X;
                    y = current.Y;
                }
            }

            merged.Add(new Point(x, y));

            return merged;
        }
    }
}