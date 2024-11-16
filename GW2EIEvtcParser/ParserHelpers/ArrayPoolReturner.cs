using System.Buffers;
using System.Runtime.CompilerServices;

namespace GW2EIEvtcParser.ParserHelpers;
internal struct ArrayPoolReturner<T> : IDisposable
{
    public readonly T[] Array;
    public int Length;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayPoolReturner(int length)
    {
        this.Length = length;
        this.Array = ArrayPool<T>.Shared.Rent(length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<T> AsSpan() => this.Array.AsSpan(0, this.Length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<T>(in ArrayPoolReturner<T> _this) => _this.Array.AsSpan(0, _this.Length);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<T>(in ArrayPoolReturner<T> _this) => (ReadOnlySpan<T>)_this.Array.AsSpan(0, _this.Length);
    public readonly Span<T> this[Range range] => this.Array.AsSpan(0, this.Length)[range];
    public readonly ref T this[int index] => ref this.Array[index];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    readonly void IDisposable.Dispose() => ArrayPool<T>.Shared.Return(this.Array);
}
