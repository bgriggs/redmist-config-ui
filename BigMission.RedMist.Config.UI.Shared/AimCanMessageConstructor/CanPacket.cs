namespace BigMission.RedMist.Config.UI.Shared.AimCanMessageConstructor;

public class CanPacket
{
    public int CanId { get; set; }
    public bool IsExtended { get; set; }
    public int Length { get; set; }
    public bool IsBigEndian { get; set; }
    public int Frequency { get; set; }

    public List<CanChannelAssignment> ChannelAssignments { get; } = [];
}
