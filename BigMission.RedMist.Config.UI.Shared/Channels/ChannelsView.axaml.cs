using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace BigMission.RedMist.Config.UI.Shared.Channels
{
    public partial class ChannelsView : UserControl
    {
        public ChannelsView()
        {
            InitializeComponent();
            //this.Loaded += ChannelsView_Loaded;
        }

        //private void ChannelsView_Loaded(object? sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        //{
        //    var vm = DataContext as ChannelsViewModel;
        //    if (vm is not null)
        //    {
        //        vm.RefreshIsUsed();
        //    }
        //}

        /// <summary>
        /// Support editing row from tap on a touch screen.
        /// </summary>
        private async void DataGrid_Tapped(object? sender, global::Avalonia.Input.TappedEventArgs e)
        {
            if (e.Source is Visual v && e.Pointer.Type == global::Avalonia.Input.PointerType.Touch)
            {
                await HandleRowEdit(v);
            }
        }

        /// <summary>
        /// Support editing row from a mouse click.
        /// </summary>
        private async void DataGrid_DoubleTapped(object? sender, global::Avalonia.Input.TappedEventArgs e)
        {
            if (e.Source is Visual v && e.Pointer.Type == global::Avalonia.Input.PointerType.Mouse)
            {
                await HandleRowEdit(v);
            }
        }

        private static async Task HandleRowEdit(Visual visual)
        {
            var row = visual.FindAncestorOfType<DataGridRow>();
            if (row is not null && row.DataContext is ChannelMappingRowViewModel rvm)
            {
                await rvm.EditChannelClick(rvm.Data);
            }
        }

        /// <summary>
        /// Handle delete from keyboard.
        /// </summary>
        private async void DataGrid_KeyUp(object? sender, global::Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == global::Avalonia.Input.Key.Delete && channelDataGrid.SelectedItem is ChannelMappingRowViewModel rvm)
            {
                await rvm.DeleteChannel(rvm.Data);
            }
        }        
    }
}
