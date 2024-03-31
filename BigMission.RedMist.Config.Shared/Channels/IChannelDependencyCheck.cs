namespace BigMission.RedMist.Config.Shared.Channels;

/// <summary>
/// Identifies parts of the configuration that use a Channel.
/// </summary>
public interface IChannelDependencyCheck
{
    IEnumerable<ChannelDependency> GetChannelDependencies();
    IEnumerable<ChannelDependency> GetChannelAssignments();
}
