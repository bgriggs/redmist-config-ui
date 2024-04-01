using BigMission.RedMist.Config.Shared.CanBus;
using BigMission.RedMist.Config.Shared.Channels;
using DialogHostAvalonia;
using Newtonsoft.Json;

namespace BigMission.RedMist.Config.UI.Shared.CanBus;

public class ByteOverlayViewModel
{
    private readonly ChannelProvider channelProvider;
    private readonly CanMessageViewModel parentMessageVm;

    public CanChannelAssignmentConfigDto? Channel { get; }

    public string Text
    {
        get
        {
            if (Channel == null)
            {
                return "Unassigned";
            }

            var channelMapping = channelProvider.GetChannel(Channel.ChannelId);
            return channelMapping?.Name ?? "???";
        }
    }

    public int CurrentOffset { get; set; }


    public ByteOverlayViewModel(CanChannelAssignmentConfigDto? channel, ChannelProvider channelProvider, CanMessageViewModel parentMessageVm)
    {
        Channel = channel;
        this.channelProvider = channelProvider;
        this.parentMessageVm = parentMessageVm;
    }

    public async Task EditChannelAssignmentAsync()
    {
        CanChannelAssignmentConfigDto? chAssignmentDto;
        var parentCanMessageDtoJson = JsonConvert.SerializeObject(parentMessageVm.Data);
        var parentCanMessageDtoCopy = JsonConvert.DeserializeObject<CanMessageConfigDto>(parentCanMessageDtoJson)!;
        if (Channel == null)
        {
            chAssignmentDto = new CanChannelAssignmentConfigDto { Offset = CurrentOffset };
        }
        else // Copy existing for editing
        {
            var json = JsonConvert.SerializeObject(Channel);
            chAssignmentDto = JsonConvert.DeserializeObject<CanChannelAssignmentConfigDto>(json)!;

            // Remove the existing channel so it can be replaced as if it was new. Otherwise,
            // it will likely fail validation if offset/length is changed.
            parentCanMessageDtoCopy.ChannelAssignments.Remove(Channel);
        }

        var vm = new CanChannelSelectionDialogViewModel(channelProvider, chAssignmentDto, parentCanMessageDtoCopy);
        var result = await DialogHost.Show(vm, "MainDialogHost");
        if (result is CanChannelAssignmentConfigDto updatedChannelData)
        {
            if (Channel != null)
            {
                // Remove old assignment to be replaced
                var old = parentMessageVm.ChannelAssignments.FirstOrDefault(c => c.ChannelId == Channel.ChannelId);
                if (old != null)
                {
                    parentMessageVm.ChannelAssignments.Remove(old);
                }
            }

            if (updatedChannelData.ChannelId > 0)
            {
                parentMessageVm.ChannelAssignments.Add(updatedChannelData);
            }
        }
    }
}
