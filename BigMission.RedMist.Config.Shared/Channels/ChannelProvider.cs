using BigMission.ChannelManagement.Shared;
using System.Collections.Immutable;
using System.Net.WebSockets;

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

    /// <summary>
    /// Gets channels that are not being populated by any data source.
    /// </summary>
    public ImmutableArray<ChannelMappingDto> GetUnusedChannels()
    {
        var usedChannels = GetChannelAssignments().Select(c => c.ChannelId).ToHashSet();
        var config = configurationProvider.GetConfiguration();
        return config.ChannelConfig.ChannelMappings.Where(c => !usedChannels.Contains(c.Id)).ToImmutableArray();
    }

    /// <summary>
    /// Gets where channel is being set by some data source.
    /// </summary>
    public ImmutableArray<ChannelDependency> GetChannelAssignments(int? channelId = null)
    {
        if (channelId.HasValue)
        {
            return channelDependencyChecks.SelectMany(c => c.GetChannelAssignments()).Where(c => c.ChannelId == channelId).ToImmutableArray();
        }
        return channelDependencyChecks.SelectMany(c => c.GetChannelAssignments()).ToImmutableArray();
    }

    /// <summary>
    /// Gets where channels are being consumed.
    /// </summary>
    public ImmutableArray<ChannelDependency> GetChannelDependencies(int? channelId = null)
    {
        if (channelId.HasValue)
        {
            return channelDependencyChecks.SelectMany(c => c.GetChannelDependencies()).Where(c => c.ChannelId == channelId).ToImmutableArray();
        }
        return channelDependencyChecks.SelectMany(c => c.GetChannelDependencies()).ToImmutableArray();
    }

    public ChannelMappingDto? GetChannel(int channelId)
    {
        return configurationProvider.GetConfiguration().ChannelConfig.ChannelMappings.FirstOrDefault(c => c.Id == channelId);
    }

    public ImmutableArray<ChannelMappingDto> GetAllChannels()
    {
        return configurationProvider.GetConfiguration().ChannelConfig.ChannelMappings.ToImmutableArray();
    }
}
