using CommandLine;

namespace Spawn.HDT.Build
{
    public class Parameters
    {
        #region Properties
        #region CopyVerb
        [VerbOption("copy", HelpText = "Copy the specified file into the specified directory")]
        public CopyParameters CopyVerb { get; set; }
        #endregion

        #region BuildVerb
        [VerbOption("build", HelpText = "Trigger a build for the specified project with the specified configuration")]
        public BuildParameters BuildVerb { get; set; }  
        #endregion
        #endregion

        #region BuildConfiguration
        public enum BuildConfiguration
        {
            Debug,
            Release
        } 
        #endregion

        public class CopyParameters
        {
            #region Properties
            #region SourceFile
            [Option("source", Required = true, HelpText = "The source file")]
            public string SourceFile { get; set; }
            #endregion

            #region TargetFileName
            [Option("target", Required = true, HelpText = "The target file name")]
            public string TargetFileName { get; set; }
            #endregion

            #region Launch
            [Option("launch", HelpText = "Launch the HearthstoneDeckTracker when finished")]
            public bool Launch { get; set; }  
            #endregion
            #endregion
        }

        public class BuildParameters
        {
            #region Properties
            #region BuildConfiguration
            [Option("config", Required = true, HelpText = "Specify the build configuration (release|debug)")]
            public BuildConfiguration BuildConfiguration { get; set; } 
            #endregion

            #region MSBuildPath
            [Option("msbuild", Required = true, HelpText = "Path to the MSBuild executable")]
            public string MSBuildPath { get; set; } //C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin 
            #endregion

            #region ProjectPath
            [Option("project", Required = true, HelpText = "Path to the project file")]
            public string ProjectPath { get; set; } 
            #endregion

            #region OutputPath
            [Option("output", HelpText = "Generated files are going to be copied into this directory")]
            public string OutputPath { get; set; } 
            #endregion
            #endregion
        }
    }
}
