using AmpHelper.Enums;
using AmpHelper.Extensions;
using AmpHelper.Helpers;
using DtxCS;
using DtxCS.DataTypes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;

namespace AmpHelper.Types
{
    /// <summary>
    /// Describes a .moggsong file.
    /// </summary>
    [JsonSourceGenerationOptions(DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault)]
    public class MoggSong
    {
        /// <summary>
        /// Describes a single track in a .moggsong file.
        /// </summary>
        public class Track
        {
            /// <summary>
            /// The name of the track
            /// </summary>
            [JsonPropertyName("name")]
            public string Name { get; set; }

            /// <summary>
            /// A list of channels.
            /// </summary>
            [JsonPropertyName("channels")]
            public List<int> Channels { get; set; }

            /// <summary>
            /// The track event.
            /// </summary>
            [JsonPropertyName("event")]
            public string Event { get; set; }
        }

        /// <summary>
        /// The song ID based on the filename.
        /// </summary>
        [JsonIgnore]
        public string ID => Path.GetFileNameWithoutExtension(MoggSongPath ?? MoggPath ?? MidiPath);

        /// <summary>
        /// Path to the .mogg file.
        /// </summary>
        [JsonPropertyName("mogg_path")]
        public string MoggPath { get; set; }

        /// <summary>
        /// Path to the .mid file.
        /// </summary>
        [JsonPropertyName("midi_path")]
        public string MidiPath { get; set; }

        /// <summary>
        /// The number of bars in the song, should be 5 less than actual.
        /// </summary>
        [JsonPropertyName("length")]
        public int? Length { get; set; }

        /// <summary>
        /// The number of bars to count in.
        /// </summary>
        [JsonPropertyName("countin")]
        public int? CountIn { get; set; }

        /// <summary>
        /// A list of <see cref="Track"/> objects for the song.
        /// </summary>
        [JsonPropertyName("tracks")]
        public List<Track> Tracks { get; set; }

        /// <summary>
        /// A list of pans for the song.
        /// </summary>
        [JsonPropertyName("pans")]
        public List<double> Pans { get; set; }

        /// <summary>
        /// A list of volumes for the song.
        /// </summary>
        [JsonPropertyName("vols")]
        public List<double> Volumes { get; set; }

        /// <summary>
        /// A list of attenuations for the song.
        /// </summary>
        [JsonPropertyName("active_track_db")]
        public List<double> Attenuation { get; internal set; }

        /// <summary>
        /// The arena path.
        /// </summary>
        [JsonPropertyName("arena_path")]
        public string ArenaPath { get; set; }

        /// <summary>
        /// Fudge factor controlling speed of arena travel
        /// </summary>
        [JsonPropertyName("tunnel_scale")]
        public double? TunnelScale { get; set; }

        /// <summary>
        /// The order in which the tracks are enabled.
        /// </summary>
        [JsonPropertyName("enable_order")]
        public List<int> EnableOrder { get; internal set; }

        /// <summary>
        /// Where to put the sections, keep this to a max of 3.
        /// </summary>
        [JsonPropertyName("section_start_bars")]
        public List<int> SectionStartBars { get; internal set; }

        /// <summary>
        /// The title of the track as shown on the right side.
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; }

        /// <summary>
        /// The title of the track as shown in the list.
        /// </summary>
        [JsonPropertyName("title_short")]
        public string ShortTitle { get; set; }

        /// <summary>
        /// The artist of the song.
        /// </summary>
        [JsonPropertyName("artist")]
        public string Artist { get; set; }

        /// <summary>
        /// The short artist of the song, this doesn't appear to be used in-game.
        /// </summary>
        [JsonPropertyName("artist_short")]
        public string ShortArtist { get; set; }

        /// <summary>
        /// The description of the song that is displayed.
        /// </summary>
        [JsonPropertyName("desc")]
        public string Description { get; set; }

        /// <summary>
        /// The raw unlock requirement.
        /// </summary>
        [JsonPropertyName("unlock_requirement")]
        public string RawUnlockRequirement { get; set; }

