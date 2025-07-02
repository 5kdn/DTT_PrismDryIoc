# AGENTS.md

## 概要

本プロジェクトは、「DCS（Digital Combat Simulator）」の日本語化作業を支援するWindowsアプリケーションです。  
アプリはPrismフレームワークとDryIocによるDIコンテナを用いたWPFアプリとして設計されています。  
また、GitHubと連携して日本語翻訳ファイルのダウンロード・アップロード、ローカルパス管理等を行います。

本ドキュメントでは、プロジェクト内の各エージェント（サービス、モジュール、責務単位）とその役割、実装場所について記述します。

---

## 1. アプリケーション・エージェント一覧

### 1.1. ファイル操作エージェント（FileService）

- **責務**  
  ファイル・ディレクトリ操作の抽象化および提供。
  - ローカルファイルの読み書き
  - ディレクトリスキャン
  - DCS翻訳ファイルの管理

- **主な実装**  
  - `MyApp.Core.Services.FileService`
  - インタフェース: `MyApp.Core.Contracts.Services.IFileService`

---

### 1.2. 設定管理エージェント（SettingsService）

- **責務**  
  アプリケーションの各種設定（例：DCSインストールパス、GitHubアカウント情報など）を永続化・管理。

- **主な実装**  
  - `MyApp.Services.SettingsService`
  - インタフェース: `MyApp.Contracts.Services.ISettingsService`

---

### 1.3. GitHub連携エージェント（GitHubService）

- **責務**  
  GitHub API（octokit等）を利用した以下の機能を提供。
  - リポジトリ一覧取得
  - 翻訳ファイルのアップロード・ダウンロード

- **主な実装**  
  - `MyApp.Services.GitHubService`
  - インタフェース: `MyApp.Contracts.Services.IGitHubService`

---

### 1.4. テーマ切り替えエージェント（ThemeSelectorService）

- **責務**  
  アプリケーションのUIテーマ（明るい/暗いなど）の切り替え・管理

- **主な実装**  
  - `MyApp.Services.ThemeSelectorService`
  - インタフェース: `MyApp.Contracts.Services.IThemeSelectorService`

---

### 1.5. システム情報エージェント（SystemService）

- **責務**  
  OSバージョンやハードウェア等のシステム情報の取得

- **主な実装**  
  - `MyApp.Services.SystemService`
  - インタフェース: `MyApp.Contracts.Services.ISystemService`

---

### 1.6. 状態永続化エージェント（PersistAndRestoreService）

- **責務**  
  アプリケーションの状態保存とリストア機能（ウィンドウ位置、サイズ等）

- **主な実装**  
  - `MyApp.Services.PersistAndRestoreService`
  - インタフェース: `MyApp.Contracts.Services.IPersistAndRestoreService`

---

### 1.7. アプリケーション情報エージェント（ApplicationInfoService）

- **責務**  
  アプリのバージョンや製品名等、アプリ自身の情報提供

- **主な実装**  
  - `MyApp.Services.ApplicationInfoService`
  - インタフェース: `MyApp.Contracts.Services.IApplicationInfoService`

---

## 2. ViewModel・UIエージェント

PrismのMVVM構成により、各画面（ページ）ごとにViewModelが存在します。  
各ViewModelは上記サービス群をDI経由で利用し、UIロジックを担当します。

- 例:  
  - `MainViewModel`
  - `SettingsViewModel`
  - `TranslationUploadViewModel`
  - など

---

## 3. テストエージェント

- **目的**  
  各サービス・ViewModelの単体テスト、結合テストを担う

- **主な実装**  
  - `MyApp.Tests`プロジェクト配下  
  - xUnit＋Moqで実装

---

## 4. その他

- **DI管理エージェント**  
  DryIocを用いて全サービス・ViewModelの依存性を一元管理

---

## 備考

- 新規エージェントを追加する場合は、各責務を明確化し、インタフェースを分離して実装すること。
- サービスの実装は「Contracts/Interfaces」と「Services」フォルダで分離することが推奨されます。
- ViewModelやサービス間の責務の分担を明確にすることで、保守性・テスト容易性を向上させます。

---

**最終更新**: 2025-07-02
