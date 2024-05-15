using Avalonia.Controls;

namespace BigMission.RedMist.Config.UI.Shared.Logging;

public partial class LoggingView : UserControl
{
    public LoggingView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Handle delete from keyboard.
    /// </summary>
    private void DataGrid_KeyUp(object? sender, global::Avalonia.Input.KeyEventArgs e)
    {
        if (e.Key == global::Avalonia.Input.Key.Delete && dataGrid.SelectedItem is LogEntryViewModel rvm && DataContext is LoggingViewModel vm)
        {
            vm.RemoveLogEntry(rvm);
        }
    }
}
