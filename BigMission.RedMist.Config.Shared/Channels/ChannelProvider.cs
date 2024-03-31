using BigMission.ChannelManagement.Shared;

namespace BigMission.RedMist.Config.Shared.Channels;

public class ChannelProvider
{
    private readonly IEnumerable<IChannelDependencyCheck> channelDependencyChecks;
    private readonly IDriverSyncConfigurationProvider configurationProvider;

    public ChannelProvider(IEnumerable<IChannelDependencyCheck> channelDependencyChecks, IDriverSyncConfigurationProvider configurationProvider)
    {
        this.channelDependencyChecks = channelDependencyChecks;
        this.configurationProvider = configurationProvider;
    }

    public ChannelMappingDto[] GetUnusedChannels()
    {
        var usedChannels = GetChannelAssignments().Select(c => c.ChannelId).ToHashSet();
        var config = configurationProvider.GetConfiguration();
        return config.ChannelConfig.ChannelMappings.Where(c => !usedChannels.Contains(c.Id)).ToArray();
    }

    public ChannelDependency[] GetChannelAssignments(int? channelId = null)
    {
        if (channelId.HasValue)
        {
            return channelDependencyChecks.SelectMany(c => c.GetChannelAssignments()).Where(c => c.ChannelId == channelId).ToArray();
        }
        return channelDependencyChecks.SelectMany(c => c.GetChannelAssignments()).ToArray();
    }

    public ChannelDependency[] GetChannelDependencies(int? channelId = null)
    {
        if (channelId.HasValue)
        {
            return channelDependencyChecks.SelectMany(c => c.GetChannelDependencies()).Where(c => c.ChannelId == channelId).ToArray();
        }
        return channelDependencyChecks.SelectMany(c => c.GetChannelDependencies()).ToArray();
    }

    public ChannelMappingDto? GetChannel(int channelId)
    {
        return configurationProvider.GetConfiguration().ChannelConfig.ChannelMappings.FirstOrDefault(c => c.Id == channelId);
    }
}
