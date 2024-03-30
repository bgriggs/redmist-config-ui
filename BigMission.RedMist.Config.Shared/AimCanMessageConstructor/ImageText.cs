namespace BigMission.RedMist.Config.Shared.AimCanMessageConstructor;

public class ImageText
{
    public int Index { get; set; }
    public string Text { get; set; } = string.Empty;
    public byte[]? Image { get; set; }
}

public class ImageTextGroup : ImageText
{
    public List<ImageText> Channels { get; } = [];
}
