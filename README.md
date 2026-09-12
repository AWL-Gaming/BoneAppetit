# BoneAppetit

AWL Gaming maintains this compatibility build of RockerKitten's BoneAppetit for current Valheim versions.

BoneAppetit provides its own food progression, Grill, Griddle, Prep Table, cooking skill integration, drops, and related content. Valharvest detects and integrates with BoneAppetit but does not replace this content.

## AWL maintenance release

Version 3.3.2 is based on the published BoneAppetit 3.3.1 package and preserves its embedded `customfood` and `grill` asset bundles. The public upstream source tree currently identifies its plugin source as 3.2.4, so the 3.3.1 published package was used as the compatibility baseline rather than silently downgrading content.

Validated on the current AWL Valheim 1.0 stack with BepInEx 5.4.23.5 and Jotunn 2.30.0. Runtime validation confirmed the Grill, Griddle, Prep Table, representative food/item prefabs, ObjectDB item registration, Jotunn registration, and Valharvest compatibility detection.

## Build

Requirements:

- .NET SDK capable of targeting .NET Framework 4.7.2
- Current BepInEx core assemblies
- Jotunn
- Current Valheim dedicated-server managed assemblies

Set either MSBuild properties or environment variables:

- `BepInExCoreDir` or `BEPINEX_CORE_DIR`: directory containing `BepInEx.dll` and `0Harmony.dll`
- `JotunnDir` or `JOTUNN_DIR`: directory containing `Jotunn.dll`
- `ValheimManagedDir` or `VALHEIM_MANAGED_DIR`: Valheim `valheim_server_Data\Managed` directory

Then run:

```powershell
dotnet build .\BoneAppetit.csproj -c Release
```

## Upstream and license

Original project: https://github.com/RockerKitten/BoneAppetit

The upstream repository is distributed under the WTFPL v2. The upstream license is retained verbatim in `LICENSE`. See `NOTICE.md` for provenance of the 3.3.2 maintenance build.
