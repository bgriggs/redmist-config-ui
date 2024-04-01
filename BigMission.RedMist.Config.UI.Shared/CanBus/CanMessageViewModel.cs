using BigMission.Avalonia.Utilities;
using BigMission.RedMist.Config.Shared.CanBus;
using BigMission.RedMist.Config.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using Newtonsoft.Json;

namespace BigMission.RedMist.Config.UI.Shared.CanBus;

/// <summary>
/// View model for CAN Message Row in the CAN table.
/// </summary>
public partial class CanMessageViewModel : ObservableObject
{
    public CanMessageConfigDto Data { get; }
    private readonly CanBusViewModel parent;
    public ChannelProvider ChannelProvider { get; }

    public bool IsEnabled
    {
        get => Data.IsEnabled;
        set => SetProperty(Data.IsEnabled, value, Data, (u, n) => u.IsEnabled = n);
    }
    public string CanId => "0x" + Data.CanId.ToString("X");
    public string RxTx => Data.IsReceive ? "RX" : "TX";
    public string ByteOrder => Data.IsBigEndian ? "Big/Normal" : "Little/Word Swap";

    private readonly LargeObservableCollection<CanChannelAssignmentConfigDto> channelAssignments = [];
    public LargeObservableCollection<CanChannelAssignmentConfigDto> ChannelAssignments => channelAssignments;


    public CanMessageViewModel(CanMessageConfigDto data, CanBusViewModel parent, ChannelProvider channelProvider)
    {
        Data = data;
        this.parent = parent;
        ChannelProvider = channelProvider;
        channelAssignments.SetRange(data.ChannelAssignments);
        ChannelAssignments.CollectionChanged += ChannelAssignments_CollectionChanged;
    }

    /// <summary>
    /// Synchronize the source data model when the channel assignments change.
    /// </summary>
    private void ChannelAssignments_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        Data.ChannelAssignments.Clear();
        Data.ChannelAssignments.AddRange(ChannelAssignments);
    }

    public async Task EditCanMessageAsync()
    {
        // Create a copy for editing
        var json = JsonConvert.SerializeObject(Data);
        var dtoCopy = JsonConvert.DeserializeObject<CanMessageConfigDto>(json)!;
        var vm = new CanMessageDialogViewModel(dtoCopy);
        var result = await DialogHost.Show(vm, "MainDialogHost");
        if (result is CanMessageDialogViewModel)
        {
            parent.Messages.Remove(this);
            var messageVm = new CanMessageViewModel(dtoCopy, parent, ChannelProvider);
            parent.Messages.Add(messageVm);
        }
    }

    public async Task DeleteCanMessageAsync()
    {
        var box = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams
        {
            ButtonDefinitions = [new ButtonDefinition { Name = "Yes", IsDefault = true }, new ButtonDefinition { Name = "No", IsCancel = true }],
            ContentTitle = "Confirm Deletion",
            ContentMessage = "Are you sure you want to delete this message?",
            Icon = Icon.Error,
            MaxWidth = 500,
        });

        var result = await box.ShowAsync();
        if (result == "Yes")
        {
            parent.Messages.Remove(this);
        }
    }
}
