The native Windows workflow requires a current Valheim Managed directory, BepInEx core directory and Jotunn directory. Configure `AWL_VALHEIM_MANAGED_DIR`, `AWL_BEPINEX_CORE_DIR` and `AWL_JOTUNN_DIR` as repository variables. Existing runner environment variables `VALHEIM_MANAGED_DIR`, `BEPINEX_CORE_DIR` and `JOTUNN_DIR` are also supported.

`Prepare-NativeReferences.ps1` publicizes fresh game assemblies into a new owned directory and records input hashes. `NativeReferences.targets` gives these references precedence over older bundled libraries. Neither helper writes to the game directory. The build artifact contains only the mod DLL and reference manifest.

The workflow retains AWL-hosted Windows runners and builds with an existing stable .NET 10 SDK. Compilation and reference validation do not establish in-game acceptance.
