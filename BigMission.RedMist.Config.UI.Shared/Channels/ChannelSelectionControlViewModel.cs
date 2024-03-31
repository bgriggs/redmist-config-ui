using BigMission.RedMist.Config.Shared;
using BigMission.RedMist.Config.Shared.Channels;
using BigMission.RedMist.Config.UI.Shared.CanBus;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;

namespace BigMission.RedMist.Config.UI.Shared.Channels;

public partial class ChannelSelectionControlViewModel : ObservableObject
{
    private readonly ChannelProvider channelProvider;
    [ObservableProperty]
    private int selectedChannelId;

    public string ChannelName
    {
        get
        {
            var channel = channelProvider.GetChannel(SelectedChannelId);
            return channel?.Name ?? "???";
        }
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
        if (result is ChannelSelectionDialogViewModel)
        {

        }
    }
}
