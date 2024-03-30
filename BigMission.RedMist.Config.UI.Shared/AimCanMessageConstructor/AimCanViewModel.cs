using Avalonia.Controls.Platform;
using Avalonia.Media.Imaging;
using BigMission.Avalonia.Utilities;
using BigMission.RedMist.Config.Shared.AimCanMessageConstructor;
using CommunityToolkit.Mvvm.ComponentModel;
using MathNet.Numerics.Distributions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace BigMission.RedMist.Config.UI.Shared.AimCanMessageConstructor;

[NotifyDataErrorInfo]
public partial class AimCanViewModel : ObservableValidator
{
    #region Settings

    public MessageSettings MessageData { get; set; } = new();

    [Range(1, 0x1FFFFFFF)]
    public int StartId
    {
        get => MessageData.StartId;
        set => SetProperty(MessageData.StartId, value, MessageData, (u, n) => u.StartId = n, validate: true);
    }

    public bool IsExtendedId
    {
        get => MessageData.IsExtendedId;
        set => SetProperty(MessageData.IsExtendedId, value, MessageData, (u, n) => u.IsExtendedId = n, validate: true);
    }

    public bool IsBigEndian
    {
        get => MessageData.IsBigEndian;
        set => SetProperty(MessageData.IsBigEndian, value, MessageData, (u, n) => u.IsBigEndian = n, validate: true);
    }

    [CustomValidation(typeof(AimCanViewModel), nameof(FrequencyValidate))]
    public int FrequencyHz
    {
        get => MessageData.FrequencyHz;
        set => SetProperty(MessageData.FrequencyHz, value, MessageData, (u, n) => u.FrequencyHz = n, validate: true);
    }

    public bool IncludePowerChannels
    {
        get => MessageData.IncludePowerChannels;
        set => SetProperty(MessageData.IncludePowerChannels, value, MessageData, (u, n) => u.IncludePowerChannels = n, validate: true);
    }

    [Range(1, 2)]
    public int PowerCurrentLength
    {
        get => MessageData.PowerCurrentLength;
        set => SetProperty(MessageData.PowerCurrentLength, value, MessageData, (u, n) => u.PowerCurrentLength = n, validate: true);
    }

    [Range(1, 100)]
    public int PowerCurrentMultiplier
    {
        get => MessageData.PowerCurrentMultiplier;
        set => SetProperty(MessageData.PowerCurrentMultiplier, value, MessageData, (u, n) => u.PowerCurrentMultiplier = n, validate: true);
    }

    public bool IncludeAlarmChannels
    {
        get => MessageData.IncludeAlarmChannels;
        set => SetProperty(MessageData.IncludeAlarmChannels, value, MessageData, (u, n) => u.IncludeAlarmChannels = n, validate: true);
    }

    public ObservableCollection<ChannelDefaultViewModel> InternalDefaults { get; } = [];

    #endregion

    public LargeObservableCollection<ChannelViewModel> Channels { get; } = [];

    public AimCanViewModel()
    {
        Channels.Add(new ChannelViewModel { Group = "sdfasdf" });

        foreach (var id in MessageData.InternalGroup.Channels)
        {
            InternalDefaults.Add(new ChannelDefaultViewModel { Data = id });
        }

    }

    public async void LoadFile()
    {
        // TODO: Load and populate the channels
        var json = await File.ReadAllTextAsync(@"C:\Code\AimConfigAutomation\AimConfigAutomation\bin\Debug\net8.0-windows\channels.json");
        var channelGroups = JsonConvert.DeserializeObject<List<ImageTextGroup>>(json);

        if (channelGroups is not null)
        {
            Channels.BeginBulkOperation();
            try
            {
                Channels.Clear();
                foreach (var grp in channelGroups)
                {
                    var grpImage = BytesToBitmap(grp.Image);
                    foreach (var ch in grp.Channels)
                    {
                        var chImage = BytesToBitmap(ch.Image);
                        var chVm = new ChannelViewModel { Data = ch, Group = grp.Text, GroupImage = grpImage, GroupIndex = grp.Index, NameImage = chImage };
                        ApplyDefaults(chVm);
                        Channels.Add(chVm);
                    }
                }
            }
            finally
            {
                Channels.EndBulkOperation();
            }
        }
    }

