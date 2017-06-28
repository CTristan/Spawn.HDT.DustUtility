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
            object objActionParameters = null;

            bool blnSuccess = Parser.Default.ParseArguments(args, new Parameters(), (action, instance) =>
            {
                strAction = action;
                objActionParameters = instance;
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
                            Parameters.CopyParameters copyParameters = (Parameters.CopyParameters)objActionParameters;

                            Log(LogLevel.Trace, $"SourceFile=\"{copyParameters.SourceFile}\"");
                            Log(LogLevel.Trace, $"TargetFileName=\"{copyParameters.TargetFileName}\"");
                            Log(LogLevel.Trace, $"Launch=\"{copyParameters.Launch}\"");
                            Log(LogLevel.Trace, $"---------------------------------");

                            if (new CopyAction().Execute(copyParameters))
                            {
                                Log(LogLevel.Trace, "Action successful");
                            }
                            else
                            {
                                Log(LogLevel.Error, "Couln't complete action!");
                            }
                        }
                        break;
                    case "build":
                        {
                            Parameters.BuildParameters buildParameters = (Parameters.BuildParameters)objActionParameters;

                            Log(LogLevel.Trace, $"BuildConfiguration=\"{buildParameters.BuildConfiguration}\"");
                            Log(LogLevel.Trace, $"MSBuildPath=\"{buildParameters.MSBuildPath}\"");
                            Log(LogLevel.Trace, $"ProjectPath=\"{buildParameters.ProjectPath}\"");
                            Log(LogLevel.Trace, $"OutputPath=\"{buildParameters.OutputPath}\"");
                            Log(LogLevel.Trace, $"---------------------------------");

                            if (new BuildAction().Execute(buildParameters))
                            {
                                Log(LogLevel.Trace, "Action successful");
                            }
                            else
                            {
                                Log(LogLevel.Error, "Couln't complete action!");
                            }
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