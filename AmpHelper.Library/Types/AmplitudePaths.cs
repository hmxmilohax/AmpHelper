using AmpHelper.Enums;
using AmpHelper.Helpers;
using System.IO;

namespace AmpHelper.Types
{
    /// <summary>
    /// Contains various paths for the game.
    /// </summary>
    internal class AmplitudePaths
    {
        /// <summary>
        /// The base unpacked path.
        /// </summary>
        public string BasePath { get; private set; }

        /// <summary>
        /// The path to amp_config.
        /// </summary>
        public string Config { get; private set; }

        /// <summary>
        /// The path to amp_songs_config.
        /// </summary>
        public string SongsConfig { get; private set; }

        /// <summary>
        /// The path to the songs directory.
        /// </summary>
        public string Songs { get; private set; }

        /// <summary>
        /// Sets the paths of the class
        /// </summary>
        /// <param name="unpackedPath">The base unpacked path.</param>
        /// <param name="consoleType">The console type.</param>
        private void SetPaths(string unpackedPath, ConsoleType consoleType)
        {
            var platform = consoleType.ToString().ToLower();

            BasePath = Path.GetFullPath(unpackedPath);
            Config = Path.GetFullPath(Path.Combine(unpackedPath, platform, "config", $"amp_config.dta_dta_{platform}"));
            SongsConfig = Path.GetFullPath(Path.Combine(unpackedPath, platform, "config", $"amp_songs_config.dta_dta_{platform}"));
            Songs = Path.GetFullPath(Path.Combine(unpackedPath, platform, "songs"));
        }

        /// <summary>
        /// Initializes the class.
        /// </summary>
        /// <param name="unpackedPath">The path to the unpacked files.</param>
        /// <param name="consoleType">The console type.</param>
        public AmplitudePaths(string unpackedPath, ConsoleType? consoleType = null)
        {
            SetPaths(unpackedPath, consoleType.HasValue ? consoleType.Value : HelperMethods.ConsoleTypeFromPath(unpackedPath, GamePathType.Unpacked));
        }

    }
}
