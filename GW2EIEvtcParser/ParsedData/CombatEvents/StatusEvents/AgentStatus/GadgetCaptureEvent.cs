using System.Numerics;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public class GadgetCaptureEvent : StatusEvent
{
    public long EndTime { get; private set; } = long.MinValue;
    public bool EndIsSet => EndTime != long.MinValue;

    public byte OriginalOwner { get; private set; }

    public bool IsCircle => _points.Length == 1;

    public float Radius => IsCircle ? _points[0].X : 0;

    private Vector3[] _points = [];

    public IReadOnlyList<(long Time, float Progress, byte From, byte By)> Progress => _progress;
    private readonly List<(long Time, float Progress, byte From, byte By)> _progress = [];
    internal GadgetCaptureEvent(CombatItem evtcItem, AgentData agentData) : base(evtcItem, agentData)
    {
        OriginalOwner = evtcItem.IsBuff;
    }

    internal void AddPoint(CombatItem evtcItem)
    {
        if (_points.Length == 0)
        {
            _points = new Vector3[(int)evtcItem.OverstackValue];
        }
        int index = (int)evtcItem.DstAgent;
        if (index >= _points.Length)
        {
            return;
        }
        _points[index] = new Vector3(
            BitConverter.Int32BitsToSingle(evtcItem.Value),
            BitConverter.Int32BitsToSingle(evtcItem.BuffDmg),
            0
        );
    }

    internal void SetEnd(CombatItem evtcItem)
    {
        if (EndIsSet)
        {
            return;
        }
        EndTime = evtcItem.Time;
    }

    internal void AddProgress(CombatItem evtcItem)
    {
        if (EndIsSet)
        {
            return;
        }
        if (_progress.Count == 0)
        {
            OriginalOwner = evtcItem.IsBuff;
        }
        _progress.Add((evtcItem.Time, BitConverter.Int32BitsToSingle(evtcItem.Value) * 100.0f, evtcItem.IsBuff, evtcItem.Result));
    }

    public IReadOnlyList<Vector3> GetRelativePoints(Vector3 position)
    {
        if (IsCircle)
        {
            throw new InvalidOperationException("Capture area is a circle");
        }
        var relativePoints = new List<Vector3>(_points.Length);
        foreach (var point in _points)
        {
            relativePoints.Add(new Vector3(
                point.XY() - position.XY(),
                position.Z
            ));
        }
        return relativePoints;
    }

    public Color GetColor(byte owner)
    {
        switch (owner)
        {
            default:
            case 0:
                return Colors.White;
            case 1:
                return Colors.Red;
            case 2:
                return Colors.LightBlue;
            case 3:
                return Colors.Green;
        }
    }
}
