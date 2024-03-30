namespace BigMission.RedMist.Config.UI.Shared.AimCanMessageConstructor;

public class CanChannelAssignment
{
    public string Name { get; set; } = string.Empty;
    public int GroupIndex { get; set; }
    public int ChannelIndex { get; set; }
    public int Offset { get; set; }
    public int Length { get; set; }
    public bool IsSigned { get; set; }
    public double FormulaMultiplier { get; set; }
}
