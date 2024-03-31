using BigMission.RedMist.Config.Shared.CanBus;
using BigMission.RedMist.Config.Shared.Channels;
using BigMission.RedMist.Config.Shared.General;
using Newtonsoft.Json;

namespace BigMission.RedMist.Config.Shared;

public class MasterDriverSyncConfig
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public GeneralConfigDto GeneralConfig { get; set; } = new();
    public ChannelConfigDto ChannelConfig { get; set; } = new();
    public List<CanBusConfigDto> CanBusConfigs { get; set; } = [];

    public string Serialize()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static MasterDriverSyncConfig? Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<MasterDriverSyncConfig>(json);
    }
}
