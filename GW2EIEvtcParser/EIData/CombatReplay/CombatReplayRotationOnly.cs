using System.Drawing;
using System.Numerics;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

public class CombatReplayRotationOnly : CombatReplay
{

    internal CombatReplayRotationOnly(ParsedEvtcLog log) : base(log)
    {
    }

    internal override void AddPosition(ParametricPoint3D position)
    {
    }

    internal override void AddVelocity(ParametricPoint3D velocity)
    {
    }

    internal override void CopyPositionsFrom(CombatReplay other)
    {
        _Positions = other.Positions.ToList();
        _PolledPositions = other.PolledPositions.ToArray();
        _Velocities = other.Velocities.ToList();
    }
    internal override void PollingRate(long logDuration, bool forcePolling)
    {
        if (_Positions.Count == 0 && forcePolling)
        {
            _Positions = [new(int.MinValue, int.MinValue, 0, 0)];
        }
        else if (_Positions.Count == 0)
        {
            return;
        }

        int rate = ParserHelper.CombatReplayPollingRate;

        bool doRotation = _Rotations.Count > 0;

        int rotationTableIndex = 0;
        int polledRotationTableIndex = 0;

        long posStartOffset = Math.Min(0, rate * ((_Positions[0].Time / rate) - 1));
        long rotStartOffset = doRotation ? Math.Min(0, rate * ((_Rotations[0].Time / rate) - 1)) : 0;
        long startOffset = Math.Min(posStartOffset, rotStartOffset);
        int capacity = (int)(logDuration - startOffset) / rate + 1;
        _PolledRotations = doRotation ? new ParametricPoint3D[capacity] : [];

        if (doRotation)
        {
            for (long t = startOffset; t < logDuration; t += rate)
            {
                HandleRotation(t, ref polledRotationTableIndex, ref rotationTableIndex, rate);
            }
        }
    }
}

