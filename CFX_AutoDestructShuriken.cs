using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
	public bool OnlyDeactivate;

	private void OnEnable()
	{
		((MonoBehaviour)this).StartCoroutine("CheckIfAlive");
	}

	private IEnumerator CheckIfAlive()
	{
		ParticleSystem ps = ((Component)this).GetComponent<ParticleSystem>();
		while ((Object)(object)ps != (Object)null)
		{
			yield return (object)new WaitForSeconds(0.5f);
			if (!ps.IsAlive(true))
			{
				if (OnlyDeactivate)
				{
					((Component)this).gameObject.SetActive(false);
				}
				else
				{
					Object.Destroy((Object)(object)((Component)this).gameObject);
				}
				break;
			}
		}
	}
}
