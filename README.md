# DcsTranslateTool (Prism + DryIoc)

DCS: World の日本語化をサポートする**非公式**ツールです。

主な機能

- [翻訳ファイルのリポジトリ](https://github.com/5kdn/test_DCS)から翻訳ファイルをダウンロード
- 翻訳ファイルを `.miz` ファイルへ注入
- ユーザーが作成した翻訳ファイルをリポジトリへアップロード

---

## 技術スタック

- .NET: **net8.0-windows**
- DI/アーキテクチャ: **Prism.DryIoc 9.0.537**
- IDE: **Visual Studio 2022** 推奨

---

## プロジェクト構成（抜粋）

```text
/
├─ DcsTranslateTool.Win/     # エントリ（起動）プロジェクト
├─ DcsTranslateTool.Core/    # コアライブラリ
├─ DcsTranslateTool.Tests/   # テスト
├─ BuildTasks/               # ビルド関連タスク
├─ DcsTranslateTool.sln
├─ Directory.Build.props
├─ Directory.Packages.props
```

---

## セットアップ

### 1. クローン & 依存解決

```powershell
git clone https://github.com/5kdn/DTT_PrismDryIoc.git
cd DTT_PrismDryIoc
dotnet restore
```

### 2. ローカル開発用の設定（必要な場合）

リリースビルド時に必要な環境変数は GitHub Actions（workflow）で付与されます。
ローカル開発では任意に **`Directory.Build.props.user`** を作成してプロパティを上書きできます。

例:

```xml
<Project>
  <PropertyGroup>
    <Version>1.2.3</Version>
    <Algo>algorithm-version</Algo>
    <!-- 例: 秘密鍵などをローカル検証用に設定 -->
    <GH_APP_PRIVATE_KEY>dummy-private-key</GH_APP_PRIVATE_KEY>
  </PropertyGroup>
</Project>
```

> **メモ**: 上記のキー名/値は開発例です。実運用のシークレットは *workflow の secrets* を利用し、平文でのコミットは避けてください。

---

## 実行方法

### エントリプロジェクト

- **`DcsTranslateTool.Win`**（Windows デスクトップアプリ）

### CLI

```powershell
dotnet build -c Debug
dotnet run --project .\DcsTranslateTool.Win\DcsTranslateTool.Win.csproj
```

### IDE（Visual Studio 2022）

1. ソリューションを開く
2. `DcsTranslateTool.Win` を **スタートアッププロジェクト**に設定
3. `F5`（デバッグ実行）または `Ctrl+F5`（デバッグなし）

---

## テスト

- 使用フレームワーク
  - **xUnit 2.9.3**
  - **Moq 4.20.72**

実行コマンド:

```powershell
dotnet test .\DcsTranslateTool.sln -c Debug --nologo --verbosity minimal
```

---

## リリース運用

- 自動リリース: `.github/workflows/publish-to-GitHub-Release.yml`
- バージョニング/リリース PR 自動化: `.github/workflows/release-please.yml`
  - 設定ファイル: `release-please-config.json`, `.release-please-manifest.json`
- リポジトリ戦略: **GitHub Flow**
  - **Squash-merge** で `master` にマージ
  - `master` は常にデプロイ可能状態を維持

> リリース時に必要な環境変数・シークレットは **workflow 内で付与** されます（ローカルでは不要）。

---

## ライセンス / コントリビュート

- **License**: MIT（`LICENSE` 参照）
- **Contributing**: `CONTRIBUTING.md`
- **Code of Conduct**: `CODE_OF_CONDUCT.md`
- **Security Policy**: `SECURITY.md`

---

## 注意事項

- 本ツールは **非公式** です。DCS: World の利用規約/EULA の範囲でご利用ください。
- `.miz` への注入動作はバックアップを取得してから実行してください。
