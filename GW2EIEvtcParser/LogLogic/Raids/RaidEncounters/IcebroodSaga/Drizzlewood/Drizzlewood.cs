namespace GW2EIEvtcParser.LogLogic;

internal class Drizzlewood : IcebroodSagaRaidEncounter
{
    public Drizzlewood(int triggerID) : base(triggerID)
    {
        LogCategoryInformation.SubCategory = LogCategories.SubLogCategory.Drizzlewood;
    }
}
