using System;
using HarmonyLib;
using Jotunn;

namespace Boneappetit;

internal static class Patch
{
    [HarmonyPatch(typeof(CookingStation), "CookItem")]
    private static class CookingStationCookItemPatch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Normal)]
        private static void Postfix(bool __result)
        {
            try
            {
                BoneAppetit.Instance?.OnCookingStationCookItem(__result);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }

    [HarmonyPatch(typeof(Inventory), "AddItem", new Type[]
    {
        typeof(string), typeof(int), typeof(int), typeof(int), typeof(long), typeof(string), typeof(bool), typeof(bool)
    })]
    private static class InventoryAddItemPatch
    {
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Normal)]
        private static void Postfix(string name, int stack, int quality, int variant, long crafterID, string crafterName, bool cheated, bool pickedUp)
        {
            try
            {
                BoneAppetit.Instance?.OnInventoryAddItem(name, crafterID, crafterName);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}
