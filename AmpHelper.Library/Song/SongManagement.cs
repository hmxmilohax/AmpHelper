using AmpHelper.Library.Delegates;
using AmpHelper.Library.Enums;
using AmpHelper.Library.Extensions;
using AmpHelper.Library.Helpers;
using AmpHelper.Library.Types;
using DtxCS.DataTypes;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AmpHelper.Library.Song
{
    public static class SongManagement
    {

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

            var ampConfigDta = ampConfig.ToDta();
            var songsConfigDta = songsConfig.ToDta();
            var dbNode = ampConfig.FindArrayByChild("db").FirstOrDefault();
            var unlockTokensNode = dbNode.FindArrayByChild("unlock_tokens");
            var campaignNode = dbNode.FindArrayByChild("campaign");
            var moggSongFiles = new List<string>();

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
    }
}
