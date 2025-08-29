using DcsTranslateTool.Core.Models;
using DcsTranslateTool.Win.Enums;
using DcsTranslateTool.Win.ViewModels;

namespace DcsTranslateTool.Win.Contracts.ViewModels.Factories;
/// <summary>
/// <see cref="EntryViewModel"/> を生成するファクトリーのインターフェースである。
/// </summary>
public interface IEntryViewModelFactory {
    /// <summary>
    /// モデルを指定して <see cref="EntryViewModel"/> を生成する。
    /// </summary>
    /// <param name="model">元となるモデル</param>
    /// <param name="parent">親ノード</param>
    /// <param name="checkState">初期チェック状態</param>
    /// <returns>生成された <see cref="EntryViewModel"/></returns>
    EntryViewModel Create(
        Entry model,
        EntryViewModel? parent = null,
        CheckState checkState = CheckState.Unchecked );

    /// <summary>
    /// パスとディレクトリ指定から <see cref="EntryViewModel"/> を生成する。
    /// </summary>
    /// <param name="absolutePath">対象の絶対パス</param>
    /// <param name="isDirectory">ディレクトリかどうか</param>
    /// <param name="parent">親ノード</param>
    /// <param name="checkState">初期チェック状態</param>
    /// <returns>生成された <see cref="EntryViewModel"/></returns>
    EntryViewModel Create(
        string absolutePath,
        bool isDirectory,
        EntryViewModel? parent = null,
        CheckState checkState = CheckState.Unchecked );
}