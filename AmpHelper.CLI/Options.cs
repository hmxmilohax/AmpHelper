using AmpHelper.Enums;
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

        [Verb("compact", HelpText = "Compacts a set of ark files to reduce the number of arks and remove unreferenced data.")]
        internal class ArkCompactOptions
        {
            [Value(0, Required = true, MetaName = "input hdr", HelpText = "Path to the input .hdr file")]
            public FileInfo InputHeader { get; set; }

            public ConsoleType ConsoleType => InputHeader.Name.ToLower() == "main_ps3.hdr" ? ConsoleType.PS3 : ConsoleType.PS4;
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

            [Option('f', "force", HelpText = "Allow removal of songs included with the base game.")]
            public bool Force { get; set; }

            [Option('d', "delete", HelpText = "Delete song folder")]
            public bool Delete { get; internal set; }

            public ConsoleType ConsoleType => Directory.Exists(Path.Combine(InputPath.FullName, "ps3")) ? ConsoleType.PS3 : ConsoleType.PS4;
        }

        [Verb("remove-customs", HelpText = "Remove all custom songs from the dta files", Hidden = true)]
        internal class SongRemoveCustomsOptions
        {
            [Value(0, Required = true, MetaName = "input path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo InputPath { get; set; }

            public ConsoleType ConsoleType => Directory.Exists(Path.Combine(InputPath.FullName, "ps3")) ? ConsoleType.PS3 : ConsoleType.PS4;
        }

        [Verb("add", HelpText = "Add a song manually placed in the songs folder")]
        internal class SongAddOptions
        {
            [Value(0, Required = true, MetaName = "input path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo InputPath { get; set; }

            [Value(1, Required = true, MetaName = "song name", HelpText = "Name of the song being added")]
            public string SongName { get; set; }

            public ConsoleType ConsoleType => Directory.Exists(Path.Combine(InputPath.FullName, "ps3")) ? ConsoleType.PS3 : ConsoleType.PS4;
        }

        [Verb("add-all", HelpText = "Add all songs in the songs folder")]
        internal class SongAddAllOptions
        {
            [Value(0, Required = true, MetaName = "input path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo InputPath { get; set; }

            public ConsoleType ConsoleType => Directory.Exists(Path.Combine(InputPath.FullName, "ps3")) ? ConsoleType.PS3 : ConsoleType.PS4;
        }

        [Verb("import", HelpText = "Import a song")]
        internal class SongImportOptions
        {
            [Value(0, Required = true, MetaName = "input path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo InputPath { get; set; }

            [Value(1, Required = true, MetaName = "song path", HelpText = "Path to the song being added")]
            public string SongPath { get; set; }

            [Option('r', "replace", HelpText = "Replace any existing song with the same identifier")]
            public bool Replace { get; set; }

            public ConsoleType ConsoleType => Directory.Exists(Path.Combine(InputPath.FullName, "ps3")) ? ConsoleType.PS3 : ConsoleType.PS4;
        }

        [Option('j', "output-json")]
        public bool _do_not_use_j { get; set; }

        [Option('p', "pretty")]
        public bool _do_not_use_p { get; set; }

        [Option('f', "force")]
        public bool _do_not_use_f { get; set; }

        [Option('d', "delete")]
        public bool _do_not_use_d { get; set; }

        [Option('r', "replace")]
        public bool _do_not_use_r { get; set; }
    }

    [Verb("tweak", HelpText = "Useful tweaks")]
    internal class TweakOptions { }
}
