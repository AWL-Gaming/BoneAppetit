using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boneappetit;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency(Jotunn.Main.ModGuid)]
[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
public sealed class BoneAppetit : BaseUnityPlugin
{
    public const string PluginGUID = "com.rockerkitten.boneappetit";
    public const string PluginName = "BoneAppetit";
    public const string PluginVersion = "3.3.21";
    private const string BundleResourceName = "BoneAppetit.assets";
    private const string LegacyGrillResourceName = "BoneAppetit.grill";
    private const string LegacyFoodResourceName = "BoneAppetit.customfood";
    private const string AssetRoot = "assets/boneappetit6000";
    private const string HammerPieceTable = "_HammerPieceTable";

    private readonly Dictionary<string, CustomItem> _items = new Dictionary<string, CustomItem>(StringComparer.Ordinal);
    private readonly Dictionary<string, CustomPiece> _pieces = new Dictionary<string, CustomPiece>(StringComparer.Ordinal);
    private readonly Dictionary<ConeEffect, SE_Stats> _coneEffects = new Dictionary<ConeEffect, SE_Stats>();
    private readonly Dictionary<string, Texture2D> _legacyTextures = new Dictionary<string, Texture2D>(StringComparer.OrdinalIgnoreCase);
    private Harmony _harmony;
    private AssetBundle _assets;
    private AssetBundle _legacyGrillAssets;
    private AssetBundle _legacyFoodAssets;
    private bool _dropsApplied;
    private bool _addingExtraItem;
    private bool? _appliedOriginalGrill;

    public static BoneAppetit Instance { get; private set; }
    public Sprite CookingSprite { get; private set; }
    public Skills.SkillType rkCookingSkill;
    public static ConfigEntry<int> NexusId;
    public ConfigEntry<bool> ConesEnable { get; private set; }
    public ConfigEntry<bool> PorkRindEnable { get; private set; }
    public ConfigEntry<bool> KabobEnable { get; private set; }
    public ConfigEntry<bool> FriedLoxEnable { get; private set; }
    public ConfigEntry<bool> GlazedCarrotEnable { get; private set; }
    public ConfigEntry<bool> BaconEnable { get; private set; }
    public ConfigEntry<bool> SmokedFishEnable { get; private set; }
    public ConfigEntry<bool> PancakesEnable { get; private set; }
    public ConfigEntry<bool> PizzaEnable { get; private set; }
    public ConfigEntry<bool> CoffeeEnable { get; private set; }
    public ConfigEntry<bool> LatteEnable { get; private set; }
    public ConfigEntry<bool> SmokelessEnable { get; private set; }
    public ConfigEntry<bool> HaggisEnable { get; private set; }
    public ConfigEntry<bool> CandiedTurnipEnable { get; private set; }
    public ConfigEntry<bool> MoochiEnable { get; private set; }
    public ConfigEntry<bool> Nut_EllaEnable { get; private set; }
    public ConfigEntry<bool> BrothEnable { get; private set; }
    public ConfigEntry<bool> FishStewEnable { get; private set; }
    public ConfigEntry<bool> ButterEnable { get; private set; }
    public ConfigEntry<bool> BloodSausageEnable { get; private set; }
    public ConfigEntry<bool> OmletteEnable { get; private set; }
    public ConfigEntry<bool> BurgerEnable { get; private set; }
    public ConfigEntry<bool> PorridgeEnable { get; private set; }
    public ConfigEntry<bool> PBJEnable { get; private set; }
    public ConfigEntry<bool> BoiledEggEnable { get; private set; }
    public ConfigEntry<bool> CakeEnable { get; private set; }
    public ConfigEntry<bool> GrillOriginal { get; private set; }
    public ConfigEntry<bool> CarrotSticksEnable { get; private set; }
    public ConfigEntry<bool> CheffHatEnable { get; private set; }
    public ConfigEntry<bool> MeadEnable { get; private set; }
    public ConfigEntry<bool> CookingSkillEnable { get; private set; }
    public ConfigEntry<bool> BonusWhenCookingEnabled { get; private set; }
    public ConfigEntry<bool> HatSEMessage { get; private set; }
    public ConfigEntry<float> HatXpGain { get; private set; }

    private void Awake()
    {
        Instance = this;
        RegisterBuiltInEnglishTranslations();
        LoadTranslations();
        CreateConfigValues();
        _assets = LoadEmbeddedAssetBundle(BundleResourceName);
        _legacyGrillAssets = LoadEmbeddedAssetBundle(LegacyGrillResourceName);
        _legacyFoodAssets = LoadEmbeddedAssetBundle(LegacyFoodResourceName);
        if (_assets == null || _legacyGrillAssets == null || _legacyFoodAssets == null)
        {
            Logger.LogError("BoneAppetit could not load its Unity 6000 asset bundle.");
            return;
        }
        BuildLegacyTextureMap();
        CookingSprite = LoadSprite("icon_rk_chef");
        AddCookingSkill();
        PrefabManager.OnVanillaPrefabsAvailable += RegisterRuntimeContent;
        ItemManager.OnItemsRegistered += AddDrops;
        SynchronizationManager.OnConfigurationSynchronized += OnConfigurationSynchronized;
        PieceManager.OnPiecesRegistered += EnsureCookingStationsRegistered;
        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginGUID);
        Logger.LogInfo("BoneAppetit modern runtime initialized.");
    }

    private static string TokenKey(string prefab, string field) => "boneappetit_" + prefab + "_" + field;
    private static string Token(string prefab, string field) => "$" + TokenKey(prefab, field);

    private void RegisterBuiltInEnglishTranslations()
    {
        foreach (ItemDefinition definition in BoneAppetitData.Items)
        {
            LocalizationManager.Instance.AddToken(TokenKey(definition.Prefab, "name"), definition.Name, true);
            LocalizationManager.Instance.AddToken(TokenKey(definition.Prefab, "description"), definition.Description ?? string.Empty, true);
        }

        RegisterPieceEnglish("rk_grill", "Grill");
        RegisterPieceEnglish("rk_griddle", "Stone Griddle");
        RegisterPieceEnglish("rk_prep", "Prep Table");
        RegisterPieceEnglish("rk_oven", "Oven");
        RegisterPieceEnglish("rk_campfire", "Smokeless Firepit");
        RegisterPieceEnglish("rk_hearth", "Smokeless Hearth");
        RegisterPieceEnglish("rk_brazier", "Smokeless Brazier");
    }

    private static void RegisterPieceEnglish(string prefab, string name)
    {
        LocalizationManager.Instance.AddToken(TokenKey(prefab, "name"), name, true);
        LocalizationManager.Instance.AddToken(TokenKey(prefab, "description"), string.Empty, true);
    }

    private void LoadTranslations()
    {
        string dllDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        if (string.IsNullOrEmpty(dllDirectory)) return;

        // Manual installs can put the DLL directly in shared BepInEx/plugins.
        // Never scan another plugin's translations or shared Assets directories.
        bool flat = string.Equals(
            Path.GetFileName(dllDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)),
            "plugins", StringComparison.OrdinalIgnoreCase);
        var candidates = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        string[] ownedDirectories = flat
            ? new[] { Path.Combine(dllDirectory, "AWLGaming-BoneAppetit"), Path.Combine(dllDirectory, "BoneAppetit") }
            : new[] { dllDirectory };

        foreach (string ownedDirectory in ownedDirectories)
        {
            if (!Directory.Exists(ownedDirectory)) continue;
            string translationsDirectory = Path.Combine(ownedDirectory, "Translations");
            if (Directory.Exists(translationsDirectory))
            {
                foreach (string file in Directory.GetFiles(translationsDirectory, "*.json", SearchOption.AllDirectories))
                    candidates[file] = translationsDirectory;
            }

            foreach (string file in Directory.GetFiles(ownedDirectory, "*.json", SearchOption.TopDirectoryOnly))
            {
                if (!string.Equals(Path.GetFileName(file), "manifest.json", StringComparison.OrdinalIgnoreCase))
                    candidates[file] = translationsDirectory;
            }
        }

        // Flat manual installs can use BoneAppetit.English.json beside the DLL.
        // No other root-level JSON file belongs to BoneAppetit.
        if (flat)
        {
            foreach (string file in Directory.GetFiles(dllDirectory, "BoneAppetit.*.json", SearchOption.TopDirectoryOnly))
                candidates[file] = Path.Combine(dllDirectory, "Translations");
        }

        foreach (KeyValuePair<string, string> entry in candidates)
        {
            try
            {
                string language = ResolveTranslationLanguage(entry.Key, entry.Value);
                LocalizationManager.Instance.AddJson(language, File.ReadAllText(entry.Key));
                Logger.LogInfo("BoneAppetit loaded " + language + " translations from " + Path.GetFileName(entry.Key) + ".");
            }
            catch (Exception ex)
            {
                Logger.LogWarning("BoneAppetit could not load translation file " + Path.GetFileName(entry.Key) + ": " + ex.Message);
            }
        }
    }

    private static string ResolveTranslationLanguage(string file, string translationsDirectory)
    {
        string directory = Path.GetDirectoryName(file) ?? string.Empty;
        if (Directory.Exists(translationsDirectory) &&
            directory.StartsWith(translationsDirectory, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(directory, translationsDirectory, StringComparison.OrdinalIgnoreCase))
        {
            return new DirectoryInfo(directory).Name;
        }

        string name = Path.GetFileNameWithoutExtension(file);
        const string prefix = "BoneAppetit.";
        return name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ? name.Substring(prefix.Length) : name;
    }
    private void OnDestroy()
    {
        PrefabManager.OnVanillaPrefabsAvailable -= RegisterRuntimeContent;
        ItemManager.OnItemsRegistered -= AddDrops;
        SynchronizationManager.OnConfigurationSynchronized -= OnConfigurationSynchronized;
        PieceManager.OnPiecesRegistered -= EnsureCookingStationsRegistered;
        _harmony?.UnpatchSelf();
        if (_assets) _assets.Unload(false);
        if (_legacyGrillAssets) _legacyGrillAssets.Unload(false);
        if (_legacyFoodAssets) _legacyFoodAssets.Unload(false);
        _assets = null;
        _legacyGrillAssets = null;
        _legacyFoodAssets = null;
        _legacyTextures.Clear();
        if (ReferenceEquals(Instance, this)) Instance = null;
    }

    private void CreateConfigValues()
    {
        Config.SaveOnConfigSet = true;
        NexusId = Config.Bind("Hidden", "NexusId", 1250, new ConfigDescription("NexusId", null,
            new ConfigurationManagerAttributes { IsAdminOnly = false, Browsable = false, ReadOnly = true }));
        ConesEnable = BindToggle("Cones", "All Cones Enable");
        PorkRindEnable = BindToggle("Pork Rind", "Pork Rind Enable");
        KabobEnable = BindToggle("Kabob", "Kabob Enable");
        FriedLoxEnable = BindToggle("Fried Lox", "Fried Lox Enable");
        GlazedCarrotEnable = BindToggle("Glazed Carrots", "Glazed Carrots Enable");
        BaconEnable = BindToggle("Bacon", "Bacon Enable");
        SmokedFishEnable = BindToggle("Smoked Fish", "Smoked Fish Enable");
        PancakesEnable = BindToggle("Pancakes", "Pancakes Enable");
        PizzaEnable = BindToggle("Pizza", "Pizza Enable");
        CoffeeEnable = BindToggle("Coffee", "Coffee Enable");
        LatteEnable = BindToggle("Latte", "Latte Enable");
        PorridgeEnable = BindToggle("Porridge", "Porridge Enable");
        PBJEnable = BindToggle("PBJ", "PBJ Enable");
        CakeEnable = BindToggle("Cake", "Birthday Cake Enable");
        HaggisEnable = BindToggle("Haggis", "Haggis Enable");
        CandiedTurnipEnable = BindToggle("Candied Turnip", "Candied Turnip Enable");
        MoochiEnable = BindToggle("Moochi", "Moochi Enable");
        Nut_EllaEnable = BindToggle("Nut_Ella", "Nut_Ella Enable");
        BurgerEnable = BindToggle("Burger", "Burger Enable");
        OmletteEnable = BindToggle("Omlette", "Omlette Enable");
        BrothEnable = BindToggle("Broth", "Broth Enable");
        FishStewEnable = BindToggle("Fish Stew", "Fish Stew Enable");
        ButterEnable = BindToggle("Butter", "Butter Enable");
        BloodSausageEnable = BindToggle("Blood Sausage", "Blood Sausage Enable");
        BoiledEggEnable = BindToggle("Boiled Egg", "Boiled Egg Enable");
        CarrotSticksEnable = BindToggle("Carrot Sticks", "Carrot Sticks Enable");
        CheffHatEnable = BindToggle("Chef Hat", "Chef Hat Enable");
        MeadEnable = BindToggle("Mead", "Mead Enable");
        GrillOriginal = BindToggle("Original", "Use original grill");
        SmokelessEnable = BindToggle("Smokeless", "Enable to allow building of smokeless fires");
        CookingSkillEnable = Config.Bind("Cooking Skill", "Enable Cooking Skill", true, AdminDescription("Enable Cooking Skill", 1));
        BonusWhenCookingEnabled = Config.Bind("Cooking Skill", "Enable Cooking Bonus", true, AdminDescription("Enable Cooking Bonus", 2));
        HatXpGain = Config.Bind("Cooking Skill", "Chef Hat XP Gain", 5f, AdminDescription("XP Gain multiplier when cooking while wearing the Chef Hat", 3));
        HatSEMessage = Config.Bind("Cooking Skill", "Enable Chef Hat Message", true,
            new ConfigDescription("Enable Message when equipping the Chef Hat", null,
                new ConfigurationManagerAttributes { IsAdminOnly = false, Order = 4 }));
    }

    private ConfigEntry<bool> BindToggle(string section, string description) =>
        Config.Bind(section, "Enable", true, new ConfigDescription(description, null,
            new ConfigurationManagerAttributes { IsAdminOnly = true }));

    private static ConfigDescription AdminDescription(string text, int order) =>
        new ConfigDescription(text, null, new ConfigurationManagerAttributes { IsAdminOnly = true, Order = order });

    private void AddCookingSkill()
    {
        if (!CookingSkillEnable.Value) return;
        rkCookingSkill = SkillManager.Instance.AddSkill(new SkillConfig
        {
            Identifier = PluginGUID,
            Name = "Gore-mand",
            Description = "Learn to cook and eat like a Viking!",
            Icon = CookingSprite,
            IncreaseStep = 1f
        });
    }

    private void RegisterRuntimeContent()
    {
        try
        {
            RegisterPieces();
            RegisterItems();
            ApplyConfiguration();
            Logger.LogInfo($"BoneAppetit registered {_items.Count} items and {_pieces.Count} pieces against current Valheim prefabs.");
        }
        catch (Exception ex)
        {
            Logger.LogError($"BoneAppetit runtime registration failed: {ex}");
        }
        finally
        {
            PrefabManager.OnVanillaPrefabsAvailable -= RegisterRuntimeContent;
        }
    }

    private void RegisterItems()
    {
        foreach (ItemDefinition definition in BoneAppetitData.Items)
        {
            ItemConfig itemConfig = null;
            if (definition.Recipe != null)
            {
                itemConfig = new ItemConfig
                {
                    Name = Token(definition.Prefab, "name"),
                    Description = Token(definition.Prefab, "description"),
                    Enabled = definition.Enabled(this),
                    Amount = definition.Recipe.Amount,
                    CraftingStation = definition.Recipe.Station,
                    MinStationLevel = definition.Recipe.StationLevel,
                    Icon = LoadSprite("icon_" + definition.Prefab),
                    Requirements = definition.Recipe.Requirements.Select(ToRequirementConfig).ToArray()
                };
            }
            CustomItem item = itemConfig == null
                ? new CustomItem(definition.Prefab, definition.BasePrefab)
                : new CustomItem(definition.Prefab, definition.BasePrefab, itemConfig);
            ApplyItemData(item, definition);
            ReplaceItemVisual(item.ItemPrefab, definition.Prefab);
            if (!ItemManager.Instance.AddItem(item)) throw new InvalidOperationException("Failed to register item " + definition.Prefab);
            _items.Add(definition.Prefab, item);
        }
    }

    private void ApplyItemData(CustomItem item, ItemDefinition definition)
    {
        ItemDrop.ItemData.SharedData shared = item.ItemDrop.m_itemData.m_shared;
        Sprite icon = LoadSprite("icon_" + definition.Prefab);
        shared.m_name = Token(definition.Prefab, "name");
        shared.m_description = Token(definition.Prefab, "description");
        shared.m_itemType = definition.ItemType;
        shared.m_icons = new[] { icon };
        shared.m_maxStackSize = definition.StackSize;
        shared.m_maxQuality = 1;
        shared.m_weight = definition.Weight;
        shared.m_value = 0;
        shared.m_teleportable = true;
        shared.m_equipDuration = 1f;
        shared.m_movementModifier = 0f;
        shared.m_eitrRegenModifier = 0f;
        shared.m_food = definition.Food;
        shared.m_foodStamina = definition.Stamina;
        shared.m_foodEitr = definition.Eitr;
        shared.m_foodBurnTime = definition.BurnTime;
        shared.m_foodRegen = definition.Regen;
        shared.m_armor = definition.Armor;
        shared.m_armorPerLevel = definition.ArmorPerLevel;
        shared.m_consumeStatusEffect = null;
        shared.m_equipStatusEffect = null;
        shared.m_setName = string.Empty;
        shared.m_setSize = 0;
        shared.m_setStatusEffect = null;
        if (definition.ConeEffect != ConeEffect.None) shared.m_consumeStatusEffect = GetConeEffect(definition.ConeEffect, icon);
        if (definition.Prefab == "rk_chef")
        {
            shared.m_helmetHideHair = ItemDrop.ItemData.HelmetHairType.Default;
            shared.m_helmetHideBeard = ItemDrop.ItemData.HelmetHairType.Default;
            shared.m_helmetHairSettings?.Clear();
            shared.m_helmetBeardSettings?.Clear();
            shared.m_equipStatusEffect = ScriptableObject.CreateInstance<SE_CheffHat>();
        }
    }

    private SE_Stats GetConeEffect(ConeEffect effect, Sprite icon)
    {
        if (_coneEffects.TryGetValue(effect, out SE_Stats existing)) return existing;
        var status = ScriptableObject.CreateInstance<SE_Stats>();
        status.m_icon = icon;
        status.m_ttl = 300f;
        status.m_activationAnimation = "gpower";
        status.m_startMessageType = (MessageHud.MessageType)1;
        status.m_stopMessageType = (MessageHud.MessageType)1;
        status.m_repeatMessageType = (MessageHud.MessageType)1;
        status.m_healthRegenMultiplier = 1f;
        status.m_staminaRegenMultiplier = 1f;
        status.m_eitrRegenMultiplier = 1f;
        status.m_damageModifier = 1f;
        status.m_mods = new List<HitData.DamageModPair>();
        switch (effect)
        {
            case ConeEffect.Frost:
                status.name = "frost resist";
                status.m_name = "Fire Cone";
                status.m_tooltip = "What's that smell? Are you on fire!?!?";
                status.m_startMessage = "It burns!";
                status.m_mods.Add(new HitData.DamageModPair { m_type = HitData.DamageType.Frost, m_modifier = HitData.DamageModifier.VeryResistant });
                break;
            case ConeEffect.Fire:
                status.name = "fire resist";
                status.m_name = "Ice Cone";
                status.m_tooltip = "You feel you are cold, walking through fire acutaly sound good.";
                status.m_startMessage = "Your lips have turned blue.";
                status.m_mods.Add(new HitData.DamageModPair { m_type = HitData.DamageType.Fire, m_modifier = HitData.DamageModifier.VeryResistant });
                break;
            case ConeEffect.Lightning:
                status.name = "electric resist";
                status.m_name = "Shock Cone";
                status.m_tooltip = "You just ate Guck, you don't think much could Shock you now.";
                status.m_startMessage = "You feel green";
                status.m_mods.Add(new HitData.DamageModPair { m_type = HitData.DamageType.Lightning, m_modifier = HitData.DamageModifier.VeryResistant });
                break;
            default: throw new ArgumentOutOfRangeException(nameof(effect), effect, null);
        }
        _coneEffects.Add(effect, status);
        return status;
    }

    private void RegisterPieces()
    {
        RegisterCraftingStation("rk_grill", "Grill", "forge", true, GrillOriginal.Value ? "rk_grill_original" : "rk_grill_custom", Req("Stone", 10), Req("Iron", 2));
        RegisterGriddle();
        RegisterCraftingStation("rk_prep", "Prep Table", "forge", false, "rk_prep", Req("Wood", 4), Req("Tin", 5), Req("Stone", 3));
        RegisterOven();
        RegisterSmokelessFire("rk_campfire", "Smokeless Firepit", "fire_pit", string.Empty, "rk_campfire", Req("Stone", 5), Req("Wood", 2));
        RegisterSmokelessFire("rk_hearth", "Smokeless Hearth", "hearth", "piece_stonecutter", "rk_hearth", Req("Stone", 15));
        RegisterSmokelessFire("rk_brazier", "Smokeless Brazier", "piece_brazierceiling01", "forge", "rk_brazier", Req("Bronze", 5), Req("Coal", 2), Req("Chain", 1));
    }

    private void RegisterGriddle()
    {
        const string prefabName = "rk_griddle";
        string localizedName = Token(prefabName, "name");
        string localizedDescription = Token(prefabName, "description");
        var config = new PieceConfig
        {
            Name = localizedName,
            Description = localizedDescription,
            CraftingStation = string.Empty,
            AllowedInDungeons = false,
            Enabled = true,
            PieceTable = HammerPieceTable,
            Icon = LoadSprite("piece_icon_rk_griddle"),
            Requirements = new[] { Req("Stone", 10) }
        };

        GameObject prefab = _legacyGrillAssets.LoadAsset<GameObject>(prefabName);
        if (prefab == null) throw new InvalidOperationException("Missing original BoneAppetit prefab " + prefabName);

        var customPiece = new CustomPiece(prefab, true, config);
        CraftingStation station = prefab.GetComponent<CraftingStation>();
        ZNetView zNetView = prefab.GetComponent<ZNetView>();
        WearNTear wear = prefab.GetComponent<WearNTear>();
        if (station == null || zNetView == null || wear == null)
        {
            throw new InvalidOperationException(prefabName + " is missing required build or network components.");
        }

        station.m_name = localizedName;
        station.m_icon = config.Icon;
        station.m_craftRequireRoof = false;
        station.m_craftRequireFire = true;
        customPiece.Piece.m_name = localizedName;
        customPiece.Piece.m_description = localizedDescription;
        customPiece.Piece.m_icon = config.Icon;
        customPiece.Piece.m_craftingStation = null;

        RestorePieceShaders(prefab);
        ConfigureStationInteraction(prefab);
        AddPiece(customPiece);
    }
    private void EnsureCookingStationsRegistered()
    {
        EnsurePieceRegistered("rk_grill", "Stone Grill");
        EnsurePieceRegistered("rk_griddle", "Stone Griddle");
        EnsurePieceRegistered("rk_prep", "Prep Table");
        EnsurePieceRegistered("rk_oven", "Oven");
        EnsurePieceRegistered("rk_campfire", "Smokeless Firepit");
        EnsurePieceRegistered("rk_hearth", "Smokeless Hearth");
        EnsurePieceRegistered("rk_brazier", "Smokeless Brazier");
        EnsureBuildStationResolved("rk_grill", "forge", "Stone Grill");
        EnsureBuildStationResolved("rk_prep", "forge", "Prep Table");
        EnsureBuildStationResolved("rk_hearth", "piece_stonecutter", "Smokeless Hearth");
        EnsureBuildStationResolved("rk_brazier", "forge", "Smokeless Brazier");
        RepairGrillStationReferences();
    }

    private void RepairGrillStationReferences()
    {
        if (!_pieces.TryGetValue("rk_grill", out CustomPiece grill) || grill?.PiecePrefab == null) return;
        CraftingStation registeredGrill = grill.PiecePrefab.GetComponent<CraftingStation>();
        if (registeredGrill == null) return;

        int repairedRecipes = 0;
        foreach (ItemDefinition definition in BoneAppetitData.Items)
        {
            if (definition.Recipe == null || definition.Recipe.Station != "rk_grill") continue;
            if (!_items.TryGetValue(definition.Prefab, out CustomItem item) || item?.Recipe?.Recipe == null) continue;
            Recipe recipe = item.Recipe.Recipe;
            if (recipe.m_craftingStation == registeredGrill) continue;
            recipe.m_craftingStation = registeredGrill;
            repairedRecipes++;
        }

        bool repairedOven = false;
        if (_pieces.TryGetValue("rk_oven", out CustomPiece oven) && oven?.PiecePrefab != null)
        {
            StationExtension extension = oven.PiecePrefab.GetComponent<StationExtension>();
            if (extension != null && extension.m_craftingStation != registeredGrill)
            {
                extension.m_craftingStation = registeredGrill;
                repairedOven = true;
            }
        }

        if (repairedRecipes > 0 || repairedOven)
        {
            Logger.LogInfo("Repaired Grill station references: recipes=" + repairedRecipes + ", oven=" + repairedOven + ".");
        }
    }

    private void EnsureBuildStationResolved(string prefabName, string stationPrefabName, string displayName)
    {
        if (!_pieces.TryGetValue(prefabName, out CustomPiece customPiece) || customPiece?.PiecePrefab == null) return;

        Piece piece = customPiece.Piece;
        GameObject stationPrefab = ZNetScene.instance != null ? ZNetScene.instance.GetPrefab(stationPrefabName) : null;
        if (stationPrefab == null) stationPrefab = PrefabManager.Instance.GetPrefab(stationPrefabName);
        CraftingStation buildStation = stationPrefab != null ? stationPrefab.GetComponent<CraftingStation>() : null;
        if (piece == null || buildStation == null)
        {
            Logger.LogWarning(displayName + " could not resolve build station " + stationPrefabName + " during late registration.");
            return;
        }

        bool repaired = piece.m_craftingStation != buildStation;
        piece.m_craftingStation = buildStation;
        if (repaired) Logger.LogInfo(displayName + " repaired build station reference to " + stationPrefabName + ".");
    }

    private void EnsurePieceRegistered(string prefabName, string displayName)
    {
        if (!_pieces.TryGetValue(prefabName, out CustomPiece customPiece) || customPiece?.PiecePrefab == null)
        {
            return;
        }

        GameObject prefab = customPiece.PiecePrefab;
        PieceTable hammer = PieceManager.Instance.GetPieceTable(HammerPieceTable);
        if (hammer == null)
        {
            Logger.LogWarning(displayName + " could not find the Hammer piece table during late registration.");
            return;
        }

        bool addedToHammer = false;
        bool addedToZNet = false;

        if (!hammer.m_pieces.Contains(prefab))
        {
            PieceManager.Instance.RegisterPieceInPieceTable(prefab, HammerPieceTable);
            addedToHammer = true;
        }

        if (ZNetScene.instance != null && ZNetScene.instance.GetPrefab(prefab.name) == null)
        {
            PrefabManager.Instance.RegisterToZNetScene(prefab);
            addedToZNet = true;
        }

        Logger.LogInfo(displayName + " late registration: hammer=" + hammer.m_pieces.Contains(prefab) +
            ", znet=" + (ZNetScene.instance != null && ZNetScene.instance.GetPrefab(prefab.name) != null) +
            ", repairedHammer=" + addedToHammer + ", repairedZNet=" + addedToZNet + ".");
    }
    private void RegisterCraftingStation(string prefabName, string displayName, string buildStation, bool requiresFire, string visualName, params RequirementConfig[] requirements)
    {
        string localizedName = Token(prefabName, "name");
        string localizedDescription = Token(prefabName, "description");
        var config = new PieceConfig { Name = localizedName, Description = localizedDescription, CraftingStation = buildStation, AllowedInDungeons = false, Enabled = true, PieceTable = HammerPieceTable, Icon = LoadSprite("piece_icon_" + prefabName), Requirements = requirements };
        var customPiece = new CustomPiece(prefabName, "piece_cauldron", config);
        GameObject prefab = customPiece.PiecePrefab;
        CraftingStation station = prefab.GetComponent<CraftingStation>();
        if (station == null) throw new InvalidOperationException(prefabName + " did not inherit a current CraftingStation.");
        station.m_name = localizedName;
        station.m_icon = config.Icon;
        station.m_craftRequireRoof = false;
        station.m_craftRequireFire = requiresFire;
        customPiece.Piece.m_name = localizedName;
        customPiece.Piece.m_description = localizedDescription;
        customPiece.Piece.m_icon = config.Icon;
        if (string.IsNullOrEmpty(buildStation)) customPiece.Piece.m_craftingStation = null;
        ReplacePieceVisual(prefab, visualName);
        if (prefabName == "rk_grill") ConfigureStationInteraction(prefab);
        AddPiece(customPiece);
    }

    private void RegisterOven()
    {
        const string prefabName = "rk_oven";
        string localizedName = Token(prefabName, "name");
        string localizedDescription = Token(prefabName, "description");
        var config = new PieceConfig
        {
            Name = localizedName,
            Description = localizedDescription,
            CraftingStation = string.Empty,
            AllowedInDungeons = false,
            Enabled = true,
            PieceTable = HammerPieceTable,
            ExtendStation = "rk_grill",
            Icon = LoadSprite("piece_icon_rk_oven"),
            Requirements = new[] { Req("SurtlingCore", 2), Req("TrophySurtling", 1), Req("Stone", 10) }
        };

        GameObject prefab = _legacyGrillAssets.LoadAsset<GameObject>(prefabName);
        if (prefab == null) throw new InvalidOperationException("Missing original BoneAppetit prefab " + prefabName);

        var customPiece = new CustomPiece(prefab, true, config);
        customPiece.Piece.m_name = localizedName;
        customPiece.Piece.m_description = localizedDescription;
        customPiece.Piece.m_icon = config.Icon;
        customPiece.Piece.m_craftingStation = null;

        StationExtension extension = prefab.GetComponent<StationExtension>();
        WearNTear wear = prefab.GetComponent<WearNTear>();
        if (extension == null) throw new InvalidOperationException(prefabName + " is missing its StationExtension.");
        if (wear == null) throw new InvalidOperationException(prefabName + " is missing its WearNTear.");
        extension.m_maxStationDistance = 8f;
        wear.m_wet = null;
        ConfigureOvenVisuals(prefab);

        AddPiece(customPiece);
    }

    private static void ConfigureOvenVisuals(GameObject prefab)
    {
        foreach (Transform transform in prefab.GetComponentsInChildren<Transform>(true))
        {
            if (transform.name == "flames" || transform.name == "FireWarmth" || transform.name == "Point light" || transform.name == "steam")
            {
                transform.gameObject.SetActive(true);
            }
        }

        foreach (ParticleSystem particle in prefab.GetComponentsInChildren<ParticleSystem>(true))
        {
            if (particle.gameObject.name != "flames" && particle.gameObject.name != "steam") continue;

            ParticleSystem.MainModule main = particle.main;
            main.loop = true;
            main.playOnAwake = true;

            ParticleSystem.EmissionModule emission = particle.emission;
            emission.enabled = true;

            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            if (renderer != null) renderer.enabled = true;
        }

        if (prefab.GetComponent<OvenVisualState>() == null) prefab.AddComponent<OvenVisualState>();
        RepairOvenParticleShaders(prefab);
    }
    internal static void RepairOvenParticleShaders(GameObject oven)
    {
        if (Application.isBatchMode || oven == null) return;

        foreach (ParticleSystemRenderer renderer in oven.GetComponentsInChildren<ParticleSystemRenderer>(true))
        {
            string effect = renderer.gameObject.name;
            if (effect != "flames" && effect != "steam") continue;

            Material original = renderer.sharedMaterial;
            if (original == null || (original.shader != null && original.shader.isSupported)) continue;

            Material vanilla = FindSupportedVanillaParticle(effect == "steam");
            if (vanilla == null)
            {
                renderer.enabled = false;
                Instance?.Logger.LogWarning("Oven " + effect + " effect hidden because no supported particle shader was available.");
                continue;
            }

            Material fixedMaterial = new Material(vanilla) { name = "BoneAppetit Oven " + effect };
            if (original.HasProperty("_MainTex") && fixedMaterial.HasProperty("_MainTex"))
            {
                fixedMaterial.SetTexture("_MainTex", original.GetTexture("_MainTex"));
            }

            if (original.HasProperty("_TintColor"))
            {
                Color tint = original.GetColor("_TintColor");
                if (fixedMaterial.HasProperty("_TintColor")) fixedMaterial.SetColor("_TintColor", tint);
                else if (fixedMaterial.HasProperty("_Color")) fixedMaterial.SetColor("_Color", tint);
            }

            renderer.sharedMaterial = fixedMaterial;
            renderer.enabled = true;
            Instance?.Logger.LogInfo("Oven " + effect + " now uses supported vanilla particle shader " + fixedMaterial.shader.name + ".");
        }
    }

    private static Material FindSupportedVanillaParticle(bool smoke)
    {
        Material fallback = null;
        foreach (string prefabName in new[] { "fire_pit", "hearth", "bonfire" })
        {
            GameObject prefab = PrefabManager.Instance.GetPrefab(prefabName);
            if (prefab == null) continue;

            foreach (ParticleSystemRenderer renderer in prefab.GetComponentsInChildren<ParticleSystemRenderer>(true))
            {
                Material material = renderer.sharedMaterial;
                if (material == null || material.shader == null || !material.shader.isSupported ||
                    !material.HasProperty("_MainTex")) continue;

                if (fallback == null) fallback = material;
                string name = renderer.gameObject.name;
                if (smoke && (name.IndexOf("smoke", StringComparison.OrdinalIgnoreCase) >= 0 ||
                              name.IndexOf("steam", StringComparison.OrdinalIgnoreCase) >= 0))
                    return material;
                if (!smoke && (name.IndexOf("fire", StringComparison.OrdinalIgnoreCase) >= 0 ||
                               name.IndexOf("flame", StringComparison.OrdinalIgnoreCase) >= 0))
                    return material;
            }
        }

        return fallback;
    }

    private void RegisterSmokelessFire(string prefabName, string displayName, string basePrefab, string buildStation, string visualName, params RequirementConfig[] requirements)
    {
        Piece basePiece = PrefabManager.Instance.GetPrefab(basePrefab)?.GetComponent<Piece>();
        string localizedName = Token(prefabName, "name");
        string localizedDescription = Token(prefabName, "description");
        var config = new PieceConfig { Name = localizedName, Description = localizedDescription, CraftingStation = buildStation, AllowedInDungeons = false, Enabled = SmokelessEnable.Value, PieceTable = HammerPieceTable, Icon = basePiece != null ? basePiece.m_icon : null, Requirements = requirements };
        var customPiece = new CustomPiece(prefabName, basePrefab, config);
        customPiece.Piece.m_name = localizedName;
        customPiece.Piece.m_description = localizedDescription;
        if (config.Icon != null) customPiece.Piece.m_icon = config.Icon;
        Fireplace fireplace = customPiece.PiecePrefab.GetComponent<Fireplace>();
        if (fireplace == null) throw new InvalidOperationException(prefabName + " did not inherit a current Fireplace.");
        // Keep BoneAppetit's original smokeless models while retaining the modern
        // vanilla Fireplace, fire effects, lighting, and placement components.
        ReplacePieceVisual(customPiece.PiecePrefab, visualName);
        RestoreSmokelessMaterials(customPiece.PiecePrefab, PrefabManager.Instance.GetPrefab(basePrefab));
        DisableSmokeEmission(customPiece.PiecePrefab, fireplace);
        TuneSmokelessFireEffects(customPiece.PiecePrefab, prefabName);
        RemoveSmokelessFuelPuffs(fireplace);
        AddPiece(customPiece);
    }

    private void RestoreSmokelessMaterials(GameObject prefab, GameObject vanilla)
    {
        Transform visual = prefab.transform.Find("BoneAppetitVisual");
        if (visual == null || vanilla == null) return;

        // The converted bundle does not retain every shader property and keyword.
        // Use complete current-game materials on the original fireplace meshes.
        foreach (ShaderReplacerNew replacer in visual.GetComponentsInChildren<ShaderReplacerNew>(true))
            DestroyImmediate(replacer);
        foreach (PieceVisualShaderState refresher in visual.GetComponentsInChildren<PieceVisualShaderState>(true))
            DestroyImmediate(refresher);

        var nativeMaterials = new Dictionary<string, Material>(StringComparer.Ordinal);
        foreach (GameObject source in new[] { vanilla, PrefabManager.Instance.GetPrefab("fire_pit") })
        {
            if (source == null) continue;
            foreach (MeshRenderer renderer in source.GetComponentsInChildren<MeshRenderer>(true))
            foreach (Material material in renderer.sharedMaterials)
            {
                if (material == null) continue;
                string key = LegacyTextureKey(material.name);
                if (!nativeMaterials.ContainsKey(key)) nativeMaterials.Add(key, material);
            }
        }

        var restoredMaterials = new Dictionary<string, Material>(StringComparer.Ordinal);
        int restored = 0;
        int missing = 0;
        foreach (MeshRenderer renderer in visual.GetComponentsInChildren<MeshRenderer>(true))
        {
            Material[] materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; ++i)
            {
                Material original = materials[i];
                if (original == null) continue;
                string key = LegacyTextureKey(original.name);
                if (key == "fireplaceashquadglow") key = "fireplaceashglowing";
                if (!nativeMaterials.TryGetValue(key, out Material native))
                {
                    ++missing;
                    Logger.LogWarning("No current fireplace material found for " + original.name + ".");
                    continue;
                }
                if (!restoredMaterials.TryGetValue(key, out Material replacement))
                {
                    replacement = new Material(native) { name = native.name };
                    restoredMaterials.Add(key, replacement);
                }
                materials[i] = replacement;
                ++restored;
            }
            renderer.sharedMaterials = materials;
        }
        Logger.LogInfo("Restored current fireplace materials for " + prefab.name +
            ": slots=" + restored + ", unmatched=" + missing + ".");
    }

    private static void DisableSmokeEmission(GameObject prefab, Fireplace fireplace)
    {
        fireplace.m_smokeSpawner = null;

        foreach (SmokeSpawner spawner in prefab.GetComponentsInChildren<SmokeSpawner>(true))
        {
            spawner.enabled = false;
        }

        foreach (ParticleSystem particle in prefab.GetComponentsInChildren<ParticleSystem>(true))
        {
            // The inherited low_flames effect renders like opaque white smoke on these pieces.
            // Keep the other native flames, sparks, warmth, and fire light.
            string effectName = particle.gameObject.name;
            if (effectName.IndexOf("smoke", StringComparison.OrdinalIgnoreCase) < 0 &&
                !string.Equals(effectName, "low_flames", StringComparison.OrdinalIgnoreCase)) continue;

            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ParticleSystem.EmissionModule emission = particle.emission;
            emission.enabled = false;

            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            if (renderer != null) renderer.enabled = false;
        }
    }

    private static void TuneSmokelessFireEffects(GameObject prefab, string prefabName)
    {
        // Retain the existing native fire effects. On the smokeless Hearth and
        // Brazier, the high-quality flame sprites are oversized for the custom meshes.
        // Limit their size without suppressing flames, sparks, or lighting.
        if (prefabName != "rk_hearth" && prefabName != "rk_brazier") return;

        foreach (ParticleSystem particle in prefab.GetComponentsInChildren<ParticleSystem>(true))
        {
            if (!particle.gameObject.name.StartsWith("flames", StringComparison.OrdinalIgnoreCase)) continue;
            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            if (renderer == null || renderer.sharedMaterial == null ||
                !renderer.sharedMaterial.name.StartsWith("flameball_flipbook_gradient", StringComparison.OrdinalIgnoreCase))
                continue;

            ParticleSystem.MainModule main = particle.main;
            main.startSize = main.startSize.constant * 0.55f;
            if (prefabName == "rk_hearth")
            {
                // The inherited Hearth effect emits 200 large sprites a second.
                ParticleSystem.EmissionModule emission = particle.emission;
                emission.rateOverTime = Mathf.Min(35f, emission.rateOverTime.constant);
            }
        }
    }

    private static void RemoveSmokelessFuelPuffs(Fireplace fireplace)
    {
        // Fueling keeps its native sound, but the vanilla fuel-added smoke
        // burst must not appear on the smokeless fire pieces.
        EffectList.EffectData[] effects = fireplace.m_fuelAddedEffects.m_effectPrefabs;
        if (effects == null || effects.Length == 0) return;

        fireplace.m_fuelAddedEffects.m_effectPrefabs = effects.Where(effect =>
        {
            string name = effect?.m_prefab != null ? effect.m_prefab.name : string.Empty;
            return !string.Equals(name, "vfx_FireAddFuel", StringComparison.Ordinal) &&
                   !string.Equals(name, "vfx_HearthAddFuel", StringComparison.Ordinal);
        }).ToArray();
    }

    private void AddPiece(CustomPiece piece)
    {
        if (!PieceManager.Instance.AddPiece(piece)) throw new InvalidOperationException("Failed to register piece " + piece.PiecePrefab.name);
        _pieces.Add(piece.PiecePrefab.name, piece);
    }

    private static void ConfigureStationInteraction(GameObject prefab)
    {
        CraftingStation station = prefab.GetComponent<CraftingStation>();
        if (station == null) return;

        station.m_useDistance = Mathf.Max(station.m_useDistance, 3f);

        Transform existing = prefab.transform.Find("BoneAppetitInteraction");
        if (existing != null) UnityEngine.Object.DestroyImmediate(existing.gameObject);

        if (!TryGetLocalVisualBounds(prefab, out Bounds visualBounds)) return;

        var interaction = new GameObject("BoneAppetitInteraction");
        interaction.transform.SetParent(prefab.transform, false);
        int interactionLayer = LayerMask.NameToLayer("piece_nonsolid");
        interaction.layer = interactionLayer >= 0 ? interactionLayer : prefab.layer;

        BoxCollider collider = interaction.AddComponent<BoxCollider>();
        collider.isTrigger = false;
        collider.center = visualBounds.center;
        collider.size = new Vector3(
            Mathf.Max(visualBounds.size.x + 0.3f, 1.2f),
            Mathf.Max(visualBounds.size.y + 0.4f, 1.4f),
            Mathf.Max(visualBounds.size.z + 0.3f, 1.2f));
    }

    private static bool TryGetLocalVisualBounds(GameObject root, out Bounds bounds)
    {
        bounds = default;
        bool found = false;

        foreach (MeshFilter filter in root.GetComponentsInChildren<MeshFilter>(true))
        {
            MeshRenderer renderer = filter.GetComponent<MeshRenderer>();
            Mesh mesh = filter.sharedMesh;
            if (renderer == null || !renderer.enabled || mesh == null) continue;
            EncapsulateLocalBounds(root.transform, filter.transform, mesh.bounds, ref bounds, ref found);
        }

        foreach (SkinnedMeshRenderer renderer in root.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (!renderer.enabled) continue;
            EncapsulateLocalBounds(root.transform, renderer.transform, renderer.localBounds, ref bounds, ref found);
        }

        return found;
    }

    private static void EncapsulateLocalBounds(Transform root, Transform source, Bounds sourceBounds, ref Bounds result, ref bool found)
    {
        Vector3 min = sourceBounds.min;
        Vector3 max = sourceBounds.max;
        for (int x = 0; x < 2; ++x)
        {
            for (int y = 0; y < 2; ++y)
            {
                for (int z = 0; z < 2; ++z)
                {
                    Vector3 corner = new Vector3(
                        x == 0 ? min.x : max.x,
                        y == 0 ? min.y : max.y,
                        z == 0 ? min.z : max.z);
                    Vector3 local = root.InverseTransformPoint(source.TransformPoint(corner));
                    if (!found)
                    {
                        result = new Bounds(local, Vector3.zero);
                        found = true;
                    }
                    else
                    {
                        result.Encapsulate(local);
                    }
                }
            }
        }
    }
    private void BuildLegacyTextureMap()
    {
        _legacyTextures.Clear();
        foreach (UnityEngine.Object asset in _legacyFoodAssets.LoadAllAssets())
        {
            if (asset is Texture2D texture)
            {
                _legacyTextures[LegacyTextureKey(texture.name)] = texture;
                continue;
            }

            if (asset is not GameObject gameObject) continue;
            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>(true))
            {
                foreach (Material material in renderer.sharedMaterials)
                {
                    if (material == null) continue;
                    foreach (string propertyName in material.GetTexturePropertyNames())
                    {
                        if (material.GetTexture(propertyName) is Texture2D materialTexture)
                        {
                            _legacyTextures[LegacyTextureKey(materialTexture.name)] = materialTexture;
                        }
                    }
                }
            }
        }
    }

    private void RestoreLegacyTextures(GameObject root)
    {
        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                if (material == null) continue;
                foreach (string propertyName in material.GetTexturePropertyNames())
                {
                    Texture current = material.GetTexture(propertyName);
                    if (current == null) continue;
                    if (_legacyTextures.TryGetValue(LegacyTextureKey(current.name), out Texture2D legacyTexture))
                    {
                        material.SetTexture(propertyName, legacyTexture);
                    }
                }
            }
        }
    }

    private static string LegacyTextureKey(string textureName)
    {
        if (string.IsNullOrEmpty(textureName)) return string.Empty;

        int suffix = textureName.LastIndexOf("_0000", StringComparison.OrdinalIgnoreCase);
        if (suffix >= 0 && suffix + 5 < textureName.Length)
        {
            bool isExportSuffix = true;
            for (int i = suffix + 5; i < textureName.Length; ++i)
            {
                if (!Uri.IsHexDigit(textureName[i]))
                {
                    isExportSuffix = false;
                    break;
                }
            }
            if (isExportSuffix) textureName = textureName.Substring(0, suffix);
        }

        char[] normalized = new char[textureName.Length];
        int count = 0;
        foreach (char c in textureName)
        {
            if (!char.IsLetterOrDigit(c)) continue;
            normalized[count++] = char.ToLowerInvariant(c);
        }
        return new string(normalized, 0, count);
    }

    private void ReplaceItemVisual(GameObject target, string visualName)
    {
        DisableRenderersAndEffects(target);
        GameObject source = LoadVisual(visualName);
        if (visualName == "rk_chef")
        {
            Transform sourceAttach = source.transform.Find("attach");
            Transform targetAttach = target.transform.Find("attach");
            if (sourceAttach == null || targetAttach == null) throw new InvalidOperationException("Chef Hat attach hierarchy is missing.");
            targetAttach.localPosition = sourceAttach.localPosition;
            targetAttach.localRotation = sourceAttach.localRotation;
            targetAttach.localScale = sourceAttach.localScale;
            for (int i = 0; i < sourceAttach.childCount; ++i) Instantiate(sourceAttach.GetChild(i).gameObject, targetAttach, false);
            for (int i = 0; i < source.transform.childCount; ++i)
            {
                Transform child = source.transform.GetChild(i);
                if (child != sourceAttach) Instantiate(child.gameObject, target.transform, false);
            }
            ApplyVisualShader(targetAttach.gameObject, "Custom/Creature");
            RestoreLegacyTextures(targetAttach.gameObject);
            return;
        }
        GameObject visual = Instantiate(source, target.transform, false);
        visual.name = "BoneAppetitVisual";
        visual.transform.localPosition = source.transform.localPosition;
        visual.transform.localRotation = source.transform.localRotation;
        visual.transform.localScale = source.transform.localScale;
        ApplyVisualShader(visual, "Custom/Creature");
        RestoreLegacyTextures(visual);
    }

    private void ReplacePieceVisual(GameObject target, string visualName)
    {
        Transform oldVisual = target.transform.Find("BoneAppetitVisual");
        if (oldVisual != null) DestroyImmediate(oldVisual.gameObject);
        foreach (MeshRenderer renderer in target.GetComponentsInChildren<MeshRenderer>(true)) renderer.enabled = false;
        foreach (SkinnedMeshRenderer renderer in target.GetComponentsInChildren<SkinnedMeshRenderer>(true)) renderer.enabled = false;
        GameObject source = LoadVisual(visualName);
        GameObject visual = Instantiate(source, target.transform, false);
        visual.name = "BoneAppetitVisual";
        visual.transform.localPosition = source.transform.localPosition;
        visual.transform.localRotation = source.transform.localRotation;
        visual.transform.localScale = source.transform.localScale * ((visualName == "rk_oven" || visualName == "rk_griddle") ? 0.6f : 1f);
        RestorePieceShaders(visual);
        visual.AddComponent<PieceVisualShaderState>();
        foreach (ParticleSystem particle in visual.GetComponentsInChildren<ParticleSystem>(true))
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            if (renderer != null) renderer.enabled = false;
        }
        foreach (Light light in visual.GetComponentsInChildren<Light>(true)) light.enabled = false;
    }

    private static readonly Dictionary<string, Shader> ResolvedShaders = new Dictionary<string, Shader>(StringComparer.Ordinal);

    private static Shader ResolveLoadedShader(string shaderName)
    {
        if (ResolvedShaders.TryGetValue(shaderName, out Shader cached) && cached != null) return cached;

        Shader shader = Shader.Find(shaderName);
        if (shader == null)
        {
            foreach (Shader loaded in Resources.FindObjectsOfTypeAll<Shader>())
            {
                if (loaded != null && string.Equals(loaded.name, shaderName, StringComparison.Ordinal))
                {
                    shader = loaded;
                    break;
                }
            }
        }

        if (shader == null)
        {
            foreach (Material material in Resources.FindObjectsOfTypeAll<Material>())
            {
                if (material != null && material.shader != null &&
                    string.Equals(material.shader.name, shaderName, StringComparison.Ordinal))
                {
                    shader = material.shader;
                    break;
                }
            }
        }

        if (shader != null) ResolvedShaders[shaderName] = shader;
        return shader;
    }
    internal static void RestorePieceShaders(GameObject root)
    {
        Shader piece = ResolveLoadedShader("Custom/Piece");
        Shader staticRock = ResolveLoadedShader("Custom/StaticRock");
        Shader standardTwoSided = ResolveLoadedShader("Standard TwoSided");
        Shader standardSpecular = ResolveLoadedShader("Standard (Specular setup)");

        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                if (material == null) continue;
                string name = material.name;

                if ((name.StartsWith("HearthNew_mat", StringComparison.Ordinal) ||
                     name.StartsWith("HearthBroken_mat", StringComparison.Ordinal) ||
                     name.StartsWith("HearthAshes_mat", StringComparison.Ordinal) ||
                     name.StartsWith("HearthAshesBurn_mat", StringComparison.Ordinal) ||
                     name.StartsWith("Brazier_Mat", StringComparison.Ordinal)) &&
                    piece != null && piece.isSupported)
                {
                    material.shader = piece;
                }
                if (name.StartsWith("Brazier_Mat", StringComparison.Ordinal))
                {
                    // Recover the original metallic response after the Unity shader replacement.
                    if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", 1f);
                    if (material.HasProperty("_MetallicAlphaGloss")) material.SetFloat("_MetallicAlphaGloss", 0.711f);
                    if (material.HasProperty("_MetalColor")) material.SetColor("_MetalColor", Color.white);
                }
                else if ((name.StartsWith("stone_", StringComparison.Ordinal) ||
                     name.StartsWith("stones_", StringComparison.Ordinal) ||
                     name.StartsWith("GriddleRock", StringComparison.Ordinal)) &&
                    staticRock != null)
                {
                    material.shader = staticRock;
                }
                else if (name.StartsWith("fireplace_ash_glowing_", StringComparison.Ordinal) && standardSpecular != null)
                {
                    material.shader = standardSpecular;
                }
                else if (name.StartsWith("fireplace_ash_", StringComparison.Ordinal) && standardTwoSided != null)
                {
                    material.shader = standardTwoSided;
                }
            }
        }
    }
    private static void ApplyVisualShader(GameObject root, string shaderName)
    {
        Shader shader = ResolveLoadedShader(shaderName);
        if (shader == null) return;
        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            if (renderer is ParticleSystemRenderer) continue;
            foreach (Material material in renderer.sharedMaterials)
            {
                if (material != null) material.shader = shader;
            }
        }
    }
    private static void DisableRenderersAndEffects(GameObject target)
    {
        foreach (Renderer renderer in target.GetComponentsInChildren<Renderer>(true)) renderer.enabled = false;
        foreach (ParticleSystem particle in target.GetComponentsInChildren<ParticleSystem>(true)) particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        foreach (Light light in target.GetComponentsInChildren<Light>(true)) light.enabled = false;
    }

    private static AssetBundle LoadEmbeddedAssetBundle(string resourceName)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                throw new InvalidOperationException("Embedded BoneAppetit asset bundle was not found: " + resourceName);
            }

            byte[] data = new byte[stream.Length];
            int offset = 0;
            while (offset < data.Length)
            {
                int read = stream.Read(data, offset, data.Length - offset);
                if (read <= 0)
                {
                    throw new EndOfStreamException("Embedded BoneAppetit asset bundle ended unexpectedly.");
                }
                offset += read;
            }

            return AssetBundle.LoadFromMemory(data);
        }
    }
    private GameObject LoadVisual(string name)
    {
        string path = $"{AssetRoot}/prefabs/{name}.prefab";
        GameObject prefab = _assets.LoadAsset<GameObject>(path);
        if (prefab == null) throw new InvalidOperationException("Missing BoneAppetit visual asset " + path);
        return prefab;
    }

    private Sprite LoadSprite(string name)
    {
        string path = $"{AssetRoot}/icons/{name}.asset";
        Sprite sprite = _assets.LoadAsset<Sprite>(path);
        if (sprite == null) throw new InvalidOperationException("Missing BoneAppetit icon asset " + path);
        return sprite;
    }

    private void AddDrops()
    {
        if (_dropsApplied) return;
        try
        {
            AddCharacterDrop("Boar", "rk_pork");
            AddCharacterDrop("Hatchling", "rk_dragonegg");
            AddBirdDrop("Seagal", "rk_egg");
            AddBirdDrop("Crow", "rk_egg");
            _dropsApplied = true;
        }
        catch (Exception ex) { Logger.LogError("BoneAppetit drop registration failed: " + ex); }
        finally { ItemManager.OnItemsRegistered -= AddDrops; }
    }

    private static void AddCharacterDrop(string creatureName, string itemName)
    {
        GameObject creature = PrefabManager.Instance.GetPrefab(creatureName);
        GameObject item = PrefabManager.Instance.GetPrefab(itemName);
        CharacterDrop characterDrop = creature?.GetComponent<CharacterDrop>();
        if (characterDrop == null || item == null) throw new InvalidOperationException($"Could not add {itemName} to {creatureName} drops.");
        if (characterDrop.m_drops.Any(drop => drop.m_prefab != null && drop.m_prefab.name == itemName)) return;
        characterDrop.m_drops.Add(new CharacterDrop.Drop { m_prefab = item, m_amountMin = 1, m_amountMax = 1, m_chance = 1f, m_levelMultiplier = true, m_onePerPlayer = false });
    }

    private static void AddBirdDrop(string creatureName, string itemName)
    {
        GameObject creature = PrefabManager.Instance.GetPrefab(creatureName);
        GameObject item = PrefabManager.Instance.GetPrefab(itemName);
        DropOnDestroyed dropOnDestroyed = creature?.GetComponent<DropOnDestroyed>();
        if (dropOnDestroyed == null || item == null) throw new InvalidOperationException($"Could not add {itemName} to {creatureName} drops.");
        DropTable table = dropOnDestroyed.m_dropWhenDestroyed;
        if (!table.m_drops.Any(drop => drop.m_item != null && drop.m_item.name == itemName)) table.m_drops.Add(new DropTable.DropData { m_item = item, m_stackMin = 1, m_stackMax = 1, m_weight = 1f });
        table.m_oneOfEach = true;
        table.m_dropMin = 2;
        table.m_dropMax = 2;
        table.m_dropChance = 1f;
        dropOnDestroyed.m_spawnYStep = 0.3f;
        dropOnDestroyed.m_spawnYOffset = 0.5f;
    }

    private void OnConfigurationSynchronized(object sender, ConfigurationSynchronizationEventArgs args) => ApplyConfiguration();

    private void ApplyConfiguration()
    {
        foreach (ItemDefinition definition in BoneAppetitData.Items)
        {
            if (definition.Recipe != null && _items.TryGetValue(definition.Prefab, out CustomItem item) && item.Recipe != null) item.Recipe.Recipe.m_enabled = definition.Enabled(this);
        }
        SetPieceEnabled("rk_campfire", SmokelessEnable.Value);
        SetPieceEnabled("rk_hearth", SmokelessEnable.Value);
        SetPieceEnabled("rk_brazier", SmokelessEnable.Value);
        if (_pieces.TryGetValue("rk_grill", out CustomPiece grill) && _appliedOriginalGrill != GrillOriginal.Value)
        {
            ReplacePieceVisual(grill.PiecePrefab, GrillOriginal.Value ? "rk_grill_original" : "rk_grill_custom");
            ConfigureStationInteraction(grill.PiecePrefab);
            _appliedOriginalGrill = GrillOriginal.Value;
        }
    }

    private void SetPieceEnabled(string name, bool enabled)
    {
        if (_pieces.TryGetValue(name, out CustomPiece piece)) piece.Piece.m_enabled = enabled;
    }

    internal void OnCookingStationCookItem(bool result)
    {
        if (result && CookingSkillEnable.Value) RaiseCookingSkill();
    }

    internal void OnInventoryAddItem(string itemName, long crafterId, string crafterName)
    {
        if (_addingExtraItem || !CookingSkillEnable.Value || Player.m_localPlayer == null) return;
        if (string.IsNullOrEmpty(crafterName) || crafterId < 1 || !IsConsumable(itemName)) return;
        if (!IsCookingCraftingStation(Player.m_localPlayer.GetCurrentCraftingStation())) return;
        float skillLevel = GetCookingSkillLevel();
        if (BonusWhenCookingEnabled.Value)
        {
            if (IsCrafterLucky(skillLevel)) AddExtraItem(itemName);
            if (skillLevel > 25f && IsCrafterLucky(skillLevel / 4f)) AddExtraItem(itemName);
        }
        RaiseCookingSkill();
    }

    private void RaiseCookingSkill()
    {
        if (!CookingSkillEnable.Value || Player.m_localPlayer == null || (int)rkCookingSkill == 0) return;
        Player.m_localPlayer.RaiseSkill(rkCookingSkill, 1f);
    }

    private float GetCookingSkillLevel()
    {
        if (Player.m_localPlayer == null || (int)rkCookingSkill == 0) return 0f;
        Skills.Skill skill = Player.m_localPlayer.GetSkills().GetSkillList().FirstOrDefault(entry => entry.m_info.m_skill == rkCookingSkill);
        return skill?.m_level ?? 0f;
    }

    private static bool IsCookingCraftingStation(CraftingStation station)
    {
        if (station == null) return false;
        string stationName = station.gameObject.name.Replace("(Clone)", string.Empty);
        return stationName == "rk_griddle" || stationName == "rk_grill" || stationName == "rk_prep" || stationName == "piece_cauldron";
    }

    private static bool IsConsumable(string prefabName)
    {
        GameObject prefab = ObjectDB.instance?.GetItemPrefab(prefabName);
        ItemDrop itemDrop = prefab?.GetComponent<ItemDrop>();
        return itemDrop != null && itemDrop.m_itemData.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Consumable;
    }

    private void AddExtraItem(string itemName)
    {
        GameObject itemPrefab = ObjectDB.instance?.GetItemPrefab(itemName);
        if (itemPrefab == null || Player.m_localPlayer == null) return;
        Inventory inventory = Player.m_localPlayer.GetInventory();
        if (!inventory.CanAddItem(itemPrefab, 1)) return;
        _addingExtraItem = true;
        try { inventory.AddItem(itemName, 1, 1, 0, Player.m_localPlayer.GetPlayerID(), Player.m_localPlayer.GetPlayerName(), false, false); }
        finally { _addingExtraItem = false; }
    }

    private static bool IsCrafterLucky(float skillLevel) => skillLevel >= 1f && Random.Range(1, 100) < skillLevel;

    private static RequirementConfig ToRequirementConfig(RequirementDefinition definition) => new RequirementConfig { Item = definition.Item, Amount = definition.Amount, Recover = definition.Recover };
    private static RequirementConfig Req(string item, int amount) => new RequirementConfig { Item = item, Amount = amount, Recover = true };
}

