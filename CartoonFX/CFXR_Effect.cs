using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using CameraCallback = UnityEngine.Camera.CameraCallback;
using Object = UnityEngine.Object;

namespace CartoonFX;

[RequireComponent(typeof(ParticleSystem))]
[DisallowMultipleComponent]
public class CFXR_Effect : MonoBehaviour
{
	[Serializable]
	public class CameraShake
	{
		public enum ShakeSpace
		{
			Screen,
			World
		}

		public static bool editorPreview = true;

		public bool enabled;

		[Space]
		public bool useMainCamera;

		public List<Camera> cameras;

		[Space]
		public float delay;

		public float duration;

		public ShakeSpace shakeSpace;

		public Vector3 shakeStrength;

		public AnimationCurve shakeCurve;

		[Space]
		[Range(0f, 0.1f)]
		public float shakesDelay;

		[NonSerialized]
		public bool isShaking;

		private Dictionary<Camera, Vector3> camerasPreRenderPosition;

		private Vector3 shakeVector;

		private float delaysTimer;

		private static bool s_CallbackRegistered;

		private static List<CameraShake> s_CameraShakes = new List<CameraShake>();

		private static void OnPreRenderCamera_Static(Camera cam)
		{
			int count = s_CameraShakes.Count;
			for (int i = 0; i < count; i++)
			{
				CameraShake cameraShake = s_CameraShakes[i];
				cameraShake.onPreRenderCamera(cam);
			}
		}

		private static void OnPostRenderCamera_Static(Camera cam)
		{
			int count = s_CameraShakes.Count;
			for (int num = count - 1; num >= 0; num--)
			{
				CameraShake cameraShake = s_CameraShakes[num];
				cameraShake.onPostRenderCamera(cam);
			}
		}

		private static void RegisterStaticCallback(CameraShake cameraShake)
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Expected O, but got Unknown
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Expected O, but got Unknown
			s_CameraShakes.Add(cameraShake);
			if (!s_CallbackRegistered)
			{
				Camera.onPreRender = (CameraCallback)Delegate.Combine((Delegate)(object)Camera.onPreRender, (Delegate)new CameraCallback(OnPreRenderCamera_Static));
				Camera.onPostRender = (CameraCallback)Delegate.Combine((Delegate)(object)Camera.onPostRender, (Delegate)new CameraCallback(OnPostRenderCamera_Static));
				s_CallbackRegistered = true;
			}
		}

		private static void UnregisterStaticCallback(CameraShake cameraShake)
		{
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Expected O, but got Unknown
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Expected O, but got Unknown
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Expected O, but got Unknown
			s_CameraShakes.Remove(cameraShake);
			if (s_CallbackRegistered && s_CameraShakes.Count == 0)
			{
				Camera.onPreRender = (CameraCallback)Delegate.Remove((Delegate)(object)Camera.onPreRender, (Delegate)new CameraCallback(OnPreRenderCamera_Static));
				Camera.onPostRender = (CameraCallback)Delegate.Remove((Delegate)(object)Camera.onPostRender, (Delegate)new CameraCallback(OnPostRenderCamera_Static));
				s_CallbackRegistered = false;
			}
		}

