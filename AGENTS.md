# AGENTS.md

**Prism + DryIoc** を用いた WPF アプリ (**DcsTranslateTool**) 向けに、コーディングエージェント/自動化ツールが迷わず安全に作業するためのガイドです。人間とエージェントの両方が同じルールに従える、予測可能な作業場所を提供します。

---

## プロジェクト概要

- ソリューション: `DcsTranslateTool.sln`
- アプリ/ライブラリ
  - `DcsTranslateTool.Win/` — WPF デスクトップアプリ (Prism アプリのエントリ)
  - `DcsTranslateTool.Core/` — ドメイン/サービスの共有ライブラリ
  - `DcsTranslateTool.Tests/` — ユニットテスト
  - `BuildTasks/` — ビルド/配布補助 (MSBuild/PowerShell 等)
- リポジトリ共通設定
  - 依存は中央管理: `Directory.Packages.props`
  - ビルド既定: `Directory.Build.props`
  - ポリシー: `CODE_OF_CONDUCT.md`, `CONTRIBUTING.md`, `SECURITY.md`
  - ライセンス: MIT (`LICENSE`)

> エージェントへのヒント: 実際のターゲット フレームワーク (TFM) やアナライザー設定は `*.csproj`/`Directory.*.props` を先に読み取って確定してください。

---

## セットアップ & ビルド

これらは .NET WPF/Prism プロジェクト向けの安全なデフォルト設定です。csprojを確認した後、TFM（netX.Y-windows）を調整してください。

```powershell
# SDK 状態の確認 (任意)
dotnet --info

# 依存関係の復元とビルド
 dotnet restore DcsTranslateTool.sln
 dotnet build   DcsTranslateTool.sln -c Debug --nologo

# テスト実行
 dotnet test    DcsTranslateTool.sln -c Debug --nologo --verbosity minimal

# リリースビルド
 dotnet build   DcsTranslateTool.sln -c Release --nologo
```

### アプリ実行 (Windows)

デバッグビルド後、以下の実行ファイルを起動します (TFM は csproj を確認)。

```text
DcsTranslateTool.Win/bin/Debug/<TFM>/DcsTranslateTool.Win.exe
```

`<TFM>` 値の例: `net8.0-windows` 。 `DcsTranslateTool.Win.csproj` で確認してください。

---

## ビルド/テストの期待値

- **テストは常にグリーン** であること。最終確認は `dotnet test -c Release`。
- **フォーマット**: ルートの `.editorconfig` に従い、`dotnet format` を実行。
- **Warnings as Errors** が有効な場合は警告を解消すること。
- **中央管理パッケージ**: 可能な限り `Directory.Packages.props` でバージョン更新。個別プロジェクトの明示オプトアウトがない限り、局所上書きは避ける。

### テスト追加の方針

- 既存のテストフレームワークを使用して、新しいユニットテストを `DcsTranslateTool.Tests/` 配下に配置してください。
- テスト名は明確に命名し、可能な限り1つのテストにつき1つのアサーション/動作とする。
- `Core` にパブリックAPIを追加する場合は、それらをカバーするユニットテストを追加してください。

---

## Prism + DryIoc の規約 (エージェントが守るべきこと)

- **起動**: `App.xaml.cs` に `PrismApplication` (または同等) 実装。
- 登録:
  - 依存は `RegisterTypes(IContainerRegistry container)`、またはモジュール (`IModule`) で登録。
  - アプリ全体で共有するものは `RegisterSingleton`、都度生成は `Register`。
  - ナビゲーション対象ビューは `RegisterForNavigation<View, ViewModel>("RouteName")` で登録。
- **MVVM**:
  - ベースVMは通常`BindableBase`を継承する（プロパティ変更サポートのため）。
  - サービス依存関係にはコンストラクタ注入を優先する。
  - UIナビゲーションは直接ビューインスタンス化ではなく、Prismのナビゲーションサービス経由で行うべきである。
- モジュール（存在する場合）： `IModule`  を実装し、 `OnInitialized` / `RegisterTypes` 内で関連するサービス/ビューを登録する。

