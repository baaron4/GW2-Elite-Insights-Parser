using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GW2EIEvtcParser {

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
        public static void IncrementValue<K>(this Dictionary<K, int> dict, in K key, int value) where K : notnull
        {
            dict[key] = dict.TryGetValue(key, out var old) ? old + value : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IncrementValue<K>(this Dictionary<K, int> dict, in K key) where K : notnull
        {
            dict[key] = dict.TryGetValue(key, out var old) ? old + 1 : 1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IncrementValue<K>(this Dictionary<K, long> dict, in K key, long value) where K : notnull
        {
            dict[key] = dict.TryGetValue(key, out var old) ? old + value : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IncrementValue<K>(this Dictionary<K, long> dict, in K key) where K : notnull
        {
            dict[key] = dict.TryGetValue(key, out var old) ? old + 1 : 1;
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
                public bool MoveNext() => this.Current != null  && this.moved++ == 0;

                public readonly void Reset() { }
                public readonly void Dispose() { }
            }
        }

        /// Reserves space at least 'count' additional elements in the list should they not fit already.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ReserveAdditional<T>(this List<T> list, int count)
        {
            if(list.Capacity < list.Count + count)
            {
                list.Capacity = (int)Math.Max(list.Capacity * 1.4, list.Count + count);
            }
        }

        //TODO(Rennorb) @cleanup @unstable
        public static Span<T> AsSpan<T>(this List<T> list)
        {
            var array = (T[])ListInternals<T>.ItemsField.GetValue(list);
            return array.AsSpan(0, list.Count);
        }

        static class ListInternals<T>
        {
            public static FieldInfo ItemsField;
            static ListInternals()
            {
                ItemsField = typeof(List<T>).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
                Debug.Assert(ItemsField != null);
            }
        }
    }
}

//TODO not needed in c#12?
namespace System.Diagnostics.CodeAnalysis
{
     /// <summary>Specifies that the method or property will ensure that the listed field and property members have not-null values.</summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class MemberNotNullAttribute : Attribute
    {
        /// <summary>Initializes the attribute with a field or property member.</summary>
        /// <param name="member">
        /// The field or property member that is promised to be not-null.
        /// </param>
        public MemberNotNullAttribute(string member) => Members = [member];
 
        /// <summary>Initializes the attribute with the list of field and property members.</summary>
        /// <param name="members">
        /// The list of field and property members that are promised to be not-null.
        /// </param>
        public MemberNotNullAttribute(params string[] members) => Members = members;
 
        /// <summary>Gets field or property member names.</summary>
        public string[] Members { get; }
    }
 

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class MemberNotNullWhenAttribute : Attribute
    {
        /// <summary>Initializes the attribute with the specified return value condition and a field or property member.</summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter will not be null.
        /// </param>
        /// <param name="member">
        /// The field or property member that is promised to be not-null.
        /// </param>
        public MemberNotNullWhenAttribute(bool returnValue, string member)
        {
            ReturnValue = returnValue;
            Members = new[] { member };
        }

        /// <summary>Initializes the attribute with the specified return value condition and list of field and property members.</summary>
        /// <param name="returnValue">
        /// The return value condition. If the method returns this value, the associated parameter will not be null.
        /// </param>
        /// <param name="members">
        /// The list of field and property members that are promised to be not-null.
        /// </param>
        public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
        {
            ReturnValue = returnValue;
            Members = members;
        }

        /// <summary>Gets the return value condition.</summary>
        public bool ReturnValue { get; }

        /// <summary>Gets field or property member names.</summary>
        public string[] Members { get; }
    }
}
