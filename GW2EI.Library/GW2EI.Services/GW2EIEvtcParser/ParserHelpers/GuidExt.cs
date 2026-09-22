using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
namespace GW2EIEvtcParser;

[StructLayout(LayoutKind.Explicit, Size = 16)]
#pragma warning disable CA1815 // Override equals and operator equals on value types
public readonly struct GUIDWrapper
#pragma warning restore CA1815 // Override equals and operator equals on value types
{
    [FieldOffset(0)] public readonly ulong ZeroToEight;
    [FieldOffset(0)] public readonly uint ZeroToFour;
    [FieldOffset(4)] public readonly ushort FourToSix;
    [FieldOffset(6)] public readonly ushort SixToEight;
    [FieldOffset(8)] public readonly ulong EightToSixteen;

    [FieldOffset(0)] public readonly Guid GUID;

    public GUIDWrapper(ulong first8, ulong last8, bool reverseEndian)
    {
        GUID = default;
        if (!reverseEndian)
        {
            ZeroToEight = first8;
        }
        else
        {
            first8 = BinaryPrimitives.ReverseEndianness(first8);
            var last4OfFirst8 = (uint)(first8 & 0xFFFFFFFF);
            ZeroToFour = (uint)(first8 >> 32);
            FourToSix = (ushort)(last4OfFirst8 >> 16);
            SixToEight = (ushort)(last4OfFirst8 & 0xFFFF);
        }
        EightToSixteen = last8;
    }
}
public static class GuidExt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(this Guid guid, ulong first8, ulong last8, bool reverseEndian)
    {
        return guid == new GUIDWrapper(first8, last8, reverseEndian).GUID;
    }
}
