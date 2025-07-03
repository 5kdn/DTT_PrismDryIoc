namespace DcsTranslateTool.Contracts.Providers;

/// <summary>
/// ダイアログを提供するインタフェース
/// </summary>
public interface IDialogProvider
{
    /// <summary>
    /// フォルダ選択ダイアログを表示し、選択されたパスを out で返します。
    /// 例外発生時は false を返し、selectedPath は null になります。
    /// </summary>
    /// <param name="initialDirectory">初期表示ディレクトリ</param>
    /// <param name="selectedPath">選択されたパス（キャンセル時は null）</param>
    /// <returns>ダイアログで OK が押されたら true。それ以外は false を返します。</returns>
    /// <exception cref="System.Runtime.InteropServices.COMException">ダイアログの表示または COM 操作に失敗した場合にスローされます。</exception>
    /// <exception cref="System.Exception">その他の予期しないエラーが発生した場合にスローされます。</exception>
    bool ShowFolderPicker( string initialDirectory, out string? selectedPath );
}
