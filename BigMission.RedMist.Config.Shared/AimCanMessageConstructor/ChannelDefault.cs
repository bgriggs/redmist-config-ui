namespace BigMission.RedMist.Config.Shared.AimCanMessageConstructor;

public class ChannelDefault
{
    public int Index { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Include { get; set; }
    public int Length { get; set; }
    public int Multiplier { get; set; } = 1;
    public int FrequencyHz { get; set; } = 1;
}
