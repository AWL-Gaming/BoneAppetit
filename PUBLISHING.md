# Publishing BoneAppetit

## Public identities

- GitHub: https://github.com/AWL-Gaming/BoneAppetit
- Thunderstore: AWLGaming/BoneAppetit in the Valheim community
- Hexium: AWLGaming/BoneAppetit in the Valheim community
- AWL Gaming: https://awlgaming.net
- Optional AWL maintenance support: https://patreon.awlgaming.net

This is an AWL-maintained compatibility release of the original BoneAppetit by RockerKitten. Keep original-author attribution, upstream links, NOTICE.md, and license/rights status in every public package.

## Current version

3.3.15

## Package requirements

The package root must contain manifest.json, README.md, icon.png, CHANGELOG.md, NOTICE.md, and BoneAppetit.dll. It must also contain LICENSE and Translations/English.json.

Thunderstore namespace: AWLGaming. Hexium namespace: AWLGaming. Install scope: Client & Server.

Before publication, verify the rendered README, dependency versions, package version, icon, source link, AWL website/support links, attribution, NOTICE/license status, and public download.

Thunderstore and Hexium versions are immutable. Bump the package version for any later package-content update. Bump the plugin version only when the plugin binary version changes.

## 3.3.5 package revision

3.3.5 is a storefront package-only revision. It retains the tested BoneAppetit 3.3.3 plugin binary and therefore does not change the BepInEx plugin version or gameplay behavior.

## 3.3.7 runtime release

3.3.7 contains the tested BoneAppetit 3.3.3 plugin binary. This release fixes Valheim compatibility issues affecting asset loading, Chef Hat equipping, and the Stone Griddle placement requirement while preserving the existing food, recipe, drop, cooking-skill, and Valharvest integration behavior.

## 3.3.8 maintenance release

3.3.8 fixes cooking-station visual scale, material lighting in the game, and adds external JSON translations loaded through Jotunn.

## 3.3.9 maintenance release

3.3.9 fixes localization token resolution and translation-file loading, corrects Chef Hat and smokeless-fire material lighting, fixes the Oven scale, and restores the Stone Griddle as its own buildable networked piece at the original size.


## 3.3.10 maintenance release

3.3.10 fixes the Oven showing Invalid Placement, restores its original placement and material behavior, and fixes an error that could appear when leaving the game after BoneAppetit was loaded.

## 3.3.12 maintenance release

3.3.12 fixes a Stone Grill client crash and interaction targeting, restores the Oven's smoke, fire, and lighting, and fixes the Oven effects repeatedly restarting or looping during use.

## 3.3.13 maintenance release

3.3.13 fixes the Stone Grill becoming unplaceable after removing or moving it, fixes the Prep Table sometimes failing to appear as a buildable piece, and improves build-piece registration reliability for BoneAppetit cooking stations.

## 3.3.14 maintenance release

3.3.14 fixes the Smokeless Firepit, Smokeless Hearth, and Smokeless Brazier still producing smoke.

## 3.3.15 maintenance release

3.3.15 fixes the Stone Grill sometimes refusing to place after the 3.3.14 update, including after removing and rebuilding it near an Oven.
