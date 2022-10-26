using AmpHelper.Delegates;
using AmpHelper.Enums;
using AmpHelper.Helpers;
using DanTheMan827.TempFolders;
using DtxCS;
using Mackiloha;
using Mackiloha.Ark;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AmpHelper
{
    /// <summary>
    /// Contains methods for packing and unpacking ark files.
    /// </summary>
    public static class Ark
    {
        #region Ark Packing
        private static readonly string[] serializableExtensions = new string[]
        {
            ".dta",
            ".fusion",
            ".moggsong",
            ".script"
        };

        /// <summary>
        /// Packs a directory into ark files and a header.
        /// </summary>
        /// <param name="inputPath">The path to the unpacked files.</param>
        /// <param name="headerFile">The path to the main_ps3.hdr / main_ps4.hdr file.</param>
        /// <param name="progress">An action that will be invoked with a message and the progress.</param>
        public static void Pack(string inputPath, string headerFile, ProgressAction progress = null) => Pack(new DirectoryInfo(inputPath), new FileInfo(headerFile), HelperMethods.ConsoleTypeFromPath(inputPath, GamePathType.Packed), progress);

        /// <summary>
        /// Packs a directory into ark files and a header.
        /// </summary>
        /// <param name="inputPath">The path to the unpacked files.</param>
        /// <param name="headerFile">The path to the main_ps3.hdr / main_ps4.hdr file.</param>
        /// <param name="consoleType"></param>
        /// <param name="progress">An action that will be invoked with a message and the progress.</param>
        public static void Pack(string inputPath, string headerFile, ConsoleType consoleType, ProgressAction progress = null) => Pack(new DirectoryInfo(inputPath), new FileInfo(headerFile), consoleType, progress);

        /// <summary>
        /// Packs a directory into ark files and a header.
        /// </summary>
        /// <param name="inputPath">The path to the unpacked files.</param>
        /// <param name="headerFile">The path to the main_ps3.hdr / main_ps4.hdr file.</param>
        /// <param name="progress">An action that will be invoked with a message and the progress.</param>
        public static void Pack(DirectoryInfo inputPath, FileInfo headerFile, ProgressAction progress = null) => Pack(inputPath, headerFile, HelperMethods.ConsoleTypeFromPath(inputPath.FullName, GamePathType.Packed), progress);

        /// <summary>
        /// Packs a directory into ark files and a header.
        /// </summary>
        /// <param name="inputPath">The path to the unpacked files.</param>
        /// <param name="headerFile">The path to the main_ps3.hdr / main_ps4.hdr file.</param>
        /// <param name="consoleType">The console type.</param>
        /// <param name="progress">An action that will be invoked with a message and the progress.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static void Pack(DirectoryInfo inputPath, FileInfo headerFile, ConsoleType consoleType, ProgressAction progress = null)
        {
            if (!headerFile.Name.ToLower().EndsWith(".hdr"))
            {
                throw new ArgumentException("Header must have a .hdr extension");
            }

            if (!inputPath.Exists)
            {
                throw new DirectoryNotFoundException($"Can't find directory \"{inputPath.FullName}\"");
            }

            var arkPath = headerFile.Directory;

            if (!arkPath.Exists)
            {
                Directory.CreateDirectory(arkPath.FullName);
            }

            uint arkPartSizeLimit = 1U * 1024 * 1024 * 1024;
            var ark = ArkFile.Create(headerFile.FullName, ArkVersion.V9, consoleType == ConsoleType.PS3 ? AmplitudeData.PS3Key : AmplitudeData.PS4Key);

            ark.ForcedXor = 0xFF;
            ark.ForcedExtraFlag = consoleType == ConsoleType.PS3 ? AmplitudeData.PS3Extra : AmplitudeData.PS4Extra;

            var files = inputPath.GetFiles("*", SearchOption.AllDirectories);

            var currentPartSize = 0u;
            var dotRegex = new Regex(@"\([.]+\)/");

            using var temp = new EasyTempFolder();
            long totalSize = files.Select(file => file.Length).Sum() + 1;
            long currentSize = 0;

            progress?.Invoke("Starting", currentSize, totalSize);

            foreach (var fileInfo in files)
            {
                var file = fileInfo;
                string internalPath = FileHelper.GetRelativePath(file.FullName, inputPath.FullName)
                    .Replace("\\", "/"); // Must be "/" in ark

                if (serializableExtensions.Contains(file.Extension.ToLower()) && !File.Exists($"{file.FullName}_dta_{consoleType.ToString().ToLower()}"))
                {
                    internalPath = $"{internalPath}_dta_{consoleType.ToString().ToLower()}";

                    using var fileStream = File.OpenRead(fileInfo.FullName);

                    file = new FileInfo(Path.Combine(temp.Path, FileHelper.GetRelativePath(file.FullName, inputPath.FullName)));

                    fileStream.Position = 0;

                    var dta = DTX.FromDtaStream(fileStream);
                    if (file.Exists)
                    {
                        file.Delete();
                    }

                    file.Directory.Create();

                    using var compiled = File.Create(file.FullName);

                    DTX.ToDtb(dta, compiled);
                    compiled.Dispose();
                    file.Refresh();
                }

                if (dotRegex.IsMatch(internalPath))
                {
                    internalPath = dotRegex.Replace(internalPath, x => $"{x.Value.Substring(1, x.Length - 3)}/");
                }

                var fileSizeLong = file.Length;
                var fileSize = (uint)fileSizeLong;
                var potentialPartSize = currentPartSize + fileSize;

                if (fileSizeLong > uint.MaxValue)
                {
                    throw new NotSupportedException($"File size above 4GB is unsupported for \"{file.FullName}\"");
                }

                ark.CommitChanges(false);

                if (potentialPartSize > arkPartSizeLimit)
                {
                    ark.AddAdditionalPart();
                    currentPartSize = 0;
                }

                string fileName = Path.GetFileName(internalPath);
                string dirPath = Path.GetDirectoryName(internalPath).Replace("\\", "/"); // Must be "/" in ark

                var pendingEntry = new PendingArkEntry(fileName, dirPath)
                {
                    LocalFilePath = file.FullName
                };

                ark.AddPendingEntry(pendingEntry);

                currentSize += fileInfo.Length;

                progress?.Invoke($"Added {internalPath}", currentSize, totalSize);

                currentPartSize += fileSize;
            }

            ark.CommitChanges(true);

            if (consoleType == ConsoleType.PS4)
            {
                using var header = new FileStream(headerFile.FullName, FileMode.Open, FileAccess.ReadWrite);
                header.Seek(0, SeekOrigin.Begin);
                using var writer = new AwesomeWriter(header);
                writer.Write(AmplitudeData.PS4EncryptedVersion);
            }

            progress?.Invoke($"Wrote {headerFile.FullName}", currentSize + 1, totalSize);
        }
        #endregion
        #region Ark Unpacking
        private static string CombinePath(string basePath, string path)
        {
            // Consistent slash
            basePath = FileHelper.FixSlashes(basePath ?? "");
            path = FileHelper.FixSlashes(path ?? "");

            path = ReplaceDotsInPath(path);
            return Path.Combine(basePath, path);
        }

        private static string ReplaceDotsInPath(string path)
        {
            var dotRegex = new Regex(@"[.]+[\/\\]");

            if (dotRegex.IsMatch(path))
            {
                // Replaces dotdot path
                path = dotRegex.Replace(path, x => $"({x.Value[..^1]}){x.Value.Last()}");
            }

            return path;
        }

        private static string ExtractEntry(Archive ark, ArkEntry entry, string filePath)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var fs = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var stream = ark.GetArkEntryFileStream(entry))
                {
                    stream.CopyTo(fs);
                }
            }

            return filePath;
        }

        /// <summary>
        /// Unpacks ark files and a header file to a directory.
        /// </summary>
        /// <param name="headerFile">The path to the main_ps3.hdr / main_ps4.hdr file.</param>
        /// <param name="outputPath">The path to unpack the files to.</param>
        /// <param name="dtbConversion">Convert dtb files to dta.</param>
        /// <param name="keepOriginalDtb">Keep the original dtb files</param>
        /// <param name="progress">An action that will be invoked with a message and the progress.</param>
        public static void Unpack(string headerFile, string outputPath, bool dtbConversion, bool keepOriginalDtb, ProgressAction progress = null) => Unpack(new FileInfo(headerFile), new DirectoryInfo(outputPath), dtbConversion, keepOriginalDtb, HelperMethods.ConsoleTypeFromPath(headerFile, GamePathType.HeaderFile), progress);

        /// <summary>
        /// Unpacks ark files and a header file to a directory.
        /// </summary>
        /// <param name="headerFile">The path to the main_ps3.hdr / main_ps4.hdr file.</param>
        /// <param name="outputPath">The path to unpack the files to.</param>
        /// <param name="dtbConversion">Convert dtb files to dta.</param>
        /// <param name="keepOriginalDtb">Keep the original dtb files</param>
        /// <param name="consoleType">The console type</param>
        /// <param name="progress">An action that will be invoked with a message and the progress.</param>
        public static void Unpack(string headerFile, string outputPath, bool dtbConversion, bool keepOriginalDtb, ConsoleType consoleType, ProgressAction progress = null) => Unpack(new FileInfo(headerFile), new DirectoryInfo(outputPath), dtbConversion, keepOriginalDtb, consoleType, progress);

        /// <summary>
        /// Unpacks ark files and a header file to a directory.
        /// </summary>
        /// <param name="headerFile">The path to the main_ps3.hdr / main_ps4.hdr file.</param>
        /// <param name="outputPath">The path to unpack the files to.</param>
        /// <param name="dtbConversion">Convert dtb files to dta.</param>
        /// <param name="keepOriginalDtb">Keep the original dtb files</param>
        /// <param name="progress">An action that will be invoked with a message and the progress.</param>
        public static void Unpack(FileInfo headerFile, DirectoryInfo outputPath, bool dtbConversion, bool keepOriginalDtb, ProgressAction progress = null) => Unpack(headerFile, outputPath, dtbConversion, keepOriginalDtb, HelperMethods.ConsoleTypeFromPath(headerFile.FullName, GamePathType.HeaderFile), progress);

        /// <summary>
        /// Unpacks ark files and a header file to a directory.
        /// </summary>
        /// <param name="headerFile">The path to the main_ps3.hdr / main_ps4.hdr file.</param>
        /// <param name="outputPath">The path to unpack the files to.</param>
        /// <param name="dtbConversion">Convert dtb files to dta.</param>
        /// <param name="keepOriginalDtb">Keep the original dtb files</param>
        /// <param name="consoleType">The console type</param>
        /// <param name="progress">An action that will be invoked with a message and the progress.</param>
        public static void Unpack(FileInfo headerFile, DirectoryInfo outputPath, bool dtbConversion, bool keepOriginalDtb, ConsoleType consoleType, ProgressAction progress = null)
        {
            var ark = ArkFile.FromFile(headerFile.FullName);
            using var temp = new EasyTempFolder();

            long totalSize = ark.Entries.Cast<OffsetArkEntry>().Select(entry => (long)Math.Max(entry.Size, entry.InflatedSize)).Sum();
            long currentSize = 0;

            progress?.Invoke("Starting", currentSize, totalSize);

            foreach (var entry in ark.Entries.OfType<OffsetArkEntry>())
            {
                string filePath;
                var entrySize = Math.Max(entry.Size, entry.InflatedSize);
                currentSize += entrySize;

                if (dtbConversion && entry.FileName.EndsWith($"_dta_{consoleType.ToString().ToLower()}"))
                {
                    var entryNameWithoutDta = entry.FullPath.Substring(0, entry.FullPath.Length - 8);
                    var fileInfo = new FileInfo(CombinePath(outputPath.FullName, entryNameWithoutDta));
                    var entryExtracted = false;
                    fileInfo.Directory.Create();

                    if (ark.Entries.Where(e => e.FullPath == entryNameWithoutDta).Count() > 0)
                    {
                        filePath = ExtractEntry(ark, entry, CombinePath(outputPath.FullName, entry.FullPath));
                        progress?.Invoke($"Unpacked {entry.FullPath}", currentSize, totalSize);
                        continue;
                    }

                    using var dtbStream = ark.GetArkEntryFileStream(entry);
                    var dtaArray = DTX.FromDtb(dtbStream);
                    var sb = new StringBuilder();

                    foreach (var x in dtaArray.Children)
                    {
                        sb.AppendLine(x.ToString(0));
                    }

                    var dtaString = sb.ToString();
                    var canReparse = false;
                    try
                    {
                        DTX.FromDtaString(dtaString);
                        canReparse = true;
                    }
                    catch (Exception ex)
                    {
                        // DTA can't be re-parsed, skip writing
                        canReparse = false;
                    }

                    if (canReparse)
                    {
                        File.WriteAllText(fileInfo.FullName, dtaString);
                        //byte[] dta = Encoding.UTF8.GetBytes(dtaString);
                        //File.WriteAllBytes(fileInfo.FullName, dta);
                        progress?.Invoke($"Unpacked {entryNameWithoutDta}", currentSize, totalSize);

                        if (!keepOriginalDtb)
                        {
                            continue;
                        }
                    }
                }

                filePath = ExtractEntry(ark, entry, CombinePath(outputPath.FullName, entry.FullPath));
                progress?.Invoke($"Unpacked {entry.FullPath}", currentSize, totalSize);
            }
        }
        #endregion
    }
}
