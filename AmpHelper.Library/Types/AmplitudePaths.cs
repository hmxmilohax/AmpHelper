using AmpHelper.Library.Enums;
using AmpHelper.Library.Helpers;
using System.IO;

namespace AmpHelper.Library.Types
{
    internal class AmplitudePaths
    {
        public string BasePath { get; private set; }
        public string Config { get; private set; }
        public string SongsConfig { get; private set; }
        public string Songs { get; private set; }

        private void SetPaths(string unpackedPath, ConsoleType consoleType)
        {
            var platform = consoleType.ToString().ToLower();

            BasePath = Path.GetFullPath(unpackedPath);
            Config = Path.GetFullPath(Path.Combine(unpackedPath, platform, "config", $"amp_config.dta_dta_{platform}"));
            SongsConfig = Path.GetFullPath(Path.Combine(unpackedPath, platform, "config", $"amp_songs_config.dta_dta_{platform}"));
            Songs = Path.GetFullPath(Path.Combine(unpackedPath, platform, "songs"));
        }

        public AmplitudePaths(string unpackedPath, ConsoleType consoleType) => SetPaths(unpackedPath, consoleType);
        public AmplitudePaths(string unpackedPath) => SetPaths(unpackedPath, HelperMethods.ConsoleTypeFromPath(unpackedPath, GamePathType.Unpacked));

    }
}
