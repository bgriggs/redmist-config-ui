namespace BigMission.RedMist.Config.Shared.CanBus;

public class CanMessageConfigDto
{
    public int Id { get; set; }
    public bool IsEnabled { get; set; }

    /// <summary>
    /// 11 or 29 bit identifier.
    /// </summary>
    public int CanId { get; set; }
    
    /// <summary>
    /// CAN interface port, such as can0, can1, etc.
    /// </summary>
    public int CanBusId { get; set; }
    
    /// <summary>
    /// True if the message is an extended message 29-bit.
    /// </summary>
    public bool IsExtended { get; set; }
    
    /// <summary>
    /// 1-8 bytes.
    /// </summary>
    public int Length { get; set; }
    public bool IsBigEndian { get; set; }
    public bool IsReceive { get; set; }
    public TimeSpan TransmitRate { get; set; }

    public List<CanChannelAssignmentConfigDto> ChannelAssignments { get; } = [];
}
