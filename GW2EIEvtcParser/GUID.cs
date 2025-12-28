using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
namespace GW2EIEvtcParser;

[StructLayout(LayoutKind.Sequential, Pack = 8)]
public readonly struct GUID : IEquatable<GUID>
{
    //NOTE(Rennorb): Could also use `fixed readonly byte bytes[16];`,
    // but this makes the comparison easy and I have not experimented with how MemoryExtensions.SequenceEquals performs compared to two long comparisons, since its a fixed length.
    readonly UInt64 first8, last8;

    public GUID(UInt64 first8, UInt64 last8)
	{
        this.first8 = first8;
        this.last8 = last8;
    }

    /// Requires 16 bytes
    public unsafe GUID(ReadOnlySpan<byte> bytes)
	{
        Debug.Assert(bytes.Length >= 16);
        fixed(byte* ptr = bytes)
        {
            first8 = *(UInt64*)ptr;
            last8 = *(((UInt64*)ptr) + 1);
        }
    }


    static readonly byte[]  InverseHexLookup =
    [
        0,  0,  0,  0,  0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, 0,
        0,  0,  0,  0,  0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, 0,
        0,  0,  0,  0,  0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, 0,
        0,  1,  2,  3,  4,  5,  6,  7, 8, 9, 0, 0, 0, 0, 0, 0,
        0, 10, 11, 12, 13, 14, 15, 16, 0, 0, 0, 0, 0, 0, 0, 0,
        0,  0,  0,  0,  0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, 0,
        0, 10, 11, 12, 13, 14, 15, 16, 0, 0, 0, 0, 0, 0, 0, 0,
        0,  0,  0,  0,  0,  0,  0,  0, 0, 0, 0, 0, 0, 0, 0, 0,
    ];

    public unsafe GUID(ReadOnlySpan<char> hex)
    {
        Debug.Assert(hex.Length >= 32);
        Debug.Assert(ValidateHex(hex));
        fixed (ulong* _ptr = &first8)
        {
            var ptr = (byte*)_ptr;
            for (int i = 0; i < 32; i += 2)
            {
                *ptr++ = (byte)((InverseHexLookup[hex[i] & 127] << 4) | InverseHexLookup[hex[i + 1] & 127]);
            }
        }
    }

    /// <summary>
    /// Validate the <paramref name="hex"/> to be only containing 0-F characters.
    /// </summary>
    private static bool ValidateHex(ReadOnlySpan<char> hex)
    {
        return Regex.IsMatch(hex.ToString(), @"\A\b[0-9a-fA-F]+\b\Z");
    }

    public readonly unsafe string ToHex()
    {
        fixed(UInt64* ptr = &first8)
        {
            return ParserHelper.ToHexString(new ReadOnlySpan<byte>(ptr, 16));
        }
    }
    public readonly unsafe string ToBase64()
    {
        fixed(UInt64* ptr = &first8)
        {
            return Convert.ToBase64String(new ReadOnlySpan<byte>(ptr, 16));
        }
    }

    public readonly bool Equals(GUID other) => first8 == other.first8 && last8 == other.last8;
    public readonly bool Equals(ulong otherFirst8, ulong otherLast8) => first8 == otherFirst8 && last8 == otherLast8;
    public override readonly bool Equals(object? obj) => obj is GUID other && Equals(other);
    public static bool operator==(in GUID l, in GUID r) => l.Equals(r);
    public static bool operator!=(in GUID l, in GUID r) => !l.Equals(r);

    public override readonly int GetHashCode() => HashCode.Combine(first8.GetHashCode(), last8.GetHashCode());

    public override readonly string ToString() => ToHex();
}
