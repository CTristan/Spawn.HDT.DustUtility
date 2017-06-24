using CommandLine;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Spawn.HDT.Build
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Parameters parameters = new Parameters();
            
            if (Parser.Default.ParseArguments(args, parameters))
            {
                Console.WriteLine($"[INFO] SPAWN BUILD TOOL v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}");
                Console.WriteLine($"[INFO] Parameters");
                Console.WriteLine($"[INFO] SourceFile=\"{parameters.SourceFile}\"");
                Console.WriteLine($"[INFO] TargetFileName=\"{parameters.TargetFileName}\"");
                Console.WriteLine($"[INFO] Launch=\"{parameters.Launch}\"");

                string strProcessName = "HearthstoneDeckTracker";

                if (StopProcess(strProcessName))
                {
                    CopyPlugin(parameters, strProcessName);
                }
                else { }

                Console.WriteLine($"[INFO] Finished.");
            }
            else { }
        }

        private static bool StopProcess(string strProcessName)
        {
            Process[] vProcesses = Process.GetProcessesByName(strProcessName);

            bool blnRet = false;

            if (vProcesses.Length > 0)
            {
                Console.WriteLine("[INFO] HDT process found, closing...");

                using (Process p = vProcesses[0])
                {
                    p.Kill();

                    p.WaitForExit(500);

                    if (p.HasExited)
                    {
                        Console.WriteLine("[INFO] HDT successfuly closed");

                        blnRet = true;
                    }
                    else
                    {
                        Console.WriteLine("[ERROR] Couldn't close HDT!");
                    }
                }
            }
            else
            {
                Console.WriteLine("[INFO] HDT not running");

                blnRet = true;
            }

            return blnRet;
        }

        private static void CopyPlugin(Parameters parameters, string strProcessName)
        {
            Console.WriteLine("[INFO] Copying plugin...");

            AssemblyName asm = AssemblyName.GetAssemblyName(parameters.SourceFile);
            string strTargetFileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $@"{strProcessName}\Plugins\{parameters.TargetFileName}.v{asm.Version.ToString(2)}.dll");

            File.Copy(parameters.SourceFile, strTargetFileName, true);

            if (File.Exists(strTargetFileName))
            {
                Console.WriteLine("[INFO] Plugin copied");

                if (parameters.Launch)
                {
                    Console.WriteLine("[INFO] Launching HDT...");

                    string strBasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), strProcessName);
                    string strFileName = Path.Combine(strBasePath, $@"{GetLatestVersionDirectory(strBasePath)}\{strProcessName}.exe");

                    Process.Start(strFileName);
                }
                else { }
            }
            else
            {
                Console.WriteLine("[ERROR] Couldn't copy plugin!");
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

            return strRet;
        }
    }
}