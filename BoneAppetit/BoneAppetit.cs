using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Logger = Jotunn.Logger;

namespace Boneappetit;

[BepInPlugin("com.rockerkitten.boneappetit", "BoneAppetit", "3.3.2")]
[BepInDependency(Jotunn.Main.ModGuid)]
[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.Minor)]
public class BoneAppetit : BaseUnityPlugin
{
	public const string PluginGUID = "com.rockerkitten.boneappetit";

	public const string PluginName = "BoneAppetit";

	public const string PluginVersion = "3.3.2";

	public AssetBundle GrillAssetBundle;

	public AssetBundle FoodAssetBundle;

	public static BoneAppetit Instance;

	private Harmony _harmony;

	public Sprite CookingSprite;

	public Skills.SkillType rkCookingSkill;

	public static ConfigEntry<int> NexusId;

	public ConfigEntry<bool> PorkRindEnable;

	public ConfigEntry<bool> KabobEnable;

	public ConfigEntry<bool> FriedLoxEnable;

	public ConfigEntry<bool> GlazedCarrotEnable;

	public ConfigEntry<bool> BaconEnable;

	public ConfigEntry<bool> SmokedFishEnable;

	public ConfigEntry<bool> PancakesEnable;

	public ConfigEntry<bool> PizzaEnable;

	public ConfigEntry<bool> CoffeeEnable;

	public ConfigEntry<bool> LatteEnable;

	public ConfigEntry<bool> SmokelessEnable;

	public ConfigEntry<bool> HaggisEnable;

	public ConfigEntry<bool> CandiedTurnipEnable;

	public ConfigEntry<bool> MoochiEnable;

	public ConfigEntry<bool> Nut_EllaEnable;

	public ConfigEntry<bool> BrothEnable;

	public ConfigEntry<bool> FishStewEnable;

	public ConfigEntry<bool> ButterEnable;

	public ConfigEntry<bool> BloodSausageEnable;

	public ConfigEntry<bool> OmletteEnable;

	public ConfigEntry<bool> BurgerEnable;

	public ConfigEntry<bool> PorridgeEnable;

	public ConfigEntry<bool> PBJEnable;

	public ConfigEntry<bool> BoiledEggEnable;

	public ConfigEntry<bool> CakeEnable;

	public ConfigEntry<bool> GrillOriginal;

	public ConfigEntry<bool> CarrotSticksEnable;

	public ConfigEntry<bool> CheffHatEnable;

	public ConfigEntry<bool> MeadEnable;

	public ConfigEntry<bool> CookingSkillEnable;

	public ConfigEntry<bool> BonusWhenCookingEnabled;

	public ConfigEntry<bool> HatSEMessage;

	public ConfigEntry<float> HatXpGain;

	public EffectList buildStone;

	public EffectList cookingSound;

	public EffectList breakStone;

	public EffectList hitStone;

	public EffectList buildKitten;

	public EffectList hearthAddFuel;

	public EffectList fireAddFuel;

	public Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();

	public GameObject icecream_prefab;

	public CustomItem icecream;

	public GameObject porkrind_prefab;

	public CustomItem porkrind;

	public GameObject kabob_prefab;

	public CustomItem kabob;

	public GameObject friedlox_prefab;

	public CustomItem friedlox;

	public GameObject glazedcarrot_prefab;

	public CustomItem glazedcarrot;

	public GameObject bacon_prefab;

	public CustomItem bacon;

	public GameObject smokedfish_prefab;

	public CustomItem smokedfish;

	public GameObject pancake_prefab;

	public CustomItem pancake;

	public GameObject pizza_prefab;

	public CustomItem pizza;

	public GameObject coffee_prefab;

	public CustomItem coffee;

	public GameObject latte_prefab;

	public CustomItem latte;

	public GameObject firecream_prefab;

	public CustomItem firecream;

	public CustomItem electriccream;

	public GameObject electriccream_prefab;

	public CustomItem acidcream;

	public GameObject acidcream_prefab;

	public GameObject porridge_prefab;

	public CustomItem porridge;

	public GameObject pbj_prefab;

	public CustomItem pbj;

	public GameObject cake_prefab;

	public CustomItem cake;

	public AudioSource fireVol;

	public GameObject haggisFab;

	public CustomItem haggis;

	public GameObject candiedTurnipFab;

	public CustomItem candiedTurnip;

	public GameObject moochiFab;

	public CustomItem moochi;

	public GameObject omletteFab;

	public CustomItem omlette;

	public GameObject fishStewFab;

	public CustomItem fishStew;

	public GameObject brothFab;

	public CustomItem broth;

	public GameObject butterFab;

	public CustomItem butter;

	public GameObject bloodsausageFab;

	public CustomItem bloodsausage;

	public GameObject burgerFab;

	public CustomItem burger;

	public GameObject nut_ellaFab;

	public CustomItem nut_ella;

	public GameObject boiledeggFab;

	public CustomItem boiledegg;

	public GameObject carrotstickFab;

	public CustomItem carrotstick;

	public GameObject meadFab;

	public CustomItem mead;

	public GameObject hatFab;

	public CustomItem hat;

	public GameObject fireFab1;

	public CustomPiece fire1;

	public GameObject fireFab2;

	public CustomPiece fire2;

	public GameObject fireFab3;

	private CustomPiece fire3;

	public GameObject eggFab;

	public GameObject deggFab;

	public GameObject porkFab;

	private static bool _isAddingExtraItem;

	public ConfigEntry<bool> ConesEnable { get; private set; }

	public BoneAppetit()
	{
		Instance = this;
	}

		public void Awake()
	{
		CreateConfigValues();
		AssetLoad();
		AddSkills();
		PrefabManager.OnVanillaPrefabsAvailable += LoadSounds;
		ItemManager.OnItemsRegistered += NewDrops;
		SynchronizationManager.OnConfigurationSynchronized += delegate(object obj, ConfigurationSynchronizationEventArgs attr)
		{
			if (attr.InitialSynchronization)
			{
				Logger.LogMessage((object)"Initial Config sync event received");
				LoadFood();
			}
			else
			{
				Logger.LogMessage((object)"Config sync event received");
			}
		};
		_harmony = Harmony.CreateAndPatchAll(typeof(BoneAppetit).Assembly, "com.rockerkitten.boneappetit");
	}

		private void OnDestroy()
	{
		Harmony harmony = _harmony;
		if (harmony != null)
		{
			harmony.UnpatchSelf();
		}
	}

