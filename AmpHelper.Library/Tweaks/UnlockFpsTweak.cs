﻿using AmpHelper.Attributes;
using AmpHelper.Enums;
using AmpHelper.Extensions;
using AmpHelper.Helpers;
using AmpHelper.Interfaces;
using DtxCS;
using DtxCS.DataTypes;
using System.IO;
using System.Linq;

namespace AmpHelper.Tweaks
{
    /// <summary>
    /// This tweak will unlock the frame rate to run at the highest possible.
    /// </summary>
    [TweakInfo(
        Name: "Unlock FPS",
        Verb: "unlock-fps",
        Description: "Unlocks the frame rate to run at the highest possible",
        EnableText: "Removed the framerate limit, the game will run at the maximum possible FPS.",
        DisableText: "Locked the frame rate to either 30/60 FPS for the PS3/PS4 respectively.",
        EnabledText: "The frame rate limit has been removed, the game will run at the maximum possible FPS.",
        DisabledText: "The frame rate is locked to either 30/60 FPS for PS3/PS4 respectively.")]
    public class UnlockFpsTweak : ITweak
    {
        private string ConfigPath { get; set; }
        private ConsoleType Platform { get; set; }
        public ITweak SetPath(string path)
        {
            Platform = HelperMethods.ConsoleTypeFromPath(path);
            string platformLabel = Platform.ToString().ToLower();
            ConfigPath = Path.Combine(path, platformLabel, "system", "data", "config", $"default.dta_dta_{platformLabel}");

            if (!File.Exists(ConfigPath))
            {
                throw new FileNotFoundException(ConfigPath);
            }

            return this;
        }
        public bool IsEnabled()
        {
            using var helper = new DtxFileHelper<bool>(ConfigPath, dtx =>
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
            });

            return helper.Run().ReturnValue;
        }

        public bool SetState(bool enabled)
        {
            if (Platform == ConsoleType.PS4)
            {
                return false;
            }

            using var helper = new DtxFileHelper<bool>(ConfigPath, dtx =>
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

            var returnValue = helper.Run().Rebuild().ReturnValue;

            return returnValue;
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
