using System.Diagnostics;

namespace GW2EIEvtcParser.ParsedData;

public class Token : IEquatable<Token>
{
    public readonly ulong Data;

    public string DataString => ToString();

    private const ulong LetterMask = ulong.MaxValue >> (64 - DigitShift);

    private const byte LetterLowerBase = (byte)'a' - 1;

    private const byte LetterUpperBase = (byte)'A' - 1;

    private const int DigitShift = 60;

    private const byte DigitBase = (byte)'0';

    public Token(ulong data)
    {
        Data = data;
    }

    public Token(ReadOnlySpan<char> name)
    {
        Debug.Assert(name.Length <= 14);
        Data = 0;
        for (var i = 0; i < name.Length; i++)
        {
            var el = name[i];
            if (char.IsAsciiLetterLower(el))
            {
                var code = el - LetterLowerBase;
                Data |= (ulong)code << 5 * i;
            }
            else if (char.IsAsciiLetterUpper(el))
            {
                var code = el - LetterUpperBase;
                Data |= (ulong)code << 5 * i;
            }
            else if (char.IsAsciiDigit(el))
            {
                Debug.Assert(i + 2 == name.Length);
                var last = name[i + 1];
                Debug.Assert(char.IsAsciiDigit(last));
                var digits = last - DigitBase + 10 * (el - DigitBase);
                Data |= (ulong)digits << DigitShift;
                break;
            }
        }
    }

    public bool Equals(Token? other) => other is not null && Data == other.Data;

    public override bool Equals(object? obj) => Equals(obj as Token);

    public override int GetHashCode() => Data.GetHashCode();

    public static bool operator ==(in Token left, in Token right) => left.Equals(right);

    public static bool operator !=(in Token left, in Token right) => !left.Equals(right);

    public override string ToString()
    {
        var result = "";

        var letters = Data & LetterMask;
        while (letters != 0)
        {
            var code = letters & 0x1F;
            char el = code != 0 ? (char)(code + LetterLowerBase) : ' ';
            result += el;
            letters >>= 5;
        }

        var digits = Data >> DigitShift;
        if (digits != 0)
        {
            result += digits.ToString("00");
        }

        return result;
    }
}
