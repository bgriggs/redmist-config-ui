namespace BigMission.RedMist.Config.Shared.Logging;

public class LoggingConfigDto
{
    public bool EnableRollingLog { get; set; }

    public List<LogEntry> LogEntries { get; set; } = [];
}
