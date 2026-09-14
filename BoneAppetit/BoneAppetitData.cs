using System;

namespace Boneappetit;

internal sealed class RequirementDefinition
{
    internal RequirementDefinition(string item, int amount, bool recover = false)
    {
        Item = item;
        Amount = amount;
        Recover = recover;
    }

    internal string Item { get; }
    internal int Amount { get; }
    internal bool Recover { get; }
}

internal sealed class RecipeDefinition
{
    internal RecipeDefinition(string station, int stationLevel, int amount, params RequirementDefinition[] requirements)
    {
        Station = station;
        StationLevel = stationLevel;
        Amount = amount;
        Requirements = requirements;
    }

    internal string Station { get; }
    internal int StationLevel { get; }
    internal int Amount { get; }
    internal RequirementDefinition[] Requirements { get; }
}

internal sealed class ItemDefinition
{
    internal string Prefab { get; set; }
    internal string BasePrefab { get; set; }
    internal string Name { get; set; }
    internal string Description { get; set; }
    internal ItemDrop.ItemData.ItemType ItemType { get; set; }
    internal int StackSize { get; set; }
    internal float Weight { get; set; }
    internal float Food { get; set; }
    internal float Stamina { get; set; }
    internal float Eitr { get; set; }
    internal float BurnTime { get; set; }
    internal float Regen { get; set; }
    internal float Armor { get; set; }
    internal float ArmorPerLevel { get; set; }
    internal RecipeDefinition Recipe { get; set; }
    internal Func<BoneAppetit, bool> Enabled { get; set; }
    internal ConeEffect ConeEffect { get; set; }
}

internal enum ConeEffect
{
    None,
    Frost,
    Fire,
    Lightning
}

