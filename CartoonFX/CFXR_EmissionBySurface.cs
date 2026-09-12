using UnityEngine;

namespace CartoonFX;

[RequireComponent(typeof(ParticleSystem))]
public class CFXR_EmissionBySurface : MonoBehaviour
{
	public bool active = true;

	public float particlesPerUnit = 10f;

	[Tooltip("This is to avoid slowdowns in the Editor if the value gets too high")]
	public float maxEmissionRate = 5000f;

	[HideInInspector]
	public float density = 0f;

	private bool attachedToEditor = false;

	private ParticleSystem ps;
}