internal sealed class PieceVisualShaderState : MonoBehaviour
{
    private void Start()
    {
        RefreshShaders();
        Invoke(nameof(RefreshShaders), 0.2f);
    }

    private void RefreshShaders()
    {
        BoneAppetit.RestorePieceShaders(gameObject);
    }
}


internal sealed class OvenVisualState : MonoBehaviour
{
    private void Start()
    {
        RefreshLights();
        Invoke(nameof(RefreshLights), 0.2f);
    }

    private void RefreshLights()
    {
        BoneAppetit.RepairOvenParticleShaders(gameObject);
        foreach (Light light in GetComponentsInChildren<Light>(true))
        {
            light.gameObject.SetActive(true);
            LightLod lightLod = light.GetComponent<LightLod>();
            if (lightLod != null)
            {
                lightLod.enabled = false;
                Destroy(lightLod);
            }
            light.enabled = true;
        }
    }
}

[HarmonyPatch(typeof(StationExtension), nameof(StationExtension.StartConnectionEffect), new Type[] { typeof(Vector3), typeof(float) })]
internal static class OvenExtensionConnectionPatch
{
    private static bool Prefix(StationExtension __instance)
    {
        if (__instance == null || __instance.gameObject == null) return true;
        return __instance.gameObject.name.Replace("(Clone)", string.Empty) != "rk_oven";
    }
}
