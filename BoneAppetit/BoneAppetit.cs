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
    public const string PluginVersion = "3.3.14";
    private const string BundleResourceName = "BoneAppetit.assets";
    private const string LegacyGrillResourceName = "BoneAppetit.grill";
    private const string AssetRoot = "assets/boneappetit6000";
    private const string HammerPieceTable = "_HammerPieceTable";

    private readonly Dictionary<string, CustomItem> _items = new Dictionary<string, CustomItem>(StringComparer.Ordinal);
    private readonly Dictionary<string, CustomPiece> _pieces = new Dictionary<string, CustomPiece>(StringComparer.Ordinal);
    private readonly Dictionary<ConeEffect, SE_Stats> _coneEffects = new Dictionary<ConeEffect, SE_Stats>();
    private Harmony _harmony;
    private AssetBundle _assets;
    private AssetBundle _legacyGrillAssets;
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
        if (_assets == null || _legacyGrillAssets == null)
        {
            Logger.LogError("BoneAppetit could not load its Unity 6000 asset bundle.");
            return;
        }
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
        string pluginDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        string translationsDirectory = Path.Combine(pluginDirectory, "Translations");
        var translationFiles = new List<string>();

        if (Directory.Exists(translationsDirectory))
        {
            translationFiles.AddRange(Directory.GetFiles(translationsDirectory, "*.json", SearchOption.AllDirectories));
        }

        translationFiles.AddRange(
            Directory.GetFiles(pluginDirectory, "*.json", SearchOption.TopDirectoryOnly)
                .Where(file => !string.Equals(Path.GetFileName(file), "manifest.json", StringComparison.OrdinalIgnoreCase)));

        foreach (string file in translationFiles.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            try
            {
                string language = ResolveTranslationLanguage(file, translationsDirectory);
                LocalizationManager.Instance.AddJson(language, File.ReadAllText(file));
                Logger.LogInfo("BoneAppetit loaded " + language + " translations from " + Path.GetFileName(file) + ".");
            }
            catch (Exception ex)
            {
                Logger.LogWarning("BoneAppetit could not load translation file " + Path.GetFileName(file) + ": " + ex.Message);
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
        _assets = null;
        _legacyGrillAssets = null;
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
        ReplacePieceVisual(customPiece.PiecePrefab, visualName);
        DisableSmokeEmission(customPiece.PiecePrefab, fireplace);
        AddPiece(customPiece);
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
            if (particle.gameObject.name.IndexOf("smoke", StringComparison.OrdinalIgnoreCase) < 0) continue;

            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ParticleSystem.EmissionModule emission = particle.emission;
            emission.enabled = false;

            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            if (renderer != null) renderer.enabled = false;
        }
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
            return;
        }
        GameObject visual = Instantiate(source, target.transform, false);
        visual.name = "BoneAppetitVisual";
        visual.transform.localPosition = source.transform.localPosition;
        visual.transform.localRotation = source.transform.localRotation;
        visual.transform.localScale = source.transform.localScale;
        ApplyVisualShader(visual, "Custom/Creature");
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
        foreach (ParticleSystem particle in visual.GetComponentsInChildren<ParticleSystem>(true))
        {
            particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            if (renderer != null) renderer.enabled = false;
        }
        foreach (Light light in visual.GetComponentsInChildren<Light>(true)) light.enabled = false;
    }

    private static void RestorePieceShaders(GameObject root)
    {
        Shader staticRock = Shader.Find("Custom/StaticRock");
        Shader standardTwoSided = Shader.Find("Standard TwoSided");
        Shader standardSpecular = Shader.Find("Standard (Specular setup)");

        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                if (material == null) continue;
                string name = material.name;

                if ((name.StartsWith("stone_", StringComparison.Ordinal) ||
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
        Shader shader = Shader.Find(shaderName);
        if (shader == null) return;
        foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
        {
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

internal sealed class OvenVisualState : MonoBehaviour
{
    private void Start()
    {
        RefreshLights();
        Invoke(nameof(RefreshLights), 0.2f);
    }

    private void RefreshLights()
    {
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
