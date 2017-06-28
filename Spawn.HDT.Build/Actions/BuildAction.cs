using System;
using System.Diagnostics;
using System.IO;
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

                Process.Start(startInfo).WaitForExit();

                Log(LogLevel.Trace, "Build complete");

                if (!string.IsNullOrEmpty(parameters.OutputPath))
                {
                    CopyGeneratedFiles(parameters);
                }
                else { }

                blnRet = true;
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, $"Exception occured: {ex}");
            }

            return blnRet;
        }
        #endregion

        #region CopyGeneratedFiles
        private void CopyGeneratedFiles(Parameters.BuildParameters parameters)
        {
            string strBuildDir = Path.Combine(Path.GetDirectoryName(parameters.ProjectPath), $"bin\\{parameters.BuildConfiguration}");

            if (!Directory.Exists(parameters.OutputPath))
            {
                Directory.CreateDirectory(parameters.OutputPath);
            }
            else { }
            
            if (Directory.Exists(strBuildDir))
            {
                Log(LogLevel.Trace, $"Build directory: \"{strBuildDir}\"");

                string[] vFiles = Directory.GetFiles(strBuildDir, "*.dll");

                for (int i = 0; i < vFiles.Length; i++)
                {
                    string strFileName = Path.GetFileName(vFiles[i]);

                    Log(LogLevel.Trace, $"Copying \"{vFiles[i]}\" to \"{parameters.OutputPath}\"");

                    File.Copy(vFiles[i], Path.Combine(parameters.OutputPath, strFileName), true);
                }
            }
            else
            {
                Log(LogLevel.Warning, $"Build directory \"{strBuildDir}\" not found!");
            }
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
