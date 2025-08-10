using System.Numerics;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParsedData.LogData;

namespace GW2EIEvtcParser.EIData;

public abstract class PhaseDataWithMetaData : PhaseData
{
    public readonly bool Success;
    public readonly LogMode Mode = LogMode.NotSet;
    public readonly LogStartStatus StartStatus = LogStartStatus.NotSet;
    public bool IsLateStart => StartStatus == LogStartStatus.Late || MissingPreEvent;
    public bool MissingPreEvent => StartStatus == LogStartStatus.NoPreEvent;
    public bool IsCM => Mode == LogMode.CMNoName || Mode == LogMode.CM;
    public bool IsLegendaryCM => Mode == LogMode.LegendaryCM;

    public readonly string NameNoMode = "";
    public readonly string Icon;
    internal PhaseDataWithMetaData(long start, long end, bool success, string icon, LogMode mode, PhaseType type) : base(start, end, type)
    {
        Success = success;
        Mode = mode;
        Icon = icon;
    }

    internal PhaseDataWithMetaData(long start, long end, string name, bool success, string icon, LogMode mode, PhaseType type) : this(start, end, success, icon, mode, type)
    {
        NameNoMode = name;
        Name = name
            + (Mode == LogMode.CM ? " CM" : "")
            + (Mode == LogMode.LegendaryCM ? " LCM" : "")
            + (Mode == LogMode.Story ? " Story" : "")
            + (IsLateStart && !MissingPreEvent ? " (Late Start)" : "")
            + (MissingPreEvent ? " (No Pre-Event)" : ""); ;
    }

    internal PhaseDataWithMetaData(long start, long end, ParsedEvtcLog log, PhaseType type) : this(start, end, log.LogData.Success, log.LogData.Logic.Icon, log.LogData.Mode, type)
    {
        StartStatus = log.LogData.StartStatus;
    }

    internal PhaseDataWithMetaData(long start, long end, string name, ParsedEvtcLog log, PhaseType type) : this(start, end, name, log.LogData.Success, log.LogData.Logic.Icon, log.LogData.Mode, type)
    {
    }
}
