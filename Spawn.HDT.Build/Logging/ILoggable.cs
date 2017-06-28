using Spawn.SDK.Logging;

namespace Spawn.HDT.Build.Logging
{
    public interface ILoggable
    {
        LogEntry Log(LogLevel level, string strMessage, params object[] vArgs);
    }
}
