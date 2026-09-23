using System.Numerics;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData;

public class GadgetCaptureEvent : StatusEvent
{
    public long EndTime => _endIsSet ? _endTime : Src.LastAware;
    private long _endTime = long.MinValue;
    private bool _endIsSet => _endTime != long.MinValue;

    public byte OriginalOwner { get; private set; }

    public bool IsCircle => _points.Length == 1;

    public float Radius => IsCircle ? _points[0].X : 0;
    /// <summary>
    /// True if no points related data, unuseable
    /// </summary>
    public bool IsValid => _points.Length > 0;

    private Vector3[] _points = [];

    public abstract class StateWithOwner
    {
        private readonly byte From;
        private readonly byte By;
        public bool IsDecaying => By == 0;
        protected StateWithOwner(byte from, byte by)
        {
            From = from;
            By = by;
        }

        public Color GetByColor()
        {
            return GetColor(By);
        }
        public Color GetFromColor()
        {
            return GetColor(From);
        }

        public bool IsEqualOwnerState(byte from, byte by)
        {
            return From == from && By == by;
        }
    }
    public class OwnerState : StateWithOwner
    {
        public long Time;
        internal OwnerState(long time, byte from, byte by) : base(from, by)
        {
            Time = time;
        }
    }
    public IReadOnlyList<OwnerState> OwnerStates => _ownerStates;
    private readonly List<OwnerState> _ownerStates = [];

    public class ProgressState : StateWithOwner
    {
        public IReadOnlyList<(long Time, double Progress)> Progresses => _progresses;
        private readonly List<(long Time, double Progress)> _progresses;
        internal ProgressState((long Time, double Progress) firstState, byte from, byte by) : base(from, by)
        {
            _progresses = [firstState];
        }

        internal void AddState((long Time, double Progress) state)
        {
            var replaceFirst = false;
            if (_progresses.Count == 1)
            {
                var first = _progresses[0];
                replaceFirst = (IsDecaying && first.Progress == 100) || (!IsDecaying && first.Progress == 0);
            }
            if ((IsDecaying && state.Progress == 100) || (!IsDecaying && state.Progress == 0))
            {
                return;
            }
            if (replaceFirst)
            {
                _progresses[0] = state;
            }
            else
            {
                _progresses.Add(state);
            }
        }
    }

    public IReadOnlyList<ProgressState> ProgressStates => _progressStates;
    private readonly List<ProgressState> _progressStates = [];

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
        if (_endIsSet)
        {
            return;
        }
        _endTime = evtcItem.Time;
    }

    internal void AddProgress(CombatItem evtcItem)
    {
        if (_endIsSet)
        {
            return;
        }
        var progress = Math.Round(BitConverter.Int32BitsToSingle(evtcItem.Value) * 100.0f, 2);
        if (_progressStates.Count == 0)
        {
            OriginalOwner = evtcItem.IsBuff;
            _progressStates.Add(new((evtcItem.Time, progress), evtcItem.IsBuff, evtcItem.Result));
            _ownerStates.Add(new(evtcItem.Time, evtcItem.IsBuff, evtcItem.Result));
        }
        else
        {
            var last = _progressStates[^1];
            var colorStateChanged = !last.IsEqualOwnerState(evtcItem.IsBuff, evtcItem.Result);
            if (colorStateChanged)
            {
                _ownerStates.Add(new(evtcItem.Time, evtcItem.IsBuff, evtcItem.Result));
            }
            if (last.Progresses[^1].Progress != progress && !colorStateChanged)
            {
                last.AddState((evtcItem.Time, progress));
            }
            else if (colorStateChanged)
            {
                // switch due to owner change
                if (progress == 100 || progress == 0)
                {
                    last.AddState((evtcItem.Time - 1, progress));
                }
                _progressStates.Add(new((evtcItem.Time, progress), evtcItem.IsBuff, evtcItem.Result));
            }
        }
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

    public Color GetOriginalOwnerColor()
    {
        return GetColor(OriginalOwner);
    }

    private static Color GetColor(byte owner)
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
