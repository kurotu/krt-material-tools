# 変更履歴
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### 追加

- Material Diff: マテリアルの差分を表示する機能を追加。
- Quick Variant: ゲームオブジェクトからMaterial Variantを一括作成するツール。
- Material Variant: 複数のマテリアルからMaterial Variantを一括作成するツール。
- Texture Replacer: プロジェクトビューでマテリアルを選択したときにウィンドウの対象のマテリアルを更新するオプションを追加。

### 変更

- 置換対象が見つからないときにメッセージを表示するように変更。
- asmdef で Auto Referenced を false に変更。

### 修正

- Texture Replacer: 選択したマテリアルが変更されたときに置換テクスチャがクリアされない問題を修正。

## [1.0.0] - 2023-11-08

初版。[Material Replacer](https://github.com/kurotu/MaterialReplacer) から移行。
