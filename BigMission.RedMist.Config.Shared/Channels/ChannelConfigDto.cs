using BigMission.ChannelManagement.Shared;
using BigMission.RedMist.Config.Shared.Extensions;

namespace BigMission.RedMist.Config.Shared.Channels;

public class ChannelConfigDto
{
    public int NextChannelId { get; set; }

    public List<ChannelMappingDto> ChannelMappings { get; set; } = [];

    /// <summary>
    /// Get a new channel ID and increment the last channel ID placeholder.
    /// </summary>
    /// <returns></returns>
    public int IncNextId()
    {
        var ids = ChannelMappings.Select(c => c.Id);
        return ids.NextId();
    }
}
