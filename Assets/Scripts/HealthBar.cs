using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public sealed class HealthBar : MonoBehaviour
	{
		[SerializeField] private Slider _mainSlider;
		[SerializeField] private Slider _lerpSlider;
		[SerializeField, Min(0.01F)] private float _lerpSpeed = 0.05F;

		private Health _health;

		private void Awake()
		{
			if (_mainSlider == null)
			{
				Debug.LogError("No Main Slider assigned, script disabled.", this);
				enabled = false;
				return;
			}

			if (_lerpSlider == null)
			{
				Debug.LogError("No Lerp Slider assigned, script disabled.", this);
				enabled = false;
				return;
			}

			const string targetTag = "Player";
			GameObject player = GameObject.FindWithTag(targetTag);

			if (player == null)
			{
				Debug.LogError($"No GameObject with tag '{targetTag}' found, script disabled.", this);
				enabled = false;
				return;
			}

			_health = player.GetComponent<Health>();

			if (_health == null)
			{
				Debug.LogError("Player object doesn't have a Health component, script disabled.", this);
				enabled = false;
				return;
			}
		}

		private void OnEnable()
		{
			if (_health != null)
			{
				_health.HealthChanged += OnHealthChanged;
			}
		}

		private void Start()
		{
			if (_health == null || _mainSlider == null || _lerpSlider == null)
			{
				return;
			}

			_mainSlider.maxValue = _health.MaxHealth;
			_lerpSlider.maxValue = _health.MaxHealth;
			_mainSlider.value = _health.CurrentHealth;
			_lerpSlider.value = _health.CurrentHealth;
		}

		private void Update()
		{
			if (_mainSlider == null || _lerpSlider == null)
			{
				return;
			}

			if (_mainSlider.value != _lerpSlider.value)
			{
				_lerpSlider.value = Mathf.Lerp(_lerpSlider.value, _mainSlider.value, _lerpSpeed);
			}
		}

		private void OnDisable()
		{
			if (_health != null)
			{
				_health.HealthChanged -= OnHealthChanged;
			}
		}

		private void OnHealthChanged(int newHealth)
		{
			if (_mainSlider == null)
			{
				return;
			}

			_mainSlider.value = newHealth;
		}
	}
}
