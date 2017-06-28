using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Spawn.HDT.Build.Logging;
using Spawn.SDK.Logging;

namespace Spawn.HDT.Build.Action
{
    public class CopyAction : IAction<Parameters.CopyParameters>, ILoggable
    {
        private string m_strProcessName = "HearthstoneDeckTracker";

        #region Execute
        public bool Execute(Parameters.CopyParameters parameters)
        {
            bool blnRet = false;
            
            if (StopProcess())
            {
                Copy(parameters);

                blnRet = true;
            }
            else { }

            return blnRet;
        }
        #endregion

        #region StopProcess
        private bool StopProcess()
        {
            Process[] vProcesses = Process.GetProcessesByName(m_strProcessName);

            bool blnRet = false;

            if (vProcesses.Length > 0)
            {
                Log(LogLevel.Trace, $"HDT process found, closing...");

                using (Process p = vProcesses[0])
                {
                    p.Kill();

                    p.WaitForExit(500);

                    if (p.HasExited)
                    {
                        Log(LogLevel.Trace, $"HDT successfuly closed");

                        blnRet = true;
                    }
                    else
                    {
                        Log(LogLevel.Warning, $"Couldn't close HDT!");
                    }
                }
            }
            else
            {
                Log(LogLevel.Trace, $"HDT not running");

                blnRet = true;
            }

            return blnRet;
        }
        #endregion

        #region Copy
        private void Copy(Parameters.CopyParameters parameters)
        {
            Log(LogLevel.Trace, $"Copying plugin...");

            AssemblyName asm = AssemblyName.GetAssemblyName(parameters.SourceFile);
            string strTargetFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $@"{m_strProcessName}\Plugins\{parameters.TargetFileName}.v{asm.Version.ToString(2)}.dll");

            File.Copy(parameters.SourceFile, strTargetFileName, true);

            if (File.Exists(strTargetFileName))
            {
                Log(LogLevel.Trace, $"Plugin copied");

                if (parameters.Launch)
                {
                    Log(LogLevel.Trace, $"Launching HDT...");

                    string strBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), m_strProcessName);
                    string strFileName = Path.Combine(strBasePath, $@"{GetLatestVersionDirectory(strBasePath)}\{m_strProcessName}.exe");

                    Process.Start(strFileName);
                }
                else { }
            }
            else
            {
                Log(LogLevel.Error, $"Couldn't copy plugin!");
            }
        }
        #endregion

        #region GetLatestVersionDirectory
        private string GetLatestVersionDirectory(string strBasePath)
        {
            string strRet = string.Empty;

            string[] vDirs = Directory.GetDirectories(strBasePath);

            Version latestVersion = null;

            for (int i = 0; i < vDirs.Length; i++)
            {
                string strCurrentDir = Path.GetFileName(vDirs[i]);

                if (Version.TryParse(strCurrentDir.Replace("app-", string.Empty), out Version tmp))
                {
                    if (latestVersion == null)
                    {
                        latestVersion = tmp;

                        strRet = strCurrentDir;
                    }
                    else if (tmp.CompareTo(latestVersion) == 1)
                    {
                        latestVersion = tmp;

                        strRet = strCurrentDir;
                    }
                    else { }
                }
                else { }
            }

            Log(LogLevel.Trace, $"Latest version: v{strRet.Replace("app-", string.Empty)}");

            return strRet;
        }
        #endregion

        #region Log
        public LogEntry Log(LogLevel level, string strMessage, params object[] vArgs)
        {
            return Logger.Default.Log(level, "CopyAction", strMessage, vArgs);
        }
        #endregion
    }
}
