using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using BigMission.RedMist.Config.Shared;
using BigMission.RedMist.Config.UI.ViewModels;
using System.IO;
using System.Linq;

namespace BigMission.RedMist.Config.UI.Views;

public partial class MainView : UserControl
{
    private MainViewModel mainViewModel = new();
    public MainView()
    {
        InitializeComponent();
        DataContext = mainViewModel;
    }

    private async void SaveFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null)
        {
            var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save File",
                DefaultExtension = "json", 
            });

            if (file is not null)
            {
                await using var stream = await file.OpenWriteAsync();
                using var streamWriter = new StreamWriter(stream);
                var d = mainViewModel.Data?.Serialize();
                await streamWriter.WriteLineAsync(d);
            }
        }
    }

    // TODO: Test loading to ensure the UI is updated to reflect the new data.
    private async void LoadFileButton_Clicked(object sender, RoutedEventArgs args)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null)
        {
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open File",
                AllowMultiple = false,
            });

            if (files.Any())
            {
                await using var stream = await files[0].OpenReadAsync();
                using var streamReader = new StreamReader(stream);
                var fileContent = await streamReader.ReadToEndAsync();
                if (!string.IsNullOrEmpty(fileContent))
                {
                    var mvm = new MainViewModel
                    {
                        Data = MasterDeviceConfigDto.Deserialize(fileContent) ?? new MasterDeviceConfigDto(),
                    };
                    DataContext = mvm;
                }
            }
        }
    }
}
