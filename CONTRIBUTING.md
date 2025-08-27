# CONTRIBUTING

<!-- TODO: uv + ruffを想定したpythonプロジェクトの例です。実際のプロジェクトに合わせて変更してください。 -->
このリポジトリへの貢献に関心をお寄せいただきありがとうございます。以下のガイドラインに従って、開発・レビュー・リリースに参加してください。

---

## 方針の要約（重要）

- **リポジトリフロー:** GitHub Flow
- **リポジトリ構成:** 単一リポジトリ（モノレポ）
- **デプロイ対象:** `master` ブランチのみ（常にデプロイ可能状態）
- **マージ方式:** **Squash and Merge** をデフォルト
- **コミット/PRタイトル:** [Conventional Commits](https://www.conventionalcommits.org/ja/v1.0.0/) 準拠
- **`master` への取り込み手順:**
  1. 作業ブランチを作成
  2. 作業ブランチにコミット
  3. Pull Request (PR) を作成
  4. テスト・CI・レビューをクリア
  5. `master` へ **Squash Merge**

---

## 開発環境（Python / uv / ruff）

<!-- TODO: uv + ruffを想定したpythonプロジェクトの例です。実際のプロジェクトに合わせて変更してください。 -->
このプロジェクトは **uv** を前提にした Python プロジェクトです。linter/formatter は **ruff** を使用します。

### 前提ツール

<!-- TODO: uv + ruffを想定したpythonプロジェクトの例です。実際のプロジェクトに合わせて変更してください。 -->
- Python（推奨: 3.11 以降）
- [uv](https://docs.astral.sh/uv/)（Python パッケージ/環境管理）
- Git, GitHub アカウント

### 初回セットアップ

<!-- TODO: uv + ruffを想定したpythonプロジェクトの例です。実際のプロジェクトに合わせて変更してください。 -->
```bash
# uv が未導入なら
curl -LsSf https://astral.sh/uv/install.sh | sh

# 仮想環境の作成（.venv）
uv venv --seed

# 依存関係の同期（pyproject.toml がある前提）
uv sync

# 開発ツール一式の導入（必要に応じて）
uv add --dev ruff
# （テストを使う場合は）
uv add --dev pytest pytest-cov

# シェルへ入る
source .venv/bin/activate  # Windows: .venv\Scripts\activate
```

### フォーマッタ & リンター (ruff)

<!-- TODO: uv + ruffを想定したpythonプロジェクトの例です。実際のプロジェクトに合わせて変更してください。 -->
推奨設定は `pyproject.toml` に集約します。

日常コマンド:

```bash
# 自動整形
uv run ruff format .

# 静的解析（自動修正も）
uv run ruff check . --fix
```

<!-- TODO: 必要であればgithooksに追加 -->
> PR 作成前に `ruff format` と `ruff check --fix` を必ず実行してください。

### テスト（任意・採用している場合）

<!-- TODO: uv + ruffを想定したpythonプロジェクトの例です。実際のプロジェクトに合わせて変更してください。 -->
```bash
uv run pytest -q
uv run pytest --cov=src --cov-report=term-missing
```

> テスト基盤を採用しているディレクトリ/パッケージでは、PR でテスト追加/更新をお願いします。

---

## ブランチ運用（GitHub Flow）

- 基本ブランチは `master` です。`master` は常に **デプロイ可能** な状態を保ちます。
- 作業は課題単位で **短命ブランチ** を切って進めます。
  - 例: `feat/short-description`, `fix/issue-123`, `docs/update-contributing`
- `master` から分岐 → 変更 → PR → レビュー/CI 通過 → **Squash Merge** → `master` へ反映。

### コミット & PR タイトル規約（Conventional Commits）

- 形式: `type(scope): summary`
- 主な `type`: `feat`, `fix`, `docs`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`
- 例:
  - `feat(api): add user search endpoint`
  - `fix(db): handle null on migration`
  - `docs: update README for setup with uv`

> **PR タイトル** は Squash 時にコミットメッセージとして採用されるため、Conventional Commits に準拠してください。

---

## Pull Request（PR）

**PR を小さく、レビューしやすく** 保つことを推奨します。

### PR 作成前チェックリスト

- [ ] `ruff format` を実行した
- [ ] `ruff check --fix` で警告/エラーを解消した
- [ ] （採用している場合）テストを追加/更新し、ローカルで成功した
- [ ] 破壊的変更があれば、ドキュメントと変更点を明記した

### PR 説明テンプレート（例）
<!-- TODO: .github\PULL_REQUEST_TEMPLATE.md に合わせる -->

```markdown
## 目的
この PR の背景と目的を簡潔に記載

## 変更点
- 主要な変更点の箇条書き

## 動作確認
- 実施した確認手順

## 影響範囲
- 影響するモジュール/利用者/設定など

## 備考
- レビュー観点、追加情報 など
```

### レビュー

- **レビュー観点**: 設計妥当性、テスト、影響範囲、可読性、Conventional Commits、CI 結果。
- **マージ権限**: 原則として **Maintainer** が最終マージ（必要に応じて Reviewer も可）。
- **Squash and Merge** をデフォルトとし、PR タイトルを最終コミットメッセージに採用します。

---

## ドキュメント

- 設定・仕様・設計に変更があれば、`README.md` / `docs/` / `CHANGELOG` 等を更新してください。
<!-- TODO: uv + ruffを想定したpythonプロジェクトの例です。実際のプロジェクトに合わせて変更してください。 -->
- コード例・コマンド・設定は可能な限り `pyproject.toml` に集約し、`uv` 経由で再現可能にします。

---

## CI/CD

- `master` への取り込み前に **CI がグリーン** であることが必須です。
- セキュリティ/品質/テストの自動チェックはワークフローで実施します（内容は `.github/workflows/` を参照）。

---

## リリース

- `master` が常にデプロイ可能であることを前提に運用します。
- リリース実施方法やアセット添付の自動化は、ワークフローおよび運用ドキュメントに従ってください（本ドキュメントでは詳細規定しません）。

---

## コードスタイル

<!-- TODO: uv + ruffを想定したpythonプロジェクトの例です。実際のプロジェクトに合わせて変更してください。 -->
- .editorconfig を確認してください。
- Python: PEP 8 ベース。**ruff** に従った自動整形/静的解析を必須とします。
- 型: 可能な限り型注釈（`typing` / `pydantic` 等）を付与してください。
- 例外/ログ: ログ出力の方針がある場合はそれに従い、例外はコンテキストを含めて握りつぶさないこと。

---

## Issue 運用

- バグ報告・機能要望は Issue を作成し、背景/再現手順/期待動作/追加情報を記載してください。
- 着手前に Issue と PR をリンクさせて、トラッキング可能にします（例: `Fixes #123`）。

---

## 行動規範

- 参加者は互いを尊重し、建設的な議論を心がけてください。差別・ハラスメント行為は容認しません。

---

## よくある質問（FAQ）

<!-- TODO: uv + ruffを想定したpythonプロジェクトの例です。実際のプロジェクトに合わせて変更してください。 -->
**Q. Python のバージョンは？**
A. 基本は `pyproject.toml` の `requires-python` に従います。未記載の場合は 3.11 以降を推奨します。

**Q. フォーマッタとリンターは？**
A. いずれも **ruff** を使用します。`uv run ruff format` と `uv run ruff check --fix` を PR 前に必ず実行してください。

**Q. 他言語/ツールのサブプロジェクトがある場合は？**
A. モノレポ方針のもと、各パッケージ配下の `README.md`/`CONTRIBUTING.md` に追加規約を記載してください。

---

## 連絡先
<!-- TODO: 連絡先の設定 -->
質問や提案は Issue / Discussion / PR でお願いします。重大なセキュリティ問題は公開前にメンテナへ直接ご連絡ください。
