using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CartoonFX;

public class CFXR_ParticleTextFontAsset : ScriptableObject
{
	public enum LetterCase
	{
		Both,
		Upper,
		Lower
	}

	[Serializable]
	public class Kerning
	{
		public string name = "A";

		public float pre = 0f;

		public float post = 0f;
	}

	public string CharSequence = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!?-.#@$ ";

	public LetterCase letterCase = LetterCase.Upper;

	public Sprite[] CharSprites;

	public Kerning[] CharKerningOffsets;

	private void OnValidate()
	{
		((Object)this).hideFlags = (HideFlags)0;
		if (CharKerningOffsets == null || CharKerningOffsets.Length != CharSequence.Length)
		{
			CharKerningOffsets = new Kerning[CharSequence.Length];
			for (int i = 0; i < CharKerningOffsets.Length; i++)
			{
				CharKerningOffsets[i] = new Kerning
				{
					name = CharSequence[i].ToString()
				};
			}
		}
	}

	public bool IsValid()
	{
		bool flag = !string.IsNullOrEmpty(CharSequence) && CharSprites != null && CharSprites.Length == CharSequence.Length && CharKerningOffsets != null && CharKerningOffsets.Length == CharSprites.Length;
		if (!flag)
		{
			Debug.LogError((object)$"Invalid ParticleTextFontAsset: '{((Object)this).name}'\n", (Object)(object)this);
		}
		return flag;
	}
}
