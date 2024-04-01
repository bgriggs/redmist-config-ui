namespace BigMission.RedMist.Config.Shared.CanBus;

/// <summary>
/// Represents settings for a single CAN network interface.
/// </summary>
public class CanBusConfigDto
{
    public string InterfaceName { get; set; } = string.Empty;
    public int BitRate { get; set; } = 1000000;
    public bool SilentOnCanBus { get; set; }

    public List<CanMessageConfigDto> Messages { get; } = [];
}
