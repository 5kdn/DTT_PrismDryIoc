using System.Runtime.InteropServices;

using DcsTranslateTool.Win.Contracts.Providers;

using Microsoft.Win32;

namespace DcsTranslateTool.Win.Providers;

/// <summary>
/// ダイアログ表示を担当するプロバイダ
/// </summary>
public class DialogProvider : IDialogProvider {
    /// <inheritdoc/>
    public bool ShowFolderPicker( string initialDirectory, out string selectedPath ) {
        selectedPath = string.Empty;
        try {
            var dialog = new OpenFolderDialog
            {
                Title = "フォルダを選択してください",
                DefaultDirectory = initialDirectory,
                Multiselect = false,
            };
            bool? result = dialog.ShowDialog();
            if(result is true) {
                selectedPath = dialog.FolderName;
                return true;
            }
            return false;
        }
        catch(COMException) {
            return false;
        }
        catch(Exception) {
            return false;
        }
    }
}