internal static class BoneAppetitData
{
    internal static readonly ItemDefinition[] Items =
    {
        Food("rk_icecream", "Ice Cream", "So cold you feel like you could walk through fire.", 20, 0.5f, 60f, 70f, 1600f, 3f,
            new RecipeDefinition("rk_prep", 0, 2, Req("FreezeGland", 4), Req("Blueberries", 8), Req("Honey", 2), Req("rk_dragonegg", 1)), p => p.ConesEnable.Value, ConeEffect.Frost),
        Food("rk_firecream", "Fire Cream", "Ghost peppers got nothin' on this. You ready to be the burn?", 20, 0.5f, 60f, 70f, 1600f, 3f,
            new RecipeDefinition("rk_prep", 0, 2, Req("SurtlingCore", 4), Req("Raspberry", 8), Req("Honey", 2), Req("rk_dragonegg", 2)), p => p.ConesEnable.Value, ConeEffect.Fire),
        Food("rk_electriccream", "Electric Cream Cone", "That will wake you up, put pep in your step, and zap that poision's", 10, 0.5f, 60f, 70f, 1600f, 3f,
            new RecipeDefinition("rk_prep", 0, 2, Req("Crystal", 4), Req("Cloudberry", 8), Req("Honey", 2), Req("rk_dragonegg", 2)), p => p.ConesEnable.Value, ConeEffect.Lightning),
        Food("rk_acidcream", "Acid Cream", "Wait, this has WHAT in it?", 20, 0.5f, 60f, 70f, 1600f, 3f,
            new RecipeDefinition("rk_prep", 0, 2, Req("Guck", 4), Req("MushroomYellow", 8), Req("Honey", 2), Req("rk_dragonegg", 2)), p => p.ConesEnable.Value, ConeEffect.Lightning),
        Food("rk_porkrind", "Pork Rinds", "Fried WHAT now you say?!", 20, 0.5f, 35f, 35f, 1200f, 2f,
            new RecipeDefinition("rk_griddle", 0, 1, Req("LeatherScraps", 1), Req("rk_pork", 1)), p => p.PorkRindEnable.Value),
        Food("rk_kabob", "Kabob", "Bone Appetit good Viking!", 10, 0.5f, 50f, 55f, 1600f, 3f,
            new RecipeDefinition("rk_grill", 0, 1, Req("Turnip", 1), Req("Carrot", 2), Req("RawMeat", 1), Req("BoneFragments", 2)), p => p.KabobEnable.Value),
        Food("rk_friedloxmeat", "Chicken Fried Lox Meat", "You know I like my chicken fried... Chicken Fried Lox that is.", 10, 1f, 80f, 80f, 2400f, 4f,
            new RecipeDefinition("rk_grill", 0, 1, Req("LoxMeat", 2), Req("BarleyFlour", 2), Req("rk_egg", 1), Req("rk_butter", 2)), p => p.FriedLoxEnable.Value),
        Food("rk_glazedcarrots", "Honey Glazed Carrots", "What's up doc?", 20, 0.5f, 40f, 40f, 1600f, 3f,
            new RecipeDefinition("rk_griddle", 0, 1, Req("Carrot", 3), Req("Honey", 2), Req("Dandelion", 2)), p => p.GlazedCarrotEnable.Value),
        Food("rk_bacon", "Bacon", "It's bacon me crazy!", 20, 0.2f, 40f, 30f, 1200f, 3f,
            new RecipeDefinition("rk_griddle", 0, 2, Req("rk_pork", 2)), p => p.BaconEnable.Value),
        Food("rk_smokedfish", "SmokedFish", "Here fishy fishy fishy! Jump on in to my smoker now please.", 10, 0.5f, 50f, 50f, 1600f, 3f,
            new RecipeDefinition("rk_griddle", 0, 1, Req("FishRaw", 1)), p => p.SmokedFishEnable.Value),
        Food("rk_pancake", "Pancakes", "The civilized form of flappers.", 20, 0.5f, 80f, 80f, 2400f, 4f,
            new RecipeDefinition("rk_grill", 2, 1, Req("Honey", 2), Req("BarleyFlour", 3), Req("rk_butter", 5), Req("rk_egg", 2)), p => p.PancakesEnable.Value),
        Food("rk_pizza", "Pizza", "We must have all the things, then we add bread.", 10, 0.5f, 100f, 50f, 2400f, 3f,
            new RecipeDefinition("rk_grill", 2, 1, Req("Mushroom", 2), Req("BarleyFlour", 3), Req("rk_egg", 2), Req("RawMeat", 2)), p => p.PizzaEnable.Value),
        Food("rk_coffee", "Coffee", "Coffee. Delicious however you spell it.", 10, 0.5f, 25f, 60f, 900f, 5f,
            new RecipeDefinition("rk_prep", 0, 1, Req("AncientSeed", 2)), p => p.CoffeeEnable.Value),
        Food("rk_latte", "Spice Latte", "A special drink for a very special viking!", 10, 0.5f, 50f, 100f, 1200f, 6f,
            new RecipeDefinition("rk_prep", 0, 2, Req("Crystal", 2), Req("Barley", 2), Req("Honey", 10)), p => p.LatteEnable.Value),
        Food("rk_porridge", "Porridge", "Not too hot, not too cold, this porridge is just right!", 10, 0.5f, 80f, 80f, 2400f, 3f,
            new RecipeDefinition("rk_grill", 2, 1, Req("Barley", 2), Req("Cloudberry", 4), Req("Honey", 2), Req("rk_butter", 1)), p => p.PorridgeEnable.Value),
        Food("rk_pbj", "Jimmy's PBJ", "A favorite of every young Viking!", 20, 0.5f, 80f, 80f, 2400f, 4f,
            new RecipeDefinition("rk_grill", 1, 4, Req("Bread", 1), Req("QueensJam", 1), Req("rk_nut_ella", 4)), p => p.PBJEnable.Value),
        Food("rk_birthday", "Birthday Cake", "Enjoy some cake in celebration of RK's birthday!", 10, 1f, 80f, 80f, 2400f, 4f,
            new RecipeDefinition("rk_grill", 2, 1, Req("BarleyFlour", 2), Req("Honey", 4), Req("Cloudberry", 4), Req("rk_egg", 2)), p => p.CakeEnable.Value),
        Food("rk_haggis", "Haggis", "Hagis", 10, 0.5f, 55f, 50f, 1600f, 3f,
            new RecipeDefinition("rk_prep", 0, 1, Req("RawMeat", 1), Req("Carrot", 2), Req("Entrails", 2), Req("Turnip", 2)), p => p.HaggisEnable.Value),
        Food("rk_candiedturnip", "Candied Turnip", "Who doesn't love a good Viking desert?", 10, 1f, 40f, 60f, 1600f, 4f,
            new RecipeDefinition("rk_grill", 0, 1, Req("Thistle", 1), Req("Honey", 2), Req("Turnip", 2)), p => p.CandiedTurnipEnable.Value),
        Food("rk_moochi", "Moochi", "Moochi time!", 10, 0.5f, 70f, 60f, 1800f, 3f,
            new RecipeDefinition("rk_prep", 0, 1, Req("rk_dragonegg", 1), Req("Honey", 2), Req("FreezeGland", 1), Req("Blueberries", 4)), p => p.MoochiEnable.Value),
        Food("rk_nut_ella", "Nut-Ella", "Delicious on everything!", 20, 1f, 10f, 10f, 900f, 4f,
            new RecipeDefinition("rk_prep", 0, 1, Req("BeechSeeds", 6), Req("rk_butter", 1)), p => p.Nut_EllaEnable.Value),
        Food("rk_burger", "Burger", "Little bit of this and a little bit of that, throw it on a bun", 10, 1f, 80f, 80f, 2400f, 4f,
            new RecipeDefinition("rk_grill", 0, 2, Req("RawMeat", 2), Req("LoxMeat", 2), Req("Turnip", 2), Req("Bread", 1)), p => p.BurgerEnable.Value),
        Food("rk_omlette", "Omlette", "Healthy delicious omlette", 20, 0.5f, 50f, 50f, 2000f, 4f,
            new RecipeDefinition("rk_griddle", 0, 1, Req("rk_egg", 2), Req("Thistle", 2), Req("rk_pork", 1), Req("rk_butter", 1)), p => p.OmletteEnable.Value),
        Food("rk_broth", "Bone Broth", "Bone... broth, a great start to soups or stews.", 10, 1f, 10f, 10f, 900f, 4f,
            new RecipeDefinition("rk_prep", 0, 1, Req("BoneFragments", 2), Req("rk_butter", 1)), p => p.BrothEnable.Value),
        Food("rk_fishstew", "Fish Stew", "Yummy fishy stew.", 20, 0.5f, 50f, 50f, 1600f, 4f,
            new RecipeDefinition("rk_prep", 0, 1, Req("rk_broth", 1), Req("FishRaw", 2), Req("Thistle", 2), Req("rk_egg", 2)), p => p.FishStewEnable.Value),
        Food("rk_butter", "Carrot Butter", "A nessessary addition to many meals", 30, 0.5f, 10f, 10f, 900f, 4f,
            new RecipeDefinition("rk_prep", 0, 2, Req("CarrotSeeds", 8)), p => p.ButterEnable.Value),
        Food("rk_bloodsausage", "Blood Sausage", "What's better than sausage? Blood Sausage!", 20, 0.5f, 50f, 50f, 1600f, 4f,
            new RecipeDefinition("rk_grill", 0, 2, Req("Entrails", 2), Req("Bloodbag", 1), Req("Thistle", 2), Req("rk_pork", 2)), p => p.BloodSausageEnable.Value),
        Food("rk_boiledegg", "Boiled Egg", "A much smaller egg that you find in a nest, maybe you can carry", 20, 0.5f, 40f, 50f, 900f, 4f,
            new RecipeDefinition("rk_prep", 0, 1, Req("rk_egg", 2)), p => p.BoiledEggEnable.Value),
        Food("rk_carrotsticks", "Carrot Sticks", "What's up doc?", 20, 0.5f, 40f, 40f, 1600f, 3f,
            new RecipeDefinition("rk_prep", 0, 1, Req("Carrot", 2), Req("rk_nut_ella", 1)), p => p.CarrotSticksEnable.Value),
        Food("rk_mead", "Mead", "What does the glow mean? Good stuff!", 20, 1f, 50f, 50f, 1600f, 8f,
            new RecipeDefinition("rk_prep", 1, 1, Req("Barley", 3), Req("Honey", 4)), p => p.MeadEnable.Value),
        ChefHat(),
        Material("rk_pork", "Raw Pork", "Raw Pork who'd have thought?", 20, 1f),
        Material("rk_egg", "Egg", "A small blue, seagull egg.", 20, 0.5f),
        Material("rk_dragonegg", "Drake Egg", "A much smaller egg that you find in a nest, maybe you can carry", 20, 0.5f)
    };

