using AmpHelper.Enums;
using System.IO;

namespace AmpHelper.Helpers
{
    internal partial class HelperMethods
    {
        /// <summary>
        /// Attempts to discover the console type based on the contents of the path.
        /// </summary>
        /// <param name="path">Path to a heade file or directory</param>
        /// <param name="checkType">
        /// <see cref="GamePathType.Undefined"/> - Path is checked to see if any of the indicators can be found.
        /// <see cref="GamePathType.HeaderFile"/> - Filename is checked to see if it is main_ps3.hdr or main_ps4.hdr
        /// <see cref="GamePathType.Packed"/> - Directory is checked for main_ps3.hdr or main_ps4.hdr files.
        /// <see cref="GamePathType.Unpacked"/> - Directory is checked for ps3 or ps4 subfolders.
        /// </param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static ConsoleType ConsoleTypeFromPath(string path, GamePathType checkType = GamePathType.Undefined)
        {
            if ((checkType == GamePathType.Undefined || checkType == GamePathType.HeaderFile) && File.Exists(path))
            {
                var filename = Path.GetFileName(path);

                if (filename == "main_ps3.hdr")
                {
                    return ConsoleType.PS3;
                }

                if (filename == "main_ps4.hdr")
                {
                    return ConsoleType.PS4;
                }
            }

            if (Directory.Exists(path))
            {
                if ((checkType == GamePathType.Undefined || checkType == GamePathType.Unpacked) && Directory.Exists(Path.Combine(path, "ps3")))
                {
                    return ConsoleType.PS3;
                }

                if ((checkType == GamePathType.Undefined || checkType == GamePathType.Unpacked) && Directory.Exists(Path.Combine(path, "ps4")))
                {
                    return ConsoleType.PS4;
                }

                if ((checkType == GamePathType.Undefined || checkType == GamePathType.Packed) && File.Exists(Path.Combine(path, "main_ps3.hdr")))
                {
                    return ConsoleType.PS3;
                }

                if ((checkType == GamePathType.Undefined || checkType == GamePathType.Packed) && File.Exists(Path.Combine(path, "main_ps4.hdr")))
                {
                    return ConsoleType.PS4;
                }
            }

            throw new DirectoryNotFoundException($"Neither ps3 or ps4 can be found at {path}");
        }
    }
}
