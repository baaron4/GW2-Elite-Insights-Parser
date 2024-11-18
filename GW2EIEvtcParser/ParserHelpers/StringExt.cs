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
        if(pivot == -1) { return new(str, ReadOnlySpan<char>.Empty); }
        return new(str[..pivot], str[(pivot + 1)..]);
    }
}
