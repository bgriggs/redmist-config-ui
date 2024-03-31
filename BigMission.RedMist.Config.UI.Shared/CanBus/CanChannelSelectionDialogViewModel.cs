using BigMission.RedMist.Config.Shared;
using BigMission.RedMist.Config.Shared.Channels;
using BigMission.RedMist.Config.UI.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BigMission.RedMist.Config.UI.Shared.CanBus;

public class CanChannelSelectionDialogViewModel : ObservableObject
{

    public ChannelSelectionControlViewModel ChannelSelectionViewModel { get; }

    public CanChannelSelectionDialogViewModel(ChannelProvider channelProvider)
    {
        ChannelSelectionViewModel = new ChannelSelectionControlViewModel(channelProvider);
    }
}
