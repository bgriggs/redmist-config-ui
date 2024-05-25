using Avalonia.Controls;
using Avalonia.Platform.Storage;
using BigMission.Avalonia.Utilities;
using BigMission.ChannelManagement;
using BigMission.RedMist.Config.Shared.Channels;
using BigMission.RedMist.Config.Shared.Logging;
using BigMission.RedMist.Config.UI.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;
using DialogHostAvalonia;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace BigMission.RedMist.Config.UI.Shared.Logging;

/// <summary>
/// Overall view model for logging configuration.
/// </summary>
public class LoggingViewModel : ObservableObject
{
    private readonly ChannelProvider channelProvider;

    public LoggingConfigDto Data { get; }

    public bool EnableRollingLog
    {
        get => Data.EnableRollingLog;
        set => SetProperty(Data.EnableRollingLog, value, Data, (u, n) => u.EnableRollingLog = n);
    }

    public LargeObservableCollection<LogEntryViewModel> LogEntries { get; } = [];

    public LoggingViewModel(LoggingConfigDto data, ChannelProvider channelProvider)
    {
        Data = data;
        this.channelProvider = channelProvider;
        LogEntries.AddRange(data.LogEntries.Select(l => new LogEntryViewModel(l, channelProvider)));
        LogEntries.CollectionChanged += (object? sender, NotifyCollectionChangedEventArgs e) =>
        {
            Data.LogEntries.Clear();
            Data.LogEntries.AddRange(LogEntries.Select(l => l.LogEntry));
        };
    }

    public async void AddMessageClick()
    {
        var usedChannelIds = LogEntries.Select(l => l.LogEntry.ChannelId).ToArray();
        var vm = new ChannelSelectionDialogViewModel(channelProvider, usedChannelIds) { HideUsedChannels = false };
        var result = await DialogHost.Show(vm, "MainDialogHost");
        if (result is ChannelMappingDto ch)
        {
            var logEntry = new LogEntry { ChannelId = ch.Id, Rate = LoggingRate.OneHz };
            var logEntryVm = new LogEntryViewModel(logEntry, channelProvider);
            LogEntries.Add(logEntryVm);
        }
    }

    public void RemoveLogEntry(object source)
    {
        if (source is LogEntryViewModel logEntry)
        {
            LogEntries.Remove(logEntry);
        }
    }

    public async void ImportAsync(object source)
    {
        var topLevel = TopLevel.GetTopLevel((Control)source);
        if (topLevel != null)
        {
            var dir = Directory.GetCurrentDirectory();
            var defaultPath = await topLevel.StorageProvider.TryGetFolderFromPathAsync(dir);
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open File",
                AllowMultiple = false,
                SuggestedStartLocation = defaultPath,
            });

            if (files.Count == 1)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(files[0].Path.LocalPath);
                    var dto = JsonConvert.DeserializeObject<LoggingConfigDto>(json);
                    if (dto is not null)
                    {
                        EnableRollingLog = dto.EnableRollingLog;
                        LogEntries.SetRange(dto.LogEntries.Select(l => new LogEntryViewModel(l, channelProvider)));
                    }
                }
                catch (Exception ex)
                {
                    var box = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams
                    {
                        ButtonDefinitions = [new ButtonDefinition { Name = "OK", IsDefault = true }],
                        ContentTitle = "Load Error",
                        ContentMessage = "Failed to load selected file: " + ex.Message,
                        Icon = Icon.Error,
                        MaxWidth = 500,
                    });
                    await box.ShowAsync();
                }
            }
        }
    }

    public async void ExportAsync(object source)
    {
        var topLevel = TopLevel.GetTopLevel((Control)source);
        if (topLevel != null)
        {
            var dir = Directory.GetCurrentDirectory();
            var defaultPath = await topLevel.StorageProvider.TryGetFolderFromPathAsync(dir);
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save File",
                SuggestedStartLocation = defaultPath,
            });

            if (file is not null)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(Data, Formatting.Indented);
                    await File.WriteAllTextAsync(file.Path.LocalPath, json);
                }
                catch (Exception ex)
                {
                    var box = MessageBoxManager.GetMessageBoxCustom(new MessageBoxCustomParams
                    {
                        ButtonDefinitions = [new ButtonDefinition { Name = "OK", IsDefault = true }],
                        ContentTitle = "Save Error",
                        ContentMessage = "Failed to save file: " + ex.Message,
                        Icon = Icon.Error,
                        MaxWidth = 500,
                    });
                    await box.ShowAsync();
                }
            }
        }
    }
}
