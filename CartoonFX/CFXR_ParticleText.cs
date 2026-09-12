using System;
using UnityEngine;
using MainModule = UnityEngine.ParticleSystem.MainModule;
using MinMaxCurve = UnityEngine.ParticleSystem.MinMaxCurve;
using MinMaxGradient = UnityEngine.ParticleSystem.MinMaxGradient;
using TextureSheetAnimationModule = UnityEngine.ParticleSystem.TextureSheetAnimationModule;
using CustomDataModule = UnityEngine.ParticleSystem.CustomDataModule;
using Object = UnityEngine.Object;

namespace CartoonFX;

[RequireComponent(typeof(ParticleSystem))]
public class CFXR_ParticleText : MonoBehaviour
{
	[Header("Dynamic")]
	[Tooltip("Allow changing the text at runtime with the 'UpdateText' method. If disabled, this script will be excluded from the build.")]
	public bool isDynamic;

	[Header("Text")]
	[SerializeField]
	private string text;

	[SerializeField]
	private float size;

	[SerializeField]
	private float letterSpacing;

	[Header("Colors")]
	[SerializeField]
	private Color backgroundColor;

	[SerializeField]
	private Color color1;

	[SerializeField]
	private Color color2;

	[Header("Delay")]
	[SerializeField]
	private float delay;

	[SerializeField]
	private bool cumulativeDelay;

	[Range(0f, 2f)]
	[SerializeField]
	private float compensateLifetime;

	[Header("Misc")]
	[SerializeField]
	private float lifetimeMultiplier;

	[Range(-90f, 90f)]
	[SerializeField]
	private float rotation;

	[SerializeField]
	private float sortingFudgeOffset;

	[SerializeField]
	private CFXR_ParticleTextFontAsset font;

	private float baseLifetime;

	private float baseScaleX;

	private float baseScaleY;

	private float baseScaleZ;

	private Vector3 basePivot;

	private void Awake()
	{
		if (!isDynamic)
		{
			Object.Destroy((Object)(object)this);
		}
		else
		{
			InitializeFirstParticle();
		}
	}

