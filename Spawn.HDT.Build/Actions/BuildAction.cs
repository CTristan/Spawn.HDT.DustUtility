using System;
using System.Diagnostics;
using System.Text;
using Spawn.HDT.Build.Logging;
using Spawn.SDK.Logging;

namespace Spawn.HDT.Build.Action
{
    public class BuildAction : IAction<Parameters.BuildParameters>, ILoggable
    {
        #region Execute
        public bool Execute(Parameters.BuildParameters parameters)
        {
            bool blnRet = false;

            // /p:Configuration=Release
            // /nologo

            try
            {
                Log(LogLevel.Trace, "Launching MSBuild...");

                StringBuilder sb = new StringBuilder();
                sb.Append("\"").Append(parameters.ProjectPath).Append("\" ");
                sb.Append($"/p:Configuration={ parameters.BuildConfiguration}").Append(" ");
                sb.Append("/p:PreBuildEvent= /p:PostBuildEvent=").Append(" ");
                sb.Append("/nologo");

                ProcessStartInfo startInfo = new ProcessStartInfo(parameters.MSBuildPath)
                {
                    Arguments = sb.ToString(),
                    UseShellExecute = false
                };

                Process p = Process.Start(startInfo);

                p.WaitForExit();

                Log(LogLevel.Trace, "Build complete");

                blnRet = true;
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, $"Exception occured: {ex}");
            }

            return blnRet;
        } 
        #endregion

        #region Log
        public LogEntry Log(LogLevel level, string strMessage, params object[] vArgs)
        {
            return Logger.Default.Log(level, "BuildAction", strMessage, vArgs);
        } 
        #endregion
    }
}
