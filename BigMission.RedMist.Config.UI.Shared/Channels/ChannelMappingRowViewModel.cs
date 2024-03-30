using BigMission.ChannelManagement.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;

namespace BigMission.RedMist.Config.UI.Shared.Channels;

/// <summary>
/// Represents a row in the Channels data grid.
/// </summary>
public partial class ChannelMappingRowViewModel : ObservableObject
{
    public ChannelMappingDto Data { get; set; } = new();
    public ChannelsViewModel? ParentVm { get; set; }

    [ObservableProperty]
    private bool isVisible = true;

    public string Name => Data.Name;
    public string Category => Data.Category;
    public string Abbreviation => Data.Abbreviation;
    public string DataType
    {
        get
        {
            if (Data.IsStringValue) return "String";
            return Data.DataType;
        }
    }
    public bool IsUsed { get; set; }
    public string? Source { get; set; }
    public bool IsReserved => Data.IsReserved;

    public async Task EditChannelClick(object obj)
    {
        if (obj is ChannelMappingDto map)
        {
            var copyMap = map.Copy();
            var chVm = new ChannelMappingViewModel { Data = copyMap, ParentChannels = ParentVm?.Channels ?? [] };
            var result = await DialogHost.Show(chVm, "MainDialogHost");
            if (result is ChannelMappingViewModel editedMap)
            {
                ParentVm?.UpdateChannelMapping(this, editedMap.Data);
            }
        }
    }

    public async Task DeleteChannel(object obj)
    {
        if (obj is ChannelMappingDto)
        {
            if (IsReserved)
            {
                var m = MessageBoxManager.GetMessageBoxStandard(
                    "Channel Delete", "Cannot delete reserved channel.", ButtonEnum.Ok, Icon.Forbidden);
                await m.ShowAsync();
                return;
            }

            var box = MessageBoxManager.GetMessageBoxStandard(
                "Channel Delete", "Are you sure you would like to delete this channel?", ButtonEnum.YesNo, Icon.Question);

            var result = await box.ShowAsync();
            if (result.HasFlag(ButtonResult.Yes))
            {
                ParentVm?.DeleteChannelMapping(this);
            }
        }
    }
}
