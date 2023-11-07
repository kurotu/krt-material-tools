# KRT Material Tools

Small tools to manipulate materials.

[ English | [日本語](./README_JP.md) ]

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
