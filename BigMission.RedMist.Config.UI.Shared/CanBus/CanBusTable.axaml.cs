using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using BigMission.RedMist.Config.Shared;
using BigMission.RedMist.Config.Shared.CanBus;
using System.Collections;
using System.Collections.ObjectModel;

namespace BigMission.RedMist.Config.UI.Shared.CanBus;

/// <summary>
/// View for a set of messages/packets associated with a single CAN bus.
/// </summary>
public partial class CanBusTable : UserControl
{
    private const int ByteColumnWidth = 110;
    private readonly int[] ColumnWidths = [40, 50, 90, 50, 115, 45, ByteColumnWidth, ByteColumnWidth, ByteColumnWidth, ByteColumnWidth, ByteColumnWidth, ByteColumnWidth, ByteColumnWidth, ByteColumnWidth];

    public static readonly DirectProperty<CanBusTable, IEnumerable> ItemsProperty =
        AvaloniaProperty.RegisterDirect<CanBusTable, IEnumerable>(nameof(Items), o => o.Items, (o, v) => o.Items = v);
    private IEnumerable _items = new AvaloniaList<object>();

    public IEnumerable Items
    {
        get { return _items; }
        set { SetAndRaise(ItemsProperty, ref _items, value); }
    }

    public CanBusTable()
    {
        InitializeComponent();
        PropertyChanged += CanBusTable_PropertyChanged;
    }

    private void CanBusTable_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == ItemsProperty)
        {
            if (e.OldValue is ObservableCollection<CanMessageViewModel> ov)
            {
                ov.CollectionChanged -= ViewModelItems_CollectionChanged;
            }

            if (e.NewValue is ObservableCollection<CanMessageViewModel> nv)
            {
                nv.CollectionChanged += ViewModelItems_CollectionChanged;
                SpRows.Children.Clear();
                AddRows(nv);
            }
        }
    }

    private void ViewModelItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        SpRows.Children.Clear();
        AddRows((ObservableCollection<CanMessageViewModel>)sender!);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        AddHeader();
    }

    private void AddHeader()
    {
        SpHeader.Children.Clear();
        int offset = 0;
        // Edit buttons
        SpHeader.Children.Add(CreatedHeaderCell("", ColumnWidths[offset]));

        // Enabled
        SpHeader.Children.Add(CreatedHeaderCell("On/Off", ColumnWidths[++offset]));

        // CAN ID
        SpHeader.Children.Add(CreatedHeaderCell("CAN ID", ColumnWidths[++offset]));

        // RX/TX
        SpHeader.Children.Add(CreatedHeaderCell("RX/TX", ColumnWidths[++offset]));

        // DLC
        SpHeader.Children.Add(CreatedHeaderCell("Byte Order", ColumnWidths[++offset]));

        // Length
        SpHeader.Children.Add(CreatedHeaderCell("DLC", ColumnWidths[++offset]));

        // Bytes
        for (int i = 0; i < 8; i++)
        {
            SpHeader.Children.Add(CreatedHeaderCell($"Byte {i + 1}", ColumnWidths[++offset], i == 7));
        }
    }

    private static Control CreatedHeaderCell(string text, int width, bool isLast = false)
    {
        var b = new Border
        {
            Width = width,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1, 1, 0, 1),
            Background = Brushes.LightGray,
            Child = new TextBlock
            {
                Text = text,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            }
        };

        if (isLast)
        {
            b.BorderThickness = new Thickness(1);
        }

        return b;
    }

    private void AddRows(ObservableCollection<CanMessageViewModel> canMessageViewModels)
    {
        foreach (var item in canMessageViewModels.OrderBy(c => c.CanId))
        {
            var rowSp = new StackPanel { Orientation = Orientation.Horizontal };
            int colOffset = 0;
            // Buttons
            rowSp.Children.Add(CreateCell(item, "ButtonCellDT", ColumnWidths[colOffset]));
            // Enabled checkbox
            rowSp.Children.Add(CreateCell(item, "EnabledCellDT", ColumnWidths[++colOffset]));
            // CAN ID
            rowSp.Children.Add(CreateCell(item, "TextCellDT", ColumnWidths[++colOffset], "CanId"));
            // RX/TX
            rowSp.Children.Add(CreateCell(item, "TextCellDT", ColumnWidths[++colOffset], "RxTx"));
            // Byte Order
            rowSp.Children.Add(CreateCell(item, "TextCellDT", ColumnWidths[++colOffset], "ByteOrder"));
            // DLC
            rowSp.Children.Add(CreateCell(item, "TextCellDT", ColumnWidths[++colOffset], "Data.Length"));

            for (int i = 0; i < item.Data.Length;)
            {
                var channel = item.Data.ChannelAssignments.FirstOrDefault(c => c.Offset == i);
                var vm = new ByteOverlayViewModel(channel, item.ChannelProvider);
                if (channel == null)
                {
                    vm.AvailableBytes = GetAvailableBytes(item.Data.ChannelAssignments, i);
                }

                var cell = CreateChannelCell(vm);
                rowSp.Children.Add(cell);

                if (channel != null)
                {
                    i += channel.Length;
                }
                else
                {
                    i++;
                }
            }

            SpRows.Children.Add(rowSp);
        }
    }

    private Control CreateCell(CanMessageViewModel item, string template, int width, string binding = ".") => new ContentControl
    {
        [!ContentProperty] = new Binding(binding),
        DataContext = item,
        ContentTemplate = (DataTemplate)Resources[template]!,
        Width = width,
    };

    private Control CreateChannelCell(ByteOverlayViewModel vm)
    {
        var cc = new ContentControl
        {
            [!ContentProperty] = new Binding("."),
            DataContext = vm,
            ContentTemplate = (DataTemplate)Resources["ByteCellDT"]!,
            Width = ByteColumnWidth * (vm.Channel?.Length ?? 1),
        };
        return cc;
    }

    /// <summary>
    /// Determine the number of available bytes after the current offset.
    /// </summary>
    private int GetAvailableBytes(IEnumerable<CanChannelAssignmentConfigDto> channels, int currentOffset)
    {
        int availableBytes = 1;
        for (int i = currentOffset + 1; i < 8; i++)
        {
            if (channels.FirstOrDefault(c => c.Offset == i) == null)
            {
                availableBytes++;
            }
            else
            {
                break;
            }
        }
        return availableBytes;
    }
}
