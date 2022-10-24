using AmpHelper.Library.Enums;
using CommandLine;

namespace AmpHelper.CLI
{
    [Verb("ark", HelpText = "Ark file handling")]
    internal class ArkOptions
    {
        [Verb("unpack", HelpText = "Unpack the ark files to a directory")]
        internal class ArkUnpackOptions
        {
            [Value(0, Required = true, MetaName = "input hdr", HelpText = "Path to the input .hdr file")]
            public FileInfo InputHeader { get; set; }

            [Value(1, Required = true, MetaName = "unpacked path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo OutputPath { get; set; }

            [Option('d', "dtb2a", HelpText = "Converts all dtb files to dta format (doesn't re-pack properly)")]
            public bool DtbConversion { get; set; }

            [Option('k', "keep-dtb", HelpText = "Keep the original DTB after DTA>DTB conversion")]
            public bool KeepOriginalDtb { get; set; }

            public ConsoleType ConsoleType => InputHeader.Name.ToLower() == "main_ps3.hdr" ? ConsoleType.PS3 : ConsoleType.PS4;
        }

        [Verb("pack", HelpText = "Pack a directory to ark files")]
        internal class ArkPackOptions
        {
            [Value(0, Required = true, MetaName = "input path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo InputPath { get; set; }

            [Value(1, Required = true, MetaName = "output path", HelpText = "Path to the packed files")]
            public FileInfo OutputPath { get; set; }

            public ConsoleType ConsoleType => Directory.Exists(Path.Combine(InputPath.FullName, "ps3")) ? ConsoleType.PS3 : ConsoleType.PS4;
        }

        [Option('d', "dtb2a", Hidden = true)]
        public bool _do_not_use_d { get; set; }

        [Option('k', "keep-dtb", Hidden = true)]
        public bool _do_not_use_k { get; set; }
    }

    [Verb("song", HelpText = "Song management")]
    internal class SongOptions
    {
        [Verb("list", HelpText = "List songs")]
        internal class SongListOptions
        {
            [Value(0, Required = true, MetaName = "input path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo InputPath { get; set; }

            [Option('j', "output-json", HelpText = "Outputs JSON formatted text")]
            public bool OutputJson { get; set; }

            [Option('p', "pretty", HelpText = "Indents the JSON")]
            public bool PrettyPrint { get; set; }

            public ConsoleType ConsoleType => Directory.Exists(Path.Combine(InputPath.FullName, "ps3")) ? ConsoleType.PS3 : ConsoleType.PS4;
        }

        [Verb("remove", HelpText = "Remove a song")]
        internal class SongRemoveOptions
        {
            [Value(0, Required = true, MetaName = "input path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo InputPath { get; set; }

            [Value(1, Required = true, MetaName = "song name", HelpText = "The song to remove")]
            public string SongName { get; set; }
        }

        [Verb("add", HelpText = "Add a song")]
        internal class SongAddOptions
        {
            [Value(0, Required = true, MetaName = "input path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo InputPath { get; set; }

            [Value(1, Required = true, MetaName = "song path", HelpText = "Path to the song being added")]
            public DirectoryInfo SongPath { get; set; }
        }

        [Option('j', "output-json")]
        public bool _do_not_use_j { get; set; }

        [Option('p', "pretty")]
        public bool _do_not_use_p { get; set; }
    }

    [Verb("tweak", HelpText = "Useful tweaks")]
    internal class TweakOptions { }
}
