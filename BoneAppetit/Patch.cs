using System;
using Boneappetit;
using HarmonyLib;
using Jotunn;
using UnityEngine;
using Object = UnityEngine.Object;
using Logger = Jotunn.Logger;

namespace BoneAppetit;

public static class Patch
{
	[HarmonyPatch(typeof(CookingStation))]
	public static class PatchCookingStation
	{
		[HarmonyPostfix]
		[HarmonyPatch("CookItem")]
		[HarmonyPriority(400)]
				public static void Postfix(ref bool __result)
		{
			try
			{
				Boneappetit.BoneAppetit.Instance.OnCookingStationCookItem(ref __result);
			}
			catch (Exception ex)
			{
				Logger.LogError((object)ex);
			}
		}
	}

	[HarmonyPatch(typeof(Inventory), "AddItem", new Type[]
    {
        typeof(string), typeof(int), typeof(int), typeof(int), typeof(long), typeof(string), typeof(bool), typeof(bool)
    })]
    public static class PatchInventory
	{
		[HarmonyPostfix]
		[HarmonyPriority(400)]
				public static void Postfix(string name, int stack, int quality, int variant, long crafterID, string crafterName, bool cheated, bool pickedUp)
		{
			try
			{
				Logger.LogDebug((object)"PatchInventoryPostfix");
				if ((Object)(object)Player.m_localPlayer == (Object)null)
				{
					Logger.LogDebug((object)"Player is null");
				}
				else
				{
					Boneappetit.BoneAppetit.Instance.OnInventoryAddItemPostFix(name, stack, quality, variant, crafterID, crafterName);
				}
			}
			catch (Exception ex)
			{
				Logger.LogError((object)ex);
			}
		}
	}
}
