using BigMission.RedMist.Config.Shared;
using BigMission.RedMist.Config.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BigMission.RedMist.Config.UI.Shared.Channels;

public partial class ChannelSelectionDialogViewModel : ObservableObject
{
    private readonly ChannelProvider channelProvider;

    public bool HideUsedChannels { get; set; }

    public ChannelSelectionDialogViewModel(ChannelProvider channelProvider)
    {
        this.channelProvider = channelProvider;
    }
}
