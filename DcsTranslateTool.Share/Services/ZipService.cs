using System.IO.Compression;

using DcsTranslateTool.Share.Contracts.Services;

namespace DcsTranslateTool.Share.Services;

/// <summary>
/// zip ファイル操作を提供するサービスである
/// </summary>
public class ZipService : IZipService
{
    private static void ValidatePath(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("値が null または空である", paramName);
        }
    }

    /// <inheritdoc />
    public void AddFile(string zipFilePath, string filePath, string entryPath)
    {
        ValidatePath(zipFilePath, nameof(zipFilePath));
        ValidatePath(filePath, nameof(filePath));
        ValidatePath(entryPath, nameof(entryPath));
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("追加するファイルが存在しない", filePath);
        }

        try
        {
            using FileStream stream = new(zipFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using ZipArchive archive = new(stream, ZipArchiveMode.Update);
            archive.GetEntry(entryPath)?.Delete();
            archive.CreateEntryFromFile(filePath, entryPath, CompressionLevel.Optimal);
        }
        catch (InvalidDataException ex)
        {
            throw new InvalidDataException("zip ファイルが壊れている可能性がある", ex);
        }
    }

    /// <inheritdoc />
    public void AddBytes(string zipFilePath, byte[] data, string entryPath)
    {
        ValidatePath(zipFilePath, nameof(zipFilePath));
        ValidatePath(entryPath, nameof(entryPath));
        if (data is null)
        {
            throw new ArgumentException("値が null である", nameof(data));
        }

        try
        {
            using FileStream stream = new(zipFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using ZipArchive archive = new(stream, ZipArchiveMode.Update);
            archive.GetEntry(entryPath)?.Delete();
            ZipArchiveEntry entry = archive.CreateEntry(entryPath, CompressionLevel.Optimal);
            using Stream entryStream = entry.Open();
            entryStream.Write(data);
        }
        catch (InvalidDataException ex)
        {
            throw new InvalidDataException("zip ファイルが壊れている可能性がある", ex);
        }
    }

    /// <inheritdoc />
    public void DeleteEntry(string zipFilePath, string targetPath)
    {
        ValidatePath(zipFilePath, nameof(zipFilePath));
        ValidatePath(targetPath, nameof(targetPath));
        if (!File.Exists(zipFilePath))
        {
            return;
        }

        try
        {
            using FileStream stream = new(zipFilePath, FileMode.Open, FileAccess.ReadWrite);
            using ZipArchive archive = new(stream, ZipArchiveMode.Update);
            List<ZipArchiveEntry> targets = archive.Entries
                .Where(e => e.FullName == targetPath || e.FullName.StartsWith(targetPath.TrimEnd('/') + '/'))
                .ToList();
            foreach (ZipArchiveEntry entry in targets)
            {
                entry.Delete();
            }
        }
        catch (InvalidDataException ex)
        {
            throw new InvalidDataException("zip ファイルが壊れている可能性がある", ex);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<string> GetEntryNames(string zipFilePath)
    {
        ValidatePath(zipFilePath, nameof(zipFilePath));
        if (!File.Exists(zipFilePath))
        {
            return new List<string>();
        }

        try
        {
            using FileStream stream = new(zipFilePath, FileMode.Open, FileAccess.Read);
            using ZipArchive archive = new(stream, ZipArchiveMode.Read);
            return archive.Entries.Select(e => e.FullName).ToList();
        }
        catch (InvalidDataException ex)
        {
            throw new InvalidDataException("zip ファイルが壊れている可能性がある", ex);
        }
    }
}
