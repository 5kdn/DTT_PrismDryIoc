using System.IO.Compression;

using DcsTranslateTool.Share.Contracts.Services;

namespace DcsTranslateTool.Share.Services;

/// <summary>
/// zip ファイル操作を提供するサービスである
/// </summary>
public class ZipService : IZipService {
    /// <inheritdoc />
    public IReadOnlyList<string> GetEntries( string zipFilePath ) {
        if(!File.Exists( zipFilePath )) throw new FileNotFoundException( "ファイルが存在しません", zipFilePath );
        if(!File.Exists( zipFilePath )) return new List<string>();

        try {
            using FileStream fs = new(zipFilePath, FileMode.Open, FileAccess.Read);
            using ZipArchive archive = new(fs, ZipArchiveMode.Read);
            return archive.Entries.Select( e => e.FullName ).ToList();
        }
        catch(InvalidDataException ex) {
            throw new InvalidDataException( "zip ファイルが壊れている可能性がある", ex );
        }
    }

    /// <inheritdoc />
    public void AddEntry( string zipFilePath, string entryPath, string filePath ) {
        if(!File.Exists( zipFilePath )) throw new FileNotFoundException( "ファイルが存在しません", zipFilePath );
        if(!File.Exists( filePath )) throw new FileNotFoundException( "ファイルが存在しません", filePath );
        if(string.IsNullOrEmpty( entryPath )) throw new ArgumentException( "値が null または空です", entryPath );

        try {
            using FileStream fs = new(zipFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using ZipArchive archive = new(fs, ZipArchiveMode.Update);
            archive.GetEntry( entryPath )?.Delete();
            archive.CreateEntryFromFile( filePath, entryPath, CompressionLevel.NoCompression );
        }
        catch(InvalidDataException ex) {
            throw new InvalidDataException( "zip ファイルが壊れている可能性があります", ex );
        }
    }

    /// <inheritdoc />
    public void AddEntry( string zipFilePath, string entryPath, byte[] data ) {
        if(!File.Exists( zipFilePath )) throw new FileNotFoundException( "ファイルが存在しません", zipFilePath );
        if(string.IsNullOrEmpty( entryPath )) throw new ArgumentException( "エントリーが null または空です", entryPath );

        try {
            using FileStream fs = new(zipFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using ZipArchive archive = new(fs, ZipArchiveMode.Update);
            archive.GetEntry( entryPath )?.Delete();
            ZipArchiveEntry entry = archive.CreateEntry(entryPath, CompressionLevel.NoCompression);
            using Stream entryStream = entry.Open();
            entryStream.Write( data );
        }
        catch(InvalidDataException ex) {
            throw new InvalidDataException( "zip ファイルが壊れている可能性があります", ex );
        }
    }

    /// <inheritdoc />
    public void DeleteEntry( string zipFilePath, string entryPath ) {
        if(!File.Exists( zipFilePath )) throw new FileNotFoundException( "ファイルが存在しません", zipFilePath );
        if(string.IsNullOrEmpty( entryPath )) throw new ArgumentException( "エントリーが null または空です", entryPath );
        if(!File.Exists( zipFilePath )) return;

        try {
            using FileStream stream = new(zipFilePath, FileMode.Open, FileAccess.ReadWrite);
            using ZipArchive archive = new(stream, ZipArchiveMode.Update);
            List<ZipArchiveEntry> targets = archive.Entries
                .Where(e => e.FullName == entryPath || e.FullName.StartsWith(entryPath.TrimEnd('/') + '/'))
                .ToList();
            foreach(ZipArchiveEntry entry in targets) {
                entry.Delete();
            }
        }
        catch(InvalidDataException ex) {
            throw new InvalidDataException( "zip ファイルが壊れている可能性がある", ex );
        }
    }
}
