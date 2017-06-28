using System;
using Spawn.HDT.Build.Logging;
using Spawn.SDK.Logging;

namespace Spawn.HDT.Build.Action
{
    public class BuildReleaseAction : IAction<Parameters.BuildParameters>, ILoggable
    {
        public bool Execute(Parameters.BuildParameters parameters)
        {
            throw new NotImplementedException();
        }

        public LogEntry Log(LogLevel level, string strMessage, params object[] vArgs)
        {
            return Logger.Default.Log(level, "BuildRelease", strMessage, vArgs);
        }
    }
}
