using System.Runtime.CompilerServices;

namespace GW2EIEvtcParser.ParserHelpers;

internal static class StringExt
{
    internal readonly ref struct SplitResult
    {
        /// The left part of the split.
        public readonly ReadOnlySpan<char> Tail;
        /// The right part of the split. If the split was not found this may be empty.
        public readonly ReadOnlySpan<char> Head;

        public SplitResult(ReadOnlySpan<char> tail, ReadOnlySpan<char> head)
        {
            Tail = tail;
            Head = head;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static SplitResult SplitOnce(this in ReadOnlySpan<char> str, char split)
    {
        var pivot = str.IndexOf(split);
        if (pivot == -1) { return new(str, ReadOnlySpan<char>.Empty); }
        return new(str[..pivot], str[(pivot + 1)..]);
    }

    // Returns a 53-bit unsigned integer (fits within JS Number.MAX_SAFE_INTEGER = 9,007,199,254,740,991)
    public static ulong HashCyrb53(this in ReadOnlySpan<char> str, uint seed = 0)
    {
        unchecked
        {
            uint h1 = 0xdeadbeefu ^ seed;
            uint h2 = 0x41c6ce57u ^ seed;

            for (int i = 0; i < str.Length; i++)
            {
                uint ch = str[i];
                h1 = unchecked((h1 ^ ch) * 2654435761u);
                h2 = unchecked((h2 ^ ch) * 1597334677u);
            }

            h1 = unchecked((h1 ^ (h1 >> 16)) * 2246822507u);
            h1 ^= unchecked((h2 ^ (h2 >> 13)) * 3266489909u);

            h2 = unchecked((h2 ^ (h2 >> 16)) * 2246822507u);
            h2 ^= unchecked((h1 ^ (h1 >> 13)) * 3266489909u);

            return 4294967296UL * (2097151UL & h2) + h1;
        }
    }
}
