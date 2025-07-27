using System.IO.Compression;

using DcsTranslateTool.Core.Contracts.Services;

namespace DcsTranslateTool.Core.Services;

/// <summary>
/// zip ファイル操作を提供するサービスを実装する
/// </summary>
public class ZipService : IZipService {
    /// <inheritdoc />
    public IReadOnlyList<string> GetEntries( string zipFilePath ) {
        if(string.IsNullOrWhiteSpace( zipFilePath )) throw new ArgumentException( "zip ファイルパスが null または空です", nameof( zipFilePath ) );
        if(!File.Exists( zipFilePath )) throw new FileNotFoundException( "ファイルが存在しません", zipFilePath );

        try {
            using FileStream fs = new(zipFilePath, FileMode.Open, FileAccess.Read);
            using ZipArchive archive = new(fs, ZipArchiveMode.Read);
            return [.. archive.Entries.Select( e => e.FullName )];
        }
        catch(InvalidDataException ex) {
            throw new InvalidDataException( "zip ファイルが壊れている可能性がある", ex );
        }
        catch(IOException ex) {
            throw new IOException( "zip ファイル読み込み中に入出力エラーが発生した", ex );
        }
    }

    /// <inheritdoc />
    public void AddEntry( string zipFilePath, string entryPath, string filePath ) {
        if(string.IsNullOrWhiteSpace( zipFilePath )) throw new ArgumentException( "zip ファイルパスが null または空です", nameof( zipFilePath ) );
        if(!File.Exists( zipFilePath )) throw new FileNotFoundException( "ファイルが存在しません", zipFilePath );
        if(string.IsNullOrWhiteSpace( filePath )) throw new ArgumentException( "追加するファイルパスが null または空です", nameof( filePath ) );
        if(!File.Exists( filePath )) throw new FileNotFoundException( "ファイルが存在しません", filePath );
        if(string.IsNullOrWhiteSpace( entryPath )) throw new ArgumentException( "値が null または空です", nameof( entryPath ) );

        try {
            using FileStream fs = new(zipFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            using ZipArchive archive = new(fs, ZipArchiveMode.Update);
            archive.GetEntry( entryPath )?.Delete();
            archive.CreateEntryFromFile( filePath, entryPath, CompressionLevel.NoCompression );
        }
        catch(InvalidDataException ex) {
            throw new InvalidDataException( "zip ファイルが壊れている可能性があります", ex );
        }
        catch(IOException ex) {
            throw new IOException( "zip ファイル書き込み中に入出力エラーが発生した", ex );
        }
    }

    /// <inheritdoc />
    public void AddEntry( string zipFilePath, string entryPath, byte[] data ) {
        if(string.IsNullOrWhiteSpace( zipFilePath )) throw new ArgumentException( "zip ファイルパスが null または空です", nameof( zipFilePath ) );
        if(!File.Exists( zipFilePath )) throw new FileNotFoundException( "ファイルが存在しません", zipFilePath );
        if(string.IsNullOrWhiteSpace( entryPath )) throw new ArgumentException( "エントリーが null または空です", nameof( entryPath ) );
        if(data == null || data.Length == 0) throw new ArgumentException( "追加するデータが null または空です", nameof( data ) );

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
        catch(IOException ex) {
            throw new IOException( "zip ファイル書き込み中に入出力エラーが発生した", ex );
        }
    }

    /// <inheritdoc />
    public void DeleteEntry( string zipFilePath, string entryPath ) {
        if(string.IsNullOrWhiteSpace( zipFilePath )) throw new ArgumentException( "zip ファイルパスが空です", nameof( zipFilePath ) );
        if(!File.Exists( zipFilePath )) throw new FileNotFoundException( "ファイルが存在しません", zipFilePath );
        if(string.IsNullOrWhiteSpace( entryPath )) throw new ArgumentException( "エントリーが空です", nameof( entryPath ) );

        try {
            using FileStream stream = new(zipFilePath, FileMode.Open, FileAccess.ReadWrite);
            using ZipArchive archive = new(stream, ZipArchiveMode.Update);
            List<ZipArchiveEntry> targets = [.. archive.Entries.Where(e => e.FullName == entryPath || e.FullName.StartsWith(entryPath.TrimEnd('/') + '/'))];
            targets.ForEach( entry => entry.Delete() );
        }
        catch(InvalidDataException ex) {
            throw new InvalidDataException( "zip ファイルが壊れている可能性がある", ex );
        }
        catch(IOException ex) {
            throw new IOException( "zip ファイル操作中に入出力エラーが発生した", ex );
        }
    }
}
