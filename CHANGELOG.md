# Changelog

## 3.3.3 - 2026-09-13

### Changed
- Added the standard AWL Gaming badge to the package icon for consistent storefront presentation.
- Bumped package and plugin version metadata to 3.3.3; gameplay behavior and embedded content are unchanged from the validated 3.3.2 build.

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
