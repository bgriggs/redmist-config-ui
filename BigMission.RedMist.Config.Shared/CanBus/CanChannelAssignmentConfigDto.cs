namespace BigMission.RedMist.Config.Shared.CanBus;

public class CanChannelAssignmentConfigDto
{
    public int SourceChannelId {  get; set; }
    public int Offset { get; set; }
    public int Length { get; set; }
    public int Mask { get; set; }
    public bool IsSigned { get; set; }
    public double FormulaMultiplier { get; set; }
    public double FormulaDivider { get; set; }
    public double FormulaConst { get; set; }

}