        /// <summary>
        /// The unlock requirement accessed and set via the <see cref="UnlockRequirement"/> enum.
        /// </summary>
        [JsonIgnore]
        public UnlockRequirement UnlockRequirement
        {
            get
            {
                switch (RawUnlockRequirement)
                {
                    case null:
                        return UnlockRequirement.None;

                    case "unlock_requirement_boss":
                        return UnlockRequirement.Boss;

                    case "unlock_requirement_playcount":
                        return UnlockRequirement.PlayCount;

                    case "unlock_requirement_world1":
                        return UnlockRequirement.World1;

                    case "unlock_requirement_world2":
                        return UnlockRequirement.World2;

                    case "unlock_requirement_world3":
                        return UnlockRequirement.World3;

                    case "unlock_requirement_bonus":
                        return UnlockRequirement.Bonus;
                }

                return UnlockRequirement.Unknown;
            }

            set
            {
                switch (value)
                {
                    case UnlockRequirement.None:
                        RawUnlockRequirement = null;
                        break;

                    case UnlockRequirement.Boss:
                        RawUnlockRequirement = "unlock_requirement_boss";
                        break;

                    case UnlockRequirement.PlayCount:
                        RawUnlockRequirement = "unlock_requirement_playcount";
                        break;

                    case UnlockRequirement.World1:
                        RawUnlockRequirement = "unlock_requirement_world1";
                        break;

                    case UnlockRequirement.World2:
                        RawUnlockRequirement = "unlock_requirement_world2";
                        break;

                    case UnlockRequirement.World3:
                        RawUnlockRequirement = "unlock_requirement_world3";
                        break;

                    case UnlockRequirement.Bonus:
                        RawUnlockRequirement = "unlock_requirement_bonus";
                        break;

                    case UnlockRequirement.Unknown:
                        RawUnlockRequirement = null;
                        break;
                }
            }
        }

        /// <summary>
        /// The BPM of the song.
        /// </summary>
        [JsonPropertyName("bpm")]
        public double? Bpm { get; set; }

        /// <summary>
        /// Who charted the song.
        /// </summary>
        [JsonPropertyName("charter")]
        public string Charter { get; set; }

        /// <summary>
        /// A link to a video demonstrating the song.
        /// </summary>
        [JsonPropertyName("demo_video")]
        public string DemoVideo { get; set; }

        /// <summary>
        /// What point to start the preview at in miliseconds.
        /// </summary>
        [JsonPropertyName("preview_start_ms")]
        public int? PreviewStart { get; set; }

        /// <summary>
        /// How long the preview should play in miliseconds.
        /// </summary>
        [JsonPropertyName("preview_length_ms")]
        public int? PreviewLength { get; set; }

        /// <summary>
        /// The boss level of the song.  Unsure exactly what this is for.
        /// </summary>
        [JsonPropertyName("boss_level")]
        public int? BossLevel { get; set; }

        /// <summary>
        /// The bytes for the DTX file used to create this instance if created with <see cref="FromDtx(DataArray)"/>, <see cref="FromMoggsong(FileInfo)"/>, <see cref="FromMoggsong(Stream, bool)"/>, <see cref="FromMoggsong(string)"/>, or <see cref="FromMoggsongString(string)"/>.
        /// </summary>
        [JsonIgnore()]
        public byte[] DtxBytes { get; set; }

        /// <summary>
        /// The unlock token used in the config dta files.
        /// </summary>
        [JsonPropertyName("node_id")]
        public string NodeId { get; internal set; }

        /// <summary>
        /// The path to the mogg file when parsed with <see cref="MoggSong.FromMoggsong(string)"/>, or <see cref="MoggSong.FromMoggsong(FileInfo)"/>.
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
        /// If the song is a special track (tutorial, credits).
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
            var dtxDict = dtx.PopulateNodeDictionary();