		private void onPreRenderCamera(Camera cam)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			if (!isShaking || !camerasPreRenderPosition.ContainsKey(cam))
			{
				return;
			}
			camerasPreRenderPosition[cam] = ((Component)cam).transform.localPosition;
			if (!(Time.timeScale <= 0f))
			{
				switch (shakeSpace)
				{
				case ShakeSpace.Screen:
				{
					Transform transform2 = ((Component)cam).transform;
					transform2.localPosition += ((Component)cam).transform.rotation * shakeVector;
					break;
				}
				case ShakeSpace.World:
				{
					Transform transform = ((Component)cam).transform;
					transform.localPosition += shakeVector;
					break;
				}
				}
			}
		}

		private void onPostRenderCamera(Camera cam)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			if (camerasPreRenderPosition.ContainsKey(cam))
			{
				((Component)cam).transform.localPosition = camerasPreRenderPosition[cam];
			}
		}

		public void fetchCameras()
		{
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			foreach (Camera camera in cameras)
			{
				if (!((Object)(object)camera == (Object)null))
				{
					camerasPreRenderPosition.Remove(camera);
				}
			}
			cameras.Clear();
			if (useMainCamera && (Object)(object)Camera.main != (Object)null)
			{
				cameras.Add(Camera.main);
			}
			foreach (Camera camera2 in cameras)
			{
				if (!((Object)(object)camera2 == (Object)null) && !camerasPreRenderPosition.ContainsKey(camera2))
				{
					camerasPreRenderPosition.Add(camera2, Vector3.zero);
				}
			}
		}

		public void StartShake()
		{
			if (isShaking)
			{
				StopShake();
			}
			isShaking = true;
			RegisterStaticCallback(this);
		}

		public void StopShake()
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			isShaking = false;
			shakeVector = Vector3.zero;
			UnregisterStaticCallback(this);
		}

		public void animate(float time)
		{
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00da: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_011a: Unknown result type (might be due to invalid IL or missing references)
			float num = duration + delay;
			if (time < num)
			{
				if (time < delay)
				{
					return;
				}
				if (!isShaking)
				{
					StartShake();
				}
				float num2 = Mathf.Clamp01(time / num);
				if (shakesDelay > 0f)
				{
					delaysTimer += Time.deltaTime;
					if (delaysTimer < shakesDelay)
					{
						return;
					}
					while (delaysTimer >= shakesDelay)
					{
						delaysTimer -= shakesDelay;
					}
				}
				Vector3 val = default(Vector3);
				val = new Vector3(Random.value, Random.value, Random.value);
				Vector3 val2 = Vector3.Scale(val, shakeStrength) * (float)((!(Random.value > 0.5f)) ? 1 : (-1));
				shakeVector = val2 * shakeCurve.Evaluate(num2) * 1f;
			}
			else if (isShaking)
			{
				StopShake();
			}
		}

		public CameraShake()
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			enabled = false;
			useMainCamera = true;
			cameras = new List<Camera>();
			delay = 0f;
			duration = 1f;
			shakeSpace = ShakeSpace.Screen;
			shakeStrength = new Vector3(0.1f, 0.1f, 0.1f);
			shakeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
			shakesDelay = 0f;
			camerasPreRenderPosition = new Dictionary<Camera, Vector3>();
			
		}
	}

	public enum ClearBehavior
	{
		None,
		Disable,
		Destroy
	}

	[Serializable]
	public class AnimatedLight
	{
		public static bool editorPreview = true;

		public Light light;

		public bool loop;

		public bool animateIntensity;

		public float intensityStart = 8f;

		public float intensityEnd = 0f;

		public float intensityDuration = 0.5f;

		public AnimationCurve intensityCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

		public bool perlinIntensity;

		public float perlinIntensitySpeed = 1f;

		public bool fadeIn;

		public float fadeInDuration = 0.5f;

		public bool fadeOut;

		public float fadeOutDuration = 0.5f;

		public bool animateRange;

		public float rangeStart = 8f;

		public float rangeEnd = 0f;

		public float rangeDuration = 0.5f;

		public AnimationCurve rangeCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

		public bool perlinRange;

		public float perlinRangeSpeed = 1f;

		public bool animateColor;

		public Gradient colorGradient;

		public float colorDuration = 0.5f;

		public AnimationCurve colorCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

		public bool perlinColor;

		public float perlinColorSpeed = 1f;

		public void animate(float time)
		{
			//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
			if (!((Object)(object)light != (Object)null))
			{
				return;
			}
			if (animateIntensity)
			{
				float num = (loop ? Mathf.Clamp01(time % intensityDuration / intensityDuration) : Mathf.Clamp01(time / intensityDuration));
				num = (perlinIntensity ? Mathf.PerlinNoise(Time.time * perlinIntensitySpeed, 0f) : intensityCurve.Evaluate(num));
				light.intensity = Mathf.LerpUnclamped(intensityEnd, intensityStart, num);
				if (fadeIn && time < fadeInDuration)
				{
					Light obj = light;
					obj.intensity *= Mathf.Clamp01(time / fadeInDuration);
				}
			}
			if (animateRange)
			{
				float num2 = (loop ? Mathf.Clamp01(time % rangeDuration / rangeDuration) : Mathf.Clamp01(time / rangeDuration));
				num2 = (perlinRange ? Mathf.PerlinNoise(Time.time * perlinRangeSpeed, 10f) : rangeCurve.Evaluate(num2));
				light.range = Mathf.LerpUnclamped(rangeEnd, rangeStart, num2);
			}
			if (animateColor)
			{
				float num3 = (loop ? Mathf.Clamp01(time % colorDuration / colorDuration) : Mathf.Clamp01(time / colorDuration));
				num3 = (perlinColor ? Mathf.PerlinNoise(Time.time * perlinColorSpeed, 0f) : colorCurve.Evaluate(num3));
				light.color = colorGradient.Evaluate(num3);
			}
		}

		public void animateFadeOut(float time)
		{
			if (fadeOut && (Object)(object)light != (Object)null)
			{
				Light obj = light;
				obj.intensity *= 1f - Mathf.Clamp01(time / fadeOutDuration);
			}
		}

		public void reset()
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)light != (Object)null)
			{
				if (animateIntensity)
				{
					light.intensity = ((fadeIn || fadeOut) ? 0f : intensityEnd);
				}
				if (animateRange)
				{
					light.range = rangeEnd;
				}
				if (animateColor)
				{
					light.color = colorGradient.Evaluate(1f);
				}
			}
		}
	}

	private const float GLOBAL_CAMERA_SHAKE_MULTIPLIER = 1f;

	public static bool GlobalDisableCameraShake;

	public static bool GlobalDisableLights;

	[Tooltip("Defines an action to execute when the Particle System has completely finished playing and emitting particles.")]
	[SerializeField]
	public ClearBehavior clearBehavior = ClearBehavior.Destroy;

	[Space]
	public CameraShake cameraShake;

	[Space]
	[SerializeField]
	public List<AnimatedLight> animatedLights = new List<AnimatedLight>();

	[Tooltip("Defines which Particle System to track to trigger light fading out.\nLeave empty if not using fading out.")]
	[SerializeField]
	public ParticleSystem fadeOutReference;

	private float time;

	private ParticleSystem rootParticleSystem;

	[NonSerialized]
	private MaterialPropertyBlock materialPropertyBlock;

	[NonSerialized]
	private Renderer particleRenderer;

	private const int CHECK_EVERY_N_FRAME = 20;

	private static int GlobalStartFrameOffset;

	private int startFrameOffset;

	private bool isFadingOut;

	private float fadingOutStartTime;

	public void ResetState()
	{
		time = 0f;
		fadingOutStartTime = 0f;
		isFadingOut = false;
		if (animatedLights != null)
		{
			foreach (AnimatedLight animatedLight in animatedLights)
			{
				animatedLight.reset();
			}
		}
		if (cameraShake != null && cameraShake.enabled)
		{
			cameraShake.StopShake();
		}
	}

	private void Awake()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		if (animatedLights == null)
		{
			animatedLights = new List<AnimatedLight>();
		}
		if (cameraShake != null && cameraShake.enabled)
		{
			cameraShake.fetchCameras();
		}
		startFrameOffset = GlobalStartFrameOffset++;
		particleRenderer = (Renderer)(object)((Component)this).GetComponent<ParticleSystemRenderer>();
		if ((Object)(object)particleRenderer.sharedMaterial != (Object)null && particleRenderer.sharedMaterial.IsKeywordEnabled("_CFXR_LIGHTING_WPOS_OFFSET"))
		{
			materialPropertyBlock = new MaterialPropertyBlock();
		}
	}

	private void OnEnable()
	{
		foreach (AnimatedLight animatedLight in animatedLights)
		{
			if ((Object)(object)animatedLight.light != (Object)null)
			{
				((Behaviour)animatedLight.light).enabled = !GlobalDisableLights;
			}
		}
	}

	private void OnDisable()
	{
		ResetState();
	}

	private void Update()
	{
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		time += Time.deltaTime;
		Animate(time);
		if ((Object)(object)fadeOutReference != (Object)null && !fadeOutReference.isEmitting && (fadeOutReference.isPlaying || isFadingOut))
		{
			FadeOut(time);
		}
		if (clearBehavior != ClearBehavior.None)
		{
			if ((Object)(object)rootParticleSystem == (Object)null)
			{
				rootParticleSystem = ((Component)this).GetComponent<ParticleSystem>();
			}
			if ((Time.renderedFrameCount + startFrameOffset) % 20 == 0 && !rootParticleSystem.IsAlive(true))
			{
				if (clearBehavior == ClearBehavior.Destroy)
				{
					Object.Destroy((Object)(object)((Component)this).gameObject);
				}
				else
				{
					((Component)this).gameObject.SetActive(false);
				}
			}
		}
		if (materialPropertyBlock != null)
		{
			particleRenderer.GetPropertyBlock(materialPropertyBlock);
			materialPropertyBlock.SetVector("_GameObjectWorldPosition", (Vector4)((Component)this).transform.position);
			particleRenderer.SetPropertyBlock(materialPropertyBlock);
		}
	}

	public void Animate(float time)
	{
		if (animatedLights != null && !GlobalDisableLights)
		{
			foreach (AnimatedLight animatedLight in animatedLights)
			{
				animatedLight.animate(time);
			}
		}
		if (cameraShake != null && cameraShake.enabled && !GlobalDisableCameraShake)
		{
			cameraShake.animate(time);
		}
	}

	public void FadeOut(float time)
	{
		if (animatedLights == null)
		{
			return;
		}
		if (!isFadingOut)
		{
			isFadingOut = true;
			fadingOutStartTime = time;
		}
		foreach (AnimatedLight animatedLight in animatedLights)
		{
			animatedLight.animateFadeOut(time - fadingOutStartTime);
		}
	}
}
