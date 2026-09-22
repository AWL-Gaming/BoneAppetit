using System;
using System.Linq;
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
[HarmonyPatch(typeof(Player), "TestGhostClipping")]
internal static class GrillPlacementClippingPatch
{
    private static void Prefix(UnityEngine.GameObject ghost, out UnityEngine.Collider[] __state)
    {
        __state = Array.Empty<UnityEngine.Collider>();
        if (ghost == null) return;

        __state = ghost.GetComponentsInChildren<UnityEngine.Collider>(true)
            .Where(collider => collider != null && collider.enabled && collider.gameObject.name == "BoneAppetitInteraction")
            .ToArray();

        foreach (UnityEngine.Collider collider in __state)
        {
            collider.enabled = false;
        }
    }

    private static Exception Finalizer(Exception __exception, UnityEngine.Collider[] __state)
    {
        if (__state != null)
        {
            foreach (UnityEngine.Collider collider in __state)
            {
                if (collider != null) collider.enabled = true;
            }
        }

        return __exception;
    }
}
