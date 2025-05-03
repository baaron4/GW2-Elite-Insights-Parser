namespace GW2EIEvtcParser;

public readonly struct IDAndGUID : IEquatable<IDAndGUID>
{
    public readonly long ID;
    public readonly GUID? GUID;

    public IDAndGUID(long id, GUID? guid)
    {
        ID = id;
        GUID = guid;
    }

    public bool Equals(IDAndGUID other)
    {
        return this == other;
    }

    public override bool Equals(object? obj)
    {
        return obj is IDAndGUID other && Equals(other);
    }

    public static bool operator ==(IDAndGUID left, IDAndGUID right)
    {
        if (left.GUID != null || right.GUID != null)
        {
            if (left.GUID == null || right.GUID == null)
            {
                return false;
            }
            return left.GUID.Equals(right.GUID);
        }
        return left.ID == right.ID;
    }
    public static bool operator !=(IDAndGUID left, IDAndGUID right)
    {
        return !(left == right);
    }

    public static bool operator ==(long left, IDAndGUID right)
    {
        return left == right.ID;
    }
    public static bool operator !=(long left, IDAndGUID right)
    {
        return !(left == right);
    }

    public static bool operator ==(IDAndGUID left, long right)
    {
        return left.ID == right;
    }
    public static bool operator !=(IDAndGUID left, long right)
    {
        return !(left == right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID.GetHashCode(), GUID.GetHashCode());
    }
}
