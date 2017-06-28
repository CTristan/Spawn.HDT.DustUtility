using CommandLine;

namespace Spawn.HDT.Build
{
    public class Parameters
    {
        [VerbOption("copy", HelpText = "Copy the specified file into the specified directory")]
        public CopyParameters CopyVerb { get; set; }

        [VerbOption("build", HelpText = "Trigger a build for the specified project with the specified configuration")]
        public BuildParameters BuildVerb { get; set; }

        public enum BuildConfiguration
        {
            Debug,
            Release
        }

        public class CopyParameters
        {
            [Option("source", Required = true, HelpText = "The source file")]
            public string SourceFile { get; set; }

            [Option("target", Required = true, HelpText = "The target file name")]
            public string TargetFileName { get; set; }

            [Option("launch", HelpText = "Launch the HearthstoneDeckTracker when finished")]
            public bool Launch { get; set; }
        }

        public class BuildParameters
        {
            [Option("config", Required = true, HelpText = "Specify the build configuration (release|debug)")]
            public BuildConfiguration BuildConfiguration { get; set; }

            [Option("msbuild", Required = true, HelpText = "Path to the MSBuild executable")]
            public string MSBuildPath { get; set; } //C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\MSBuild\15.0\Bin

            [Option("project", Required = true, HelpText = "Path to the project file")]
            public string ProjectPath { get; set; }
        }
    }
}
