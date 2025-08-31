using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.ViewModels;

namespace DcsTranslateTool.Win.Contracts.ViewModels.Factories;
public interface IFileEntryViewModelFactory {
    /// <summary>
    /// モデルから <see cref="FileEntryViewModel"/> を生成する
    /// </summary>
    /// <param name="model">基となるモデル</param>
    /// <param name="parent">親の ViewModel</param>
    /// <param name="checkState">初期チェック状態</param>
    /// <returns>生成した ViewModel</returns>
    FileEntryViewModel Create( FileEntry model, FileEntryViewModel? parent = null, bool? checkState = false );

    /// <summary>
    /// パラメーターから <see cref="FileEntryViewModel"/> を生成する
    /// </summary>
    /// <param name="path">絶対パス</param>
    /// <param name="isDirectory">ディレクトリかどうか</param>
    /// <param name="parent">親の ViewModel</param>
    /// <param name="checkState">初期チェック状態</param>
    /// <returns>生成した ViewModel</returns>
    FileEntryViewModel Create( string path, bool isDirectory, FileEntryViewModel? parent = null, bool? checkState = false );
}