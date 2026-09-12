using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class ShaderReplacerNew : MonoBehaviour
{
	[Tooltip("Use this Field For Normal Renderers")]
	[SerializeField]
	internal Renderer[] _renderers = null;

	[SerializeField]
	internal ShaderType _shaderType = ShaderType.Creature;

	[SerializeField]
	internal bool DebugOutput = false;

	private void Awake()
	{
		if (IsHeadlessMode() || _renderers.Length == 0 || !((Component)this).gameObject.activeInHierarchy)
		{
			return;
		}
		Renderer[] renderers = _renderers;
		foreach (Renderer val in renderers)
		{
			if ((Object)(object)val == (Object)null)
			{
				continue;
			}
			Material[] sharedMaterials = val.sharedMaterials;
			foreach (Material val2 in sharedMaterials)
			{
				if ((Object)(object)val2 == (Object)null)
				{
					((Component)val).gameObject.SetActive(false);
				}
				else
				{
					val2.shader = Shader.Find(ReturnEnumString(_shaderType));
				}
			}
		}
	}

	internal string ReturnEnumString(ShaderType shaderchoice)
	{
		string result = "";
		switch (shaderchoice)
		{
		case ShaderType.Alpha:
			result = "Custom/AlphaParticle";
			break;
		case ShaderType.Blob:
			result = "Custom/Blob";
			break;
		case ShaderType.Bonemass:
			result = "Custom/Bonemass";
			break;
		case ShaderType.Clouds:
			result = "Custom/Clouds";
			break;
		case ShaderType.Creature:
			result = "Custom/Creature";
			break;
		case ShaderType.Decal:
			result = "Custom/Decal";
			break;
		case ShaderType.Distortion:
			result = "Custom/Distortion";
			break;
		case ShaderType.Flow:
			result = "Custom/Flow";
			break;
		case ShaderType.FlowOpaque:
			result = "Custom/FlowOpaque";
			break;
		case ShaderType.Grass:
			result = "Custom/Grass";
			break;
		case ShaderType.GuiScroll:
			result = "Custom/GuiScroll";
			break;
		case ShaderType.HeightMap:
			result = "Custom/HeightMap";
			break;
		case ShaderType.Icon:
			result = "Custom/Icon";
			break;
		case ShaderType.InteriorSide:
			result = "Custom/InteriorSide";
			break;
		case ShaderType.LitGui:
			result = "Custom/LitGui";
			break;
		case ShaderType.LitParticles:
			result = "Lux Lit Particles/ Bumped";
			break;
		case ShaderType.MapShader:
			result = "Custom/mapshader";
			break;
		case ShaderType.ParticleDetail:
			result = "Custom/ParticleDecal";
			break;
		case ShaderType.Piece:
			result = "Custom/Piece";
			break;
		default:
			throw new ArgumentOutOfRangeException("shaderchoice", shaderchoice, null);
		case ShaderType.Player:
		case ShaderType.Rug:
		case ShaderType.ShadowBlob:
		case ShaderType.SkyboxProcedural:
		case ShaderType.SkyObject:
		case ShaderType.StaticRock:
		case ShaderType.Tar:
		case ShaderType.TrilinearMap:
		case ShaderType.BGBlur:
		case ShaderType.Water:
		case ShaderType.WaterBottom:
		case ShaderType.WaterMask:
		case ShaderType.Yggdrasil:
		case ShaderType.YggdrasilRoot:
		case ShaderType.ToonDeferredShading2017:
			break;
		}
		return result;
	}

	public static bool IsHeadlessMode()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		return (int)SystemInfo.graphicsDeviceType == 4;
	}
}
