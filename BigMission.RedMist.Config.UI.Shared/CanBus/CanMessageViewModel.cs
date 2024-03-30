using BigMission.RedMist.Config.Shared.CanBus;

namespace BigMission.RedMist.Config.UI.Shared.CanBus;

public class CanMessageViewModel
{
    public CanMessageConfigDto Data { get; set; } = new();

    public string CanId => "0x" + Data.CanId.ToString("X");
}
