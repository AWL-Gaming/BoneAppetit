namespace Boneappetit;

public sealed class SE_CheffHat : SE_Stats
{
    private void OnEnable()
    {
        Configure();
    }

    public override void Setup(Character character)
    {
        Configure();
        base.Setup(character);
    }

    private void Configure()
    {
        BoneAppetit plugin = BoneAppetit.Instance;
        if (plugin == null)
        {
            return;
        }

        name = "rkCookingSkillStatusEffect";
        m_name = "Cooking Bonus";
        m_raiseSkill = plugin.rkCookingSkill;
        m_raiseSkillModifier = plugin.HatXpGain.Value;
        m_startMessageType = (MessageHud.MessageType)2;
        m_stopMessageType = (MessageHud.MessageType)2;

        if (plugin.HatSEMessage.Value)
        {
            m_startMessage = "You feel the spirit of Julia Child course through you!";
            m_stopMessage = RandomJuliaChildPhrase.GetRandomPhrase();
        }
        else
        {
            m_startMessage = string.Empty;
            m_stopMessage = string.Empty;
        }
    }
}
