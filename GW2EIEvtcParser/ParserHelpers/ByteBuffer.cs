using System.Runtime.CompilerServices;

namespace GW2EIEvtcParser.ParserHelpers;
internal ref struct ByteBuffer
{
    public Span<byte> Span;
    public int Offset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<byte> AsUsedSpan() => Span[..Offset];
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator Span<byte>(in ByteBuffer _this) => _this.Span[.._this.Offset];
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ReadOnlySpan<byte>(in ByteBuffer _this) => (ReadOnlySpan<byte>)_this.Span[.._this.Offset];



    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ByteBuffer(Span<byte> buffer)
    {
        Span = buffer;
        Offset = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void PushNative<T>(T value) where T : unmanaged
    {
        fixed(byte* ptr = &Span[Offset]) { *(T*)ptr = value; }
        Offset += sizeof(T);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void PushNative(byte value)
    {
        Span[Offset++] = value;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void PushNative(sbyte value)
    {
        Span[Offset++] = unchecked((byte)value);
    }
}
