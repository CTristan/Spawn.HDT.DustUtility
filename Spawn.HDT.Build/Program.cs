using System.Reflection;
using CommandLine;
using Spawn.HDT.Build.Action;
using Spawn.SDK.Logging;

namespace Spawn.HDT.Build
{
    public class Program
    {
        #region Main
        private static void Main(string[] args)
        {
            Logger.Default.WriteToConsole = true;
            
            string strAction = string.Empty;
            object objActionInstance = null;

            bool blnSuccess = Parser.Default.ParseArguments(args, new Parameters(), (action, instance) =>
            {
                strAction = action;
                objActionInstance = instance;
            });
            
            if (blnSuccess)
            {
                Log(LogLevel.Trace, $"SPAWN BUILD TOOL v{Assembly.GetExecutingAssembly().GetName().Version.ToString(2)}");
                Log(LogLevel.Trace, $"---------------------------------");
                Log(LogLevel.Trace, $"Action=\"{strAction.ToUpper()}\"");
                
                switch (strAction)
                {
                    case "copy":
                        {
                            Parameters.CopyParameters parameters = (Parameters.CopyParameters)objActionInstance;

                            Log(LogLevel.Trace, $"SourceFile=\"{parameters.SourceFile}\"");
                            Log(LogLevel.Trace, $"TargetFileName=\"{parameters.TargetFileName}\"");
                            Log(LogLevel.Trace, $"Launch=\"{parameters.Launch}\"");
                            Log(LogLevel.Trace, $"---------------------------------");

                            new CopyAction().Execute(parameters);
                        }
                        break;
                    case "build":
                        {
                            Parameters.BuildParameters parameters = (Parameters.BuildParameters)objActionInstance;

                            Log(LogLevel.Trace, $"BuildConfiguration=\"{parameters.BuildConfiguration}\"");
                            Log(LogLevel.Trace, $"MSBuildPath=\"{parameters.MSBuildPath}\"");
                            Log(LogLevel.Trace, $"ProjectPath=\"{parameters.ProjectPath}\"");
                            Log(LogLevel.Trace, $"---------------------------------");

                            new BuildAction().Execute(parameters);
                        }
                        break;
                    default:
                        break;
                }

                Log(LogLevel.Trace, $"Finished.");
            }
            else
            {
                Log(LogLevel.Warning, "Passed no or invalid parameters!");
            }

#if DEBUG
            System.Diagnostics.Process.GetCurrentProcess().WaitForExit();
#endif
        }
        #endregion

        #region Log
        public static LogEntry Log(LogLevel level, string strMessage, params object[] vArgs)
        {
            return Logger.Default.Log(level, "Program", strMessage, vArgs);
        } 
        #endregion
    }
}