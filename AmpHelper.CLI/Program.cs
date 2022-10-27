using AmpHelper.Attributes;
using AmpHelper.Enums;
using AmpHelper.Helpers;
using AmpHelper.Interfaces;
using CommandLine;
using System.Diagnostics;
using System.Text.Json;

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

                    (SongOptions options) => Parser.Default.ParseArguments<SongOptions.SongListOptions, SongOptions.SongAddOptions, SongOptions.SongAddAllOptions, SongOptions.SongImportOptions, SongOptions.SongRemoveOptions, SongOptions.SongRemoveCustomsOptions>(args.Skip(1))
                        .MapResult(
                            (SongOptions.SongListOptions options) => SongList(options),
                            (SongOptions.SongAddOptions options) => SongAdd(options),
                            (SongOptions.SongAddAllOptions options) => SongAdd(options),
                            (SongOptions.SongImportOptions options) => SongImport(options),
                            (SongOptions.SongRemoveOptions options) => SongRemove(options),
                            (SongOptions.SongRemoveCustomsOptions options) => SongRemove(options),
                            errors => 1),

                    (TweakOptions options) => ProcessTweak(options, args.Skip(1).ToArray()),
                    errors => 1);
        }

        private static void WriteLog(string message, long current, long total)
        {
            if (message == null)
            {
                return;
            }

            Console.WriteLine($"{String.Format("{0:0.0}", Math.Floor((double)current / total * 1000) / 10).PadLeft(5)}%: {message}");
        }

        private static int ErrorWrap(Func<int> func)
        {
            if (Debugger.IsAttached)
            {
                return func();
            }

            try
            {
                return func();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.GetType().ToString().Split('.').Last()}: {ex.Message}");

                return 1;
            }
        }

        private static int ArkPack(ArkOptions.ArkPackOptions options) => ErrorWrap(() =>
        {
            Ark.Pack(options.InputPath, options.OutputPath, options.ConsoleType, WriteLog);

            return 0;
        });

        private static int ArkUnpack(ArkOptions.ArkUnpackOptions options) => ErrorWrap(() =>
        {
            Ark.Unpack(options.InputHeader, options.OutputPath, options.DtbConversion, options.KeepOriginalDtb, options.ConsoleType, WriteLog);

            return 0;
        });

        private static int SongList(SongOptions.SongListOptions options) => ErrorWrap(() =>
        {
            var songs = Song.GetSongs(options.InputPath, options.ConsoleType);

            if (options.OutputJson)
            {
                Console.WriteLine(JsonSerializer.Serialize(songs, new JsonSerializerOptions() { WriteIndented = options.PrettyPrint }));
                return 0;
            }

            Console.WriteLine(String.Join("\n\n", songs.Select(song => $"{song.ID}:\n  Title: {song.Title}\n  Artist: {song.Artist}\n  BPM: {song.Bpm}{(string.IsNullOrEmpty(song.Charter) ? "" : $"\n  Charter: {song.Charter}")}\n  Added: {song.InGame}\n  {song.Description}")));

            return 0;
        });

        private static int SongAdd(SongOptions.SongAddOptions options) => ErrorWrap(() =>
        {
            Song.AddSong(options.InputPath, options.SongName, options.ConsoleType, WriteLog);

            return 0;
        });

        private static int SongAdd(SongOptions.SongAddAllOptions options) => ErrorWrap(() =>
        {
            Song.AddAllSongs(options.InputPath, options.ConsoleType, WriteLog);

            return 0;
        });

        private static int SongImport(SongOptions.SongImportOptions options) => ErrorWrap(() =>
        {
            var songs = Song.GetSongs(options.InputPath, options.ConsoleType).Select(e => Path.GetFileNameWithoutExtension(e.MoggSongPath.ToLower()));
            var fullPath = Path.GetFullPath(options.SongPath);
            var songPath = Path.GetFullPath(Path.Combine(options.InputPath.FullName, options.ConsoleType.ToString().ToLower(), "songs"));

            if (File.Exists(options.SongPath))
            {
                var info = new FileInfo(fullPath);

                if (options.Replace == false && !fullPath.ToLower().StartsWith(songPath.ToLower()) && songs.Contains(Path.GetFileNameWithoutExtension(options.SongPath.ToLower())))
                {
                    Console.WriteLine("Song already exists, use --replace if you want to replace");

                    return 1;
                }

                Song.ImportSong(options.InputPath, info, options.Replace, options.ConsoleType, WriteLog);
            }
            else if (Directory.Exists(options.SongPath))
            {
                var info = new DirectoryInfo(fullPath);

                if (options.Replace == false && !fullPath.ToLower().StartsWith(songPath.ToLower()) && songs.Contains(info.Name.ToLower()))
                {
                    Console.WriteLine("Song already exists, use --replace if you want to replac.");

                    return 1;
                }

                Song.ImportSong(options.InputPath, info, options.Replace, WriteLog);
            }
            else
            {
                Console.WriteLine("Input path does not exist");
            }

            return 0;
        });

        private static int SongRemove(SongOptions.SongRemoveOptions options) => ErrorWrap(() =>
        {
            if (options.Force == false && AmplitudeData.BaseSongs.Contains(options.SongName.ToLower()))
            {
                Console.WriteLine("Won't remove built-in songs without --force");
                return 1;
            }

            Song.RemoveSong(options.InputPath, options.SongName, options.Delete, options.ConsoleType, WriteLog);

            return 0;
        });

        private static int SongRemove(SongOptions.SongRemoveCustomsOptions options) => ErrorWrap(() =>
        {
            var songs = Song.GetSongs(options.InputPath, options.ConsoleType).Where(e => e.BaseSong == false && e.Special == false).Select(e => e.ID).ToArray();

            Song.RemoveSong(options.InputPath, songs, false, options.ConsoleType, WriteLog);

            return 0;
        });

        private static int ProcessTweak(TweakOptions options, string[] args) => ErrorWrap(() =>
        {
            var usageText = "Usage:\n  tweak <tweak> (status|enable|disable) <path to game files>\n\nThe following tweaks are available.";
            var type = typeof(ITweak);
            (TweakInfo Info, Type TweakType)[] tweaks = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
                .Select(tweak =>
                {
                    var info = (Attribute.GetCustomAttribute(tweak, typeof(TweakInfo)) as TweakInfo) ?? new TweakInfo(tweak.GetType().Name, tweak.GetType().Name);
                    return (info, tweak);
                }).OrderBy(t => t.info.Verb).ToArray();

            var showTweaks = (string message) =>
            {
                Console.WriteLine($"{message}\n");

                Console.WriteLine(String.Join("\n\n", tweaks.Select(t => t.Info.Description == null ? t.Info.Verb : $"{t.Info.Verb}\n  {t.Info.Description}")));

                return tweaks.Length;
            };

            if (args.Length == 0 || (args.Length == 1 && args[0].ToLower() == "help") || args.Length != 3 || (args.Length == 3 && args[1] != "status" && args[1] != "enable" && args[1] != "disable"))
            {
                var firstVerb = tweaks[0].Info.Verb;

                return showTweaks(usageText);
            }

            var verb = args[0];
            var command = args[1];
            var path = args[2];
            ConsoleType consoleType;

            var matchedTweaks = tweaks.Where(t => t.Info.Verb == args[0]).ToArray();

            if (matchedTweaks.Length == 0)
            {
                return showTweaks("That tweak is not a valid selection, the following tweaks are available");
            }

            var info = matchedTweaks[0];
            ITweak? tweak = Activator.CreateInstance(matchedTweaks[0].TweakType) as ITweak;

            if (tweak == null)
            {
                Console.WriteLine("An error occurred while creating the tweak instance.");
                return 1;
            }

            if (!Directory.Exists(path))
            {
                Console.WriteLine("The specified path cannot be found.");
                return 1;
            }

            try
            {
                consoleType = HelperMethods.ConsoleTypeFromPath(path);
            }
            catch (Exception)
            {
                Console.WriteLine($"Unable to determine console type from path \"{path}\"");
                return 1;
            }

            switch (command)
            {
                case "status":
                    bool status = tweak.SetPath(path).IsEnabled();
                    Console.WriteLine((status ? info.Info.EnabledText : info.Info.DisabledText) ?? $"The tweak is currently {(status ? "enabled" : "disabled")}");
                    return status ? 0 : 1;

                case "enable":
                    tweak.SetPath(path).EnableTweak();
                    Console.WriteLine(info.Info.EnableText ?? "Enabled the tweak.");
                    return 0;

                case "disable":
                    tweak.SetPath(path).DisableTweak();
                    Console.WriteLine(info.Info.DisableText ?? "Disabled the tweak");
                    return 0;

                default:
                    return showTweaks($"That command is not valid.\n\n{usageText}");
            }

            return 0;
        });
    }
}