using CommandLine;

namespace Spawn.HDT.Build
{
    public class Parameters
    {
        //0-SourceFileName (ARG)
        //1-AssemblyFileName (ARG)
        //2-Launch

        [Option('s', Required = true, HelpText = "The source file")]
        public string SourceFile { get; set; }

        [Option('t', Required = true, HelpText = "The target file name")]
        public string TargetFileName { get; set; }

        [Option('l', HelpText = "Launch the HearthstoneDeckTracker when finished")]
        public bool Launch { get; set; }
    }
}