            MoggPath = dtxDict.GetValueOrDefault("/mogg_path")?.GetChild<string>(1);
            MidiPath = dtxDict.GetValueOrDefault("/midi_path")?.GetChild<string>(1);
            Length = int.Parse(dtxDict.GetValueOrDefault("/song_info/length")?.GetChild<string>(1)?.Split(":").FirstOrDefault() ?? "0");
            CountIn = int.Parse(dtxDict.GetValueOrDefault("/song_info/countin")?.GetChild<string>(1) ?? "0");
            Tracks = dtxDict.GetValueOrDefault("/tracks")?.GetChild(1, trackDtx =>
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
            Pans = dtxDict.GetValueOrDefault("/pans")?.GetChild<DataArray>(1)?.Children?.Select(e => e.ToString().Transform(o => double.Parse((string)o), 0)).ToList() ?? new List<double>();
            Volumes = dtxDict.GetValueOrDefault("/vols")?.GetChild<DataArray>(1)?.Children?.Select(e => e.ToString().Transform(o => double.Parse((string)o), 0)).ToList() ?? new List<double>();
            Attenuation = dtxDict.GetValueOrDefault("/active_track_db")?.Children.Skip(1).Select(e => e.ToString().Transform(o => double.Parse((string)o), 0))?.ToList() ?? new List<double>();
            ArenaPath = dtxDict.GetValueOrDefault("/arena_path")?.GetChild<string>(1);
            TunnelScale = dtxDict.GetValueOrDefault("/tunnel_scale")?.GetChild<double>(1, e => double.Parse(e.ToString()));
            EnableOrder = dtxDict.GetValueOrDefault("/enable_order")?.GetChild<DataArray>(1)?.Children.Select(e => e.ToString().Transform(o => int.Parse((string)o), 0)).ToList() ?? new List<int>();
            SectionStartBars = dtxDict.GetValueOrDefault("/section_start_bars")?.Children.Skip(1)?.Select(e => e.ToString().Transform(o => int.Parse((string)o), 0))?.ToList() ?? new List<int>();
            Title = dtxDict.GetValueOrDefault("/title")?.GetChild<string>(1);
            ShortTitle = dtxDict.GetValueOrDefault("/title_short")?.GetChild<string>(1);
            Artist = dtxDict.GetValueOrDefault("/artist")?.GetChild<string>(1);
            ShortArtist = dtxDict.GetValueOrDefault("/artist_short")?.GetChild<string>(1);
            Description = dtxDict.GetValueOrDefault("/desc")?.GetChild<string>(1);
            RawUnlockRequirement = dtxDict.GetValueOrDefault("/unlock_requirement")?.GetChild<string>(1);
            Bpm = dtxDict.GetValueOrDefault("/bpm")?.GetChild<double>(1, e => double.Parse(e.ToString()));
            Charter = dtxDict.GetValueOrDefault("/charter")?.GetChild<string>(1)?.Trim();
            DemoVideo = dtxDict.GetValueOrDefault("/demo_video")?.GetChild<string>(1)?.Trim();
            PreviewStart = dtxDict.GetValueOrDefault("/preview_start_ms")?.GetChild<int>(1, e => int.Parse(e.ToString()));
            PreviewLength = dtxDict.GetValueOrDefault("/preview_length_ms")?.GetChild<int>(1, e => int.Parse(e.ToString()));
            BossLevel = dtxDict.GetValueOrDefault("/boss_level")?.GetChild<int>(1, e => int.Parse(e.ToString()));

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
        /// Converts the instance to a <see cref="DataArray"/>.
        /// </summary>
        /// <param name="makeNew">Create a new <see cref="DataArray"/> instead of using the original dtx as a base.</param>
        /// <returns></returns>
        public DataArray ToDtx(bool makeNew = false)
        {
            DataArray dtx;

            if (makeNew == false && DtxBytes != null)
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

            var knownNodes = dtx.PopulateNodeDictionary();

            DataArray node;

            if ((node = GetOrDeleteArray(dtx, "mogg_path", knownNodes.GetValueOrDefault("/mogg_path"), MoggPath)) != null)
            {
                node.RemoveAllAfter(0).AddNode(DataSymbol.Symbol(MoggPath));
            }

            if ((node = GetOrDeleteArray(dtx, "midi_path", knownNodes.GetValueOrDefault("/midi_path"), MidiPath)) != null)
            {
                node.RemoveAllAfter(0).AddNode(DataSymbol.Symbol(MidiPath));
            }

            if ((node = GetOrDeleteArray(dtx, "song_info", knownNodes.GetValueOrDefault("/song_info"), Length.HasValue || CountIn.HasValue ? "" : null)) != null)
            {
                DataArray innerNode;

                if ((innerNode = GetOrDeleteArray(node, "length", knownNodes.GetValueOrDefault("/song_info/length"), Length)) != null)
                {
                    innerNode.AddNode(DataSymbol.Symbol($"{Length.Value}:0:0"));
                }

                if ((innerNode = GetOrDeleteArray(node, "countin", knownNodes.GetValueOrDefault("/song_info/countin"), Length)) != null)
                {
                    innerNode.AddNode(new DataAtom(CountIn.Value));
                }
            }

            if ((node = GetOrDeleteArray(dtx, "tracks", knownNodes.GetValueOrDefault("/tracks"), Tracks)) != null)
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

            if ((node = GetOrDeleteArray(dtx, "pans", knownNodes.GetValueOrDefault("/pans"), Pans)) != null)
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

            if ((node = GetOrDeleteArray(dtx, "vols", knownNodes.GetValueOrDefault("/vols"), Volumes)) != null)
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

            if ((node = GetOrDeleteArray(dtx, "active_track_db", knownNodes.GetValueOrDefault("/active_track_db"), Attenuation)) != null)
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

            if ((node = GetOrDeleteArray(dtx, "arena_path", knownNodes.GetValueOrDefault("/arena_path"), ArenaPath)) != null)
            {
                node.RemoveAllAfter(0).AddNode(DataSymbol.Symbol(ArenaPath));
            }

            if (makeNew && (node = GetOrDeleteArray(dtx, "score_goal", null, true)) != null)
            {
                for (var y = 0; y < 4; y++)
                {
                    var array = new DataArray();

                    for (var x = 0; x < 3; x++)
                    {
                        array.AddNode(new DataAtom(x + y + 1));
                    }

                    node.AddNode(array);
                }
            }

            if ((node = GetOrDeleteArray(dtx, "tunnel_scale", knownNodes.GetValueOrDefault("/tunnel_scale"), TunnelScale)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom((float)TunnelScale));
            }

            if ((node = GetOrDeleteArray(dtx, "enable_order", knownNodes.GetValueOrDefault("/enable_order"), EnableOrder)) != null)
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

            if ((node = GetOrDeleteArray(dtx, "section_start_bars", knownNodes.GetValueOrDefault("/section_start_bars"), SectionStartBars)) != null)
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

            if ((node = GetOrDeleteArray(dtx, "title", knownNodes.GetValueOrDefault("/title"), Title)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(Title));
            }

            if ((node = GetOrDeleteArray(dtx, "title_short", knownNodes.GetValueOrDefault("/title_short"), ShortTitle)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(ShortTitle));
            }

            if ((node = GetOrDeleteArray(dtx, "artist", knownNodes.GetValueOrDefault("/artist"), Artist)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(Artist));
            }

            if ((node = GetOrDeleteArray(dtx, "artist_short", knownNodes.GetValueOrDefault("/artist_short"), ShortArtist)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(ShortArtist));
            }

            if ((node = GetOrDeleteArray(dtx, "desc", knownNodes.GetValueOrDefault("/desc"), Description)) != null)
            {
                node.RemoveAllAfter(0).AddNode(DataSymbol.Symbol(Description));
            }

            if ((node = GetOrDeleteArray(dtx, "unlock_requirement", knownNodes.GetValueOrDefault("/unlock_requirement"), RawUnlockRequirement)) != null)
            {
                node.RemoveAllAfter(0).AddNode(DataSymbol.Symbol(RawUnlockRequirement));
            }

            if ((node = GetOrDeleteArray(dtx, "bpm", knownNodes.GetValueOrDefault("/bpm"), Bpm)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom((int)Bpm.Value));
            }

            if ((node = GetOrDeleteArray(dtx, "preview_start_ms", knownNodes.GetValueOrDefault("/preview_start_ms"), PreviewStart)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(PreviewStart.Value));
            }

            if ((node = GetOrDeleteArray(dtx, "preview_length_ms", knownNodes.GetValueOrDefault("/preview_length_ms"), PreviewLength)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(PreviewLength.Value));
            }

            if ((node = GetOrDeleteArray(dtx, "boss_level", knownNodes.GetValueOrDefault("/boss_level"), BossLevel)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(BossLevel.Value));
            }

            if ((node = GetOrDeleteArray(dtx, "charter", knownNodes.GetValueOrDefault("/charter"), Charter)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(Charter));
            }

            if ((node = GetOrDeleteArray(dtx, "demo_video", knownNodes.GetValueOrDefault("/demo_video"), DemoVideo)) != null)
            {
                node.RemoveAllAfter(0).AddNode(new DataAtom(DemoVideo));
            }

            return dtx;
        }

        public static implicit operator DataArray(MoggSong song) => song.ToDtx();
        public static explicit operator MoggSong(DataArray dtx) => MoggSong.FromDtx(dtx);
    }
}
