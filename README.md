# KRT Material Tools

Small tools to manipulate materials.

> [!NOTE]
> 日本語の説明は後半にあります。
> (Japanese description is available at the bottom of this page.)

## Features and Usage

All tools are available in **Tools/KRT Material Tools** menu.

### Material Replacer

Tool to replace materials at once by defining replacement rules.

This save the hassle of manually changing materials one by one. It would be useful when applying your own materials to dressed avatars.

[Demo (YouTube)](https://youtu.be/cPbJyPUZaqo)

![Material Replacer](./images/material-replacer.png)

1. Set a game object to the window.
2. Materials of child objects are listed. Set materials to **Ad Hoc Rule** column.
3. Press **Apply** button. Existing materials are replaced with **Ad Hoc Rule** materials.

#### Material Replacer Rule

Use **Create/KRT Material Tools/Material Replacer Rule** context menu to create a rule asset.

Migration is required when you used [legacy Material Replacer](https://github.com/kurotu/MaterialReplacer) in the project before.
(**Tools/KRT Material Tools/Migrate Legacy Assets** menu)

### Texture Replacer

Tool to replace textures at once by defining replacement rules.

This tool would be useful when a texture is referenced by multiple material properties.
You would not forget to change all of them.

![Texture Replacer](./images/texture-replacer.png)

1. Set a material to the window.
2. Textures are listed. Set textures to **Replacement** column.
3. Press **Apply** button to overwrite, or **Save As...** button to save as a new material. Then the textures are applied.

### Material Diff

Tool to display the difference between materials.

This tool would be useful when you want to compare materials.

1. Set two materials to the window.
2. Differences are displayed.
3. Press **→** or **←** button to copy the parameter from the left or right material to the other.

## Installation

There are two options to import the package into your project.

### [Recommended] VRChat Creator Companion (VCC)

Click [this link](https://kurotu.github.io/vpm-repos/vpm.html) to add a community repository to VCC.
Then add KRT Material Tools to your project.

[Legacy Material Replacer](https://github.com/kurotu/MaterialReplacer) will be automatically removed when installing KRT Material Tools to your project with VCC.

### Unitypackage

Download the latest .unitypackage from [the release page](https://github.com/kurotu/krt-material-tools/releases/latest) or [Booth](https://kurotu.booth.pm).

## Verified Environments

- Unity 2019.4.29f1

## Contact

- VRCID: kurotu
- Twitter: [@kurotu](https://twitter.com/kurotu)
- GitHub: [kurotu/krt-material-tools](https://github.com/kurotu/krt-material-tools)

---

# KRT Material Tools

マテリアルを操作する小さなツール群

## 機能・使い方

すべてのツールは **Tools/KRT Material Tools** メニューから利用できます。

### Material Replacer

ルールを定義して一括でマテリアルを置換するツールです。

マテリアルを一つずつ手動で変更する手間を省くことができます。衣装を着たアバターに自分のマテリアルを適用するときなどに便利です。

[デモ (YouTube)](https://youtu.be/cPbJyPUZaqo)

![Material Replacer](./images/material-replacer.png)

1. 開いたウィンドウに Game Object を設定します。
2. 子オブジェクトのマテリアルが列挙されるので、入れ替えたいマテリアルを **Ad Hoc Rule** 列に設定します。
3. **Apply** を押すと、既存のマテリアルが **Ad Hoc Rule** のマテリアルに置き換わります。

#### Material Replacer Rule

**Create/KRT Material Tools/Material Replacer Rule** コンテキストメニューからルールアセットを作成します。

過去にプロジェクト内で [旧 Material Replacer](https://github.com/kurotu/MaterialReplacer) を利用していた場合は移行処理が必要です。
(**Tools/KRT Material Tools/Migrate Legacy Assets** メニュー)

### Texture Replacer

ルールを定義して一括でテクスチャを置換するツールです。

テクスチャがマテリアルの複数プロパティで参照されているときに便利です。
すべての参照を忘れずに変更できます。

![Texture Replacer](./images/texture-replacer.png)

1. 開いたウィンドウにマテリアルを設定します。
2. テクスチャが列挙されるので、入れ替えたいテクスチャを **Replacement** 列に設定します。
3. **Apply** で上書き、または **Save As...** で別マテリアルとして保存し、設定したテクスチャを反映します。

### Material Diff

マテリアルの差分を表示するツールです。

マテリアルを比較したいときに便利です。

1. 開いたウィンドウに2つのマテリアルを設定します。
2. 差分が表示されます。
3. **→** または **←** ボタンを押すと、左右のマテリアルのパラメータをもう一方にコピーします。

## インストール

2つの方法があります。

### [推奨] VRChat Creator Companion (VCC)

[このリンク](https://kurotu.github.io/vpm-repos/vpm.html)をクリックして VCC にコミュニティリポジトリを追加します。
その後 KRT Material Tools をプロジェクトに追加します。

VCC で KRT Material Tools をプロジェクトに追加すると [旧 Material Replacer](https://github.com/kurotu/MaterialReplacer) は自動的に削除されます。

### Unitypackage

[リリースページ](https://github.com/kurotu/krt-material-tools/releases/latest) または [Booth](https://kurotu.booth.pm) から最新の .unitypackage をダウンロードします。

## 動作確認済み環境

- Unity 2019.4.29f1

## 連絡先

- VRCID: kurotu
- Twitter: [@kurotu](https://twitter.com/kurotu)
- GitHub: [kurotu/krt-material-tools](https://github.com/kurotu/krt-material-tools)
