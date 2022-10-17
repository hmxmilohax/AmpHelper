using CommandLine;

namespace AmpHelper.CLI
{
    [Verb("ark", HelpText = "Ark file handling")]
    internal class ArkOptions
    {
        [Verb("unpack", HelpText = "Unpack the ark files to a directory")]
        internal class ArkUnpackOptions
        {
            [Value(0, Required = true, MetaName = "input path", HelpText = "Path to the input .hdr file")]
            public DirectoryInfo InputPath { get; set; }

            [Value(1, Required = true, MetaName = "unpacked path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo OutputPath { get; set; }
        }

        [Verb("pack", HelpText = "Pack a directory to ark files")]
        internal class ArkPackOptions
        {
            [Value(0, Required = true, MetaName = "input path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo InputPath { get; set; }

            [Value(1, Required = true, MetaName = "output path", HelpText = "Path to the packed files")]
            public FileInfo OutputPath { get; set; }
        }
    }



    [Verb("song", HelpText = "Song management")]
    internal class SongOptions
    {
        [Verb("list", HelpText = "List songs")]
        internal class SongListOptions
        {
            [Value(0, Required = true, MetaName = "input path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo InputPath { get; set; }
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
    }


    [Verb("tweak", HelpText = "Useful tweaks")]
    internal class TweakOptions
    {
        [Verb("enable", HelpText = "Enable the tweak")]
        internal class TweakEnableOptions
        {
            [Value(0, Required = true, MetaName = "input path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo InputPath { get; set; }
        }

        [Verb("disable", HelpText = "Disable the tweak")]
        internal class TweakDisableOptions
        {
            [Value(0, Required = true, MetaName = "input path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo InputPath { get; set; }
        }

        [Verb("status", HelpText = "Display the status of the tweak")]
        internal class TweakStatusOptions
        {
            [Value(0, Required = true, MetaName = "input path", HelpText = "Path to the unpacked files")]
            public DirectoryInfo InputPath { get; set; }
        }

        [Verb("unlock-all", HelpText = "Unlock all arenas, songs, powerups, and freq mode")]
        internal class TweakUnlockAllOptions { }

        [Verb("unlock-fps", HelpText = "Unlocks the frame rate to run at the highest possible")]
        internal class TweakUnlockFpsOptions { }
    }
}
