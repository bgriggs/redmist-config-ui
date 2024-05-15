using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigMission.RedMist.Config.Shared.Logging;

/// <summary>
/// Channel and rate for logging.
/// </summary>
public class LogEntry
{
    public int ChannelId { get; set; }
    public LoggingRate Rate { get; set; }
}
