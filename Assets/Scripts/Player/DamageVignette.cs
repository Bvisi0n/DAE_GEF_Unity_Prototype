using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Assets.Scripts.Player
{
	[RequireComponent(typeof(Health))]
	public sealed class DamageVignette : MonoBehaviour
	{
		[SerializeField, Tooltip("This should be an object from the scene hiearchy.")] private Volume _volume;
		[SerializeField] private Color _color = Color.crimson;
		[SerializeField, Range(0f, 1f)] private float _peakIntensity = 0.333f;
		[SerializeField, Min(0.01f)] private float _fadeOutDuration = 0.6f;

		private Vignette _vignette;
		private Health _health;
		private float _remainingFadeTime;

		private void Awake()
		{
			if (_volume == null)
			{
				Debug.LogError("No Volume assigned, script disabled.", this);
				enabled = false;
				return;
			}

			VolumeProfile profile = _volume.profile;

			if (!profile.TryGet(out _vignette))
			{
				Debug.LogError("No Vignette override found on volume, script disabled.", this);
				enabled = false;
				return;
			}

			_vignette.intensity.overrideState = true;
			_vignette.intensity.value = 0F;
			_health = GetComponent<Health>();
		}

		private void OnEnable()
		{
			_health.Damaged += OnDamaged;
		}

		private void Update()
		{
			if (_remainingFadeTime <= 0F)
			{
				return;
			}

			// Fade out the effect.
			_remainingFadeTime -= Time.deltaTime;
			float normalizedTime = Mathf.Clamp01(_remainingFadeTime / _fadeOutDuration);
			_vignette.intensity.value = _peakIntensity * normalizedTime;
		}

		private void OnDisable()
		{
			_health.Damaged -= OnDamaged;
		}

		private void OnDamaged(int damageTaken)
		{
			// Start the effect.
			_vignette.color.value = _color;
			_vignette.intensity.value = _peakIntensity;
			_remainingFadeTime = _fadeOutDuration;
		}
	}
}