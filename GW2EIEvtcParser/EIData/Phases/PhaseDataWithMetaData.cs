using System.Numerics;
using GW2EIEvtcParser.LogLogic;
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

    public readonly long LogID = LogIDs.Unknown;

    public string NameNoMode { get; internal set; } = "";
    public readonly string Icon;

    internal PhaseDataWithMetaData(long start, long end, string name, bool success, string icon, LogMode mode, long logID, PhaseType type) : base(start, end, type)
    {
        LogID = logID;
        Success = success;
        Mode = mode;
        Icon = icon;
        NameNoMode = name;
        Name = name
            + (Mode == LogMode.CM ? " CM" : "")
            + (Mode == LogMode.LegendaryCM ? " LCM" : "")
            + (Mode == LogMode.Story ? " Story" : "");
    }

    internal PhaseDataWithMetaData(long start, long end, string name, bool success, string icon, LogMode mode, LogStartStatus startStatus, long logID, PhaseType type) : this(start, end, name, success, icon, mode, logID, type)
    {
        Name = name
            + (IsLateStart && !MissingPreEvent ? " (Late Start)" : "")
            + (MissingPreEvent ? " (No Pre-Event)" : ""); ;
    }

    internal PhaseDataWithMetaData(long start, long end, string name, ParsedEvtcLog log, PhaseType type) : this(start, end, name, log.LogData.Success, log.LogData.Logic.Icon, log.LogData.Mode, log.LogData.Logic.LogID, type)
    {
        StartStatus = log.LogData.StartStatus;
        Name = NameNoMode;
    }
}