### ありがちな落とし穴 (クイックチェック)

- **ナビゲーションできない** → `RegisterForNavigation` の有無/ルート名、XAML の Region 名一致を確認。
- **ViewModel 依存が null** → 登録漏れ/ライフタイム不一致。コンテナ登録を再確認。
- **Binding エラー** → `PresentationTraceSources.TraceLevel=High` を有効化し、Output を確認。`DataContext` が VM か確認。
- **WPF TFM の不一致** → `netX.Y-windows` かつ `UseWPF` が有効か確認。

---

## コードスタイル & クオリティゲート

- Follow `.editorconfig` at the repo root. Run:

```bash
dotnet format --verify-no-changes
```

- アクセス修飾子は明示。イベント以外の `async void` は禁止。
- `#nullable enable` が有効なら警告を解消し、呼び出し側で責務を持つ。
- 公開 API には必要に応じて XML ドキュメントコメントを付与。

---

## リポジトリ構成 (クイックマップ)

```text
DTT_PrismDryIoc/
├─ DcsTranslateTool.sln
├─ DcsTranslateTool.Win/           # WPF app (Prism/DryIoc)
├─ DcsTranslateTool.Core/          # domain & services
├─ DcsTranslateTool.Tests/         # unit tests
├─ BuildTasks/                     # build helpers
├─ Directory.Build.props           # global build setup
├─ Directory.Packages.props        # central NuGet versions
├─ .github/                        # CI workflows
├─ CONTRIBUTING.md | SECURITY.md | CODE_OF_CONDUCT.md | LICENSE
└─ AGENTS.md                       # this file
```

---

## CI & commits

- **CI**: `.github/workflows/` で `restore → build → test → (必要なら pack)` の流れを想定。
- **コミットメッセージ**: 簡潔な命令形。`.gitmessage.txt` のテンプレートに従う。
- **リリース自動化**: 手動でバージョンをいじらず、`release-please` に任せる。

---

## Security & licensing

- 脆弱性報告: `SECURITY.md` に従う。
- ライセンス: MIT。サードパーティのライセンス表記は保持すること。

---

## エージェントが安全に自動化できる作業

- 新規 View + ViewModel の追加とナビゲーション配線。
- `Core` にサービス I/F を追加し、コンテナへ登録。
- 新規/変更仕様に対するユニットテスト作成。
- `Directory.Packages.props` での NuGet 更新 (SemVer と CI を尊重)。
- アナライザー/nullable 警告の解消。

---

## ローカル・トラブルシューティング

- **Prism ブートストラップ失敗**: `OnInitialized` にブレークポイント。`MainWindow` がコンテナ経由で生成されているか確認。
- **XAML デザイナ不調**: 実行 (`F5`) での挙動を基準に判断。DI 解決はデザイナで失敗しがち。
- **実行時のファイル未検出**: `Build Action`/`Copy to Output Directory` を確認。
- **WPF リソースを要するテスト**: ロジックを `Core` に寄せ、UI 依存をモック化。

---

## 最小限のプルリクエストチェックリスト

- [ ]  `dotnet build -c Release` が警告なし（または正当な抑制あり）で成功する
- [ ]  `dotnet test -c Release` が成功する
- [ ]  `dotnet format` を実行し、差分がない
- [ ]  新規/変更された公開動作がテストでカバーされている
- [ ]  コンテナ登録が追加/更新されている
- [ ]  ユーザー可視動作が変更された場合、README/ドキュメントが更新されている

---

## メンテナ向けメモ

- 大規模リファクタよりも **加算的変更** を優先。
- 名前空間とフォルダ構成は一致させる (`Views/`, `ViewModels/`, `Services/` 等)。
- XAML の移動/改名時は `x:Class` とリソース URI の整合を取る。
- ロギング/HTTP 等の横断関心事は I/F 駆動で設計し、コンストラクタ注入する。

---

*本ドキュメントはエージェント作業のための最小限ガイドです。人間の貢献者は必要に応じて `README.md` や各ポリシー文書も参照してください。*
