using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GW2EIEvtcParser.ParsedData.tst;

class Internals
{
    public class TestEvent(long t, int order) : AbstractTimeCombatEvent(t)
    {
        public int Order = order;

        public override string ToString() => $"({Time} {Order})";
    }

    [Test]
    public void StableSortByTime_AbstractTimeCombatEvent()
    {
        //NOTE(Rennorb): Robustly testing this is actually quite hard, as the default implementation will still work for a bunch of cases.
        // But this should at least catch obvious issues.
        var list = new List<TestEvent>() {
            new(1, 3),
            new(0, 1),
            new(1, 4),
            new(0, 2),
        };

        list.SortByTime();

        for(int i = 0; i < list.Count - 1; i++)
        {
            var a = list[i];
            var b = list[i + 1];
            Assert.That(a.Time <= b.Time, "SortByTime must sort by time ascending");
            Assert.Less(a.Order, b.Order, "SortByTime must be a stable sort");
        }
    }


    class TestCombatItem(long t, int v) : CombatItem(t, default, default, v, default, default, default, default, default, default, default, 
            default, default, default, default, default, default, default, default, default, default, default, default, default)
    {
        public override string ToString()
        {
            return $"({Time} {Value})";
        }
    }

    [Test]
    public void StableSortByTime_CombatItem()
    {
        //NOTE(Rennorb): Robustly testing this is actually quite hard, as the default implementation will still work for a bunch of cases.
        // But this should at least catch obvious issues.
        var list = new List<TestCombatItem>() {
            new(1, 3),
            new(0, 1),
            new(1, 4),
            new(0, 2),
        };

        list.SortByTime();

        for(int i = 0; i < list.Count - 1; i++)
        {
            var a = list[i];
            var b = list[i + 1];
            Assert.That(a.Time <= b.Time, "SortByTime must sort by time ascending");
            Assert.Less(a.Value, b.Value, "SortByTime must be a stable sort");
        }
    }

#pragma warning disable CS0618 // Type or member is obsolete
    class TestCastEvent(long t, bool swap, int order) : AbstractCastEvent(t, new(swap), null!)
    {
        public int Order = order;

        public override string ToString()
        {
            return $"({Time} {Skill.IsSwap} {Order})";
        }
    }
#pragma warning restore CS0618 // Type or member is obsolete

    [Test]
    public void StableSortByTimeThenSwap()
    {
        //NOTE(Rennorb): Robustly testing this is actually quite hard, as the default implementation will still work for a bunch of cases.
        // But this should at least catch obvious issues.
        var list = new List<TestCastEvent>() {
            new(0, false, 0),
            new(1, false, 3),
            new(0, true , 1),
            new(1, true , 5),
            new(0, true , 2),
            new(1, false, 4),
        };

        list.SortByTimeThenSwap();

        for(int i = 0; i < list.Count - 1; i++)
        {
            var a = list[i];
            var b = list[i + 1];
            Assert.That(a.Time < b.Time || (a.Time == b.Time && Convert.ToInt32(a.Skill.IsSwap) <= Convert.ToInt32(b.Skill.IsSwap)), "SortByTimeThenSwap must sort by time ascending, then by isSwap");
            Assert.Less(a.Order, b.Order, "SortByTimeThenSwap must be a stable sort");
        }
    }

    [Test]
    public void StableSortByTimeThenNegatedSwap()
    {
        //NOTE(Rennorb): Robustly testing this is actually quite hard, as the default implementation will still work for a bunch of cases.
        // But this should at least catch obvious issues.
        var list = new List<TestCastEvent>() {
            new(0, true , 0),
            new(1, true , 3),
            new(0, true , 1),
            new(0, false, 2),
            new(1, false, 4),
            new(1, false, 5),
        };

        list.SortByTimeThenNegatedSwap();

        for(int i = 0; i < list.Count - 1; i++)
        {
            var a = list[i];
            var b = list[i + 1];
            Assert.That(a.Time < b.Time || (a.Time == b.Time && Convert.ToInt32(a.Skill.IsSwap) >= Convert.ToInt32(b.Skill.IsSwap)), "SortByTimeThenNegatedSwap must sort by time ascending, then by not isSwap");
            Assert.Less(a.Order, b.Order, $"SortByTimeThenNegatedSwap must be a stable sort. index {i} (+1)");
        }
    }
}


[TestFixtureSource(typeof(Sort_Generated), nameof(Sort_Generated.GenerateTests))]
class Sort_Generated
{
    public static IEnumerable GenerateTests
    {
        get
        {
            var r = new Random();
            var list = new List<Internals.TestEvent>(1000);
            for(int i = 0; i < 1000; i++)
            {
                list.Add(new(r.Next(), i));
                yield return new TestFixtureData(new List<Internals.TestEvent>(list)); // clone so every list has to be sorted
            }
        }
    }

    public class Order : IComparer
    {
        public static readonly Order Instance = new();
        public int Compare(object x, object y) => (int)(((Internals.TestEvent)x).Time - ((Internals.TestEvent)y).Time);
    }


    List<Internals.TestEvent> data;
    public Sort_Generated(List<Internals.TestEvent> data) => this.data = data;

    [Test]
    public void IsSorted()
    {
        data.SortByTime();

        CollectionAssert.IsOrdered(data, Order.Instance, $"{data.Count} failed");
    }
}
