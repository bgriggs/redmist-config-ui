using BigMission.RedMist.Config.Shared.Channels;

namespace BigMission.RedMist.Config.Shared.CanBus;

/// <summary>
/// Finds all the places in CAN Bus where a Channel is used.
/// </summary>
public class CanBusChannelDependencyCheck : IChannelDependencyCheck
{
    private readonly IDriverSyncConfigurationProvider configurationProvider;

    public CanBusChannelDependencyCheck(IDriverSyncConfigurationProvider configurationProvider)
    {
        this.configurationProvider = configurationProvider;
    }

    public IEnumerable<ChannelDependency> GetChannelAssignments()
    {
        // Get receiving channels
        return GetDependencies(isReceiving: true);
    }

    public IEnumerable<ChannelDependency> GetChannelDependencies()
    {
        // Get transmitting channels
        return GetDependencies(isReceiving: false);
    }

    private IEnumerable<ChannelDependency> GetDependencies(bool isReceiving)
    {
        var channelDependencies = new List<ChannelDependency>();
        var config = configurationProvider.GetConfiguration();
        foreach (var canBus in config.CanBusConfigs)
        {
            foreach (var message in canBus.Messages.Where(m => m.IsReceive == isReceiving))
            {
                foreach (var assignment in message.ChannelAssignments)
                {
                    var ca = new ChannelDependency(assignment.ChannelId, "CAN Bus", $"CAN-{canBus.InterfaceName}::0x{message.CanId:X}::offset={assignment.Offset}");
                    channelDependencies.Add(ca);
                }
            }
        }
        return channelDependencies;
    }
}
