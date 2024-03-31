namespace BigMission.RedMist.Config.Shared.Channels;

public class ChannelDependency(int channelId, string area, string description)
{
    public int ChannelId { get; set; } = channelId;
    public string Area { get; set; } = area;
    public string Description { get; set; } = description;
}

