using UnityEngine;
using Object = UnityEngine.Object;

namespace Boneappetit;

public class SE_CheffHat : SE_Stats
{
	private void OnEnable()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		((Object)this).name = "rkCookingSkillStatusEffect";
		((StatusEffect)this).m_name = "Cooking Bonus";
		base.m_raiseSkill = BoneAppetit.Instance.rkCookingSkill;
		base.m_raiseSkillModifier = BoneAppetit.Instance.HatXpGain.Value;
		((StatusEffect)this).m_startMessageType = (MessageHud.MessageType)2;
		((StatusEffect)this).m_stopMessageType = (MessageHud.MessageType)2;
		SetMessages();
	}

	private void SetMessages()
	{
		if (!BoneAppetit.Instance.HatSEMessage.Value)
		{
			if (!string.IsNullOrEmpty(((StatusEffect)this).m_startMessage) || !string.IsNullOrEmpty(((StatusEffect)this).m_stopMessage))
			{
				((StatusEffect)this).m_startMessage = string.Empty;
				((StatusEffect)this).m_stopMessage = string.Empty;
			}
			return;
		}
		string startMessage = ((StatusEffect)this).m_startMessage;
		do
		{
			((StatusEffect)this).m_startMessage = RandomJuliaChildPhrase.GetRandomPhrase();
		}
		while (((StatusEffect)this).m_startMessage.Equals(((StatusEffect)this).m_stopMessage) || ((StatusEffect)this).m_startMessage.Equals(startMessage));
		startMessage = ((StatusEffect)this).m_stopMessage;
		do
		{
			((StatusEffect)this).m_stopMessage = RandomJuliaChildPhrase.GetRandomPhrase();
		}
		while (((StatusEffect)this).m_stopMessage.Equals(((StatusEffect)this).m_startMessage) || ((StatusEffect)this).m_stopMessage.Equals(startMessage));
	}

	public override void Setup(Character character)
	{
		SetMessages();
		((SE_Stats)this).Setup(character);
	}
}
