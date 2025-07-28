# Agents.md

## プロジェクト概要

本プロジェクトは、「DCS（Digital Combat Simulator）」の日本語化作業を支援するWindows用アプリケーションである。
アプリはPrismフレームワークとDryIocによるDIコンテナを用いたWPFアプリとして設計されている。
また、GitHubと連携して日本語翻訳ファイルのダウンロード・アップロード、ローカルパス管理等を行う。

主なプロジェクト構成

- DcsTranslateTool.Win (Windows用アプリケーション本体)
    - Properties (ViewModelに定義されるプロパティ)
    - Constants (定数値をまとめるクラスやファイル)
    - Contracts (インターフェースや抽象クラス)
    - Converters (値変換ロジックを持つクラス)
    - Enums (プレゼンテーション層用Enum)
    - Extensions (拡張メソッド)
    - Helpers (汎用的な便利関数やユーティリティクラス)
    - Models (アプリケーションのデータ構造やエンティティを表すクラス)
    - Providers (インスタンスやデータを供給する役割)
    - Services (ビジネスロジックや外部とのやり取りを担う)
    - Styles (UIの見た目を一括管理するリソース)
    - ViewModels
    - Views
- DcsTranslateTool.Core (アプリケーションの「コア機能」や「ロジック部分」のみを分離したライブラリプロジェクト)
    - Contracts (インターフェースや抽象クラス)
    - Enums (ドメイン用Enum)
    - Extensions (拡張メソッド)
    - Helpers (汎用的な便利関数やユーティリティクラス)
    - Models (アプリケーションのデータ構造やエンティティを表すクラス)
    - Services (本ライブラリプロジェクト内で完結するサービス)
- DcsTranslateTool.Test (DcsTranslateToolに対するユニットテスト)
- DcsTranslateTool.Core.Test (DcsTranslateTool.Coreに対するユニットテスト)

## コーディングスタイル

- コーディングスタイルは.editorconfigに従うこと。
- 行の長さは120文字を上限とする。
- クラスや関数にはドキュメントコメントを常体である調の日本語で作成すること。
- コードおよびコメントは常体で記述し、文体はである調とすること。
- ドキュメントコメントにはsummary, param(引数がある場合), returns(返り値がある場合)を追加すること。

## テストと品質保証

1. テストメソッドの命名規則
- 関数名は「どういう状況(When/Context)」で「何をしたら(Do/Action)」「どうなるべきか(Then/Outcome)」を体言止めの日本語で自然な文章として書くこと。
- 主な命名フォーマット（日本語の場合） [関数名]は[前提]したとき[結果]になる。


2. テストケースの粒度
- 1つのテストで1つのことだけを検証すること。
- テストは失敗パターンも網羅して作成すること。

3. Arrange-Act-Assert（AAAパターン）
- AAAパターンに従ってテストを記述すること。
- それぞれのブロックの前に`// Arrange`, `// Act`, `// Assert`を追加すること。

5. テストの独立性
- 各テストケースは他のテストの実行順や状態に依存しない状態を保つこと。
- テスト実行順が変わっても結果が変わらないようにすること。
- テストケースは対象関数ごとに#regionを使用してください
- テストケースは以下の順で並び替えること
  1. 同一対象関数
  2. 通常成功パターン
  3. 条件付き成功パターン
  4. 失敗パターン

6. その他
Windows専用のコードが有る対象に対するテストコードには `[Trait("Category", "WindowsOnly")` を追加し、非Windows環境でのテストでは `dotnet test --filter "Category!=WindowsOnly"` としてテストを実行すること。

t_wadaメソッドに従ってユニットテストを作成すること。

Windows専用のコードが有る対象に対するテストコードには `[Trait("Category", "WindowsOnly")` を追加し、非Windows環境でのテストでは `dotnet test --filter "Category!=WindowsOnly"` としてテストを実行すること。

## ビルド・デプロイ手順

## **コミット・PRメッセージ規約**

コミットは小さな粒度で作成すること。

メッセージは日本語で作成すること。

メッセージには以下のルールでPrefixを追加すること。

- feat: 新しい機能
- fix: バグの修正
- docs: ドキュメントのみの変更
- style: 空白、フォーマット、セミコロン追加など
- refactor: 仕様に影響がないコード改善(リファクタ)
- perf: パフォーマンス向上関連
- test: テスト関連
- chore: ビルド、補助ツール、ライブラリ関連

メッセージは変更した理由や目的を記述すること。

## ブランチ構成
- master
  - ブランチ名は `master`
  - 直接コミット禁止。
  - develop, hotfixes branchからのPull Requestのみを受け付ける。
  - プロダクトとしてリリースするためのブランチ。リリースしたらタグ付けする。

- release branches
  - ブランチ名は `release`
  - プロダクトリリースの準備。 機能の追加やマイナーなバグフィックスとは独立させることで、 リリース時に含めるコードを綺麗な状態に保つ（機能追加中で未使用のコードなどを含まないようにする）ことができる。
  - develop ブランチにリリース予定の機能やバグフィックスがほぼ反映した状態で develop から分岐する。
  - リリース準備が整ったら, master にマージし、タグをつける。次に develop にマージする。

- develop
  - ブランチ名は `develop`
  - 開発ブランチ。
  - コードが安定し、リリース準備ができたら `release` へマージする。
  - feature ブランチからのPull Requestを受け付ける。
  - hotfixes, releaseブランチからのマージを受け付ける
- feature branches
  - ブランチ名は `feature/` から始まる。
  - 機能の追加。 `develop` から分岐し、 `develop` にマージする。
- hotfixes
  - ブランチ名は `hotfixes/` から始まる。
  - master branchから分岐し、masterとdevelop branchにマージする。
  - リリース後のクリティカルなバグフィックスなど、 現在のプロダクトのバージョンに対する変更用。
