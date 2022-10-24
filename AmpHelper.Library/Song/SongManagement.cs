using AmpHelper.Library.Delegates;
using AmpHelper.Library.Enums;
using AmpHelper.Library.Extensions;
using AmpHelper.Library.Helpers;
using AmpHelper.Library.Types;
using DtxCS.DataTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AmpHelper.Library.Song
{
    public static class SongManagement
    {
        #region Remove Song
        public static void RemoveSong(string unpackedPath, string songName, bool delete, ProgressAction Log = null) => RemoveSong(new DirectoryInfo(unpackedPath), songName, delete, HelperMethods.ConsoleTypeFromPath(unpackedPath, GamePathType.Unpacked), Log);
        public static void RemoveSong(string unpackedPath, string songName, bool delete, ConsoleType consoleType, ProgressAction Log = null) => RemoveSong(new DirectoryInfo(unpackedPath), songName, delete, consoleType, Log);
        public static void RemoveSong(DirectoryInfo unpackedPath, string songName, bool delete, ProgressAction Log = null) => RemoveSong(unpackedPath, songName, delete, HelperMethods.ConsoleTypeFromPath(unpackedPath.FullName, GamePathType.Unpacked), Log);
        public static void RemoveSong(DirectoryInfo unpackedPath, string songName, bool delete, ConsoleType consoleType, ProgressAction Log = null)
        {
            var configPath = Path.Combine(unpackedPath.FullName, consoleType.ToString().ToLower(), "config", $"amp_config.dta_dta_{consoleType.ToString().ToLower()}");
            var songsConfigPath = Path.Combine(unpackedPath.FullName, consoleType.ToString().ToLower(), "config", $"amp_songs_config.dta_dta_{consoleType.ToString().ToLower()}");
            var songsPath = Path.Combine(unpackedPath.FullName, consoleType.ToString().ToLower(), "songs");

            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException(configPath);
            }

            if (!File.Exists(songsConfigPath))
            {
                throw new FileNotFoundException(songsConfigPath);
            }

            var songs = ListSongs(unpackedPath, consoleType);

            var targetToken = songs.Where(song => song.ID == songName)?.FirstOrDefault().NodeId ?? songName.ToUpper();

            new DtxFileHelper<object>(configPath, dtx =>
            {
                var dbNode = dtx.FindArrayByChild("db").FirstOrDefault();
                var unlockTokensNode = dbNode.FindArrayByChild("unlock_tokens").FirstOrDefault();
                var campaignNode = dbNode.FindArrayByChild("campaign").FirstOrDefault();

                unlockTokensNode.DeleteDataArraysByValue(targetToken);
                campaignNode.DeleteDataArraysByValue(3, targetToken);

                return null;
            }).Run().Rebuild().Dispose();

            new DtxFileHelper<object>(songsConfigPath, dtx =>
            {
                foreach (var world in dtx.Children.OfType<DataArray>().Where(e => e.Children.Count > 1))
                {
                    world.DeleteDataArraysByValue(targetToken);
                }

                return null;
            }).Run().Rebuild().Dispose();

            if (delete && Directory.Exists(Path.Combine(songsPath, songName)))
            {
                Directory.Delete(Path.Combine(songsPath, songName), true);
            }
        }
        #endregion

        #region List Songs
        public static MoggSong[] ListSongs(string unpackedPath, ProgressAction Log = null) => ListSongs(new DirectoryInfo(unpackedPath), HelperMethods.ConsoleTypeFromPath(unpackedPath, GamePathType.Unpacked), Log);
        public static MoggSong[] ListSongs(string unpackedPath, ConsoleType consoleType, ProgressAction Log = null) => ListSongs(new DirectoryInfo(unpackedPath), consoleType, Log);
        public static MoggSong[] ListSongs(DirectoryInfo unpackedPath, ProgressAction Log = null) => ListSongs(unpackedPath, HelperMethods.ConsoleTypeFromPath(unpackedPath.FullName), Log);
        public static MoggSong[] ListSongs(DirectoryInfo unpackedPath, ConsoleType consoleType, ProgressAction Log = null)
        {
            var songs = new List<MoggSong>();
            var configPath = Path.Combine(unpackedPath.FullName, consoleType.ToString().ToLower(), "config", $"amp_config.dta_dta_{consoleType.ToString().ToLower()}");
            var songsConfigPath = Path.Combine(unpackedPath.FullName, consoleType.ToString().ToLower(), "config", $"amp_songs_config.dta_dta_{consoleType.ToString().ToLower()}");
            var songsPath = Path.Combine(unpackedPath.FullName, consoleType.ToString().ToLower(), "songs");

            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException(configPath);
            }

            if (!File.Exists(songsConfigPath))
            {
                throw new FileNotFoundException(songsConfigPath);
            }

            DataArray ampConfig = new DtxFileHelper<DataArray>(configPath, e => e).Run(true).ReturnValue;
            DataArray songsConfig = new DtxFileHelper<DataArray>(songsConfigPath, e => e).Run(true).ReturnValue;

            var dbNode = ampConfig.FindArrayByChild("db").FirstOrDefault();
            var unlockTokensNode = dbNode.FindArrayByChild("unlock_tokens");
            var campaignNode = dbNode.FindArrayByChild("campaign");
            var moggSongFiles = new List<string>();

            if (dbNode == null)
            {
                throw new InvalidDataException("db node not found in amp_config");
            }

            if (unlockTokensNode == null)
            {
                throw new InvalidDataException("unlock_tokens node not found in amp_config");
            }

            if (campaignNode == null)
            {
                throw new InvalidDataException("campaign node not found in amp_config");
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
                    var song = MoggSong.FromMoggsong(Path.Combine(songsPath, id, $"{id}.moggsong"));
                    song.NodeId = songNode.GetChild<string>(0);
                    song.BaseSong = AmplitudeData.BaseSongs.Contains(Path.GetFileNameWithoutExtension(moggSongPath).ToLower());
                    song.InGame = true;
                    song.Special = (id == "credits" || id == "tut0" || id == "tut1" || id == "tutc");

                    songs.Add(song);
                }
            }

            foreach (var directory in new DirectoryInfo(songsPath).GetDirectories())
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
                song.Special = (id == "credits" || id == "tut0" || id == "tut1" || id == "tutc");

                songs.Add(song);
            }

            return songs.DistinctBy(e => e.MoggSongPath.ToLower()).OrderBy(e => e.MoggPath).ToArray();
        }
        #endregion
    }
}
