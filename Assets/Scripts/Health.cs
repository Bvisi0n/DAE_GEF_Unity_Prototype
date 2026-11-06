using System;
using UnityEngine;

namespace Assets.Scripts
{
	public sealed class Health : MonoBehaviour
	{
		[SerializeField, Min(1)] int _maxHealth = 10;

		private int _currentHealth;

		public bool CanHealthChange { get; set; }
		public int CurrentHealth => _currentHealth;
		public int MaxHealth => _maxHealth;

		public event Action Damaged; // Used by player feedback that's only interested in damage taken.
		public event Action<int> HealthChanged; // Used by UI

		private void Start()
		{
			CanHealthChange = true;
			_currentHealth = _maxHealth;
			HealthChanged?.Invoke(_currentHealth);
		}

		private void Update()
		{
			if (_currentHealth <= 0)
			{
				Destroy(gameObject);
			}
		}

		/// <remarks>Pass negative values to decrease current health.</remarks>
		public void ChangeHealth(int amount)
		{
			if (CanHealthChange)
			{
				int previousHealth = _currentHealth;
				_currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
				HealthChanged?.Invoke(_currentHealth);
				int healthLost = previousHealth - _currentHealth;

				if (healthLost > 0)
				{
					Damaged?.Invoke();
				}
			}

			Debug.Log($"Called the method ChangeHealth({amount}), _canTakeDamage is {CanHealthChange} and current health is now {_currentHealth}.", this);
		}

		public void SetMaxHealth(int newHealth)
		{
			_maxHealth = newHealth;
			_currentHealth = newHealth;
		}

		/// <summary>Instruct the GameObject to self-destruct.</summary>
		/// <remarks>Bypasses CanTakeDamage flag.</remarks>
		public void Die()
		{
			Destroy(gameObject);
		}
	}
}
