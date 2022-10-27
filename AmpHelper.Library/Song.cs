using AmpHelper.Delegates;
using AmpHelper.Enums;
using AmpHelper.Exceptions;
using AmpHelper.Extensions;
using AmpHelper.Helpers;
using AmpHelper.Types;
using DtxCS;
using DtxCS.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AmpHelper
{
    /// <summary>
    /// Contains methods for adding, removing, and getting songs from the game data.
    /// </summary>
    public static class Song
    {
        /// <summary>
        /// Validates a song
        /// </summary>
        /// <param name="path">The path to the song.</param>
        /// <param name="name">The name of the song, this is usually the same as the folder containing it.</param>
        /// <param name="throwError">Throw errors, or return them.</param>
        /// <returns>An <see cref="AggregateException"/> of <see cref="FileNotFoundException"/> exceptions encountered during validation, or null if there are none.</returns>
        /// <exception cref="AggregateException">Only thrown if throwError == true.</exception>
        private static AggregateException ValidateSong(string path, string name, bool throwError = false)
        {
            var exceptions = new List<Exception>();

            foreach (var file in new string[] { "mogg", "mid", "moggsong" }.Select(e => Path.Combine(path, $"{name}.{e}")))
            {
                if (!File.Exists(file))
                {
                    if (throwError)
                    {
                        exceptions.Add(new FileNotFoundException(file));
                    }
                }
            }

            if (throwError && exceptions.Count > 0)
            {
                throw new AggregateException("Song validation failed", exceptions);
            }

            return exceptions.Count > 0 ? new AggregateException("Song validation failed", exceptions) : null;
        }

        #region Import Song
        /// <summary>
        /// Copies the song into the songs folder of the game, and adds it into the amp_config and amp_songs_config files.  
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="moggsong">The .moggsong file of the song, or the directory containing it.</param>
        /// <param name="replace">If any existing song with the same name should be replaced.  An exception will be thrown if one exists and this is false.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="FormatException">Thrown when the song file name does not have a .moggsong extension.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void ImportSong(string unpackedPath, string moggsong, bool replace, ProgressAction Log = null)
        {
            if (Directory.Exists(moggsong))
            {
                ImportSong(new DirectoryInfo(unpackedPath), new DirectoryInfo(moggsong), replace, HelperMethods.ConsoleTypeFromPath(moggsong, GamePathType.Unpacked), Log);
            }
            else if (File.Exists(moggsong))
            {
                var info = new FileInfo(moggsong);
                ImportSong(new DirectoryInfo(unpackedPath), info, replace, HelperMethods.ConsoleTypeFromPath(info.Directory.FullName, GamePathType.Unpacked), Log);
            }
            else
            {
                throw new AggregateException(new DirectoryNotFoundException(moggsong), new FileNotFoundException(moggsong));
            }
        }

        /// <summary>
        /// Copies the song into the songs folder of the game, and adds it into the amp_config and amp_songs_config files.  
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="moggsong">The .moggsong file of the song, or the directory containing it.</param>
        /// <param name="replace">If any existing song with the same name should be replaced.  An exception will be thrown if one exists and this is false.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="FormatException">Thrown when the song file name does not have a .moggsong extension.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void ImportSong(string unpackedPath, string moggsong, bool replace, ConsoleType consoleType, ProgressAction Log = null)
        {
            if (Directory.Exists(moggsong))
            {
                ImportSong(new DirectoryInfo(unpackedPath), new DirectoryInfo(moggsong), replace, consoleType, Log);
            }
            else if (File.Exists(moggsong))
            {
                ImportSong(new DirectoryInfo(unpackedPath), new FileInfo(moggsong), replace, consoleType, Log);
            }
            else
            {
                throw new AggregateException(new DirectoryNotFoundException(moggsong), new FileNotFoundException(moggsong));
            }
        }

        /// <summary>
        /// Copies the song into the songs folder of the game, and adds it into the amp_config and amp_songs_config files.  
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="moggsong">The path to a directory containing the .moggsong file.</param>
        /// <param name="replace">If any existing song with the same name should be replaced.  An exception will be thrown if one exists and this is false.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="FormatException">Thrown when the song file name does not have a .moggsong extension.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void ImportSong(string unpackedPath, DirectoryInfo moggsong, bool replace, ProgressAction Log = null) => ImportSong(new DirectoryInfo(unpackedPath), new FileInfo(Path.Combine(moggsong.FullName, $"{moggsong.Name}.moggsong")), replace, HelperMethods.ConsoleTypeFromPath(unpackedPath, GamePathType.Unpacked), Log);

        /// <summary>
        /// Copies the song into the songs folder of the game, and adds it into the amp_config and amp_songs_config files.  
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="moggsong">The path to a directory containing the .moggsong file.</param>
        /// <param name="replace">If any existing song with the same name should be replaced.  An exception will be thrown if one exists and this is false.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="FormatException">Thrown when the song file name does not have a .moggsong extension.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void ImportSong(string unpackedPath, DirectoryInfo moggsong, bool replace, ConsoleType consoleType, ProgressAction Log = null) => ImportSong(new DirectoryInfo(unpackedPath), new FileInfo(Path.Combine(moggsong.FullName, $"{moggsong.Name}.moggsong")), replace, consoleType, Log);

        /// <summary>
        /// Copies the song into the songs folder of the game, and adds it into the amp_config and amp_songs_config files.  
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="moggsong">The .moggsong file of the song.</param>
        /// <param name="replace">If any existing song with the same name should be replaced.  An exception will be thrown if one exists and this is false.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="FormatException">Thrown when the song file name does not have a .moggsong extension.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void ImportSong(string unpackedPath, FileInfo moggsong, bool replace, ProgressAction Log = null) => ImportSong(new DirectoryInfo(unpackedPath), moggsong, replace, HelperMethods.ConsoleTypeFromPath(unpackedPath, GamePathType.Unpacked), Log);

        /// <summary>
        /// Copies the song into the songs folder of the game, and adds it into the amp_config and amp_songs_config files.  
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="moggsong">The .moggsong file of the song.</param>
        /// <param name="replace">If any existing song with the same name should be replaced.  An exception will be thrown if one exists and this is false.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="FormatException">Thrown when the song file name does not have a .moggsong extension.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void ImportSong(string unpackedPath, FileInfo moggsong, bool replace, ConsoleType consoleType, ProgressAction Log = null) => ImportSong(new DirectoryInfo(unpackedPath), moggsong, replace, consoleType, Log);

        /// <summary>
        /// Copies the song into the songs folder of the game, and adds it into the amp_config and amp_songs_config files.  
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="moggsong">The .moggsong file of the song.</param>
        /// <param name="replace">If any existing song with the same name should be replaced.  An exception will be thrown if one exists and this is false.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="FormatException">Thrown when the song file name does not have a .moggsong extension.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void ImportSong(DirectoryInfo unpackedPath, DirectoryInfo moggsong, bool replace, ProgressAction Log = null) => ImportSong(unpackedPath, new FileInfo(Path.Combine(moggsong.FullName, $"{moggsong.Name}.moggsong")), replace, HelperMethods.ConsoleTypeFromPath(unpackedPath.FullName, GamePathType.Unpacked), Log);

        /// <summary>
        /// Copies the song into the songs folder of the game, and adds it into the amp_config and amp_songs_config files.  
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="moggsong">The .moggsong file of the song.</param>
        /// <param name="replace">If any existing song with the same name should be replaced.  An exception will be thrown if one exists and this is false.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="FormatException">Thrown when the song file name does not have a .moggsong extension.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void ImportSong(DirectoryInfo unpackedPath, DirectoryInfo moggsong, bool replace, ConsoleType consoleType, ProgressAction Log = null) => ImportSong(unpackedPath, new FileInfo(Path.Combine(moggsong.FullName, $"{moggsong.Name}.moggsong")), replace, consoleType, Log);

        /// <summary>
        /// Copies the song into the songs folder of the game, and adds it into the amp_config and amp_songs_config files.  
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="moggsong">The .moggsong file of the song.</param>
        /// <param name="replace">If any existing song with the same name should be replaced.  An exception will be thrown if one exists and this is false.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="FormatException">Thrown when the song file name does not have a .moggsong extension.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void ImportSong(DirectoryInfo unpackedPath, FileInfo moggsong, bool replace, ProgressAction Log = null) => ImportSong(unpackedPath, moggsong, replace, HelperMethods.ConsoleTypeFromPath(unpackedPath.FullName, GamePathType.Unpacked), Log);

        /// <summary>
        /// Copies the song into the songs folder of the game, and adds it into the amp_config and amp_songs_config files.  
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="moggsong">The .moggsong file of the song.</param>
        /// <param name="replace">If any existing song with the same name should be replaced.  An exception will be thrown if one exists and this is false.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="FormatException">Thrown when the song file name does not have a .moggsong extension.</exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void ImportSong(DirectoryInfo unpackedPath, FileInfo moggsong, bool replace, ConsoleType consoleType, ProgressAction Log = null)
        {
            if (!moggsong.FullName.ToLower().EndsWith(".moggsong"))
            {
                throw new FormatException($"{moggsong.FullName} is not a .moggsong file");
            }

            if (unpackedPath == null)
            {
                throw new ArgumentNullException(nameof(unpackedPath));
            }

            if (moggsong == null)
            {
                throw new ArgumentNullException(nameof(moggsong));
            }

            if (!unpackedPath.Exists)
            {
                throw new DirectoryNotFoundException(unpackedPath.FullName);
            }

            if (!moggsong.Exists)
            {
                throw new FileNotFoundException(moggsong.FullName);
            }

            var id = Path.GetFileNameWithoutExtension(moggsong.FullName);
            var paths = new AmplitudePaths(unpackedPath.FullName, consoleType);
            var destSongPath = Path.GetFullPath(Path.Combine(paths.Songs, id));
            var sourceSongPath = Path.GetFullPath(moggsong.Directory.FullName);
            var platform = consoleType.ToString().ToLower();

            ValidateSong(moggsong.Directory.FullName, id, true);

            if (Path.GetFullPath(moggsong.Directory.FullName).ToLower() == destSongPath.ToLower())
            {
                // The song being imported is already in the song path, just add it and return.
                AddSong(unpackedPath, id, consoleType);

                return;
            }

            if (Directory.Exists(destSongPath))
            {
                if (replace)
                {
                    RemoveSong(unpackedPath, id, true, consoleType);
                }
                else
                {
                    throw new Exception("Song already exists");
                }
            }

            Directory.CreateDirectory(destSongPath);

            foreach (var ext in new string[] { "moggsong", "mid", "mogg", $"mid_{platform}", $"png.dta_dta_{platform}", $"png_{platform}" })
            {
                var sourceFile = Path.Combine(sourceSongPath, $"{id}.{ext}");
                var destFile = Path.Combine(destSongPath, $"{id}.{ext}");

                if (File.Exists(sourceFile))
                {
                    File.Copy(sourceFile, destFile);
                }
            }

            AddSong(unpackedPath, id, consoleType);

            Log?.Invoke($"Imported {id}", 1, 1);
        }
        #endregion

        #region Add All Songs
        /// <summary>
        /// Adds all missing songs present in the songs folder of the game files into the amp_config and amp_songs_config files.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        public static void AddAllSongs(string unpackedPath, ProgressAction Log = null) => AddAllSongs(new DirectoryInfo(unpackedPath), HelperMethods.ConsoleTypeFromPath(unpackedPath, GamePathType.Unpacked), Log);

        /// <summary>
        /// Adds all missing songs present in the songs folder of the game files into the amp_config and amp_songs_config files.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        public static void AddAllSongs(string unpackedPath, ConsoleType consoleType, ProgressAction Log = null) => AddAllSongs(new DirectoryInfo(unpackedPath), consoleType, Log);

        /// <summary>
        /// Adds all missing songs present in the songs folder of the game files into the amp_config and amp_songs_config files.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        public static void AddAllSongs(DirectoryInfo unpackedPath, ProgressAction Log = null) => AddAllSongs(unpackedPath, HelperMethods.ConsoleTypeFromPath(unpackedPath.FullName, GamePathType.Unpacked), Log);

        /// <summary>
        /// Adds all missing songs present in the songs folder of the game files into the amp_config and amp_songs_config files.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        public static void AddAllSongs(DirectoryInfo unpackedPath, ConsoleType consoleType, ProgressAction Log = null)
        {
            var songs = GetSongs(unpackedPath, consoleType, Log).Where(e => e.InGame == false && e.Special == false).Select(e => e.ID).ToArray();

            AddSong(unpackedPath, songs, consoleType, Log);
        }
        #endregion

        #region Add Song
        /// <summary>
        /// Adds a song already present in the songs folder of the game files into the amp_config and amp_songs_config files.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="songName">The name of the song to add.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DtxException"></exception>
        public static void AddSong(string unpackedPath, string songName, ProgressAction Log = null) => AddSong(new DirectoryInfo(unpackedPath), songName, HelperMethods.ConsoleTypeFromPath(unpackedPath, GamePathType.Unpacked), Log);

        /// <summary>
        /// Adds a song already present in the songs folder of the game files into the amp_config and amp_songs_config files.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="songName">The name of the song to add.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DtxException"></exception>
        public static void AddSong(string unpackedPath, string songName, ConsoleType consoleType, ProgressAction Log = null) => AddSong(new DirectoryInfo(unpackedPath), songName, consoleType, Log);

        /// <summary>
        /// Adds a song already present in the songs folder of the game files into the amp_config and amp_songs_config files.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="songName">The name of the song to add.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DtxException"></exception>
        public static void AddSong(DirectoryInfo unpackedPath, string songName, ProgressAction Log = null) => AddSong(unpackedPath, songName, HelperMethods.ConsoleTypeFromPath(unpackedPath.FullName, GamePathType.Unpacked), Log);

        /// <summary>
        /// Adds a song already present in the songs folder of the game files into the amp_config and amp_songs_config files.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="songName">The name of the song to add.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DtxException"></exception>
        public static void AddSong(DirectoryInfo unpackedPath, string songName, ConsoleType consoleType, ProgressAction Log = null) => AddSong(unpackedPath, new string[] { songName }, consoleType, Log);

        /// <summary>
        /// Adds a series of songs already present in the songs folder of the game files into the amp_config and amp_songs_config files.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="songNames">The names of the songs to add.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DtxException"></exception>
        public static void AddSong(DirectoryInfo unpackedPath, string[] songNames, ConsoleType consoleType, ProgressAction Log = null)
        {
            var paths = new AmplitudePaths(unpackedPath.FullName, consoleType);
            var platform = consoleType.ToString().ToLower();

            if (unpackedPath == null)
            {
                throw new ArgumentNullException(nameof(unpackedPath));
            }

            if (!unpackedPath.Exists)
            {
                throw new DirectoryNotFoundException(unpackedPath.FullName);
            }

            if (!File.Exists(paths.Config))
            {
                throw new FileNotFoundException(paths.Config);
            }

            if (!File.Exists(paths.SongsConfig))
            {
                throw new FileNotFoundException(paths.SongsConfig);
            }

            RemoveSong(unpackedPath, songNames, false, consoleType);

            var configHelper = new DtxFileHelper<object>(paths.Config);
            var songsConfigHelper = new DtxFileHelper<object>(paths.SongsConfig);

            foreach (var (songName, index) in songNames.Select((item, index) => (item, index)))
            {
                var songPath = Path.GetFullPath(Path.Combine(paths.Songs, songName));
                var targetToken = songName.ToUpper();

                if (string.IsNullOrEmpty(songName))
                {
                    throw new ArgumentNullException(nameof(songName));
                }

                if (!Directory.Exists(songPath))
                {
                    throw new DirectoryNotFoundException(songPath);
                }

                ValidateSong(songPath, songName, true);

                foreach (var ext in new string[] { $"mid_{platform}", $"png.dta_dta_{platform}", $"png_{platform}" })
                {
                    var donorFile = Path.Combine(paths.Songs, "tut0", $"tut0.{ext}");
                    var destFile = Path.Combine(paths.Songs, songName, $"{songName}.{ext}");

                    if (!File.Exists(destFile))
                    {
                        File.Copy(donorFile, destFile);
                    }
                }

                var moggSong = MoggSong.FromMoggsong(Path.Combine(songPath, $"{songName}.moggsong"));
                moggSong.MoggPath = $"{songName}.mogg";
                moggSong.MidiPath = $"{songName}.mid";
                var dtx = moggSong.ToDtx();

                using (var dtbStream = File.Create(Path.Combine(songPath, $"{songName}.moggsong_dta_{platform}")))
                {
                    DTX.ToDtb(dtx, dtbStream, 3, false);
                }

                Midi.PatchMidi(Path.Combine(songPath, $"{songName}.mid_{platform}"), (float)moggSong.Bpm, consoleType);

                configHelper.SetFunc(dtx =>
                {
                    var dbNode = dtx.FindArrayByChild("db").FirstOrDefault();
                    var unlockTokensNode = dbNode.FindArrayByChild("unlock_tokens").FirstOrDefault();
                    var campaignNode = dbNode.FindArrayByChild("campaign").FirstOrDefault();

                    if (dbNode == null)
                    {
                        throw new DtxException("db node not found in amp_config");
                    }

                    if (unlockTokensNode == null)
                    {
                        throw new DtxException("unlock_tokens node not found in amp_config");
                    }

                    if (campaignNode == null)
                    {
                        throw new DtxException("campaign node not found in amp_config");
                    }

                    var unlockTokenNode = new DataArray();
                    unlockTokenNode.AddNode(DataSymbol.Symbol(targetToken));
                    unlockTokenNode.AddNode(DataSymbol.Symbol(moggSong.Title));
                    unlockTokenNode.AddNode(DataSymbol.Symbol("unlock_extra"));
                    unlockTokenNode.AddNode(DataSymbol.Symbol("unlock_extra_desc"));
                    unlockTokenNode.AddNode(DataSymbol.Symbol("ui/textures/black_square.png"));
                    unlockTokenNode.AddNode(DataSymbol.Symbol("CAMPVO_song_extra"));
                    unlockTokensNode.AddNode(unlockTokenNode);

                    var campaignUnlockNode = new DataArray();
                    campaignUnlockNode.AddNode(DataSymbol.Symbol("beat_num"));
                    campaignUnlockNode.AddNode(new DataAtom(0));
                    campaignUnlockNode.AddNode(DataSymbol.Symbol("kUnlockArena"));
                    campaignUnlockNode.AddNode(DataSymbol.Symbol(targetToken));
                    campaignNode.AddNode(campaignUnlockNode);

                    return null;
                }).Run();

                songsConfigHelper.SetFunc(dtx =>
                {
                    var node = dtx.Children.OfType<DataArray>().Where(e => e.Children.Count > 1 && e[0].ToString().StartsWith("World")).OrderBy(e => e.Children.Count).First();
                    var songNode = new DataArray();
                    songNode.AddNode(DataSymbol.Symbol(targetToken));
                    songNode.AddNode(new DataInclude($"../Songs/{songName}/{songName}.moggsong"));
                    var typeNode = new DataArray();
                    typeNode.AddNode(DataSymbol.Symbol("type"));
                    typeNode.AddNode(DataSymbol.Symbol("kSongExtra"));
                    songNode.AddNode(typeNode);
                    node.AddNode(songNode);

                    return null;
                }).Run();

                Log?.Invoke($"Added {songName}", index, songNames.Length);
            }

            configHelper.Rebuild().Dispose();
            songsConfigHelper.Rebuild().Dispose();

            Log?.Invoke("Rebuilt config files", 1, 1);
        }
        #endregion

        #region Remove Song
        /// <summary>
        /// Renives a song from the game data.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="songName">The name of the song to remove.</param>
        /// <param name="delete">If the song path should be deleted from the game files.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void RemoveSong(string unpackedPath, string songName, bool delete, ProgressAction Log = null) => RemoveSong(new DirectoryInfo(unpackedPath), songName, delete, HelperMethods.ConsoleTypeFromPath(unpackedPath, GamePathType.Unpacked), Log);

        /// <summary>
        /// Renives a song from the game data.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="songName">The name of the song to remove.</param>
        /// <param name="delete">If the song path should be deleted from the game files.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void RemoveSong(string unpackedPath, string songName, bool delete, ConsoleType consoleType, ProgressAction Log = null) => RemoveSong(new DirectoryInfo(unpackedPath), songName, delete, consoleType, Log);

        /// <summary>
        /// Renives a song from the game data.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="songName">The name of the song to remove.</param>
        /// <param name="delete">If the song path should be deleted from the game files.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void RemoveSong(DirectoryInfo unpackedPath, string songName, bool delete, ProgressAction Log = null) => RemoveSong(unpackedPath, songName, delete, HelperMethods.ConsoleTypeFromPath(unpackedPath.FullName, GamePathType.Unpacked), Log);

        /// <summary>
        /// Renives a song from the game data.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="songName">The name of the song to remove.</param>
        /// <param name="delete">If the song path should be deleted from the game files.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void RemoveSong(DirectoryInfo unpackedPath, string songName, bool delete, ConsoleType consoleType, ProgressAction Log = null) => RemoveSong(unpackedPath, new string[] { songName }, delete, consoleType, Log);

        /// <summary>
        /// Renives a series of songs from the game data.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="songNames">The namse of the songs to remove.</param>
        /// <param name="delete">If the song path should be deleted from the game files.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void RemoveSong(DirectoryInfo unpackedPath, string[] songNames, bool delete, ConsoleType consoleType, ProgressAction Log = null)
        {

            var paths = new AmplitudePaths(unpackedPath.FullName, consoleType);

            if (unpackedPath == null)
            {
                throw new ArgumentNullException(nameof(unpackedPath));
            }

            if (!unpackedPath.Exists)
            {
                throw new DirectoryNotFoundException(unpackedPath.FullName);
            }

            if (!File.Exists(paths.Config))
            {
                throw new FileNotFoundException(paths.Config);
            }

            if (!File.Exists(paths.SongsConfig))
            {
                throw new FileNotFoundException(paths.SongsConfig);
            }

            var songs = GetSongs(unpackedPath, consoleType);

            DtxFileHelper<object> configHelper = new DtxFileHelper<object>(paths.Config);
            DtxFileHelper<object> songConfigHelper = new DtxFileHelper<object>(paths.SongsConfig);

            foreach (var (songName, index) in songNames.Select((item, index) => (item, index)))
            {
                if (string.IsNullOrEmpty(songName))
                {
                    throw new ArgumentNullException(nameof(songName));
                }

                var targetToken = songs.Where(song => song.ID == songName)?.FirstOrDefault()?.NodeId ?? songName.ToUpper();

                configHelper.SetFunc(dtx =>
                {
                    var dbNode = dtx.FindArrayByChild("db").FirstOrDefault();
                    var unlockTokensNode = dbNode.FindArrayByChild("unlock_tokens").FirstOrDefault();
                    var campaignNode = dbNode.FindArrayByChild("campaign").FirstOrDefault();

                    unlockTokensNode.DeleteDataArraysByValue(targetToken);
                    campaignNode.DeleteDataArraysByValue(3, targetToken);

                    return null;
                }).Run();

                songConfigHelper.SetFunc(dtx =>
                {
                    foreach (var world in dtx.Children.OfType<DataArray>().Where(e => e.Children.Count > 1))
                    {
                        world.DeleteDataArraysByValue(targetToken);
                    }

                    return null;
                }).Run();

                if (delete && Directory.Exists(Path.Combine(paths.Songs, songName)))
                {
                    Directory.Delete(Path.Combine(paths.Songs, songName), true);
                }

                Log?.Invoke($"Removed {songName}", index, songNames.Length);
            }

            configHelper.Rebuild().Dispose();
            songConfigHelper.Rebuild().Dispose();

            Log?.Invoke("Rebuilt config files", 1, 1);
        }
        #endregion

        #region Get Songs
        /// <summary>
        /// Gets a list of songs from the game data.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <returns>An array of <see cref="MoggSong"/> objects.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DtxException"></exception>
        public static MoggSong[] GetSongs(string unpackedPath, ProgressAction Log = null) => GetSongs(new DirectoryInfo(unpackedPath), HelperMethods.ConsoleTypeFromPath(unpackedPath, GamePathType.Unpacked), Log);

        /// <summary>
        /// Gets a list of songs from the game data.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="consoleType">The console type</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <returns>An array of <see cref="MoggSong"/> objects.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DtxException"></exception>
        public static MoggSong[] GetSongs(string unpackedPath, ConsoleType consoleType, ProgressAction Log = null) => GetSongs(new DirectoryInfo(unpackedPath), consoleType, Log);

        /// <summary>
        /// Gets a list of songs from the game data.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <returns>An array of <see cref="MoggSong"/> objects.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DtxException"></exception>
        public static MoggSong[] GetSongs(DirectoryInfo unpackedPath, ProgressAction Log = null) => GetSongs(unpackedPath, HelperMethods.ConsoleTypeFromPath(unpackedPath.FullName), Log);

        /// <summary>
        /// Gets a list of songs from the game data.
        /// </summary>
        /// <param name="unpackedPath">Path to the unpacked files.</param>
        /// <param name="consoleType">The console type</param>
        /// <param name="Log">A <see cref="ProgressAction"/> for tracking progress.</param>
        /// <returns>An array of <see cref="MoggSong"/> objects.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="DtxException"></exception>
        public static MoggSong[] GetSongs(DirectoryInfo unpackedPath, ConsoleType consoleType, ProgressAction Log = null)
        {
            var songs = new List<MoggSong>();
            var paths = new AmplitudePaths(unpackedPath.FullName, consoleType);

            if (unpackedPath == null)
            {
                throw new ArgumentNullException(nameof(unpackedPath));
            }

            if (!unpackedPath.Exists)
            {
                throw new DirectoryNotFoundException(unpackedPath.FullName);
            }

            if (!File.Exists(paths.Config))
            {
                throw new FileNotFoundException(paths.Config);
            }

            if (!File.Exists(paths.SongsConfig))
            {
                throw new FileNotFoundException(paths.SongsConfig);
            }

            DataArray ampConfig = new DtxFileHelper<DataArray>(paths.Config, e => e).Run(true).ReturnValue;
            DataArray songsConfig = new DtxFileHelper<DataArray>(paths.SongsConfig, e => e).Run(true).ReturnValue;

            var dbNode = ampConfig.FindArrayByChild("db").FirstOrDefault();
            var unlockTokensNode = dbNode.FindArrayByChild("unlock_tokens");
            var campaignNode = dbNode.FindArrayByChild("campaign");
            var moggSongFiles = new List<string>();

            if (dbNode == null)
            {
                throw new DtxException("db node not found in amp_config");
            }

            if (unlockTokensNode == null)
            {
                throw new DtxException("unlock_tokens node not found in amp_config");
            }

            if (campaignNode == null)
            {
                throw new DtxException("campaign node not found in amp_config");
            }

            foreach (var world in songsConfig.Children.OfType<DataArray>().Where(e => e.Children.Count > 1))
            {
                foreach (var songNode in world.Children.Skip(1).OfType<DataArray>().Where(e => e.Children.Count > 1))
                {
                    var moggSongPath = Path.GetFullPath(Path.Combine(unpackedPath.FullName, consoleType.ToString().ToLower(), "config", songNode.GetChild<DataInclude>(1).Constant));

                    if (!File.Exists(moggSongPath))
                    {
                        throw new FileNotFoundException(moggSongPath);
                    }

                    moggSongFiles.Add(moggSongPath.ToLower());
                    var id = Path.GetFileNameWithoutExtension(moggSongPath);
                    var song = MoggSong.FromMoggsong(Path.Combine(paths.Songs, id, $"{id}.moggsong"));
                    song.NodeId = songNode.GetChild<string>(0);
                    song.BaseSong = AmplitudeData.BaseSongs.Contains(Path.GetFileNameWithoutExtension(moggSongPath).ToLower());
                    song.InGame = true;
                    song.Special = id == "credits" || id == "tut0" || id == "tut1" || id == "tutc";

                    songs.Add(song);
                }
            }

            foreach (var directory in new DirectoryInfo(paths.Songs).GetDirectories())
            {
                var moggSongPath = Path.Combine(directory.FullName, $"{directory.Name}.moggsong");

                if (!File.Exists(moggSongPath))
                {
                    continue;
                }

                var id = Path.GetFileNameWithoutExtension(moggSongPath);
                if (directory.Name == "credits" || moggSongFiles.Contains(moggSongPath.ToLower()))
                {
                    continue;
                }

                var song = MoggSong.FromMoggsong(moggSongPath);

                song.BaseSong = AmplitudeData.BaseSongs.Contains(Path.GetFileNameWithoutExtension(moggSongPath).ToLower());
                song.InGame = false;
                song.Special = id == "credits" || id == "tut0" || id == "tut1" || id == "tutc";

                songs.Add(song);
            }

            var outputSongs = songs.DistinctBy(e => e.MoggSongPath.ToLower()).OrderBy(e => e.MoggPath).ToArray();
            Log?.Invoke(null, outputSongs.Length, outputSongs.Length);

            return outputSongs;
        }
        #endregion
    }
}
