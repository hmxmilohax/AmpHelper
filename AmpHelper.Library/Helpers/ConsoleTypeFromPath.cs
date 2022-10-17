using AmpHelper.Library.Enums;
using System.IO;

namespace AmpHelper.Library.Helpers
{
    internal partial class HelperMethods
    {
        public static ConsoleType ConsoleTypeFromPath(string path)
        {
            if (Directory.Exists(Path.Combine(path, "ps3")))
            {
                return ConsoleType.PS3;
            }

            if (Directory.Exists(Path.Combine(path, "ps4")))
            {
                return ConsoleType.PS4;
            }

            throw new DirectoryNotFoundException($"Neither ps3 or ps4 can be found in {path}");
        }
    }
}
