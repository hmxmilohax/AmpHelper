using AmpHelper.Library.Enums;
using DanTheMan827.TempFolders;
using DtxCS;
using Mackiloha;
using Mackiloha.Ark;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AmpHelper.Library.Ark
{
    public class Ark
    {
        #region Ark Packing
        private static readonly string[] serializableExtensions = new string[]
        {
            ".dta",
            ".fusion",
            ".moggsong",
            ".script"
        };
        public static void Pack(string inputPath, string headerFile, ConsoleType consoleType) => Pack(new DirectoryInfo(inputPath), new FileInfo(headerFile), consoleType);
        public static void Pack(DirectoryInfo inputPath, FileInfo headerFile, ConsoleType consoleType, Action<string>? Log = null)
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

                if (potentialPartSize > arkPartSizeLimit)
                {
                    ark.CommitChanges(true);
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
                Log?.Invoke($"Added {pendingEntry.FullPath}");

                currentPartSize += fileSize;
            }

            ark.CommitChanges(true);

            Log?.Invoke($"Wrote hdr to \"{headerFile.FullName}\"");

            if (consoleType == ConsoleType.PS4)
            {
                using var header = new FileStream(headerFile.FullName, FileMode.Open, FileAccess.ReadWrite);
                header.Seek(0, SeekOrigin.Begin);
                using var writer = new AwesomeWriter(header);
                writer.Write(AmplitudeData.PS4EncryptedVersion);
            }
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

        public static void Unpack(string headerFile, string outputPath, bool dtbConversion, bool keepOriginalDtb, ConsoleType consoleType) => Unpack(new FileInfo(headerFile), new DirectoryInfo(outputPath), dtbConversion, keepOriginalDtb, consoleType);

        public static void Unpack(FileInfo headerFile, DirectoryInfo outputPath, bool dtbConversion, bool keepOriginalDtb, ConsoleType consoleType, Action<string>? Log = null)
        {
            var ark = ArkFile.FromFile(headerFile.FullName);
            using var temp = new EasyTempFolder();

            foreach (var entry in ark.Entries)
            {
                string filePath;

                if (dtbConversion && entry.FileName.EndsWith($"_dta_{consoleType.ToString().ToLower()}"))
                {
                    var entryNameWithoutDta = entry.FullPath.Substring(0, entry.FullPath.Length - 8);
                    var fileInfo = new FileInfo(CombinePath(outputPath.FullName, entryNameWithoutDta));
                    var entryExtracted = false;
                    fileInfo.Directory.Create();

                    if (ark.Entries.Where(e => e.FullPath == entryNameWithoutDta).Count() > 0)
                    {
                        filePath = ExtractEntry(ark, entry, CombinePath(outputPath.FullName, entry.FullPath));
                        Log?.Invoke($"Wrote \"{filePath}\"");
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
                        Log?.Invoke($"Wrote dta \"{fileInfo.FullName}\"");

                        if (!keepOriginalDtb)
                        {
                            continue;
                        }
                    }
                }

                filePath = ExtractEntry(ark, entry, CombinePath(outputPath.FullName, entry.FullPath));
                Log?.Invoke($"Wrote \"{filePath}\"");
            }
        }
        #endregion
    }
}
