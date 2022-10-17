using AmpHelper.Library.Interfaces;
using AmpHelper.Library.Tweaks;
using CommandLine;

namespace AmpHelper.CLI
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<ArkOptions, SongOptions, TweakOptions>(args)
                .MapResult(
                    (ArkOptions options) => Parser.Default.ParseArguments<ArkOptions.ArkPackOptions, ArkOptions.ArkUnpackOptions>(args.Skip(1))
                        .MapResult(
                            (ArkOptions.ArkPackOptions options) => ArkPack(options),
                            (ArkOptions.ArkUnpackOptions options) => ArkUnpack(options),
                            errors => 1),

                    (SongOptions options) => Parser.Default.ParseArguments<SongOptions.SongListOptions, SongOptions.SongAddOptions, SongOptions.SongRemoveOptions>(args.Skip(1))
                        .MapResult(
                            (SongOptions.SongListOptions options) => SongList(options),
                            (SongOptions.SongAddOptions options) => SongAdd(options),
                            (SongOptions.SongRemoveOptions options) => SongRemove(options),
                            errors => 1),

                    (TweakOptions options) => Parser.Default.ParseArguments<TweakOptions.TweakUnlockAllOptions, TweakOptions.TweakUnlockFpsOptions>(args.Skip(1))
                        .MapResult(
                            (TweakOptions.TweakUnlockAllOptions options) => ProcessTweak(args.Skip(2), new UnlockAllTweak()),
                            (TweakOptions.TweakUnlockFpsOptions options) => ProcessTweak(args.Skip(2), new UnlockFpsTweak()),
                            errors => 1),
                    errors => 1);
        }

        private static int ArkPack(ArkOptions.ArkPackOptions options)
        {
            throw new NotImplementedException();
        }

        private static int ArkUnpack(ArkOptions.ArkUnpackOptions options)
        {
            throw new NotImplementedException();
        }

        private static int SongList(SongOptions.SongListOptions options)
        {
            throw new NotImplementedException();
        }

        private static int SongAdd(SongOptions.SongAddOptions options)
        {
            throw new NotImplementedException();
        }

        private static int SongRemove(SongOptions.SongRemoveOptions options)
        {
            throw new NotImplementedException();
        }

        private static int ProcessTweak(IEnumerable<string> args, ITweak tweak)
        {
            return Parser.Default.ParseArguments<TweakOptions.TweakEnableOptions, TweakOptions.TweakDisableOptions, TweakOptions.TweakStatusOptions>(args)
                .MapResult(
                    (TweakOptions.TweakEnableOptions options) =>
                    {
                        tweak.SetPath(options.InputPath.FullName).DisableTweak();
                        return 0;
                    },
                    (TweakOptions.TweakDisableOptions options) =>
                    {
                        tweak.SetPath(options.InputPath.FullName).DisableTweak();
                        return 0;
                    },
                    (TweakOptions.TweakStatusOptions options) =>
                    {
                        bool status = tweak.SetPath(options.InputPath.FullName).IsEnabled();
                        Console.WriteLine($"The tweak is currently {(status ? "enabled" : "disabled")}");
                        return status ? 0 : 1;
                    },
                    errors => 255);
        }
    }
}