using AmpHelper.Library.Extensions;
using DtxCS.DataTypes;
using System.Linq;

namespace AmpHelper.Library.Types
{
    internal class MoggSongNodes
    {
        public DataArray MoggPath { get; set; }
        public DataArray MidiPath { get; set; }
        public DataArray SongInfo { get; set; }
        public DataArray Length { get; set; }
        public DataArray CountIn { get; set; }
        public DataArray Tracks { get; set; }
        public DataArray Pans { get; set; }
        public DataArray Vols { get; set; }
        public DataArray Attenuation { get; set; }
        public DataArray ArenaPath { get; set; }
        public DataArray ScoreGoal { get; set; }
        public DataArray TunnelScale { get; set; }
        public DataArray EnableOrder { get; set; }
        public DataArray SectionStartBars { get; set; }
        public DataArray Title { get; set; }
        public DataArray ShortTitle { get; set; }
        public DataArray Artist { get; set; }
        public DataArray ShortArtist { get; set; }
        public DataArray Description { get; set; }
        public DataArray UnlockRequirement { get; set; }
        public DataArray Bpm { get; set; }
        public DataArray Charter { get; set; }
        public DataArray DemoVideo { get; set; }
        public DataArray PreviewStartMs { get; set; }
        public DataArray PreviewLengthMs { get; set; }
        public DataArray BossLevel { get; set; }

        public MoggSongNodes(DataArray dtx)
        {
            foreach (var node in dtx.Children.OfType<DataArray>().Where(e => e.Children.Count >= 1))
            {
                switch (node.Children[0].ToString())
                {
                    case "mogg_path":
                        if (MoggPath == null)
                            MoggPath = node;
                        break;

                    case "midi_path":
                        if (MidiPath == null)
                            MidiPath = node;
                        break;

                    case "song_info":
                        if (SongInfo != null)
                            break;

                        SongInfo = node;
                        Length = node.FindArrayByChild("length").FirstOrDefault();
                        CountIn = node.FindArrayByChild("countin").FirstOrDefault();
                        break;

                    case "tracks":
                        if (Tracks == null)
                            Tracks = node;
                        break;

                    case "pans":
                        if (Pans == null)
                            Pans = node;
                        break;

                    case "vols":
                        if (Vols == null)
                            Vols = node;
                        break;

                    case "active_track_db":
                        if (Attenuation == null)
                            Attenuation = node;
                        break;

                    case "arena_path":
                        if (ArenaPath == null)
                            ArenaPath = node;
                        break;

                    case "score_goal":
                        if (ScoreGoal == null)
                            ScoreGoal = node;
                        break;

                    case "tunnel_scale":
                        if (TunnelScale == null)
                            TunnelScale = node;
                        break;

                    case "enable_order":
                        if (EnableOrder == null)
                            EnableOrder = node;
                        break;

                    case "section_start_bars":
                        if (SectionStartBars == null)
                            SectionStartBars = node;
                        break;

                    case "title":
                        if (Title == null)
                            Title = node;
                        break;

                    case "title_short":
                        if (ShortTitle == null)
                            ShortTitle = node;
                        break;

                    case "artist":
                        if (Artist == null)
                            Artist = node;
                        break;

                    case "artist_short":
                        if (ShortArtist == null)
                            ShortArtist = node;
                        break;

                    case "desc":
                        if (Description == null)
                            Description = node;
                        break;

                    case "unlock_requirement":
                        if (UnlockRequirement == null)
                            UnlockRequirement = node;
                        break;

                    case "bpm":
                        if (Bpm == null)
                            Bpm = node;
                        break;

                    case "charter":
                        if (Charter == null)
                            Charter = node;
                        break;

                    case "demo_video":
                        if (DemoVideo == null)
                            DemoVideo = node;
                        break;

                    case "preview_start_ms":
                        if (PreviewStartMs == null)
                            PreviewStartMs = node;
                        break;

                    case "preview_length_ms":
                        if (PreviewLengthMs == null)
                            PreviewLengthMs = node;
                        break;

                    case "boss_level":
                        if (BossLevel == null)
                            BossLevel = node;
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
