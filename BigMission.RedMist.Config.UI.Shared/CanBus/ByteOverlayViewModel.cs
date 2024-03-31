using BigMission.RedMist.Config.Shared;
using BigMission.RedMist.Config.Shared.CanBus;
using BigMission.RedMist.Config.Shared.Channels;
using BigMission.RedMist.Config.UI.Shared.Channels;
using DialogHostAvalonia;

namespace BigMission.RedMist.Config.UI.Shared.CanBus;

public class ByteOverlayViewModel
{
    private readonly ChannelProvider channelProvider;

    public CanChannelAssignmentConfigDto? Channel { get; }

    public string Text
    {
        get
        {
            if (Channel == null)
            {
                return "Unassigned";
            }

            var channelMapping = channelProvider.GetChannel(Channel.ChannelId);
            return channelMapping?.Name ?? "???";
        }
    }

    public int AvailableBytes { get; set; }


    public ByteOverlayViewModel(CanChannelAssignmentConfigDto? channel, ChannelProvider channelProvider)
    {
        Channel = channel;
        this.channelProvider = channelProvider;
    }

    public async Task EditChannelAssignmentAsync()
    {
        var vm = new CanChannelSelectionDialogViewModel(channelProvider);
        var result = await DialogHost.Show(vm, "MainDialogHost");
        if (result is CanChannelSelectionDialogViewModel)
        {
            
        }
    }
}
