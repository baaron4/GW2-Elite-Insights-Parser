using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser;

public static partial class ListExt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MaybeAdd<T>(this List<T> list, in T? optional) where T : struct
    {
        if(optional.HasValue)
        {
            list.Add(optional.Value);
        }
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void MaybeAdd<T>(this List<T> list, T? optional) where T : class
    {
        if(!Equals(optional, default))
        {
            list.Add(optional);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IReadOnlyList<V> GetValueOrEmpty<K, V>(this Dictionary<K, List<V>> dict, in K key) where K : notnull
    {
        //NOTE(Rennorb): [] internally calls to Array.Empty here, which is static on the type and doesn't need additional caching. 
        // Only one of those will be created per T, not one per call.
        return dict.TryGetValue(key, out var value) ? value : [ ];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<V> GetValueOrEmpty<K, V, L>(this Dictionary<K, L> dict, in K key) where K : notnull where L : class, IEnumerable<K>
    {
        //NOTE(Rennorb): [] internally calls to Array.Empty here, which is static on the type and doesn't need additional caching. 
        // Only one of those will be created per T, not one per call.
        return dict.TryGetValue(key, out var value) ? (IEnumerable<V>)value : [ ];
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void AddToList<K, V>(this Dictionary<K, List<V>> dict, in K key, in V value, int initialCapacity = 1) where K : notnull
    {
        if (!dict.TryGetValue(key, out var list))
        {
            dict.Add(key, list = new(Math.Max(1, initialCapacity)));
        }

        list.Add(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IncrementValue<K>(this Dictionary<K, int> dict, in K key, int value = 1) where K : notnull
    {
        dict[key] = dict.TryGetValue(key, out var old) ? old + value : value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void IncrementValue<K>(this Dictionary<K, long> dict, in K key, long value = 1) where K : notnull
    {
        dict[key] = dict.TryGetValue(key, out var old) ? old + value : value;
    }

    public static T? FirstOrNull<T>(this IEnumerable<T> enumerable) where T : struct
    {
        var e = enumerable.GetEnumerator();
        return e.MoveNext() ? e.Current : null;
    }

    public delegate bool FoNPredicate<T>(in T element);
    public static T? FirstOrNull<T>(this IEnumerable<T> enumerable, FoNPredicate<T> predicate) where T : struct
    {
        var e = enumerable.GetEnumerator();
        while(e.MoveNext())
        {
            var v = e.Current;
            if(predicate(in v)) { return v; }
        }
        return null;
    }
    public static T? FirstOrNull<T>(this IReadOnlyList<T> list, FoNPredicate<T> predicate) where T : struct
    {
        for(int i = 0; i < list.Count; i++)
        {
            var v = list[i];
            if(predicate(in v)) { return v; }
        }
        return null;
    }
    public static T? LastOrNull<T>(this IReadOnlyList<T> list, FoNPredicate<T> predicate) where T : struct
    {
        for(int i = list.Count - 1; i >= 0; i--)
        {
            var v = list[i];
            if(predicate(in v)) { return v; }
        }
        return null;
    }

    public static T? LastOrNull<T>(this IReadOnlyList<T> list) where T : struct
    {
        return list.Count == 0 ? null : list[^1];
    }

    public static MaybeEnumerator<T> ToEnumerable<T>(this T? maybeValue) where T : class => new(maybeValue);

    public readonly struct MaybeEnumerator<T>(T? maybeValue) : IEnumerable<T> where T : class
    {
        readonly T? maybeValue = maybeValue;

        public readonly IEnumerator<T> GetEnumerator() => new Enumerator(maybeValue);
        readonly IEnumerator IEnumerable.GetEnumerator() => new Enumerator(maybeValue);

        struct Enumerator(T? maybeValue) : IEnumerator<T>
        {
            readonly T? _maybeValue = maybeValue;
            public readonly T Current => _maybeValue!;

            readonly object IEnumerator.Current => _maybeValue!;

            int moved = 0;
            public bool MoveNext() => Current != null  && moved++ == 0;

            public readonly void Reset() { }
            public readonly void Dispose() { }
        }
    }

    /// <summary>Reserves space for at least 'count' additional elements in the list should they not fit already.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReserveAdditional<T>(this List<T> list, int count)
    {
        if(list.Capacity < list.Count + count)
        {
            list.Capacity = (int)Math.Max(list.Capacity * 1.4, list.Count + count);
        }
    }

    /// <summary>Reserves space for at least 'count' additional elements in the set should they not fit already.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReserveAdditional<T>(this HashSet<T> set, int count)
    {
        set.EnsureCapacity(set.Count + count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SortStable<T>(this Span<T> span, Func<T, T, int> cmp)
    {
        StableSort<T>.fluxsort(span, cmp);
        if (!typeof(T).IsValueType)
        {
            ClearableSharedArrayPool<T>.Shared.ClearAllThreadLocal(); // make sure to not mess with other threads pools
#if VALIDATE_SORT_STABLE
            // TODO(Eliphas): there is a nasty bug in this sort algorithm sometimes same elements are inserted multiple times in the span, overriding existing values and effectively deleting them
            // I'm forcing in StableSort for the algo to fall into quadsort and tail_swap immediately, seems to work fine, although less efficient, better than OrderBy.
            var encounteredElements = new HashSet<T>();
            int dupes = 0;
            int badOrder = 0;
            T? prev = default;
            foreach (var element in span)
            {
                if (prev != null)
                {
                    if (cmp(prev, element) > 0)
                    {
                        badOrder++;
                    }
                }
                prev = element;
                if (encounteredElements.Contains(element))
                {
                    dupes++;
                    //throw new InvalidDataException("Stable sort has failed");
                }
                encounteredElements.Add(element);
            }
            if (badOrder > 0)
            {
                throw new InvalidDataException("Stable sort has failed: not properly ordered");
            }
            if (dupes > 0)
            {
                throw new InvalidDataException("Stable sort has failed: some elements have been duplicated");
            }
#endif
        }
    }

    //TODO(Rennorb) @cleanup @unstable
    public static Span<T> AsSpan<T>(this List<T> list)
    {
        var array = (T[])ListInternals<T>.ItemsField.GetValue(list)!;
        return array.AsSpan(0, list.Count);
    }

    static class ListInternals<T>
    {
        public static FieldInfo ItemsField;
        static ListInternals()
        {
            ItemsField = typeof(List<T>).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic)!;
            Debug.Assert(ItemsField != null);
        }
    }
}
