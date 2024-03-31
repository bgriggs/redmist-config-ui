using BigMission.RedMist.Config.Shared.CanBus;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using MsBox.Avalonia;
using Newtonsoft.Json;
using BigMission.RedMist.Config.Shared;

namespace BigMission.RedMist.Config.UI.Shared.CanBus;

public partial class CanMessageViewModel : ObservableObject
{
    public CanMessageConfigDto Data { get; }
    private readonly CanBusViewModel parent;
    public IDriverSyncConfigurationProvider ConfigurationProvider { get; }

    public bool IsEnabled
    {
        get => Data.IsEnabled;
        set => SetProperty(Data.IsEnabled, value, Data, (u, n) => u.IsEnabled = n);
    }
    public string CanId => "0x" + Data.CanId.ToString("X");
    public string RxTx => Data.IsReceive ? "RX" : "TX";
    public string ByteOrder => Data.IsBigEndian ? "Big/Normal" : "Little/Word Swap";

    public CanMessageViewModel(CanMessageConfigDto data, CanBusViewModel parent, IDriverSyncConfigurationProvider configurationProvider)
    {
        Data = data;
        this.parent = parent;
        ConfigurationProvider = configurationProvider;
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
            var messageVm = new CanMessageViewModel(dtoCopy, parent, ConfigurationProvider);
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
