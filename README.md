# BoneAppetit

**AWL Gaming maintained compatibility build for current Valheim releases.**

BoneAppetit was originally created by **RockerKitten**. AWL Gaming maintains this build because the published 3.3.1 package provides content that is still used by current modpacks but required compatibility work for current Valheim. AWL Gaming does not claim authorship of the original foods, stations, assets, recipes, balance, or gameplay design.

## What it adds

- BoneAppetit's food progression and creature drops.
- Grill, Griddle, and Prep Table stations.
- Cooking integration, recipes, items, and associated assets.
- Compatibility used by Valharvest when BoneAppetit is installed.

Valharvest integrates with BoneAppetit but does not replace BoneAppetit's own foods or stations.

## AWL 3.3.7 maintained release

- Updated BoneAppetit for current Valheim releases while preserving the original food progression, recipes, drops, cooking stations, and cooking skill behavior.
- Fixed the Chef Hat so it can be equipped normally again.
- Fixed the Stone Griddle so it no longer incorrectly requires a forge for placement.
- Preserved Valharvest compatibility when both mods are installed.
- Compatible with current Valheim releases and Jotunn 2.30.0.
- Reconstructed from the published BoneAppetit package while retaining original author attribution and the upstream license.

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

The maintained source is public in the AWL repository above. Build requirements are .NET Framework 4.7.2 targeting support, BepInEx core assemblies, Jotunn, and current Valheim managed assemblies.

```powershell
dotnet build .\BoneAppetit.csproj -c Release
```