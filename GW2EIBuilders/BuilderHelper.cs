using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GW2EIBuilders
{
    public static class BuilderHelper
    {
        internal static T MaxBy<T, TComparable>(this IEnumerable<T> en, Func<T, TComparable> evaluate) where TComparable : IComparable<TComparable>
        {
            return en.Select(t => (value: t, eval: evaluate(t)))
                .Aggregate((max, next) => next.eval.CompareTo(max.eval) > 0 ? next : max).value;
        }

        internal static T MinBy<T, TComparable>(this IEnumerable<T> en, Func<T, TComparable> evaluate) where TComparable : IComparable<TComparable>
        {
            return en.Select(t => (value: t, eval: evaluate(t)))
                .Aggregate((max, next) => next.eval.CompareTo(max.eval) < 0 ? next : max).value;
        }

        internal static int IndexOf<T>(this IReadOnlyList<T> self, T elementToFind)
        {
            int i = 0;
            foreach (T element in self)
            {
                if (Equals(element, elementToFind))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
    }
}
