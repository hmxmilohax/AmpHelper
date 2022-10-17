using AmpHelper.Library.Extensions;
using AmpHelper.Library.Helpers;
using AmpHelper.Library.Interfaces;
using DtxCS;
using DtxCS.DataTypes;
using System.IO;
using System.Linq;

namespace AmpHelper.Library.Tweaks
{
    public class UnlockAllTweak : ITweak
    {
        private string ConfigPath { get; set; }

        private static readonly string[] DefaultUnlockTypes = new string[]
        {
            "ALLTHETIME",
            "ASSAULT_ON",
            "ASTROSIGHT",
            "BREAKFORME",
            "CONCEPT",
            "CRAZY_RIDE",
            "CRYSTAL",
            "DALATECHT",
            "DECODE_ME",
            "DIGITALPARALYSIS",
            "DONOTRETREAT",
            "DREAMER",
            "ENERGIZE",
            "ENTOMOPHOBIA",
            "FORCEQUIT",
            "HUMANLOVE",
            "IMPOSSIBLE",
            "ISEEYOU",
            "LIGHTS",
            "MAGPIE",
            "MUZE",
            "NECRODANCER",
            "PERFECTBRAIN",
            "PHANTOMS",
            "RECESSION",
            "REDGIANT",
            "SUPRASPATIAL",
            "SYNTHESIZED2014",
            "UNFINISHEDBUSINESS",
            "WAYFARER",
            "WETWARE",
            "World1",
            "World2",
            "World3",
            "autocatcher",
            "bumper",
            "crippler",
            "freestyle",
            "freq_mode",
            "multiplier",
            "skill_nightmare",
            "slowdown"
        };

        public ITweak SetPath(string path)
        {
            string platform = HelperMethods.ConsoleTypeFromPath(path).ToString().ToLower();
            ConfigPath = Path.Combine(path, platform, "config", $"amp_config.dta_dta_{platform}");

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
                var unlocks = dtx.FindArrayByChild("db")?.OfType<DataArray>()?.FirstOrDefault()?.FindArrayByChild("campaign")?.OfType<DataArray>()?.FirstOrDefault();

                if (unlocks != null)
                {
                    string[] originalNodes = (DTX.FromDtaString(LibraryResources.default_unlocks).Children[0] as DataArray)?.Children.Select(node => node.ToString()).ToArray();

                    if (originalNodes != null)
                    {
                        string[] currentNodes = unlocks.Children.Take(originalNodes.Length).Select(node => node.ToString()).ToArray();

                        return string.Join("\n", originalNodes) != string.Join("\n", currentNodes);
                    }
                }

                return false;
            });
        }

        private bool SetState(bool enabled)
        {
            return HelperMethods.DoWithDtbFile(ConfigPath, dtx =>
            {
                var unlocks = dtx.FindArrayByChild("db")?.OfType<DataArray>()?.FirstOrDefault()?.FindArrayByChild("campaign")?.OfType<DataArray>()?.FirstOrDefault();

                if (unlocks != null)
                {
                    foreach (var unlock in unlocks.Children.OfType<DataArray>())
                    {
                        unlock.Children[0] = DTX.FromDtaString("beat_num").Children[0];
                        unlock.Children[1] = DTX.FromDtaString("0").Children[0];
                        unlock.Children[2] = DTX.FromDtaString("kUnlockArena").Children[0];
                    }

                    if (enabled == false)
                    {
                        var newNodes = unlocks.Children.OfType<DataArray>().Where(node => !DefaultUnlockTypes.Contains(node.Children[3].ToString())).ToArray();
                        var originalNodes = (DTX.FromDtaString(LibraryResources.default_unlocks).Children[0] as DataArray)?.Children;

                        if (originalNodes != null)
                        {
                            unlocks.Children.Clear();
                            unlocks.Children.AddRange(originalNodes);
                            unlocks.Children.AddRange(newNodes);
                        }
                    }

                    return true;
                }

                return false;
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
