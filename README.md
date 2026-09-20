# BoneAppetit

**AWL Gaming maintained compatibility build for Valheim.**

BoneAppetit was originally created by **RockerKitten**. AWL Gaming maintains this build because the published 3.3.1 package provides content that is still used by Valheim modpacks but needed compatibility fixes. AWL Gaming does not claim authorship of the original foods, stations, assets, recipes, balance, or gameplay design.

## What it adds

- BoneAppetit's food progression and creature drops.
- Grill, Griddle, and Prep Table stations.
- Cooking integration, recipes, items, and associated assets.
- Compatibility used by Valharvest when BoneAppetit is installed.

Valharvest integrates with BoneAppetit but does not replace BoneAppetit's own foods or stations.

## AWL 3.3.14 maintained release

- Fixed the Smokeless Firepit, Smokeless Hearth, and Smokeless Brazier still producing smoke.
- Fixed the Stone Grill becoming unplaceable after removing or moving an existing one.
- Fixed the Prep Table sometimes failing to appear as a buildable piece after loading.
- Improved build-piece registration reliability for BoneAppetit cooking stations.
- Fixed a client crash when using the Stone Grill and fixed its interaction area in multiplayer.
- Restored the Oven's chimney smoke, internal fire, and lighting, and fixed the effects repeatedly restarting during use.
- Fixed the Chef Hat so it can be equipped normally again.
- Fixed the Stone Griddle so it no longer incorrectly requires a forge for placement.
- Fixed the Oven showing Invalid Placement and restored its original placement, collision, and material behavior.
- Fixed an error that could appear when leaving the game after BoneAppetit was loaded.
- Restored the Stone Griddle to the Hammer/build lists at its original size and fixed the Oven scale.
- Fixed localization token display and translation-file loading for normal and flattened mod-manager installs.
- Fixed dim or discolored materials on the Chef Hat and smokeless fire pieces.
- Compatible with Valheim and Jotunn 2.30.0.
- Reconstructed from the published BoneAppetit package while retaining original author attribution and the upstream license.

## Localization

BoneAppetit ships Translations/English.json as the translation template. Copy it to another JSON file, keep the token keys unchanged, and translate the values. Translation JSON files can be loaded from the Translations folder or beside BoneAppetit.dll so they continue to work with mod managers that flatten package folders.

## Installation

Install on both the server and every client that connects to it. Jotunn and BepInEx are required and are declared as package dependencies.

## AWL maintenance and support

- AWL Gaming website: https://awlgaming.net
- Maintained source: https://github.com/AWL-Gaming/BoneAppetit
- Bug reports for this maintained build: https://github.com/AWL-Gaming/BoneAppetit/issues
- Optional support for AWL compatibility maintenance and testing: https://patreon.awlgaming.net

Support is optional and is for AWL's compatibility, testing, packaging, and maintenance work on this fork. The mod remains available regardless of support.

## Original project and attribution

- Original author: RockerKitten
- Original source: https://github.com/RockerKitten/BoneAppetit
- Original Thunderstore package: https://thunderstore.io/c/valheim/p/RockerKitten/BoneAppetit/
- Upstream license: WTFPL Version 2, preserved in `LICENSE`.

The original project README is retained in the source repository as `README_UPSTREAM.md` for historical documentation and credits.

## Source and build

The maintained source is public in the AWL repository above. Build requirements are .NET Framework 4.7.2 targeting support, BepInEx core assemblies, Jotunn, and the game's managed assemblies.

```powershell
dotnet build .\BoneAppetit.csproj -c Release
```
