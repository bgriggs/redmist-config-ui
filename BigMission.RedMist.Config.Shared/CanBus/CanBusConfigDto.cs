namespace BigMission.RedMist.Config.Shared.CanBus;

public class CanBusConfigDto
{
    public int Id { get; set; }
    public string InterfaceName { get; set; } = string.Empty;
    public int BaudRate { get; set; }
    private bool SilentOnCanBus { get; set; }

    public List<CanMessageConfigDto> Messages { get; } = [];
}
