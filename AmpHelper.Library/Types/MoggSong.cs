using AmpHelper.Library.Extensions;
using AmpHelper.Library.Helpers;
using DtxCS;
using DtxCS.DataTypes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace AmpHelper.Library.Types
{
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
    public class MoggSong
    {
        public class Track
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("channels")]
            public List<int> Channels { get; set; }

            [JsonPropertyName("event")]
            public string Event { get; set; }
        }

        [JsonIgnore]
        public string ID => Path.GetFileNameWithoutExtension(MoggPath ?? MidiPath);


        [JsonPropertyName("mogg_path")]
        public string MoggPath { get; set; }

        [JsonPropertyName("midi_path")]
        public string MidiPath { get; set; }

        /// <summary>
        /// Should be 5 less than actual.
        /// </summary>
        [JsonPropertyName("length")]
        public int? Length { get; set; }

        [JsonPropertyName("countin")]
        public int? CountIn { get; set; }

        [JsonPropertyName("tracks")]
        public List<Track> Tracks { get; set; }

        [JsonPropertyName("pans")]
        public List<double> Pans { get; set; }

        [JsonPropertyName("vols")]
        public List<double> Volumes { get; set; }

        [JsonPropertyName("active_track_db")]
        public List<double> Attenuation { get; internal set; }

        [JsonPropertyName("arena_path")]
        public string ArenaPath { get; set; }

        /// <summary>
        /// Fudge factor controlling speed of arena travel
        /// </summary>
        [JsonPropertyName("tunnel_scale")]
        public double? TunnelScale { get; set; }

        [JsonPropertyName("enable_order")]
        public List<int> EnableOrder { get; internal set; }

        [JsonPropertyName("section_start_bars")]
        public List<int> SectionStartBars { get; internal set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("title_short")]
        public string ShortTitle { get; set; }

        [JsonPropertyName("artist")]
        public string Artist { get; set; }

        [JsonPropertyName("artist_short")]
        public string ShortArtist { get; set; }

        [JsonPropertyName("desc")]
        public string Description { get; set; }

        [JsonPropertyName("unlocK_requirement")]
        public string UnlockRequirement { get; set; }

        [JsonPropertyName("bpm")]
        public double? Bpm { get; set; }

        [JsonPropertyName("charter")]
        public string Charter { get; set; }

        [JsonPropertyName("demo_video")]
        public string DemoVideo { get; set; }

        [JsonPropertyName("preview_start_ms")]
        public int? PreviewStart { get; set; }

        [JsonPropertyName("preview_length_ms")]
        public int? PreviewLength { get; set; }

        [JsonPropertyName("boss_level")]
        public int? BossLevel { get; set; }

        [JsonIgnore()]
        public byte[] DtxBytes { get; set; }

        /// <summary>
        /// The unlock token used in the config dta files.
        /// </summary>
        [JsonPropertyName("node_id")]
        public string NodeId { get; internal set; }

        /// <summary>
        /// The path to the mogg file when parsed with <see cref="MoggSong.FromMoggsong(string)"/> or <see cref="MoggSong.FromMoggsong(FileInfo)"/>.
        /// </summary>
        [JsonPropertyName("moggsong_path")]
        public string MoggSongPath { get; internal set; }

        /// <summary>
        /// If the song is included with the game.
        /// </summary>
        [JsonPropertyName("base_song")]
        public bool BaseSong { get; internal set; } = false;

        /// <summary>
        /// If the song has been added to the game
        /// </summary>
        [JsonPropertyName("in_game")]
        public bool InGame { get; internal set; } = false;

        /// <summary>
        /// Is this a special track? (tutorial, credits)
        /// </summary>
        [JsonPropertyName("special")]
        public bool Special { get; internal set; } = false;

        public MoggSong()
        {
            Tracks = new List<Track>();
            Pans = new List<double>();
            Volumes = new List<double>();
            Attenuation = new List<double>();
            EnableOrder = new List<int>();
            SectionStartBars = new List<int>();
        }

        /// <summary>
        /// Initializes the class from a <see cref="DataArray"/>.
        /// </summary>
        /// <param name="dtx">The input <see cref="DataArray"/>.</param>
        public MoggSong(DataArray dtx) : base()
        {
            MoggPath = dtx.FindArrayByChild("mogg_path").FirstOrDefault()?.GetChild<string>(1);
            MidiPath = dtx.FindArrayByChild("midi_path").FirstOrDefault()?.GetChild<string>(1);
            Length = int.Parse(dtx.FindArrayByChild("song_info").FirstOrDefault()?.FindArrayByChild("length").FirstOrDefault().GetChild<string>(1)?.Split(":").FirstOrDefault() ?? "0");
            CountIn = int.Parse(dtx.FindArrayByChild("song_info").FirstOrDefault()?.FindArrayByChild("countin").FirstOrDefault()?.GetChild<string>(1) ?? "0");
            Tracks = dtx.FindArrayByChild("tracks").FirstOrDefault()?.GetChild(1, trackDtx =>
            {
                var tracks = new List<Track>();

                foreach (DataArray node in (trackDtx as DataArray).Children)
                {
                    tracks.Add(new Track()
                    {
                        Name = node.GetChild<string>(0),
                        Channels = node.GetChild<DataArray>(1)?.Transform(e =>
                        {
                            var child = (DataArray)e;

                            return child.Children.Select(chan => int.Parse(chan.ToString())).ToList();
                        }) ?? new List<int>(),
                        Event = node.GetChild<string>(2)
                    });
                }

                return tracks;
            }) ?? new List<Track>();
            Pans = dtx.FindArrayByChild("pans").FirstOrDefault()?.GetChild<DataArray>(1)?.Children?.Select(e => e.ToString().Transform(o => double.Parse((string)o), 0)).ToList() ?? new List<double>();
            Volumes = dtx.FindArrayByChild("vols").FirstOrDefault()?.GetChild<DataArray>(1)?.Children?.Select(e => e.ToString().Transform(o => double.Parse((string)o), 0)).ToList() ?? new List<double>();
            Attenuation = dtx.FindArrayByChild("active_track_db").FirstOrDefault()?.Children.Skip(1).Select(e => e.ToString().Transform(o => double.Parse((string)o), 0))?.ToList() ?? new List<double>();
            ArenaPath = dtx.FindArrayByChild("arena_path").FirstOrDefault()?.GetChild<string>(1);
            TunnelScale = dtx.FindArrayByChild("tunnel_scale").FirstOrDefault()?.GetChild<double>(1, e => double.Parse(e.ToString()));
            EnableOrder = dtx.FindArrayByChild("enable_order").FirstOrDefault()?.GetChild<DataArray>(1)?.Children.Select(e => e.ToString().Transform(o => int.Parse((string)o), 0)).ToList() ?? new List<int>();
            SectionStartBars = dtx.FindArrayByChild("section_start_bars").FirstOrDefault()?.Children.Skip(1)?.Select(e => e.ToString().Transform(o => int.Parse((string)o), 0))?.ToList() ?? new List<int>();
            Title = dtx.FindArrayByChild("title").FirstOrDefault()?.GetChild<string>(1);
            ShortTitle = dtx.FindArrayByChild("title_short").FirstOrDefault()?.GetChild<string>(1);
            Artist = dtx.FindArrayByChild("artist").FirstOrDefault()?.GetChild<string>(1);
            ShortArtist = dtx.FindArrayByChild("artist_short").FirstOrDefault()?.GetChild<string>(1);
            Description = dtx.FindArrayByChild("desc").FirstOrDefault()?.GetChild<string>(1);
            UnlockRequirement = dtx.FindArrayByChild("unlock_requirement").FirstOrDefault()?.GetChild<string>(1);
            Bpm = dtx.FindArrayByChild("bpm").FirstOrDefault()?.GetChild<double>(1, e => double.Parse(e.ToString()));
            Charter = dtx.FindArrayByChild("charter").FirstOrDefault()?.GetChild<string>(1)?.Trim();
            DemoVideo = dtx.FindArrayByChild("demo_video").FirstOrDefault()?.GetChild<string>(1)?.Trim();
            PreviewStart = dtx.FindArrayByChild("preview_start_ms").FirstOrDefault()?.GetChild<int>(1, e => int.Parse(e.ToString()));
            PreviewLength = dtx.FindArrayByChild("preview_length_ms").FirstOrDefault()?.GetChild<int>(1, e => int.Parse(e.ToString()));
            BossLevel = dtx.FindArrayByChild("boss_level").FirstOrDefault()?.GetChild<int>(1, e => int.Parse(e.ToString()));

            using (var ms = new MemoryStream())
            {
                DTX.ToDtb(dtx, ms, 3, false);
                ms.Position = 0;
                DtxBytes = ms.ToArray();
            }
        }

        /// <summary>
        /// Returns an instance of <see cref="MoggSong"/> initialized from a dta string.
        /// </summary>
        /// <param name="dta">The dta string.</param>
        /// <returns></returns>
        public static MoggSong FromMoggsongString(string dta) => FromDtx(DtxCS.DTX.FromDtaString(dta));

        /// <summary>
        /// Returns an instance of <see cref="MoggSong"/> initialized from a dta/dtb file.
        /// </summary>
        /// <param name="path">The dta/dtb file path.</param>
        /// <returns></returns>
        public static MoggSong FromMoggsong(string path)
        {
            var song = FromMoggsong(File.OpenRead(path), true);
            song.MoggSongPath = Path.GetFullPath(path);

            return song;
        }

        /// <summary>
        /// Returns an instance of <see cref="MoggSong"/> initialized from a dta/dtb file.
        /// </summary>
        /// <param name="path">The dta/dtb <see cref="FileInfo"/>.</param>
        /// <returns></returns>
        public static MoggSong FromMoggsong(FileInfo path)
        {
            var song = FromMoggsong(path.OpenRead(), true);
            song.MoggSongPath = Path.GetFullPath(path.FullName);

            return song;
        }

        /// <summary>
        /// Returns an instance of <see cref="MoggSong"/> initialized from a dta/dtb stream.
        /// </summary>
        /// <param name="dtxStream">A seekable stream with dta/dtb data.</param>
        /// <param name="dispose">Whether or not to dispose the stream.</param>
        /// <returns></returns>
        public static MoggSong FromMoggsong(Stream dtxStream, bool dispose = false) => new DtxStreamHelper<MoggSong>(dtxStream, (dtx) => new MoggSong(dtx)).Run(dispose).ReturnValue;

        /// <summary>
        /// Returns an instance of <see cref="MoggSong"/> initialized from a DataArray.
        /// </summary>
        /// <param name="dtx"></param>
        /// <returns></returns>
        public static MoggSong FromDtx(DataArray dtx) => new MoggSong(dtx);

        /// <summary>
        /// Helper function for <see cref="ToDtx"/>.
        /// 
        /// Returns potentialNode if it exists and data is not null.  If potentialNode is null but data isn't, return a <see cref="DataArray"/> that was appended to the input <see cref="DataArray"/>.
        /// 
        /// If potentialNode is not null but data is, potentialNode is removed from the input <see cref="DataArray"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dtx">The input <see cref="DataArray"/>.</param>
        /// <param name="name">The name of the first <see cref="DataSymbol"/> to be appended to the new node if potentialNode is null.</param>
        /// <param name="potentialNode">The potential <see cref="DataArray"/>.</param>
        /// <param name="data">The data to check if null.</param>
        /// <returns>If data is false, null.  If data is not null, the potentialNode is returned, or a new <see cref="DataArray"/> is created/appended and returned with the first element being a <see cref="DataSymbol"/> specified by the name parameter.</returns>
        private DataArray GetOrDeleteArray<T>(DataArray dtx, string name, DataArray potentialNode, T data)
        {
            if (dtx is null)
            {
                return null;
            }

            if (data is null)
            {
                if (potentialNode != null)
                {
                    dtx.Children.Remove(potentialNode);
                }

                return null;
            }

            if (potentialNode == null && data != null)
            {
                var newNode = new DataArray();
                newNode.AddNode(DataSymbol.Symbol(name));
                dtx.AddNode(newNode);

                return newNode;
            }

            return potentialNode;
        }

        /// <summary>
        /// Converts the instance to a DataArray.
        /// </summary>
        /// <returns></returns>
        public DataArray ToDtx()
        {
            DataArray dtx;

            if (DtxBytes != null)
            {
                using (var ms = new MemoryStream(DtxBytes))
                {
                    dtx = DTX.FromDtb(ms);
                }

                foreach (var child in dtx.Children.OfType<DataArray>().Where(e => e.Children.Count >= 1))
                {
                    switch (child.Children[0].ToString())
                    {

                    }
                }
            }
            else
            {
                dtx = new DataArray();
            }

            var knownNodes = new MoggSongNodes(dtx);

            DataArray node;

            if ((node = GetOrDeleteArray(dtx, "mogg_path", knownNodes.MoggPath, MoggPath)) != null)
            {
                node.RemoveAllAfter(0).AddNode(DataSymbol.Symbol(MoggPath));
            }

            if ((node = GetOrDeleteArray(dtx, "midi_path", knownNodes.MidiPath, MidiPath)) != null)
            {
                node.RemoveAllAfter(0).AddNode(DataSymbol.Symbol(MidiPath));
            }

            if ((node = GetOrDeleteArray(dtx, "song_info", knownNodes.SongInfo, Length.HasValue || CountIn.HasValue ? "" : null)) != null)
            {
                DataArray innerNode;

                if ((innerNode = GetOrDeleteArray(node, "length", knownNodes.Length, Length)) != null)
                {
                    innerNode.AddNode(DataSymbol.Symbol($"{Length.Value}:0:0"));
                }

                if ((innerNode = GetOrDeleteArray(node, "countin", knownNodes.CountIn, Length)) != null)
                {
                    innerNode.AddNode(new DataAtom(CountIn.Value));
                }
            }

            if ((node = GetOrDeleteArray(dtx, "tracks", knownNodes.Tracks, Tracks)) != null)
            {
                if (Tracks.Count == 0)
                {
                    dtx.Children.Remove(node);
                }
                else
                {
                    node.RemoveAllAfter(0);

                    var array = new DataArray();

                    foreach (var track in Tracks)
                    {
                        var trackArray = new DataArray();
                        trackArray.AddNode(DataSymbol.Symbol(track.Name));

                        var channelsArray = new DataArray();

                        foreach (var atom in track.Channels.Select(e => new DataAtom(e)))
                        {
                            channelsArray.AddNode(atom);
                        }

                        trackArray.AddNode(channelsArray);

                        if (track.Event != null)
                        {
                            trackArray.AddNode(DataSymbol.Symbol(track.Event));
                        }

                        array.AddNode(trackArray);
                    }

                    node.AddNode(array);
                }
            }

            if ((node = GetOrDeleteArray(dtx, "pans", knownNodes.Pans, Pans)) != null)
            {
                if (Pans.Count == 0)
                {
                    dtx.Children.Remove(node);
                }
                else
                {
                    node.RemoveAllAfter(0);

                    var array = new DataArray();

                    foreach (var atom in Pans.Select(e => new DataAtom((float)e)))
                    {
                        array.AddNode(atom);
                    }

                    node.AddNode(array);
                }
            }

            if ((node = GetOrDeleteArray(dtx, "vols", knownNodes.Vols, Volumes)) != null)
            {
                if (Volumes.Count == 0)
                {
                    dtx.Children.Remove(node);
                }
                else
                {
                    node.RemoveAllAfter(0);

                    var array = new DataArray();

                    foreach (var atom in Volumes.Select(e => new DataAtom((float)e)))
                    {
                        array.AddNode(atom);
                    }

                    node.AddNode(array);
                }
            }

            if ((node = GetOrDeleteArray(dtx, "active_track_db", knownNodes.Attenuation, Attenuation)) != null)
            {
                if (Attenuation.Count == 0)
                {
                    dtx.Children.Remove(node);
                }
                else
                {
                    node.RemoveAllAfter(0);

                    foreach (var atom in Attenuation.Select(e => new DataAtom((float)e)))
                    {
                        node.AddNode(atom);
                    }
                }
            }

            if ((node = GetOrDeleteArray(dtx, "arena_path", knownNodes.ArenaPath, ArenaPath)) != null)
            {
                node.RemoveAllAfter(0).AddNode(DataSymbol.Symbol(ArenaPath));
            }

            if ((node = GetOrDeleteArray(dtx, "tunnel_scale", knownNodes.TunnelScale, TunnelScale)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom((float)TunnelScale));
            }

            if ((node = GetOrDeleteArray(dtx, "enable_order", knownNodes.EnableOrder, EnableOrder)) != null)
            {
                if (EnableOrder.Count == 0)
                {
                    dtx.Children.Remove(node);
                }
                else
                {
                    node.RemoveAllAfter(0);

                    var array = new DataArray();

                    foreach (var pan in EnableOrder.Select(e => new DataAtom(e)))
                    {
                        array.AddNode(pan);
                    }

                    node.AddNode(array);
                }
            }

            if ((node = GetOrDeleteArray(dtx, "section_start_bars", knownNodes.SectionStartBars, SectionStartBars)) != null)
            {
                if (SectionStartBars.Count == 0)
                {
                    dtx.Children.Remove(node);
                }
                else
                {
                    node.RemoveAllAfter(0);

                    foreach (var atom in SectionStartBars.Select(e => new DataAtom(e)))
                    {
                        node.AddNode(atom);
                    }
                }
            }

            if ((node = GetOrDeleteArray(dtx, "title", knownNodes.Title, Title)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(Title));
            }

            if ((node = GetOrDeleteArray(dtx, "title_short", knownNodes.ShortTitle, ShortTitle)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(ShortTitle));
            }

            if ((node = GetOrDeleteArray(dtx, "artist", knownNodes.Artist, Artist)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(Artist));
            }

            if ((node = GetOrDeleteArray(dtx, "artist_short", knownNodes.ShortArtist, ShortArtist)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(ShortArtist));
            }

            if ((node = GetOrDeleteArray(dtx, "desc", knownNodes.Description, Description)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(Description));
            }

            if ((node = GetOrDeleteArray(dtx, "unlock_requirement", knownNodes.UnlockRequirement, UnlockRequirement)) != null)
            {
                node.RemoveAllAfter(0).AddNode(DataSymbol.Symbol(UnlockRequirement));
            }

            if ((node = GetOrDeleteArray(dtx, "bpm", knownNodes.Bpm, Bpm)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom((float)Bpm.Value));
            }

            if ((node = GetOrDeleteArray(dtx, "preview_start_ms", knownNodes.PreviewStartMs, PreviewStart)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(PreviewStart.Value));
            }

            if ((node = GetOrDeleteArray(dtx, "preview_length_ms", knownNodes.PreviewLengthMs, PreviewLength)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(PreviewLength.Value));
            }

            if ((node = GetOrDeleteArray(dtx, "boss_level", knownNodes.BossLevel, BossLevel)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(BossLevel.Value));
                ;
            }

            if ((node = GetOrDeleteArray(dtx, "charter", knownNodes.Charter, Charter)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(Charter));
            }

            if ((node = GetOrDeleteArray(dtx, "demo_video", knownNodes.DemoVideo, DemoVideo)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(DemoVideo));
            }

            return dtx;
        }

        public static implicit operator DataArray(MoggSong song) => song.ToDtx();
        public static explicit operator MoggSong(DataArray dtx) => MoggSong.FromDtx(dtx);
    }
}
