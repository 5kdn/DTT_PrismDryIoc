# Agents.md

## プロジェクト概要

本プロジェクトは、「DCS（Digital Combat Simulator）」の日本語化作業を支援するWindows用アプリケーションです。
アプリはPrismフレームワークとDryIocによるDIコンテナを用いたWPFアプリとして設計されています。
また、GitHubと連携して日本語翻訳ファイルのダウンロード・アップロード、ローカルパス管理等を行います。

プロジェクト構成

- DcsTranslateTool (アプリケーション本体)
    - Properties (ViewModelに定義されるプロパティ)
    - Constants (定数値をまとめるクラスやファイル)
    - Contracts (インターフェースや抽象クラス)
    - Converters (値変換ロジックを持つクラス)
    - Helpers (汎用的な便利関数やユーティリティクラス)
    - Models (アプリケーションのデータ構造やエンティティを表すクラス)
    - Providers (インスタンスやデータを供給する役割)
    - Services (ビジネスロジックや外部とのやり取りを担う)
    - Styles (UIの見た目を一括管理するリソース)
    - ViewModels
    - Views
- DcsTranslateTool.Core (アプリケーションの「コア機能」や「ロジック部分」のみを分離したライブラリプロジェクト)
    - Contracts (インターフェースや抽象クラス)
    - Models (アプリケーションのデータ構造やエンティティを表すクラス)
    - Services (本ライブラリプロジェクト内で完結するサービス)
- DcsTranslateTool.Test (DcsTranslateToolに対するユニットテスト)
- DcsTranslateTool.Core.Test (DcsTranslateTool.Coreに対するユニットテスト)

## コーディングスタイル

- コーディングスタイルは.editorconfigに従ってください。
- 行の長さは120文字を上限としてください。
- クラスや関数にはドキュメントコメントを体言止めの日本語で作成してください。
- ドキュメントコメントにはsummary, param(引数がある場合), returns(返り値がある場合)を追加してください。

## テストと品質保証

t_wadaメソッドに従ってユニットテストを作成してください。

Windows専用のコードが有る対象に対するテストコードには `[Trait("Category", "WindowsOnly")` を追加し、非Windows環境でのテストでは `dotnet test --filter "Category!=WindowsOnly"` としてテストを実行してください。

`Fact` や `Theory` には体言止めの日本語で `DisplayName` を追加してください。

## ビルド・デプロイ手順

## **コミット・PRメッセージ規約**

コミットは小さな粒度で作成してください。

メッセージは日本語で作成してください。

メッセージには以下のルールでPrefixを追加してください。

- feat: 新しい機能
- fix: バグの修正
- docs: ドキュメントのみの変更
- style: 空白、フォーマット、セミコロン追加など
- refactor: 仕様に影響がないコード改善(リファクタ)
- perf: パフォーマンス向上関連
- test: テスト関連
- chore: ビルド、補助ツール、ライブラリ関連

メッセージは変更した理由や目的を記述してください。