	public void CreateConfigValues()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected O, but got Unknown
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Expected O, but got Unknown
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Expected O, but got Unknown
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Expected O, but got Unknown
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Expected O, but got Unknown
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Expected O, but got Unknown
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Expected O, but got Unknown
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Expected O, but got Unknown
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Expected O, but got Unknown
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Expected O, but got Unknown
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Expected O, but got Unknown
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Expected O, but got Unknown
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Expected O, but got Unknown
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Expected O, but got Unknown
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Expected O, but got Unknown
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Expected O, but got Unknown
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Expected O, but got Unknown
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Expected O, but got Unknown
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Expected O, but got Unknown
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Expected O, but got Unknown
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Expected O, but got Unknown
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Expected O, but got Unknown
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Expected O, but got Unknown
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Expected O, but got Unknown
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Expected O, but got Unknown
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046f: Expected O, but got Unknown
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Expected O, but got Unknown
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Expected O, but got Unknown
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Expected O, but got Unknown
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Expected O, but got Unknown
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Expected O, but got Unknown
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Expected O, but got Unknown
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Expected O, but got Unknown
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Expected O, but got Unknown
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_058d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Expected O, but got Unknown
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Expected O, but got Unknown
		//IL_05c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d3: Expected O, but got Unknown
		//IL_05d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dd: Expected O, but got Unknown
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0607: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Expected O, but got Unknown
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Expected O, but got Unknown
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Expected O, but got Unknown
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Expected O, but got Unknown
		//IL_067c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0681: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Expected O, but got Unknown
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Expected O, but got Unknown
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06be: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c7: Expected O, but got Unknown
		//IL_06c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d1: Expected O, but got Unknown
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Expected O, but got Unknown
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Expected O, but got Unknown
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_0738: Unknown result type (might be due to invalid IL or missing references)
		//IL_0741: Expected O, but got Unknown
		//IL_0741: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Expected O, but got Unknown
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_0775: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Expected O, but got Unknown
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Expected O, but got Unknown
		//IL_07ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Expected O, but got Unknown
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d1: Expected O, but got Unknown
		//IL_07f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_0810: Expected O, but got Unknown
		//IL_0810: Unknown result type (might be due to invalid IL or missing references)
		//IL_081a: Expected O, but got Unknown
		//IL_0843: Unknown result type (might be due to invalid IL or missing references)
		//IL_0848: Unknown result type (might be due to invalid IL or missing references)
		//IL_0850: Unknown result type (might be due to invalid IL or missing references)
		//IL_085d: Expected O, but got Unknown
		//IL_085d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0867: Expected O, but got Unknown
		//IL_088c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0891: Unknown result type (might be due to invalid IL or missing references)
		//IL_0899: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Expected O, but got Unknown
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Expected O, but got Unknown
		((BaseUnityPlugin)this).Config.SaveOnConfigSet = true;
		NexusId = ((BaseUnityPlugin)this).Config.Bind<int>("Hidden", "NexusId", 1250, new ConfigDescription("NexusId", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = false,
			Browsable = false,
			ReadOnly = true
		} }));
		ConesEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Cones", "Enable", true, new ConfigDescription("All Cones Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		PorkRindEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Pork Rind", "Enable", true, new ConfigDescription("Pork Rind Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		KabobEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Kabob", "Enable", true, new ConfigDescription("Kabob Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		FriedLoxEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Fried Lox", "Enable", true, new ConfigDescription("Fried Lox Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		GlazedCarrotEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Glazed Carrots", "Enable", true, new ConfigDescription("Glazed Carrots Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		BaconEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Bacon", "Enable", true, new ConfigDescription("Bacon Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		SmokedFishEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Smoked Fish", "Enable", true, new ConfigDescription("Smoked Fish Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		PancakesEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Pancakes", "Enable", true, new ConfigDescription("Pancakes Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		PizzaEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Pizza", "Enable", true, new ConfigDescription("Pizza Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		CoffeeEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Coffee", "Enable", true, new ConfigDescription("Coffee Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		LatteEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Latte", "Enable", true, new ConfigDescription("Latte Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		PorridgeEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Porridge", "Enable", true, new ConfigDescription("Porridge Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		PBJEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("PBJ", "Enable", true, new ConfigDescription("PBJ Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		CakeEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Cake", "Enable", true, new ConfigDescription("Birthday Cake Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		HaggisEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Haggis", "Enable", true, new ConfigDescription("Haggis Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		CandiedTurnipEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Candied Turnip", "Enable", true, new ConfigDescription("Candied Turnip Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		MoochiEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Moochi", "Enable", true, new ConfigDescription("Moochi Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		Nut_EllaEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Nut_Ella", "Enable", true, new ConfigDescription("Nut_Ella Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		BurgerEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Burger", "Enable", true, new ConfigDescription("Burger Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		OmletteEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Omlette", "Enable", true, new ConfigDescription("Omlette Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		BrothEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Broth", "Enable", true, new ConfigDescription("Broth Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		FishStewEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Fish Stew", "Enable", true, new ConfigDescription("Fish Stew Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		ButterEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Butter", "Enable", true, new ConfigDescription("Butter Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		BloodSausageEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Blood Sausage", "Enable", true, new ConfigDescription("Blood Sausage Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		BoiledEggEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Boiled Egg", "Enable", true, new ConfigDescription("Boiled Egg Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		CarrotSticksEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Carrot Sticks", "Enable", true, new ConfigDescription("Carrot Sticks Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		CheffHatEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Chef Hat", "Enable", true, new ConfigDescription("Chef Hat Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		MeadEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Mead", "Enable", true, new ConfigDescription("Mead Enable", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		GrillOriginal = ((BaseUnityPlugin)this).Config.Bind<bool>("Original", "Enable", true, new ConfigDescription("Use original grill", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		SmokelessEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Smokeless", "Enable", true, new ConfigDescription("Enable to allow building of smokeless fires", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true
		} }));
		CookingSkillEnable = ((BaseUnityPlugin)this).Config.Bind<bool>("Cooking Skill", "Enable Cooking Skill", true, new ConfigDescription("Enable Cooking Skill", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true,
			Order = 1
		} }));
		BonusWhenCookingEnabled = ((BaseUnityPlugin)this).Config.Bind<bool>("Cooking Skill", "Enable Cooking Bonus", true, new ConfigDescription("Enable Cooking Bonus", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true,
			Order = 2
		} }));
		HatXpGain = ((BaseUnityPlugin)this).Config.Bind<float>("Cooking Skill", "Chef Hat XP Gain", 5f, new ConfigDescription("XP Gain multiplier when cooking while wearing the Chef Hat", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = true,
			Order = 3
		} }));
		HatSEMessage = ((BaseUnityPlugin)this).Config.Bind<bool>("Cooking Skill", "Enable Chef Hat Message", true, new ConfigDescription("Enable Message when equipping the Chef Hat", (AcceptableValueBase)null, new object[1] { (object)new ConfigurationManagerAttributes
		{
			IsAdminOnly = false,
			Order = 4
		} }));
	}

	public void LoadFood()
	{
		icecream.Recipe.Recipe.m_enabled = ConesEnable.Value;
		porkrind.Recipe.Recipe.m_enabled = PorkRindEnable.Value;
		kabob.Recipe.Recipe.m_enabled = KabobEnable.Value;
		friedlox.Recipe.Recipe.m_enabled = FriedLoxEnable.Value;
		glazedcarrot.Recipe.Recipe.m_enabled = GlazedCarrotEnable.Value;
		bacon.Recipe.Recipe.m_enabled = BaconEnable.Value;
		smokedfish.Recipe.Recipe.m_enabled = SmokedFishEnable.Value;
		pancake.Recipe.Recipe.m_enabled = PancakesEnable.Value;
		pizza.Recipe.Recipe.m_enabled = PizzaEnable.Value;
		coffee.Recipe.Recipe.m_enabled = CoffeeEnable.Value;
		latte.Recipe.Recipe.m_enabled = LatteEnable.Value;
		firecream.Recipe.Recipe.m_enabled = ConesEnable.Value;
		electriccream.Recipe.Recipe.m_enabled = ConesEnable.Value;
		acidcream.Recipe.Recipe.m_enabled = ConesEnable.Value;
		porridge.Recipe.Recipe.m_enabled = PorridgeEnable.Value;
		pbj.Recipe.Recipe.m_enabled = PBJEnable.Value;
		cake.Recipe.Recipe.m_enabled = CakeEnable.Value;
		haggis.Recipe.Recipe.m_enabled = HaggisEnable.Value;
		candiedTurnip.Recipe.Recipe.m_enabled = CandiedTurnipEnable.Value;
		moochi.Recipe.Recipe.m_enabled = MoochiEnable.Value;
		nut_ella.Recipe.Recipe.m_enabled = Nut_EllaEnable.Value;
		burger.Recipe.Recipe.m_enabled = BurgerEnable.Value;
		omlette.Recipe.Recipe.m_enabled = OmletteEnable.Value;
		broth.Recipe.Recipe.m_enabled = BrothEnable.Value;
		fishStew.Recipe.Recipe.m_enabled = FishStewEnable.Value;
		butter.Recipe.Recipe.m_enabled = ButterEnable.Value;
		bloodsausage.Recipe.Recipe.m_enabled = BloodSausageEnable.Value;
		boiledegg.Recipe.Recipe.m_enabled = BoiledEggEnable.Value;
		carrotstick.Recipe.Recipe.m_enabled = CarrotSticksEnable.Value;
		mead.Recipe.Recipe.m_enabled = MeadEnable.Value;
		fire1.Piece.m_enabled = SmokelessEnable.Value;
		fire2.Piece.m_enabled = SmokelessEnable.Value;
		fire3.Piece.m_enabled = SmokelessEnable.Value;
	}

	public void AssetLoad()
	{
		GrillAssetBundle = AssetUtils.LoadAssetBundleFromResources("grill", Assembly.GetExecutingAssembly());
		FoodAssetBundle = AssetUtils.LoadAssetBundleFromResources("customfood", Assembly.GetExecutingAssembly());
		CookingSprite = FoodAssetBundle.LoadAsset<Sprite>("rkcookingsprite");
		Logger.LogMessage((object)"Prepping Kitchen...");
		LoadDropFab();
		Logger.LogMessage((object)"Big thanks to MeatwareMonster!");
	}

	public void LoadSounds()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Expected O, but got Unknown
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Expected O, but got Unknown
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Expected O, but got Unknown
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Expected O, but got Unknown
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Expected O, but got Unknown
		try
		{
			GameObject prefab = PrefabManager.Instance.GetPrefab("sfx_build_hammer_stone");
			GameObject prefab2 = PrefabManager.Instance.GetPrefab("vfx_Place_stone_wall_2x1");
			GameObject prefab3 = PrefabManager.Instance.GetPrefab("sfx_cooking_station_done");
			GameObject prefab4 = PrefabManager.Instance.GetPrefab("sfx_rock_destroyed");
			GameObject prefab5 = PrefabManager.Instance.GetPrefab("vfx_FireAddFuel");
			GameObject prefab6 = PrefabManager.Instance.GetPrefab("sfx_FireAddFuel");
			GameObject prefab7 = PrefabManager.Instance.GetPrefab("sfx_rock_hit");
			GameObject prefab8 = PrefabManager.Instance.GetPrefab("vfx_HearthAddFuel");
			EffectList val = new EffectList();
			val.m_effectPrefabs = (EffectList.EffectData[])(object)new EffectList.EffectData[2]
			{
				new EffectList.EffectData
				{
					m_prefab = prefab
				},
				new EffectList.EffectData
				{
					m_prefab = prefab2
				}
			};
			buildStone = val;
			val = new EffectList();
			val.m_effectPrefabs = (EffectList.EffectData[])(object)new EffectList.EffectData[1]
			{
				new EffectList.EffectData
				{
					m_prefab = prefab3
				}
			};
			cookingSound = val;
			val = new EffectList();
			val.m_effectPrefabs = (EffectList.EffectData[])(object)new EffectList.EffectData[1]
			{
				new EffectList.EffectData
				{
					m_prefab = prefab4
				}
			};
			breakStone = val;
			val = new EffectList();
			val.m_effectPrefabs = (EffectList.EffectData[])(object)new EffectList.EffectData[1]
			{
				new EffectList.EffectData
				{
					m_prefab = prefab7
				}
			};
			hitStone = val;
			val = new EffectList();
			val.m_effectPrefabs = (EffectList.EffectData[])(object)new EffectList.EffectData[1]
			{
				new EffectList.EffectData
				{
					m_prefab = prefab
				}
			};
			buildKitten = val;
			val = new EffectList();
			val.m_effectPrefabs = (EffectList.EffectData[])(object)new EffectList.EffectData[2]
			{
				new EffectList.EffectData
				{
					m_prefab = prefab8
				},
				new EffectList.EffectData
				{
					m_prefab = prefab6
				}
			};
			hearthAddFuel = val;
			val = new EffectList();
			val.m_effectPrefabs = (EffectList.EffectData[])(object)new EffectList.EffectData[2]
			{
				new EffectList.EffectData
				{
					m_prefab = prefab5
				},
				new EffectList.EffectData
				{
					m_prefab = prefab6
				}
			};
			fireAddFuel = val;
			Logger.LogMessage((object)"Loaded Game VFX and SFX");
			Butter();
			Nut_Ella();
			IceCream();
			PorkRind();
			Kabob();
			FriedLox();
			GlazedCarrot();
			Bacon();
			SmokedFish();
			Pancakes();
			Pizza();
			Coffee();
			Latte();
			FireCream();
			ElectricCream();
			AcidCream();
			Porridge();
			PBJ();
			Cake();
			Haggis();
			CandiedTurnip();
			Moochi();
			Broth();
			FishStew();
			BloodSausage();
			Burger();
			Omlette();
			BoiledEgg();
			CarrotSticks();
			ChefHatt();
			Mead();
			LoadGrillItem();
			LoadGriddle();
			Oven();
			LoadFire();
			LoadHearth();
			Prepstation();
			Brazier();
			fireVol = AccessTools.FieldRefAccess<AudioMan, AudioSource>("m_ambientLoopSource")(AudioMan.instance);
		}
		catch (Exception ex)
		{
			Logger.LogError((object)("Error while running OnVanillaLoad: " + ex.Message));
		}
		finally
		{
			Logger.LogMessage((object)"Load Complete. Bone Appetit yall.");
			PrefabManager.OnVanillaPrefabsAvailable -= LoadSounds;
		}
	}

	public void NewDrops()
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Expected O, but got Unknown
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Expected O, but got Unknown
		GameObject prefab = PrefabManager.Instance.GetPrefab("Boar");
		GameObject prefab2 = PrefabManager.Instance.GetPrefab("Hatchling");
		GameObject prefab3 = PrefabManager.Instance.GetPrefab("Seagal");
		GameObject prefab4 = PrefabManager.Instance.GetPrefab("Crow");
		GameObject prefab5 = PrefabManager.Instance.GetPrefab("rk_pork");
		GameObject prefab6 = PrefabManager.Instance.GetPrefab("rk_egg");
		GameObject prefab7 = PrefabManager.Instance.GetPrefab("rk_dragonegg");
		DropOnDestroyed component = prefab3.GetComponent<DropOnDestroyed>();
		component.m_dropWhenDestroyed.m_drops.Add(new DropTable.DropData
		{
			m_item = prefab6,
			m_stackMin = 1,
			m_stackMax = 1,
			m_weight = 1f
		});
		component.m_dropWhenDestroyed.m_oneOfEach = true;
		component.m_dropWhenDestroyed.m_dropMax = 2;
		component.m_dropWhenDestroyed.m_dropMin = 2;
		component.m_dropWhenDestroyed.m_dropChance = 1f;
		component.m_spawnYStep = 0.3f;
		component.m_spawnYOffset = 0.5f;
		DropOnDestroyed component2 = prefab4.GetComponent<DropOnDestroyed>();
		component2.m_dropWhenDestroyed.m_drops.Add(new DropTable.DropData
		{
			m_item = prefab6,
			m_stackMin = 1,
			m_stackMax = 1,
			m_weight = 1f
		});
		component2.m_dropWhenDestroyed.m_oneOfEach = true;
		component2.m_dropWhenDestroyed.m_dropMax = 2;
		component2.m_dropWhenDestroyed.m_dropMin = 2;
		component2.m_dropWhenDestroyed.m_dropChance = 1f;
		component2.m_spawnYStep = 0.3f;
		component2.m_spawnYOffset = 0.5f;
		prefab.GetComponent<CharacterDrop>().m_drops.Add(new CharacterDrop.Drop
		{
			m_prefab = prefab5,
			m_amountMin = 1,
			m_amountMax = 1,
			m_chance = 1f,
			m_levelMultiplier = true,
			m_onePerPlayer = false
		});
		prefab2.GetComponent<CharacterDrop>().m_drops.Add(new CharacterDrop.Drop
		{
			m_prefab = prefab7,
			m_amountMin = 1,
			m_amountMax = 1,
			m_chance = 1f,
			m_levelMultiplier = true,
			m_onePerPlayer = false
		});
		ItemManager.OnItemsRegistered -= NewDrops;
	}

	public void AddSkills()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if (CookingSkillEnable.Value)
		{
			rkCookingSkill = SkillManager.Instance.AddSkill(new SkillConfig
			{
				Identifier = "com.rockerkitten.boneappetit",
				Name = "Gore-mand",
				Description = "Learn to cook and eat like a Viking!",
				Icon = CookingSprite,
				IncreaseStep = 1f
			});
		}
	}

	public void LoadDropFab()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Expected O, but got Unknown
		porkFab = FoodAssetBundle.LoadAsset<GameObject>("rk_pork");
		ItemManager.Instance.AddItem(new CustomItem(porkFab, false));
		eggFab = FoodAssetBundle.LoadAsset<GameObject>("rk_egg");
		ItemManager.Instance.AddItem(new CustomItem(eggFab, false));
		deggFab = FoodAssetBundle.LoadAsset<GameObject>("rk_dragonegg");
		ItemManager.Instance.AddItem(new CustomItem(deggFab, false));
	}

	private void LoadGrillItem()
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Expected O, but got Unknown
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Expected O, but got Unknown
		GameObject val;
		if (GrillOriginal.Value)
		{
			val = GrillAssetBundle.LoadAsset<GameObject>("rk_grill");
			if ((Object)(object)val.GetComponent<WearNTear>() == (Object)null)
			{
				WearNTear val2 = val.AddComponent<WearNTear>();
				val2.m_broken = val;
				val2.m_new = val;
				val2.m_wet = val;
				val2.m_worn = val;
				val2.m_noSupportWear = false;
			}
			if ((Object)(object)val.GetComponent<WearNTear>() != (Object)null)
			{
				WearNTear component = val.GetComponent<WearNTear>();
				component.m_supports = false;
				component.m_noSupportWear = false;
			}
		}
		else
		{
			val = FoodAssetBundle.LoadAsset<GameObject>("rk_grill");
			if ((Object)(object)val.GetComponent<WearNTear>() == (Object)null)
			{
				WearNTear val3 = val.AddComponent<WearNTear>();
				val3.m_broken = val;
				val3.m_new = val;
				val3.m_wet = val;
				val3.m_worn = val;
			}
			if ((Object)(object)val.GetComponent<WearNTear>() != (Object)null)
			{
				WearNTear component2 = val.GetComponent<WearNTear>();
				component2.m_supports = false;
				component2.m_noSupportWear = false;
			}
		}
		GameObject obj = val;
		PieceConfig val4 = new PieceConfig();
		val4.CraftingStation = "forge";
		val4.AllowedInDungeons = false;
		val4.Enabled = GrillOriginal.Value;
		val4.PieceTable = "_HammerPieceTable";
		val4.Requirements = (RequirementConfig[])(object)new RequirementConfig[2]
		{
			new RequirementConfig
			{
				Item = "Stone",
				Amount = 10,
				Recover = true
			},
			new RequirementConfig
			{
				Item = "Iron",
				Amount = 2,
				Recover = true
			}
		};
		CustomPiece val5 = new CustomPiece(obj, true, val4);
		Piece component3 = val.GetComponent<Piece>();
		component3.m_placeEffect = buildKitten;
		WearNTear component4 = val.GetComponent<WearNTear>();
		component4.m_hitEffect = hitStone;
		component4.m_destroyedEffect = breakStone;
		CraftingStation component5 = val.GetComponent<CraftingStation>();
		component5.m_craftItemEffects = cookingSound;
		PieceManager.Instance.AddPiece(val5);
	}

	private void LoadGriddle()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		GameObject val = GrillAssetBundle.LoadAsset<GameObject>("rk_griddle");
		PieceConfig val2 = new PieceConfig();
		val2.CraftingStation = "";
		val2.AllowedInDungeons = false;
		val2.Enabled = true;
		val2.PieceTable = "_HammerPieceTable";
		val2.Requirements = (RequirementConfig[])(object)new RequirementConfig[1]
		{
			new RequirementConfig
			{
				Item = "Stone",
				Amount = 10,
				Recover = true
			}
		};
		CustomPiece val3 = new CustomPiece(val, true, val2);
		Piece component = val.GetComponent<Piece>();
		component.m_placeEffect = buildStone;
		CraftingStation component2 = val.GetComponent<CraftingStation>();
		component2.m_craftItemEffects = cookingSound;
		WearNTear component3 = val.GetComponent<WearNTear>();
		component3.m_destroyedEffect = breakStone;
		component3.m_hitEffect = hitStone;
		PieceManager.Instance.AddPiece(val3);
	}

	private void Oven()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		GameObject val = GrillAssetBundle.LoadAsset<GameObject>("rk_oven");
		PieceConfig val2 = new PieceConfig();
		val2.CraftingStation = "";
		val2.AllowedInDungeons = false;
		val2.Enabled = true;
		val2.PieceTable = "_HammerPieceTable";
		val2.ExtendStation = "rk_grill";
		val2.Requirements = (RequirementConfig[])(object)new RequirementConfig[3]
		{
			new RequirementConfig
			{
				Item = "SurtlingCore",
				Amount = 2,
				Recover = true
			},
			new RequirementConfig
			{
				Item = "TrophySurtling",
				Amount = 1,
				Recover = true
			},
			new RequirementConfig
			{
				Item = "Stone",
				Amount = 10,
				Recover = true
			}
		};
		CustomPiece val3 = new CustomPiece(val, true, val2);
		Piece component = val.GetComponent<Piece>();
		component.m_placeEffect = buildStone;
		WearNTear component2 = val.GetComponent<WearNTear>();
		component2.m_supports = false;
		component2.m_noSupportWear = false;
		PieceManager.Instance.AddPiece(val3);
	}

	private void Prepstation()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		GameObject val = GrillAssetBundle.LoadAsset<GameObject>("rk_prep");
		PieceConfig val2 = new PieceConfig();
		val2.PieceTable = "_HammerPieceTable";
		val2.AllowedInDungeons = false;
		val2.CraftingStation = "forge";
		val2.Enabled = true;
		val2.Requirements = (RequirementConfig[])(object)new RequirementConfig[3]
		{
			new RequirementConfig
			{
				Item = "Wood",
				Amount = 4,
				Recover = true
			},
			new RequirementConfig
			{
				Item = "Tin",
				Amount = 5,
				Recover = true
			},
			new RequirementConfig
			{
				Item = "Stone",
				Amount = 3,
				Recover = true
			}
		};
		CustomPiece val3 = new CustomPiece(val, true, val2);
		Piece component = val.GetComponent<Piece>();
		component.m_placeEffect = buildKitten;
		WearNTear component2 = val.GetComponent<WearNTear>();
		component2.m_hitEffect = hitStone;
		component2.m_destroyedEffect = breakStone;
		fireVol = val.GetComponentInChildren<AudioSource>();
		PieceManager.Instance.AddPiece(val3);
	}

	private void IceCream()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		icecream_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_icecream");
		GameObject obj = icecream_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Ice Cream";
		val.Enabled = ConesEnable.Value;
		val.Amount = 2;
		val.CraftingStation = "rk_prep";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "FreezeGland",
				Amount = 4
			},
			new RequirementConfig
			{
				Item = "Blueberries",
				Amount = 8
			},
			new RequirementConfig
			{
				Item = "Honey",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "rk_dragonegg",
				Amount = 1
			}
		};
		icecream = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(icecream);
	}

	private void Nut_Ella()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		nut_ellaFab = FoodAssetBundle.LoadAsset<GameObject>("rk_nut_ella");
		GameObject obj = nut_ellaFab;
		ItemConfig val = new ItemConfig();
		val.Name = "Nut-Ella";
		val.Enabled = Nut_EllaEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_prep";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[2]
		{
			new RequirementConfig
			{
				Item = "BeechSeeds",
				Amount = 6
			},
			new RequirementConfig
			{
				Item = "rk_butter",
				Amount = 1
			}
		};
		nut_ella = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(nut_ella);
	}

	private void CarrotSticks()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		carrotstickFab = FoodAssetBundle.LoadAsset<GameObject>("rk_carrotsticks");
		GameObject obj = carrotstickFab;
		ItemConfig val = new ItemConfig();
		val.Name = "Carrot Sticks";
		val.Enabled = CarrotSticksEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_prep";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[2]
		{
			new RequirementConfig
			{
				Item = "Carrot",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "rk_nut_ella",
				Amount = 1
			}
		};
		carrotstick = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(carrotstick);
	}

	private void BoiledEgg()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		boiledeggFab = FoodAssetBundle.LoadAsset<GameObject>("rk_boiledegg");
		GameObject obj = boiledeggFab;
		ItemConfig val = new ItemConfig();
		val.Name = "Boiled Egg";
		val.Enabled = BoiledEggEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_prep";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[1]
		{
			new RequirementConfig
			{
				Item = "rk_egg",
				Amount = 2
			}
		};
		boiledegg = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(boiledegg);
	}

	private void Butter()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		butterFab = FoodAssetBundle.LoadAsset<GameObject>("rk_butter");
		GameObject obj = butterFab;
		ItemConfig val = new ItemConfig();
		val.Name = "Carrot Butter";
		val.Enabled = ButterEnable.Value;
		val.Amount = 2;
		val.CraftingStation = "rk_prep";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[1]
		{
			new RequirementConfig
			{
				Item = "CarrotSeeds",
				Amount = 8
			}
		};
		butter = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(butter);
	}

	private void Broth()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		brothFab = FoodAssetBundle.LoadAsset<GameObject>("rk_broth");
		GameObject obj = brothFab;
		ItemConfig val = new ItemConfig();
		val.Name = "Bone Broth";
		val.Enabled = BrothEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_prep";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[2]
		{
			new RequirementConfig
			{
				Item = "BoneFragments",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "rk_butter",
				Amount = 1
			}
		};
		broth = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(broth);
	}

	private void FishStew()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		fishStewFab = FoodAssetBundle.LoadAsset<GameObject>("rk_fishstew");
		GameObject obj = fishStewFab;
		ItemConfig val = new ItemConfig();
		val.Name = "Fish Stew";
		val.Enabled = FishStewEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_prep";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "rk_broth",
				Amount = 1
			},
			new RequirementConfig
			{
				Item = "FishRaw",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "Thistle",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "rk_egg",
				Amount = 2
			}
		};
		fishStew = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(fishStew);
	}

	private void Burger()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		burgerFab = FoodAssetBundle.LoadAsset<GameObject>("rk_burger");
		GameObject obj = burgerFab;
		ItemConfig val = new ItemConfig();
		val.Name = "Burger";
		val.Enabled = BurgerEnable.Value;
		val.Amount = 2;
		val.CraftingStation = "rk_grill";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "RawMeat",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "LoxMeat",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "Turnip",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "Bread",
				Amount = 1
			}
		};
		burger = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(burger);
	}

	private void BloodSausage()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		bloodsausageFab = FoodAssetBundle.LoadAsset<GameObject>("rk_bloodsausage");
		GameObject obj = bloodsausageFab;
		ItemConfig val = new ItemConfig();
		val.Name = "Blood Sausage";
		val.Enabled = BloodSausageEnable.Value;
		val.Amount = 2;
		val.CraftingStation = "rk_grill";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "Entrails",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "Bloodbag",
				Amount = 1
			},
			new RequirementConfig
			{
				Item = "Thistle",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "rk_pork",
				Amount = 2
			}
		};
		bloodsausage = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(bloodsausage);
	}

	private void Omlette()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		omletteFab = FoodAssetBundle.LoadAsset<GameObject>("rk_omlette");
		GameObject obj = omletteFab;
		ItemConfig val = new ItemConfig();
		val.Name = "Omlette";
		val.Enabled = OmletteEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_griddle";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "rk_egg",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "Thistle",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "rk_pork",
				Amount = 1
			},
			new RequirementConfig
			{
				Item = "rk_butter",
				Amount = 1
			}
		};
		omlette = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(omlette);
	}

	private void PorkRind()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Expected O, but got Unknown
		porkrind_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_porkrind");
		GameObject obj = porkrind_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Pork Rinds";
		val.Enabled = PorkRindEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_griddle";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[2]
		{
			new RequirementConfig
			{
				Item = "LeatherScraps",
				Amount = 1
			},
			new RequirementConfig
			{
				Item = "rk_pork",
				Amount = 1
			}
		};
		porkrind = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(porkrind);
	}

	private void Haggis()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		haggisFab = FoodAssetBundle.LoadAsset<GameObject>("rk_haggis");
		GameObject obj = haggisFab;
		ItemConfig val = new ItemConfig();
		val.Name = "Haggis";
		val.Enabled = HaggisEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_prep";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "RawMeat",
				Amount = 1
			},
			new RequirementConfig
			{
				Item = "Carrot",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "Entrails",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "Turnip",
				Amount = 2
			}
		};
		haggis = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(haggis);
	}

	private void CandiedTurnip()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		candiedTurnipFab = FoodAssetBundle.LoadAsset<GameObject>("rk_candiedturnip");
		GameObject obj = candiedTurnipFab;
		ItemConfig val = new ItemConfig();
		val.Name = "Candied Turnip";
		val.Enabled = CandiedTurnipEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_grill";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[3]
		{
			new RequirementConfig
			{
				Item = "Thistle",
				Amount = 1
			},
			new RequirementConfig
			{
				Item = "Honey",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "Turnip",
				Amount = 2
			}
		};
		candiedTurnip = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(candiedTurnip);
	}

	private void Moochi()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		moochiFab = FoodAssetBundle.LoadAsset<GameObject>("rk_moochi");
		GameObject obj = moochiFab;
		ItemConfig val = new ItemConfig();
		val.Name = "Moochi";
		val.Enabled = MoochiEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_prep";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "rk_dragonegg",
				Amount = 1
			},
			new RequirementConfig
			{
				Item = "Honey",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "FreezeGland",
				Amount = 1
			},
			new RequirementConfig
			{
				Item = "Blueberries",
				Amount = 4
			}
		};
		moochi = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(moochi);
	}

	private void Kabob()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		kabob_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_kabob");
		GameObject obj = kabob_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Kabob";
		val.Enabled = KabobEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_grill";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "Turnip",
				Amount = 1
			},
			new RequirementConfig
			{
				Item = "Carrot",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "RawMeat",
				Amount = 1
			},
			new RequirementConfig
			{
				Item = "BoneFragments",
				Amount = 2
			}
		};
		kabob = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(kabob);
	}

	private void FriedLox()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		friedlox_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_friedloxmeat");
		GameObject obj = friedlox_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Chicken Fried Lox Meat";
		val.Enabled = FriedLoxEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_grill";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "LoxMeat",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "BarleyFlour",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "rk_egg",
				Amount = 1
			},
			new RequirementConfig
			{
				Item = "rk_butter",
				Amount = 2
			}
		};
		friedlox = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(friedlox);
	}

	private void GlazedCarrot()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Expected O, but got Unknown
		glazedcarrot_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_glazedcarrots");
		GameObject obj = glazedcarrot_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Honey Glazed Carrots";
		val.Enabled = GlazedCarrotEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_griddle";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[3]
		{
			new RequirementConfig
			{
				Item = "Carrot",
				Amount = 3
			},
			new RequirementConfig
			{
				Item = "Honey",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "Dandelion",
				Amount = 2
			}
		};
		glazedcarrot = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(glazedcarrot);
	}

	private void Bacon()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		bacon_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_bacon");
		GameObject obj = bacon_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Bacon";
		val.Enabled = BaconEnable.Value;
		val.Amount = 2;
		val.CraftingStation = "rk_griddle";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[1]
		{
			new RequirementConfig
			{
				Item = "rk_pork",
				Amount = 2
			}
		};
		bacon = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(bacon);
	}

	private void SmokedFish()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		smokedfish_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_smokedfish");
		GameObject obj = smokedfish_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "SmokedFish";
		val.Enabled = SmokedFishEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_griddle";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[1]
		{
			new RequirementConfig
			{
				Item = "FishRaw",
				Amount = 1
			}
		};
		smokedfish = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(smokedfish);
	}

	private void Pancakes()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		pancake_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_pancake");
		GameObject obj = pancake_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Pancakes";
		val.Enabled = PancakesEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_grill";
		val.MinStationLevel = 2;
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "Honey",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "BarleyFlour",
				Amount = 3
			},
			new RequirementConfig
			{
				Item = "rk_butter",
				Amount = 5
			},
			new RequirementConfig
			{
				Item = "rk_egg",
				Amount = 2
			}
		};
		pancake = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(pancake);
	}

	private void Pizza()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		pizza_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_pizza");
		GameObject obj = pizza_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Pizza";
		val.Enabled = PizzaEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_grill";
		val.MinStationLevel = 2;
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "Mushroom",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "BarleyFlour",
				Amount = 3
			},
			new RequirementConfig
			{
				Item = "rk_egg",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "RawMeat",
				Amount = 2
			}
		};
		pizza = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(pizza);
	}

	private void Coffee()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		coffee_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_coffee");
		GameObject obj = coffee_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Coffee";
		val.Enabled = CoffeeEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_prep";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[1]
		{
			new RequirementConfig
			{
				Item = "AncientSeed",
				Amount = 2
			}
		};
		coffee = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(coffee);
	}

	private void Latte()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		latte_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_latte");
		GameObject obj = latte_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Spice Latte";
		val.Enabled = LatteEnable.Value;
		val.Amount = 2;
		val.CraftingStation = "rk_prep";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[3]
		{
			new RequirementConfig
			{
				Item = "Crystal",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "Barley",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "Honey",
				Amount = 10
			}
		};
		latte = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(latte);
	}

	private void FireCream()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		firecream_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_firecream");
		GameObject obj = firecream_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Fire Cream";
		val.Enabled = ConesEnable.Value;
		val.Amount = 2;
		val.CraftingStation = "rk_prep";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "SurtlingCore",
				Amount = 4
			},
			new RequirementConfig
			{
				Item = "Raspberry",
				Amount = 8
			},
			new RequirementConfig
			{
				Item = "Honey",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "rk_dragonegg",
				Amount = 2
			}
		};
		firecream = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(firecream);
	}

	private void ElectricCream()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		electriccream_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_electriccream");
		GameObject obj = electriccream_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Electric Cream";
		val.Enabled = ConesEnable.Value;
		val.Amount = 2;
		val.CraftingStation = "rk_prep";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "Crystal",
				Amount = 4
			},
			new RequirementConfig
			{
				Item = "Cloudberry",
				Amount = 8
			},
			new RequirementConfig
			{
				Item = "Honey",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "rk_dragonegg",
				Amount = 2
			}
		};
		electriccream = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(electriccream);
	}

	private void AcidCream()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		acidcream_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_acidcream");
		GameObject obj = acidcream_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Acid Cream Cone";
		val.Enabled = ConesEnable.Value;
		val.Amount = 2;
		val.CraftingStation = "rk_prep";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "Guck",
				Amount = 4
			},
			new RequirementConfig
			{
				Item = "MushroomYellow",
				Amount = 8
			},
			new RequirementConfig
			{
				Item = "Honey",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "rk_dragonegg",
				Amount = 2
			}
		};
		acidcream = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(acidcream);
	}

	private void Porridge()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		porridge_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_porridge");
		GameObject obj = porridge_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Porridge";
		val.Enabled = PorridgeEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_grill";
		val.MinStationLevel = 2;
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "Barley",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "Cloudberry",
				Amount = 4
			},
			new RequirementConfig
			{
				Item = "Honey",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "rk_butter",
				Amount = 1
			}
		};
		porridge = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(porridge);
	}

	private void PBJ()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		pbj_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_pbj");
		GameObject obj = pbj_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Jimmy's PBJ";
		val.Enabled = PBJEnable.Value;
		val.Amount = 4;
		val.CraftingStation = "rk_grill";
		val.MinStationLevel = 1;
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[3]
		{
			new RequirementConfig
			{
				Item = "Bread",
				Amount = 1
			},
			new RequirementConfig
			{
				Item = "QueensJam",
				Amount = 1
			},
			new RequirementConfig
			{
				Item = "rk_nut_ella",
				Amount = 4
			}
		};
		pbj = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(pbj);
	}

	private void Cake()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Expected O, but got Unknown
		cake_prefab = FoodAssetBundle.LoadAsset<GameObject>("rk_birthday");
		GameObject obj = cake_prefab;
		ItemConfig val = new ItemConfig();
		val.Name = "Birthday Cake";
		val.Enabled = CakeEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_grill";
		val.MinStationLevel = 2;
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[4]
		{
			new RequirementConfig
			{
				Item = "BarleyFlour",
				Amount = 2
			},
			new RequirementConfig
			{
				Item = "Honey",
				Amount = 4
			},
			new RequirementConfig
			{
				Item = "Cloudberry",
				Amount = 4
			},
			new RequirementConfig
			{
				Item = "rk_egg",
				Amount = 2
			}
		};
		cake = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(cake);
	}

	private void ChefHatt()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		hatFab = FoodAssetBundle.LoadAsset<GameObject>("rk_chef");
		GameObject obj = hatFab;
		ItemConfig val = new ItemConfig();
		val.Name = "Chef Hat";
		val.Description = "Improves Cooking Skill XP Earned.";
		val.Enabled = CheffHatEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[1]
		{
			new RequirementConfig
			{
				Item = "Dandelion",
				Amount = 5
			}
		};
		hat = new CustomItem(obj, true, val);
		ItemDrop itemDrop = hat.ItemDrop;
		SE_CheffHat equipStatusEffect = ScriptableObject.CreateInstance<SE_CheffHat>();
		itemDrop.m_itemData.m_shared.m_equipStatusEffect = (StatusEffect)(object)equipStatusEffect;
		ItemManager.Instance.AddItem(hat);
	}

	private void Mead()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		meadFab = FoodAssetBundle.LoadAsset<GameObject>("rk_mead");
		GameObject obj = meadFab;
		ItemConfig val = new ItemConfig();
		val.Name = "Mead";
		val.Enabled = MeadEnable.Value;
		val.Amount = 1;
		val.CraftingStation = "rk_prep";
		val.MinStationLevel = 1;
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[2]
		{
			new RequirementConfig
			{
				Item = "Barley",
				Amount = 3
			},
			new RequirementConfig
			{
				Item = "Honey",
				Amount = 4
			}
		};
		mead = new CustomItem(obj, false, val);
		ItemManager.Instance.AddItem(mead);
	}

	private void LoadFire()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		fireFab1 = GrillAssetBundle.LoadAsset<GameObject>("rk_campfire");
		GameObject obj = fireFab1;
		PieceConfig val = new PieceConfig();
		val.CraftingStation = "";
		val.AllowedInDungeons = false;
		val.Enabled = SmokelessEnable.Value;
		val.PieceTable = "_HammerPieceTable";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[2]
		{
			new RequirementConfig
			{
				Item = "Stone",
				Amount = 5,
				Recover = true
			},
			new RequirementConfig
			{
				Item = "Wood",
				Amount = 2,
				Recover = true
			}
		};
		fire1 = new CustomPiece(obj, true, val);
		fire1.Piece.m_icon = PrefabManager.Instance.GetPrefab("fire_pit").gameObject.GetComponent<Piece>().m_icon;
		Piece component = fireFab1.GetComponent<Piece>();
		component.m_placeEffect = buildStone;
		WearNTear component2 = fireFab1.GetComponent<WearNTear>();
		component2.m_destroyedEffect = breakStone;
		Fireplace component3 = fireFab1.GetComponent<Fireplace>();
		component3.m_fuelAddedEffects = fireAddFuel;
		component3.m_fuelItem = PrefabManager.Instance.GetPrefab("Wood").gameObject.GetComponent<ItemDrop>();
		fireVol = fireFab1.GetComponentInChildren<AudioSource>();
		PieceManager.Instance.AddPiece(fire1);
	}

	private void LoadHearth()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		fireFab2 = GrillAssetBundle.LoadAsset<GameObject>("rk_hearth");
		GameObject obj = fireFab2;
		PieceConfig val = new PieceConfig();
		val.CraftingStation = "piece_stonecutter";
		val.AllowedInDungeons = false;
		val.Enabled = SmokelessEnable.Value;
		val.PieceTable = "_HammerPieceTable";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[1]
		{
			new RequirementConfig
			{
				Item = "Stone",
				Amount = 15,
				Recover = true
			}
		};
		fire2 = new CustomPiece(obj, true, val);
		Piece component = fireFab2.GetComponent<Piece>();
		component.m_placeEffect = buildStone;
		fire2.Piece.m_icon = PrefabManager.Instance.GetPrefab("hearth").gameObject.GetComponent<Piece>().m_icon;
		WearNTear component2 = fireFab2.GetComponent<WearNTear>();
		component2.m_destroyedEffect = breakStone;
		Fireplace component3 = fireFab2.GetComponent<Fireplace>();
		component3.m_fuelAddedEffects = hearthAddFuel;
		component3.m_fuelItem = PrefabManager.Instance.GetPrefab("Wood").gameObject.GetComponent<ItemDrop>();
		fireVol = fireFab2.GetComponentInChildren<AudioSource>();
		PieceManager.Instance.AddPiece(fire2);
	}

	private void Brazier()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		fireFab3 = GrillAssetBundle.LoadAsset<GameObject>("rk_brazier");
		GameObject obj = fireFab3;
		PieceConfig val = new PieceConfig();
		val.CraftingStation = "forge";
		val.AllowedInDungeons = false;
		val.Enabled = SmokelessEnable.Value;
		val.PieceTable = "_HammerPieceTable";
		val.Requirements = (RequirementConfig[])(object)new RequirementConfig[3]
		{
			new RequirementConfig
			{
				Item = "Bronze",
				Amount = 5,
				Recover = true
			},
			new RequirementConfig
			{
				Item = "Coal",
				Amount = 2,
				Recover = true
			},
			new RequirementConfig
			{
				Item = "Chain",
				Amount = 1,
				Recover = true
			}
		};
		fire3 = new CustomPiece(obj, true, val);
		fire3.Piece.m_icon = PrefabManager.Instance.GetPrefab("piece_brazierceiling01").gameObject.GetComponent<Piece>().m_icon;
		Piece component = fireFab3.GetComponent<Piece>();
		component.m_placeEffect = buildStone;
		WearNTear component2 = fireFab3.GetComponent<WearNTear>();
		component2.m_destroyedEffect = breakStone;
		Fireplace component3 = fireFab3.GetComponent<Fireplace>();
		component3.m_fuelAddedEffects = hearthAddFuel;
		component3.m_fuelItem = PrefabManager.Instance.GetPrefab("Wood").gameObject.GetComponent<ItemDrop>();
		fireVol = fireFab3.GetComponentInChildren<AudioSource>();
		PieceManager.Instance.AddPiece(fire3);
	}

	public void OnCookingStationCookItem(ref bool __result)
	{
		LogDebug($"__result : {__result}");
		if (__result && CookingSkillEnable.Value)
		{
			RaiseCookingSkill();
		}
	}

	private void RaiseCookingSkill()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		PrintCookingSkillInfo();
		((Character)Player.m_localPlayer).RaiseSkill(rkCookingSkill, 1f);
		LogDebug("Cooking Skill Raised");
		PrintCookingSkillInfo();
	}

	[Conditional("DEBUG")]
	private void PrintCookingSkillInfo()
	{
		object arg = ((Character)Player.m_localPlayer).GetSkills().GetSkillList().FirstOrDefault(delegate(Skills.Skill s)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return s.m_info.m_skill == rkCookingSkill;
		})?.m_level ?? 0f;
		Skills.Skill value = ((Character)Player.m_localPlayer).GetSkills().GetSkillList().FirstOrDefault(delegate(Skills.Skill s)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return s.m_info.m_skill == rkCookingSkill;
		});
		string text = $"[Skill Level Info] Current Level: {arg} ({((value != null) ? value.GetLevelPercentage() : 0f) * 100f}%), ";
		object arg2 = ((Character)Player.m_localPlayer).GetSkills().GetSkillList().FirstOrDefault(delegate(Skills.Skill s)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return s.m_info.m_skill == rkCookingSkill;
		})?.m_accumulator ?? 0f;
		Skills.Skill value2 = ((Character)Player.m_localPlayer).GetSkills().GetSkillList().FirstOrDefault(delegate(Skills.Skill s)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			return s.m_info.m_skill == rkCookingSkill;
		});
		float nextLevelRequirement = value2 != null ? (float)AccessTools.Method(typeof(Skills.Skill), "GetNextLevelRequirement").Invoke(value2, null) : 0f;
        LogDebug(text + $"Next Level: {arg2}/{nextLevelRequirement}");
	}

	private bool IsValidCookingCraftingStation(string currentCraftingStationName)
	{
		LogDebug("currentCraftingStationName : " + currentCraftingStationName);
		switch (currentCraftingStationName)
		{
		case "rk_griddle(Clone)":
		case "rk_grill(Clone)":
		case "rk_prep(Clone)":
		case "piece_cauldron(Clone)":
			return true;
		default:
			return false;
		}
	}

	private bool IsFromCrafting(long crafterID, string crafterName)
	{
		return !string.IsNullOrEmpty(crafterName) && crafterID >= 1;
	}

	private bool IsConsumable(string prefabName)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Invalid comparison between Unknown and I4
		GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(prefabName);
		if ((Object)(object)itemPrefab == (Object)null)
		{
			return false;
		}
		ItemDrop component = itemPrefab.GetComponent<ItemDrop>();
		if ((Object)(object)component == (Object)null)
		{
			return false;
		}
		return (int)component.m_itemData.m_shared.m_itemType == 2;
	}

	public void OnInventoryAddItemPostFix(string itemName, int stack, int quality, int variant, long crafterID, string crafterName)
	{
		if (_isAddingExtraItem)
		{
			return;
		}
		LogDebug($"itemName: {itemName}, crafterID: {crafterID}, crafterName: {crafterName}");
		LogDebug($"CookingSkillEnable.Value : {CookingSkillEnable?.Value}");
		ConfigEntry<bool> cookingSkillEnable = CookingSkillEnable;
		if ((cookingSkillEnable != null && !cookingSkillEnable.Value) || !IsFromCrafting(crafterID, crafterName) || !IsConsumable(itemName))
		{
			return;
		}
		CraftingStation currentCraftingStation = Player.m_localPlayer.GetCurrentCraftingStation();
		if (!IsValidCookingCraftingStation((currentCraftingStation != null) ? ((Object)currentCraftingStation).name : null))
		{
			return;
		}
		LogDebug($"BonusWhenCookingEnabled.Value : {BonusWhenCookingEnabled?.Value}");
		ConfigEntry<bool> bonusWhenCookingEnabled = BonusWhenCookingEnabled;
		if (bonusWhenCookingEnabled != null && bonusWhenCookingEnabled.Value)
		{
			float num = ((Character)Player.m_localPlayer).GetSkills().GetSkillList().FirstOrDefault(delegate(Skills.Skill s)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				return s.m_info.m_skill == rkCookingSkill;
			})?.m_level ?? 0f;
			LogDebug($"skillLevel : {num}");
			if (IsCrafterLucky(num))
			{
				LogDebug("[1][Start] -------------- ");
				AddExtraItem(itemName);
				LogDebug("[1][End] ---------------- ");
			}
			if (num > 25f && IsCrafterLucky(num / 4f))
			{
				LogDebug("[2][Start] -------------- ");
				AddExtraItem(itemName);
				LogDebug("[2][End] ---------------- ");
			}
		}
		if (CookingSkillEnable.Value)
		{
			RaiseCookingSkill();
		}
	}

	private void AddExtraItem(string itemName)
	{
		GameObject itemPrefab = ObjectDB.instance.GetItemPrefab(itemName);
		if (((Humanoid)Player.m_localPlayer).GetInventory().CanAddItem(itemPrefab, 1))
		{
			LogDebug("Trying to add extra item: " + itemName);
			AddItem(itemName);
			LogDebug("Added extra item: " + itemName);
		}
	}

	private void AddItem(string itemName)
	{
		_isAddingExtraItem = true;
		((Humanoid)Player.m_localPlayer).GetInventory().AddItem(itemName, 1, 1, 0, Player.m_localPlayer.GetPlayerID(), Player.m_localPlayer.GetPlayerName(), false, false);
		_isAddingExtraItem = false;
	}

	private bool IsCrafterLucky(float skillLevel)
	{
		if (skillLevel < 1f)
		{
			return false;
		}
		int num = Random.Range(1, 100);
		LogDebug($"Skill Level: {skillLevel} - Rand: {num}");
		LogDebug($"rand < skillLevel : {(float)num < skillLevel}");
		return (float)num < skillLevel;
	}

	[Conditional("DEBUG")]
	public static void LogDebug(string msg)
	{
		Jotunn.Logger.LogDebug((object)msg);
	}
}
