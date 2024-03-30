namespace BigMission.RedMist.Config.Shared.AimCanMessageConstructor;

public class MessageSettings
{
    public int StartId { get; set; } = 0xF00D001;
    public bool IsExtendedId { get; set; } = true;
    public bool IsBigEndian { get; set; } = true;
    public int FrequencyHz { get; set; } = 1;

    public bool IncludePowerChannels { get; set; } = true;
    public int PowerCurrentLength { get; set; } = 1;
    public int PowerCurrentMultiplier { get; set; } = 10;

    public bool IncludeAlarmChannels { get; set; } = true;

    // Reserved Channel Defaults
    public ChannelGroupDefaults InternalGroup { get; } = new() { Index = 9, Name = "Internal" };

    public MessageSettings()
    {
        // Internal
        InternalGroup.Channels.Add(new ChannelDefault { Index = 0, Name = "Battery", Include = true, Length = 1, Multiplier = 10 });
        InternalGroup.Channels.Add(new ChannelDefault { Index = 4, Name = "POTotCurrent", Include = true, Length = 2, Multiplier = 10 });
    }
}
