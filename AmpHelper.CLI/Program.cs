using AmpHelper.Library.Ark;
using AmpHelper.Library.Attributes;
using AmpHelper.Library.Enums;
using AmpHelper.Library.Helpers;
using AmpHelper.Library.Interfaces;
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

                    (TweakOptions options) => ProcessTweak(options, args.Skip(1).ToArray()),
                    errors => 1);
        }

        private static int ArkPack(ArkOptions.ArkPackOptions options)
        {
            Ark.Pack(options.InputPath, options.OutputPath, options.ConsoleType, (message) => Console.WriteLine(message));

            return 0;
        }

        private static int ArkUnpack(ArkOptions.ArkUnpackOptions options)
        {
            Ark.Unpack(options.InputHeader, options.OutputPath, options.DtbConversion, options.KeepOriginalDtb, options.ConsoleType, (message) => Console.WriteLine(message));

            return 0;
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

        private static int ProcessTweak(TweakOptions options, string[] args)
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
        }
    }
}