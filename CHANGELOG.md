# Changelog

## 3.3.11 - 2026-09-19

### Fixed
- Fixed the Stone Grill and Stone Griddle interaction areas so they can be used normally from around the station.
- Removed the Oven's repetitive chimney smoke effect.

## 3.3.10 - 2026-09-19

### Fixed
- Fixed the Oven showing Invalid Placement and refusing to build.
- Restored the Oven's original placement, collision, and material setup so it behaves correctly in the game.
- Fixed an error that could appear when leaving the game after BoneAppetit was loaded.

## 3.3.9 - 2026-09-18

### Added
- Added editable JSON localization support for BoneAppetit item and build-piece text.

### Fixed
- Fixed localization tokens appearing as raw text in-game and made translation JSON files work from both the Translations folder and beside BoneAppetit.dll.
- Fixed dim or discolored materials on the Chef Hat and smokeless fire pieces.
- Fixed the Oven appearing at the wrong size.
- Restored the Stone Griddle as its own buildable networked piece at its original size so it appears correctly in the Hammer menu and prefab tools.

## 3.3.8 - 2026-09-18

### Added
- Added external JSON localization files so BoneAppetit item and build-piece text can be translated without recompiling the mod.

### Fixed
- Fixed the Oven and Stone Griddle appearing at the wrong size.
- Fixed dim or discolored materials on the Chef Hat and smokeless fire pieces.
## 3.3.7 - 2026-09-14

### Fixed
- Fixed the Chef Hat failing when equipped on current Valheim releases.
- Fixed BoneAppetit foods, items, and cooking stations loading against the current Valheim runtime.
- Fixed the Stone Griddle incorrectly requiring a forge for placement.
- Preserved the existing recipes, food balance, drops, cooking skill behavior, and Valharvest integration.

## 3.3.2 - 2026-09-13

### Fixed
- Rebuilt the published 3.3.1 content for compatibility with the current Valheim 1.0 runtime.
- Preserved the released Grill, Griddle, Prep Table, foods, items, cooking integration, and embedded asset bundles.
- Preserved Valharvest compatibility detection and current Jotunn registration.

### Build
- Added a portable .NET Framework 4.7.2 project with explicit BepInEx, Jotunn, and Valheim reference paths.
- Updated plugin and assembly version metadata to 3.3.2.
- Suppressed only three legacy Unity serialization diagnostics for fields intentionally populated or used by Unity asset behavior (`CS0169`, `CS0414`, `CS0649`).

## 3.3.1

Upstream published release used as the maintenance baseline. See `README_UPSTREAM.md` for the upstream historical release notes.
