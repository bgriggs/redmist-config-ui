using BigMission.ChannelManagement;
using BigMission.RedMist.Config.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;

namespace BigMission.RedMist.Config.UI.Shared.Channels;

public partial class ChannelSelectionControlViewModel : ObservableObject
{
    private readonly ChannelProvider channelProvider;

    [ObservableProperty]
    private bool isSelectable;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsReserved))]
    [NotifyPropertyChangedFor(nameof(ChannelName))]
    private ChannelMappingDto? selectedChannelMapping;

    public bool IsReserved
    {
        get { return SelectedChannelMapping?.IsReserved ?? false; }
    }

    public string ChannelName
    {
        get { return SelectedChannelMapping?.Name ?? "Unassigned"; }
    }

    public ChannelSelectionControlViewModel(ChannelProvider channelProvider)
    {
        this.channelProvider = channelProvider;
    }

    public async Task SelectChannelAsync()
    {
        var dialogHost = "MainDialogHost";
        if (DialogHost.IsDialogOpen(dialogHost))
        {
            dialogHost = "NestedDialogHost";
        }

        var vm = new ChannelSelectionDialogViewModel(channelProvider);
        var result = await DialogHost.Show(vm, dialogHost);
        if (result is ChannelMappingDto ch)
        {
            SelectedChannelMapping = ch;
        }
    }

    /// <summary>
    /// Open channel details dialog when in readonly mode, or open the selection dialog when in selection mode.
    /// </summary>
    public async Task ChannelDetails()
    {
        // When in selection mode, open the selection dialog
        if (IsSelectable)
        {
            await SelectChannelAsync();
        }
        else // When in readonly mode, show the channel details
        {
            if (SelectedChannelMapping is null)
            {
                return;
            }

            var dialogHost = "MainDialogHost";
            if (DialogHost.IsDialogOpen(dialogHost))
            {
                dialogHost = "NestedDialogHost";
            }

            var vm = new ChannelMappingDetailsViewModel(SelectedChannelMapping, [], channelProvider);
            await DialogHost.Show(vm, dialogHost);
        }
    }
}
