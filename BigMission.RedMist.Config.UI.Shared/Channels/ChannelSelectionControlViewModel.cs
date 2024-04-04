using Avalonia.Controls.Primitives;
using BigMission.ChannelManagement.Shared;
using BigMission.RedMist.Config.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;
using System.Collections.Immutable;

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

    public async Task ChannelDetails()
    {
        if (SelectedChannelMapping is null)
        {
            return;
        }

        // When in selection mode, open the selection dialog
        if (IsSelectable)
        {
            await SelectChannelAsync();
        }
        else // When in readonly mode, show the channel details
        {
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