    private static ItemDefinition Food(string prefab, string name, string description, int stack, float weight, float food, float stamina, float burnTime, float regen, RecipeDefinition recipe, Func<BoneAppetit, bool> enabled, ConeEffect effect = ConeEffect.None)
    {
        return new ItemDefinition
        {
            Prefab = prefab,
            BasePrefab = "CookedMeat",
            Name = name,
            Description = description,
            ItemType = ItemDrop.ItemData.ItemType.Consumable,
            StackSize = stack,
            Weight = weight,
            Food = food,
            Stamina = stamina,
            Eitr = 0f,
            BurnTime = burnTime,
            Regen = regen,
            Recipe = recipe,
            Enabled = enabled,
            ConeEffect = effect
        };
    }

    private static ItemDefinition Material(string prefab, string name, string description, int stack, float weight)
    {
        return new ItemDefinition
        {
            Prefab = prefab,
            BasePrefab = "RawMeat",
            Name = name,
            Description = description,
            ItemType = ItemDrop.ItemData.ItemType.Material,
            StackSize = stack,
            Weight = weight,
            Food = 0f,
            Stamina = 0f,
            Eitr = 0f,
            BurnTime = 0f,
            Regen = 0f,
            Recipe = null,
            Enabled = _ => true
        };
    }

    private static ItemDefinition ChefHat()
    {
        return new ItemDefinition
        {
            Prefab = "rk_chef",
            BasePrefab = "HelmetLeather",
            Name = "Chef Hat",
            Description = "Improves Cooking Skill XP Earned.",
            ItemType = ItemDrop.ItemData.ItemType.Helmet,
            StackSize = 1,
            Weight = 0.5f,
            Food = 0f,
            Stamina = 0f,
            Eitr = 0f,
            BurnTime = 0f,
            Regen = 0f,
            Armor = 1f,
            ArmorPerLevel = 1f,
            Recipe = new RecipeDefinition(string.Empty, 0, 1, Req("Dandelion", 5)),
            Enabled = p => p.CheffHatEnable.Value
        };
    }

    private static RequirementDefinition Req(string item, int amount)
    {
        return new RequirementDefinition(item, amount);
    }
}
