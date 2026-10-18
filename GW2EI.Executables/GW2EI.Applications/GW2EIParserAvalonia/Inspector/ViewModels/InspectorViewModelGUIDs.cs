using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using GW2EIParserAvalonia.Models;

namespace GW2EIParserAvalonia.ViewModels;

internal partial class InspectorViewModel : ObservableObject
{
    public DataGridCollectionView SkillGUIDsView { get; }
    public DataGridCollectionView EffectGUIDsView { get; }
    public DataGridCollectionView MarkerGUIDsView { get; }
    public DataGridCollectionView SpeciesGUIDsView { get; }
    public DataGridCollectionView TeamGUIDsView { get; }
    public DataGridCollectionView EmoteGUIDsView { get; }
    public DataGridCollectionView TransformationGUIDsView { get; }

    // Content GUID tab filters
    [ObservableProperty]
    public partial string? ContentIdFilter { get; set; }

    [ObservableProperty]
    public partial string? ContentGuidFilter { get; set; }

    private void RefreshGUIDViews(string? oldValue, string? newValue)
    {
        if (oldValue == newValue)
        {
            return;
        }
        SkillGUIDsView.Refresh();
        EffectGUIDsView.Refresh();
        MarkerGUIDsView.Refresh();
        SpeciesGUIDsView.Refresh();
        TeamGUIDsView.Refresh();
        EmoteGUIDsView.Refresh();
        TransformationGUIDsView.Refresh();
    }

    partial void OnContentIdFilterChanged(string? oldValue, string? newValue) => RefreshGUIDViews(oldValue, newValue);

    partial void OnContentGuidFilterChanged(string? oldValue, string? newValue) => RefreshGUIDViews(oldValue, newValue);

    private bool FilterContentGUIDs(object item)
    {
        if (item is not ContentGUIDModel evt)
        {
            return false;
        }
        if (ContentIDFilter(evt.ContentID, ContentIdFilter))
        {
            return false;
        }

        if (GUIDFilter(evt.GUIDStruct, evt.GUID, ContentGuidFilter))
        {
            return false;
        }
        return true;
    }


}
