namespace GW2EIEvtcParser;

/// <summary>
/// Pool of non buff based achievement IDs used in the parser, always custom.
/// <para>Naming convention: </para>
/// <list type="bullet">
/// <item>No "id" inside the name.</item>
/// <item>Value must be strictly positive</item>
/// <item>Prefix the name with "Ach_"</item>
/// </list>
/// </summary>
public static class AchievementEligibilityIDs
{
    public const long Ach_Shatterstep = 1;
    public const long Ach_NopeRopes = 2;
    public const long Ach_TestReflexes = 3;
    public const long Ach_MostResistance = 4;
}
