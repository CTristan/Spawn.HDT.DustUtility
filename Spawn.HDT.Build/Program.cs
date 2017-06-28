using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using CommandLine;
using Spawn.SDK.Logging;

namespace Spawn.HDT.Build
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Logger.Default.WriteToConsole = true;
            
            Parameters parameters = new Parameters();
            
            if (Parser.Default.ParseArguments(args, parameters))
            {
                Log(LogLevel.Trace, $"SPAWN BUILD TOOL v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}");
                Log(LogLevel.Trace, $"Parameters");
                Log(LogLevel.Trace, $"SourceFile=\"{parameters.SourceFile}\"");
                Log(LogLevel.Trace, $"TargetFileName=\"{parameters.TargetFileName}\"");
                Log(LogLevel.Trace, $"Launch=\"{parameters.Launch}\"");

                string strProcessName = "HearthstoneDeckTracker";

                if (StopProcess(strProcessName))
                {
                    CopyPlugin(parameters, strProcessName);
                }
                else { }

                Log(LogLevel.Trace, $"Finished.");
            }
            else { }
        }

        private static bool StopProcess(string strProcessName)
        {
            Process[] vProcesses = Process.GetProcessesByName(strProcessName);

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

        private static void CopyPlugin(Parameters parameters, string strProcessName)
        {
            Log(LogLevel.Trace, $"Copying plugin...");

            AssemblyName asm = AssemblyName.GetAssemblyName(parameters.SourceFile);
            string strTargetFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $@"{strProcessName}\Plugins\{parameters.TargetFileName}.v{asm.Version.ToString(2)}.dll");

            File.Copy(parameters.SourceFile, strTargetFileName, true);

            if (File.Exists(strTargetFileName))
            {
                Log(LogLevel.Trace, $"Plugin copied");

                if (parameters.Launch)
                {
                    Log(LogLevel.Trace, $"Launching HDT...");

                    string strBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), strProcessName);
                    string strFileName = Path.Combine(strBasePath, $@"{GetLatestVersionDirectory(strBasePath)}\{strProcessName}.exe");

                    Process.Start(strFileName);
                }
                else { }
            }
            else
            {
                Log(LogLevel.Error, $"Couldn't copy plugin!");
            }
        }
        
        private static string GetLatestVersionDirectory(string strBasePath)
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

        public static LogEntry Log(LogLevel level, string strMessage, params object[] vArgs)
        {
            return Logger.Default.Log(level, strMessage, vArgs);
        }
    }
}