    private void ApplyDefaults(ChannelViewModel ch)
    {
        ch.FrequencyHz = FrequencyHz;

        // Power
        if (IncludePowerChannels && string.Compare(ch.Group, "PowerOutputs", true) == 0)
        {
            if (ch.Name.Trim().EndsWith("Current"))
            {
                ch.Include = true;
                ch.Multiplier = PowerCurrentMultiplier;
                ch.Length = PowerCurrentLength;
                ch.FrequencyHz = 5;
            }
            else if (ch.Name.Trim().EndsWith("Status"))
            {
                ch.Include = true;
                ch.Multiplier = 1;
                ch.Length = 1;
                ch.FrequencyHz = 2;
            }
        }

        // Alarms
        if (IncludeAlarmChannels && string.Compare(ch.Group, "Alarms", true) == 0)
        {
            ch.Include = true;
            ch.Multiplier = 1;
            ch.Length = 1;
            ch.FrequencyHz = 2;
        }

        // Internal
        var id = InternalDefaults.FirstOrDefault(f => string.Compare(f.Name, ch.Name, true) == 0 && f.Include);
        if (id != null)
        {
            ch.Include = true;
            ch.Multiplier = id.Multiplier;
            ch.Length = id.Length;
            ch.FrequencyHz = id.FrequencyHz;
        }
    }

    public static ValidationResult FrequencyValidate(string hz, ValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(hz)) return new ValidationResult(null);
        if (int.TryParse(hz, out int result))
        {
            int[] frequencies = [1, 2, 5, 10, 20, 50, 100];
            if (!frequencies.Contains(result))
            {
                return new ValidationResult("Invalid frequency. Must be 1, 2, 5, 10, 20, 50, 100.");
            }
        }

        return ValidationResult.Success!;
    }

    private static Bitmap? BytesToBitmap(byte[]? data)
    {
        Bitmap? image = null;
        if (data is not null && data.Length != 0)
        {
            var ms = new MemoryStream(data);
            image = new Bitmap(ms);
        }
        return image;
    }

    public void GenerateConfig()
    {
        var channels = Channels.Where(c => c.Include).ToArray();
        List<(int f, List<ChannelViewModel> chs)> packets = [];
        foreach (var channel in channels)
        {
            var freqPackets = packets.Where(t => t.f == channel.FrequencyHz).ToArray();

            // Create a new packet
            if (freqPackets is null)
            {
                CreatePacket(packets, channel);
            }
            else
            {
                bool added = false;
                foreach (var fp in freqPackets)
                {
                    int remSize = GetRemainingSize(fp);
                    // If it fits, it ships
                    if (remSize >= channel.Length)
                    {
                        fp.chs.Add(channel);
                        added = true;
                        break;
                    }
                }

                // Did not fit in any existing packets, add a new one
                if (!added)
                {
                    CreatePacket(packets, channel);
                }
            }
        }
        Console.WriteLine();
     

        var dtos = new List<CanPacket>();
        int canId = StartId;
        foreach (var p in packets)
        {
            var pDto = new CanPacket { CanId = canId, IsBigEndian = IsBigEndian, IsExtended = IsExtendedId, Frequency = p.f };
            dtos.Add(pDto);

            for (int i = 0; i < p.chs.Count; i++)
            {
                var chDto = new CanChannelAssignment
                {
                    GroupIndex = p.chs[i].GroupIndex,
                    ChannelIndex = p.chs[i].Data.Index,
                    Name = p.chs[i].Name,
                    Length = p.chs[i].Length,
                    FormulaMultiplier = p.chs[i].Multiplier,
                    Offset = i
                };
                pDto.ChannelAssignments.Add(chDto);
            }
            pDto.Length = pDto.ChannelAssignments.Sum(x => x.Length);
            canId++;
        }
        var json = JsonConvert.SerializeObject(dtos, Formatting.Indented);
        File.WriteAllText("candefinition.json", json);
    }

    /// <summary>
    /// Gets how many bytes are available in the packet.
    /// </summary>
    private static int GetRemainingSize((int f, List<ChannelViewModel> chs) fp)
    {
        int usedSize = 0;
        foreach (var m in fp.chs)
        {
            usedSize += m.Length;
        }
        int remSize = 8 - usedSize;
        return remSize;
    }

    /// <summary>
    /// Create a new packet with specified channel and add to the end of the packet list.
    /// </summary>
    private static void CreatePacket(List<(int f, List<ChannelViewModel> chs)> packets, ChannelViewModel channel)
    {
        var contents = new List<ChannelViewModel> { channel };
        var p = (channel.FrequencyHz, contents);
        packets.Add(p);
    }
}
