using BigMission.Avalonia.Utilities;
using BigMission.ChannelManagement.Shared;
using BigMission.RedMist.Config.Shared.Channels;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BigMission.RedMist.Config.UI.Shared.Channels;

public partial class ChannelSelectionDialogViewModel : ObservableObject
{
    private readonly ChannelProvider channelProvider;
    private LargeObservableCollection<ChannelMappingRowViewModel> Channels { get; } = [];

    private string searchText = string.Empty;
    public string SearchText
    {
        get => searchText;
        set
        {
            SetProperty(ref searchText, value);
            FilterRows(searchText);
        }
    }

    [ObservableProperty]
    private bool hideUsedChannels = true;

    [ObservableProperty]
    private ChannelMappingRowViewModel? selectedChannel;

    public ChannelSelectionDialogViewModel(ChannelProvider channelProvider)
    {
        this.channelProvider = channelProvider;
        InitializeChannels();
        PropertyChanged += ChannelSelectionDialogViewModel_PropertyChanged;
    }

    private void ChannelSelectionDialogViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(HideUsedChannels))
        {
            InitializeChannels();
            ClearSearch();
        }
    }

    private void InitializeChannels()
    {
        var chDtos = new List<ChannelMappingDto>();
        if (HideUsedChannels)
        {
            chDtos.AddRange(channelProvider.GetUnusedChannels());
        }
        else
        {
            chDtos.AddRange(channelProvider.GetAllChannels());
        }

        Channels.BeginBulkOperation();
        try
        {
            Channels.Clear();
            foreach (var channel in chDtos)
            {
                Channels.Add(new ChannelMappingRowViewModel(channel, null));
            }
        }
        finally
        {
            Channels.EndBulkOperation();
        }
    }

    public void FilterRows(string term)
    {
        ChannelMappingRowViewModel[] remove = [];
        if (!string.IsNullOrWhiteSpace(term))
        {
            remove = Channels.Where(c => !c.Name.Contains(term, StringComparison.CurrentCultureIgnoreCase)).ToArray();
            foreach (var row in remove)
            {
                row.IsVisible = false;
            }
        }

        var matches = Channels.Except(remove);
        foreach (var row in matches)
        {
            row.IsVisible = true;
        }
    }

    public bool ClearSearch()
    {
        SearchText = string.Empty;
        return true;
    }
}
