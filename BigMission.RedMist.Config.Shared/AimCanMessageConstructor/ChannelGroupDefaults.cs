namespace BigMission.RedMist.Config.Shared.AimCanMessageConstructor;

public class ChannelGroupDefaults
{
    public int Index { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FrequencyHz { get; set; } = 1;
    public List<ChannelDefault> Channels { get; } = [];
}
