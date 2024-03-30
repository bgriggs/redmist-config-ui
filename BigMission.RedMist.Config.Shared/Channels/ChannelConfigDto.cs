using BigMission.ChannelManagement.Shared;

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
        return ++NextChannelId;
    }
}
