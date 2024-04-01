namespace BigMission.RedMist.Config.Shared.CanBus;

/// <summary>
/// Represents a CAN packet with a unique 11 or 29 bit identifier.
/// </summary>
public class CanMessageConfigDto
{
    public bool IsEnabled { get; set; } = true;

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
    public int Length { get; set; } = 8;
    public bool IsBigEndian { get; set; } = true;
    public bool IsReceive { get; set; } = true;
    public TimeSpan TransmitRate { get; set; } = TimeSpan.FromMilliseconds(1000);

    public List<CanChannelAssignmentConfigDto> ChannelAssignments { get; } = [];

    public bool Validate(out string error)
    {
        error = string.Empty;

        if (Length < 1 || Length > 8)
        {
            error = $"Invalid message length: {Length}.";
            return false;
        }

        if (ChannelAssignments.Count == 0)
        {
            return true; // No channels assigned
        }

        // Check channel length
        var maxOffset = ChannelAssignments.Max(x => x.Offset + x.Length);
        if (maxOffset > Length)
        {
            error = $"Channel assignments exceed message length: {maxOffset} > {Length}.";
            return false;
        }

        // Check for overlapping channels
        var orderedChannels = ChannelAssignments.OrderBy(x => x.Offset).ToList();
        for (int i = 0; i < orderedChannels.Count - 1; i++)
        {
            var current = orderedChannels[i];
            var next = orderedChannels[i + 1];
            if (current.Offset + current.Length > next.Offset)
            {
                error = $"Overlapping channel assignments: {current.Offset} + {current.Length} > {next.Offset}.";
                return false;
            }
        }

        return true;
    }
}
