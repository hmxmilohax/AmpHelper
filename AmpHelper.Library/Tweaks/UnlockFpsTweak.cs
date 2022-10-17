using AmpHelper.Library.Extensions;
using AmpHelper.Library.Helpers;
using AmpHelper.Library.Interfaces;
using DtxCS;
using DtxCS.DataTypes;
using System.IO;
using System.Linq;

namespace AmpHelper.Library.Tweaks
{
    public class UnlockFpsTweak : ITweak
    {
        private string ConfigPath { get; set; }
        public ITweak SetPath(string path)
        {
            string platform = Directory.Exists(Path.Combine(path, "ps3")) ? "ps3" : "ps4";
            ConfigPath = Path.Combine(path, platform, "system", "data", "config", $"default.dta_dta_{platform}");

            if (!File.Exists(ConfigPath))
            {
                throw new FileNotFoundException(ConfigPath);
            }

            return this;
        }
        public bool IsEnabled()
        {
            return HelperMethods.DoWithDtbFile(ConfigPath, dtx =>
            {
                var rndNode = dtx.FindArrayByChild("rnd")?.OfType<DataArray>()?.FirstOrDefault();

                if (rndNode != null)
                {
                    var vsyncModeNode = rndNode.FindArrayByChild("vsync_mode")?.OfType<DataArray>().FirstOrDefault();
                    var vsyncEnabledNode = rndNode.FindArrayByChild("vsync_enabled")?.OfType<DataArray>().FirstOrDefault();

                    if (vsyncModeNode != null && vsyncEnabledNode != null && vsyncModeNode.Children.Count == 2 && vsyncEnabledNode.Children.Count == 2)
                    {
                        return vsyncModeNode.Children[1].ToString(1) == "1" && vsyncEnabledNode.Children[1].ToString(1) == "FALSE";
                    }
                }

                return false;
            }, false);
        }

        public bool SetState(bool enabled)
        {
            return HelperMethods.DoWithDtbFile(ConfigPath, dtx =>
            {
                var rndNode = dtx.FindArrayByChild("rnd")?.OfType<DataArray>()?.FirstOrDefault();

                if (rndNode != null)
                {
                    var vsyncModeNode = rndNode.FindArrayByChild("vsync_mode")?.OfType<DataArray>().FirstOrDefault();
                    var vsyncEnabledNode = rndNode.FindArrayByChild("vsync_enabled")?.OfType<DataArray>().FirstOrDefault();

                    if (vsyncModeNode != null && vsyncEnabledNode != null && vsyncModeNode.Children.Count == 2 && vsyncEnabledNode.Children.Count == 2)
                    {
                        vsyncModeNode.Children[1] = DTX.FromDtaString(enabled ? "1" : "2").Children[0];
                        vsyncEnabledNode.Children[1] = DTX.FromDtaString(enabled ? "FALSE" : "TRUE").Children[0];
                    }
                }

                return true;
            });
        }

        public void EnableTweak()
        {
            _ = SetState(true);
        }

        public void DisableTweak()
        {
            _ = SetState(false);
        }
    }
}
