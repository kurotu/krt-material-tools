# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Material Diff: Added a feature to display the difference between materials.
- Quick Variant: A tool to create material variants in bulk from a game object.
- Material Variant: A tool to create material variants in bulk from multiple materials.
- Texture Replacer: Added an option to update the target material when a material is selected in the project view.

### Changed

- Show a message when replacement targets are not found.
- Changed Auto Referenced to false in asmdef.

### Removed

- Support for Unity 2019.

### Fixed

- Texture Replacer: Replacement textures not cleared when the selected material is changed.

## [1.0.0] - 2023-11-08

Initial release. Migrated from [Material Replacer](https://github.com/kurotu/MaterialReplacer).
