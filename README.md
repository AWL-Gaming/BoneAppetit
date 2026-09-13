# BoneAppetit

**AWL Gaming maintained compatibility build for current Valheim releases.**

BoneAppetit was originally created by **RockerKitten**. AWL Gaming maintains this build because the published 3.3.1 package provides content that is still used by current modpacks but required compatibility work for current Valheim. AWL Gaming does not claim authorship of the original foods, stations, assets, recipes, balance, or gameplay design.

## What it adds

- BoneAppetit's food progression and creature drops.
- Grill, Griddle, and Prep Table stations.
- Cooking integration, recipes, items, and associated assets.
- Compatibility used by Valharvest when BoneAppetit is installed.

Valharvest integrates with BoneAppetit but does not replace BoneAppetit's own foods or stations.

## AWL 3.3.3 maintained release

- 3.3.3 keeps the validated 3.3.2 gameplay content and adds the standard AWL Gaming storefront badge; gameplay behavior is unchanged.
- Reconstructed from the actual published BoneAppetit 3.3.1 package because the public upstream source tree still identifies its plugin source as 3.2.4.
- Preserved the published `customfood` and `grill` embedded asset bundles byte-for-byte.
- Preserved Grill, Griddle, Prep Table, foods, items, drops, cooking integration, and released gameplay content.
- Runtime-validated Jotunn registration, ObjectDB item registration, representative food/item prefabs, all three stations, and Valharvest compatibility detection.
- Builds cleanly against current Valheim, BepInEx 5.4.23.5, Harmony 2.9.0, and Jotunn 2.30.0.

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