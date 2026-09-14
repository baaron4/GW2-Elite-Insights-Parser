using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
namespace GW2EIEvtcParser;

[StructLayout(LayoutKind.Explicit, Size = 16)]
#pragma warning disable CA1815 // Override equals and operator equals on value types
public readonly struct GUIDWrapper
#pragma warning restore CA1815 // Override equals and operator equals on value types
{
    [FieldOffset(0)] public readonly ulong First8;
    [FieldOffset(8)] public readonly ulong Last8;

    [FieldOffset(0)] public readonly Guid GUID;

    public GUIDWrapper(ulong first8, ulong last8)
    {
        GUID = default;
        First8 = first8;
        Last8 = last8;
    }
}
public static class GuidExt
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Equals(this Guid guid, ulong first8, ulong last8)
    {
        return guid == new GUIDWrapper(first8, last8).GUID;
    }
}