	private void InitializeFirstParticle()
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (isDynamic && ((Component)this).transform.childCount == 0)
		{
			throw new Exception("[CFXR_ParticleText] A disabled GameObject with a ParticleSystem component is required as the first child when 'isDyanmic' is enabled, so that its settings can be used as a base for the generated characters.");
		}
		ParticleSystem val = (isDynamic ? ((Component)((Component)this).transform.GetChild(0)).GetComponent<ParticleSystem>() : ((Component)this).GetComponent<ParticleSystem>());
		MainModule main = val.main;
		MinMaxCurve startLifetime = main.startLifetime;
		baseLifetime = startLifetime.constant;
		baseScaleX = main.startSizeXMultiplier;
		baseScaleY = main.startSizeYMultiplier;
		baseScaleZ = main.startSizeZMultiplier;
		basePivot = ((Component)val).GetComponent<ParticleSystemRenderer>().pivot;
		if (isDynamic)
		{
			basePivot.x = 0f;
			((Component)val).gameObject.SetActive(false);
			((Object)((Component)val).gameObject).name = "MODEL";
		}
	}

	public void UpdateText(string newText = null, float? newSize = null, Color? newColor1 = null, Color? newColor2 = null, Color? newBackgroundColor = null, float? newLifetimeMultiplier = null)
	{
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0504: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_057b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0625: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_064a: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isPlaying && !isDynamic)
		{
			throw new Exception("[CFXR_ParticleText] You cannot update the text at runtime if it's not marked as dynamic.");
		}
		if (newText != null)
		{
			switch (font.letterCase)
			{
			case CFXR_ParticleTextFontAsset.LetterCase.Lower:
				newText = newText.ToLowerInvariant();
				break;
			case CFXR_ParticleTextFontAsset.LetterCase.Upper:
				newText = newText.ToUpperInvariant();
				break;
			}
			string text = newText;
			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];
				if (!char.IsWhiteSpace(c) && font.CharSequence.IndexOf(c) < 0)
				{
					throw new Exception("[CFXR_ParticleText] Invalid character supplied for the dynamic text: '" + c + "'\nThe allowed characters from the selected font are: " + font.CharSequence);
				}
			}
			this.text = newText;
		}
		if (newSize.HasValue)
		{
			size = newSize.Value;
		}
		if (newColor1.HasValue)
		{
			color1 = newColor1.Value;
		}
		if (newColor2.HasValue)
		{
			color2 = newColor2.Value;
		}
		if (newBackgroundColor.HasValue)
		{
			backgroundColor = newBackgroundColor.Value;
		}
		if (newLifetimeMultiplier.HasValue)
		{
			lifetimeMultiplier = newLifetimeMultiplier.Value;
		}
		if (this.text == null || (Object)(object)font == (Object)null || !font.IsValid())
		{
			return;
		}
		if (((Component)this).transform.childCount == 0)
		{
			throw new Exception("[CFXR_ParticleText] A disabled GameObject with a ParticleSystem component is required as the first child when 'isDyanmic' is enabled, so that its settings can be used as a base for the generated characters.");
		}
		float num = 0f;
		int num2 = 0;
		Rect rect;
		for (int j = 0; j < this.text.Length; j++)
		{
			if (char.IsWhiteSpace(this.text[j]))
			{
				if (j > 0)
				{
					num += letterSpacing * size;
				}
				continue;
			}
			num2++;
			if (j > 0)
			{
				int num3 = font.CharSequence.IndexOf(this.text[j]);
				Sprite val = font.CharSprites[num3];
				rect = val.rect;
				float num4 = rect.width + font.CharKerningOffsets[num3].post + font.CharKerningOffsets[num3].pre;
				num += (num4 * 0.01f + letterSpacing) * size;
			}
		}
		if (num2 > 0)
		{
			int num5 = ((Component)this).transform.childCount - (isDynamic ? 1 : 0);
			if (num5 < num2)
			{
				GameObject val2 = (isDynamic ? ((Component)((Component)this).transform.GetChild(0)).gameObject : null);
				for (int k = num5; k < num2; k++)
				{
					GameObject val3 = (GameObject)(isDynamic ? ((object)Object.Instantiate<GameObject>(val2, ((Component)this).transform)) : ((object)new GameObject()));
					if (!isDynamic)
					{
						val3.transform.SetParent(((Component)this).transform);
						val3.AddComponent<ParticleSystem>();
					}
					val3.transform.localPosition = Vector3.zero;
					val3.transform.localRotation = Quaternion.identity;
				}
			}
			float num6 = num / 2f;
			num = 0f;
			int num7 = ((!isDynamic) ? (-1) : 0);
			ParticleSystem val4 = (isDynamic ? null : ((Component)this).GetComponent<ParticleSystem>());
			ParticleSystemRenderer component = ((Component)this).GetComponent<ParticleSystemRenderer>();
			for (int l = 0; l < this.text.Length; l++)
			{
				char c2 = this.text[l];
				if (char.IsWhiteSpace(c2))
				{
					num += letterSpacing * size;
					continue;
				}
				num7++;
				int num8 = font.CharSequence.IndexOf(this.text[l]);
				Sprite val5 = font.CharSprites[num8];
				float num9 = size;
				rect = val5.rect;
				float num10 = num9 * rect.width / 50f;
				num += font.CharKerningOffsets[num8].pre * 0.01f * size;
				float num11 = (num - num6) / num10;
				rect = val5.rect;
				float num12 = rect.width + font.CharKerningOffsets[num8].post;
				num += (num12 * 0.01f + letterSpacing) * size;
				GameObject gameObject = ((Component)((Component)this).transform.GetChild(num7)).gameObject;
				((Object)gameObject).name = c2.ToString();
				ParticleSystem component2 = gameObject.GetComponent<ParticleSystem>();
				MainModule main = component2.main;
				main.startSizeXMultiplier = baseScaleX * num10;
				main.startSizeYMultiplier = baseScaleY * num10;
				main.startSizeZMultiplier = baseScaleZ * num10;
				TextureSheetAnimationModule textureSheetAnimation = component2.textureSheetAnimation;
				textureSheetAnimation.SetSprite(0, val5);
				main.startRotation = (float)Math.PI / 180f * rotation;
				main.startColor = backgroundColor;
				CustomDataModule customData = component2.customData;
				customData.enabled = true;
				customData.SetColor((ParticleSystemCustomData)0, color1);
				customData.SetColor((ParticleSystemCustomData)1, color2);
				if (cumulativeDelay)
				{
					main.startDelay = delay * (float)l;
					main.startLifetime = Mathf.LerpUnclamped(baseLifetime, baseLifetime + delay * (float)(this.text.Length - l), compensateLifetime / lifetimeMultiplier);
				}
				else
				{
					main.startDelay = delay;
				}
				MinMaxCurve startLifetime = main.startLifetime;
				main.startLifetime = startLifetime.constant * lifetimeMultiplier;
				ParticleSystemRenderer component3 = ((Component)component2).GetComponent<ParticleSystemRenderer>();
				((Renderer)component3).enabled = true;
				component3.pivot = new Vector3(basePivot.x + num11, basePivot.y, basePivot.z);
				component3.sortingFudge += (float)l * sortingFudgeOffset;
			}
		}
		int m = 1;
		for (int childCount = ((Component)this).transform.childCount; m < childCount; m++)
		{
			((Component)((Component)this).transform.GetChild(m)).gameObject.SetActive(m <= num2);
		}
	}

	public CFXR_ParticleText()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		size = 1f;
		letterSpacing = 0.44f;
		backgroundColor = new Color(0f, 0f, 0f, 1f);
		color1 = new Color(1f, 1f, 1f, 1f);
		color2 = new Color(0f, 0f, 1f, 1f);
		delay = 0.05f;
		cumulativeDelay = false;
		compensateLifetime = 0f;
		lifetimeMultiplier = 1f;
		rotation = -5f;
		sortingFudgeOffset = 0.1f;
		
	}
}
