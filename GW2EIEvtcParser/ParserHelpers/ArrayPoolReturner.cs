using System.Buffers;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace GW2EIEvtcParser.ParserHelpers;


#pragma warning disable CA2002 // weak identity lock

internal struct ArrayPoolReturner<T> : IDisposable
{
    public readonly ArrayPool<T> Pool;
    public readonly T[] Array;
    public int Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayPoolReturner(int length) : this(length, ArrayPool<T>.Shared) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayPoolReturner(int length, ArrayPool<T> pool)
    {
        Tracing.Trace.TrackAverageStat($"PooledArraySize<{typeof(T)}>", length);
        this.Pool   = pool;
        this.Length = length;
        this.Array  = pool.Rent(length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> AsSpan() => Array.AsSpan(0, Length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<T>(in ArrayPoolReturner<T> _this) => _this.Array.AsSpan(0, _this.Length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<T>(in ArrayPoolReturner<T> _this) => (ReadOnlySpan<T>)_this.Array.AsSpan(0, _this.Length);
    public readonly Span<T> this[Range range] => Array.AsSpan(0, Length)[range];
    public readonly ref T this[int index] => ref Array[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly void IDisposable.Dispose() => Pool.Return(this.Array);
}

//NOTE(Rennorb): Code below is modified from the official array pool. 
// It adds one specific feature we want, which is the ability to clear buffers at a specific point in time.
// We do want to clear them out after using them, because otherwise we are holding a lot of objects alive in the pools.
// The default interface for array pools only allows clearing them when returning,
// but since we reuse them hundred thousands of times before we need to clear the buffers this would be extremely dumb.
// I also ripped out the event logging, no need for that.
// This is only relevant for reference types, so use the normal one for value types.

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/// <summary>
/// Provides an ArrayPool implementation meant to be used as the singleton returned from ArrayPool.Shared.
/// </summary>
/// <remarks>
/// The implementation uses a tiered caching scheme, with a small per-thread cache for each array size, followed
/// by a cache per array size shared by all threads, split into per-core stacks meant to be used by threads
/// running on that core.  Locks are used to protect each per-core stack, because a thread can migrate after
/// checking its processor number, because multiple threads could interleave on the same core, and because
/// a thread is allowed to check other core's buckets if its core's bucket is empty/full.
/// </remarks>
public sealed class ClearableSharedArrayPool<T> : ArrayPool<T> // where T : class this only makes sense on classes, but the restriction is annyoing to use with fluxsort
{
    public static new readonly ClearableSharedArrayPool<T> Shared = new();

    /// <summary>The number of buckets (array sizes) in the pool, one for each array length, starting from length 16.</summary>
    private const int NumBuckets = 27; // Utilities.SelectBucketIndex(1024 * 1024 * 1024 + 1)

    /// <summary>A per-thread array of arrays, to cache one array per array size per thread.</summary>
    [ThreadStatic]
    private static SharedArrayPoolThreadLocalArray[]? t_tlsBuckets;
    /// <summary>Used to keep track of all thread local buckets for trimming if needed.</summary>
    private readonly ConditionalWeakTable<SharedArrayPoolThreadLocalArray[], object?> _allTlsBuckets = new();
    /// <summary>
    /// An array of per-core partitions. The slots are lazily initialized to avoid creating
    /// lots of overhead for unused array sizes.
    /// </summary>
    private readonly SharedArrayPoolPartitions?[] _buckets = new SharedArrayPoolPartitions[NumBuckets];

    /// <summary>Allocate a new <see cref="SharedArrayPoolPartitions"/> and try to store it into the <see cref="_buckets"/> array.</summary>
    private unsafe SharedArrayPoolPartitions CreatePerCorePartitions(int bucketIndex)
    {
        var inst = new SharedArrayPoolPartitions();
        return Interlocked.CompareExchange(ref _buckets[bucketIndex], inst, null) ?? inst;
    }

    public override T[] Rent(int minimumLength)
    {
        T[]? buffer;

        // Get the bucket number for the array length. The result may be out of range of buckets,
        // either for too large a value or for 0 and negative values.
        int bucketIndex = SelectBucketIndex(minimumLength);

        // First, try to get an array from TLS if possible.
        SharedArrayPoolThreadLocalArray[]? tlsBuckets = t_tlsBuckets;
        if (tlsBuckets is not null && (uint)bucketIndex < (uint)tlsBuckets.Length)
        {
            buffer = Unsafe.As<T[]?>(tlsBuckets[bucketIndex].Array);
            if (buffer is not null)
            {
                tlsBuckets[bucketIndex].Array = null;
                return buffer;
            }
        }

        // Next, try to get an array from one of the partitions.
        SharedArrayPoolPartitions?[] perCoreBuckets = _buckets;
        if ((uint)bucketIndex < (uint)perCoreBuckets.Length)
        {
            SharedArrayPoolPartitions? b = perCoreBuckets[bucketIndex];
            if (b is not null)
            {
                buffer = Unsafe.As<T[]?>(b.TryPop());
                if (buffer is not null)
                {
                    return buffer;
                }
            }

            // No buffer available.  Ensure the length we'll allocate matches that of a bucket
            // so we can later return it.
            minimumLength = GetMaxSizeForBucket(bucketIndex);
        }
        else if (minimumLength == 0)
        {
            // We allow requesting zero-length arrays (even though pooling such an array isn't valuable)
            // as it's a valid length array, and we want the pool to be usable in general instead of using
            // `new`, even for computed lengths. But, there's no need to log the empty array.  Our pool is
            // effectively infinite for empty arrays and we'll never allocate for rents and never store for returns.
            return Array.Empty<T>();
        }
        else if(minimumLength < 0)
        {
            throw new ArgumentException($"minimumLength should not be negative, was {minimumLength}");
        }

        buffer = new T[minimumLength]; //TODO_PERF(Rennorb): skip initialization
        return buffer;
    }

    public override void Return(T[] array, bool clearArray = false)
    {
        if (array is null)
        {
            throw new ArgumentException($"array should not be null");
        }

        // Determine with what bucket this array length is associated
        int bucketIndex = SelectBucketIndex(array.Length);

        // Make sure our TLS buckets are initialized.  Technically we could avoid doing
        // this if the array being returned is erroneous or too large for the pool, but the
        // former condition is an error we don't need to optimize for, and the latter is incredibly
        // rare, given a max size of 1B elements.
        SharedArrayPoolThreadLocalArray[] tlsBuckets = t_tlsBuckets ?? InitializeTlsBuckets();

        if ((uint)bucketIndex < (uint)tlsBuckets.Length)
        {
            // Clear the array if the user requested it.
            if (clearArray)
            {
                Array.Clear(array, 0, array.Length);
            }

            // Check to see if the buffer is the correct size for this bucket.
            if (array.Length != GetMaxSizeForBucket(bucketIndex))
            {
                throw new ArgumentException("This array was not rented from this pool.");
            }

            // Store the array into the TLS bucket.  If there's already an array in it,
            // push that array down into the partitions, preferring to keep the latest
            // one in TLS for better locality.
            ref SharedArrayPoolThreadLocalArray tla = ref tlsBuckets[bucketIndex];
            Array? prev = tla.Array;
            tla = new SharedArrayPoolThreadLocalArray(array) { KnownToBeCleared = clearArray };
            if (prev is not null)
            {
                SharedArrayPoolPartitions partitionsForArraySize = _buckets[bucketIndex] ?? CreatePerCorePartitions(bucketIndex);
                partitionsForArraySize.TryPush(prev);
            }
        }
    }
    
    /// <summary> Clears all arrays in the pool. Noticeably does not destroy the arrays, only clear any remaining references they hold. </summary>
    public void ClearAll()
    {
        foreach (var tlsBuckets in _allTlsBuckets)
        {
            var buckets = tlsBuckets.Key;
            for (int i = 0; i < buckets.Length; i++)
            {
                ref var bucket = ref buckets[i];
                if (bucket.Array == null || bucket.KnownToBeCleared)
                {
                    continue;
                }

                Array.Clear(bucket.Array, 0, bucket.Array.Length);
                bucket.KnownToBeCleared = true;
            }
        }

        foreach(var bucket in _buckets)
        {
            bucket?.Clear();
        }
    }

    /// <summary>
    /// Clears all arrays in the pool. Noticeably does not destroy the arrays, only clear any remaining references they hold.\n
    /// This only clears arrays associated wit the current thread and does not touch anything that might be shared.
    /// </summary>
    /// <remarks>While this is threadsafe it is not Task-safe, as multiple tasks might run on the same thread.</remarks>
    public void ClearAllThreadLocal()
    {
        if (t_tlsBuckets == null)
        {
            return;
        }

        for (int i = 0; i < t_tlsBuckets.Length; i++)
        {
            ref var bucket = ref t_tlsBuckets[i];
            if (bucket.Array == null || bucket.KnownToBeCleared)
            {
                continue;
            }

            Array.Clear(bucket.Array, 0, bucket.Array.Length);
            bucket.KnownToBeCleared = true;
        }
    }

    private SharedArrayPoolThreadLocalArray[] InitializeTlsBuckets()
    {
        Debug.Assert(t_tlsBuckets is null, $"Non-null {nameof(t_tlsBuckets)}");

        var tlsBuckets = new SharedArrayPoolThreadLocalArray[NumBuckets];
        t_tlsBuckets = tlsBuckets;

        _allTlsBuckets.Add(tlsBuckets, null);

        return tlsBuckets;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int SelectBucketIndex(int bufferSize)
    {
        // Buffers are bucketed so that a request between 2^(n-1) + 1 and 2^n is given a buffer of 2^n
        // Bucket index is log2(bufferSize - 1) with the exception that buffers between 1 and 16 bytes
        // are combined, and the index is slid down by 3 to compensate.
        // Zero is a valid bufferSize, and it is assigned the highest bucket index so that zero-length
        // buffers are not retained by the pool. The pool will return the Array.Empty singleton for these.
        return BitOperations.Log2((uint)bufferSize - 1 | 15) - 3;
    }

     [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int GetMaxSizeForBucket(int binIndex)
    {
        int maxSize = 16 << binIndex;
        Debug.Assert(maxSize >= 0);
        return maxSize;
    }
}

// The following partition types are separated out of SharedArrayPool<T> to avoid
// them being generic, in order to avoid the generic code size increase that would
// result, in particular for Native AOT. The only thing that's necessary to actually
// be generic is the return type of TryPop, and we can handle that at the access
// site with a well-placed Unsafe.As.

/// <summary>Wrapper for arrays stored in ThreadStatic buckets.</summary>
internal struct SharedArrayPoolThreadLocalArray(Array array)
{
    /// <summary>The stored array.</summary>
    public Array? Array = array;
    /// <summary>Environment.TickCount timestamp for when this array was observed by Trim.</summary>
    public int MillisecondsTimeStamp = 0;
    public bool KnownToBeCleared = false;
}

/// <summary>Provides a collection of partitions, each of which is a pool of arrays.</summary>
internal sealed class SharedArrayPoolPartitions
{
    /// <summary>The partitions.</summary>
    private readonly Partition[] _partitions;

    /// <summary>Initializes the partitions.</summary>
    public SharedArrayPoolPartitions()
    {
        // Create the partitions.  We create as many as there are processors, limited by our max.
        var partitions = new Partition[SharedArrayPoolStatics.s_partitionCount];
        for (int i = 0; i < partitions.Length; i++)
        {
            partitions[i] = new Partition();
        }
        _partitions = partitions;
    }

    /// <summary>
    /// Try to push the array into any partition with available space, starting with partition associated with the current core.
    /// If all partitions are full, the array will be dropped.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryPush(Array array)
    {
        // Try to push on to the associated partition first.  If that fails,
        // round-robin through the other partitions.
        Partition[] partitions = _partitions;
        int index = (int)((uint)Thread.GetCurrentProcessorId() % (uint)SharedArrayPoolStatics.s_partitionCount); // mod by constant in tier 1
        for (int i = 0; i < partitions.Length; i++)
        {
            if (partitions[index].TryPush(array)) { return true; }
            if (++index == partitions.Length) { index = 0; }
        }

        return false;
    }

    /// <summary>
    /// Try to pop an array from any partition with available arrays, starting with partition associated with the current core.
    /// If all partitions are empty, null is returned.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Array? TryPop()
    {
        // Try to pop from the associated partition first.  If that fails, round-robin through the other partitions.
        Array? arr;
        Partition[] partitions = _partitions;
        int index = (int)((uint)Thread.GetCurrentProcessorId() % (uint)SharedArrayPoolStatics.s_partitionCount); // mod by constant in tier 1
        for (int i = 0; i < partitions.Length; i++)
        {
            if ((arr = partitions[index].TryPop()) is not null) { return arr; }
            if (++index == partitions.Length) { index = 0; }
        }
        return null;
    }

    public void Clear()
    {
        Partition[] partitions = _partitions;
        for (int i = 0; i < partitions.Length; i++)
        {
            partitions[i].Clear();
        }
    }

    /// <summary>Provides a simple, bounded stack of arrays, protected by a lock.</summary>
    private sealed class Partition
    {
        /// <summary>The arrays in the partition.</summary>
        private readonly Array?[] _arrays = new Array[SharedArrayPoolStatics.s_maxArraysPerPartition];
        /// <summary>Number of arrays stored in <see cref="_arrays"/>.</summary>
        private int _count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPush(Array array)
        {
            bool enqueued = false;
            Monitor.Enter(this);
            Array?[] arrays = _arrays;
            int count = _count;
            if ((uint)count < (uint)arrays.Length)
            {
                arrays[count] = array;
                _count = count + 1;
                enqueued = true;
            }
            Monitor.Exit(this);
            return enqueued;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Array? TryPop()
        {
            Array? arr = null;
            Monitor.Enter(this);
            Array?[] arrays = _arrays;
            int count = _count - 1;
            if ((uint)count < (uint)arrays.Length)
            {
                arr = arrays[count];
                arrays[count] = null;
                _count = count;
            }
            Monitor.Exit(this);
            return arr;
        }
        public void Clear()
        {
            if(_count == 0) {  return; }

            lock(this)
            {
                foreach(var arr in _arrays)
                {
                    if(arr == null) { continue; }

                    Array.Clear(arr, 0, arr.Length);
                }
            }
        }
    }
}

internal static class SharedArrayPoolStatics
{
    /// <summary>Number of partitions to employ.</summary>
    internal static readonly int s_partitionCount = GetPartitionCount();
    /// <summary>The maximum number of arrays per array size to store per partition.</summary>
    internal static readonly int s_maxArraysPerPartition = GetMaxArraysPerPartition();

    /// <summary>Gets the maximum number of partitions to shard arrays into.</summary>
    /// <remarks>Defaults to int.MaxValue.  Whatever value is returned will end up being clamped to <see cref="Environment.ProcessorCount"/>.</remarks>
    private static int GetPartitionCount()
    {
        int partitionCount = TryGetInt32EnvironmentVariable("DOTNET_SYSTEM_BUFFERS_SHAREDARRAYPOOL_MAXPARTITIONCOUNT", out int result) && result > 0 ?
            result :
            int.MaxValue; // no limit other than processor count
        return Math.Min(partitionCount, Environment.ProcessorCount);
    }

    /// <summary>Gets the maximum number of arrays of a given size allowed to be cached per partition.</summary>
    /// <returns>Defaults to 32. This does not factor in or impact the number of arrays cached per thread in TLS (currently only 1).</returns>
    private static int GetMaxArraysPerPartition()
    {
        return TryGetInt32EnvironmentVariable("DOTNET_SYSTEM_BUFFERS_SHAREDARRAYPOOL_MAXARRAYSPERPARTITION", out int result) && result > 0 ?
            result :
            32; // arbitrary limit
    }

    /// <summary>Look up an environment variable and try to parse it as an Int32.</summary>
    /// <remarks>This avoids using anything that might in turn recursively use the ArrayPool.</remarks>
    private static bool TryGetInt32EnvironmentVariable(string variable, out int result)
    {
        // Avoid globalization stack, as it might in turn be using ArrayPool.

        if (Environment.GetEnvironmentVariable(variable) is string envVar &&
            envVar.Length is > 0 and <= 32) // arbitrary limit that allows for some spaces around the maximum length of a non-negative Int32 (10 digits)
        {
            ReadOnlySpan<char> value = envVar.AsSpan().Trim(' ');
            if (!value.IsEmpty && value.Length <= 10)
            {
                long tempResult = 0;
                foreach (char c in value)
                {
                    uint digit = (uint)(c - '0');
                    if (digit > 9)
                    {
                        goto Fail;
                    }

                    tempResult = tempResult * 10 + digit;
                }

                if (tempResult is >= 0 and <= int.MaxValue)
                {
                    result = (int)tempResult;
                    return true;
                }
            }
        }

    Fail:
        result = 0;
        return false;
    }
}
#pragma warning restore CA2002 // weak identity lock
