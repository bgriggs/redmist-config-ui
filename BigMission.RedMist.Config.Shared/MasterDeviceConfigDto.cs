using BigMission.RedMist.Config.Shared.CanBus;
using BigMission.RedMist.Config.Shared.Channels;
using BigMission.RedMist.Config.Shared.General;
using Newtonsoft.Json;

namespace BigMission.RedMist.Config.Shared;

public class MasterDeviceConfigDto
{
    public GeneralConfigDto GeneralConfig { get; set; } = new();
    public ChannelConfigDto ChannelConfig { get; set; } = new();
    public CanBusConfigDto CanBusConfig { get; set; } = new();

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static MasterDeviceConfigDto? Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<MasterDeviceConfigDto>(json);
    }
}
