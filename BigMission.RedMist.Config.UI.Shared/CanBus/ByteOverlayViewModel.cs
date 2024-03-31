using BigMission.RedMist.Config.Shared;
using BigMission.RedMist.Config.Shared.CanBus;

namespace BigMission.RedMist.Config.UI.Shared.CanBus;

public class ByteOverlayViewModel
{
    private readonly IDriverSyncConfigurationProvider configurationProvider;

    public CanChannelAssignmentConfigDto? Channel { get; }

    public string Text
    {
        get
        {
            if (Channel == null)
            {
                return "Unassigned";
            }

            var channelMapping = configurationProvider.GetConfiguration().ChannelConfig.ChannelMappings.FirstOrDefault(c => c.Id == Channel.ChannelId);
            return channelMapping?.Name ?? "???";
        }
    }

    public int AvailableBytes { get; set; }


    public ByteOverlayViewModel(CanChannelAssignmentConfigDto? channel, IDriverSyncConfigurationProvider configurationProvider)
    {
        Channel = channel;
        this.configurationProvider = configurationProvider;
    }
}
