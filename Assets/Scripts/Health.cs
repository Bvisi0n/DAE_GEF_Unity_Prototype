using System;
using UnityEngine;

namespace Assets.Scripts
{
	public sealed class Health : MonoBehaviour
	{
		[SerializeField, Min(1)] int _maxHealth = 10;

		private int _currentHealth;
		private bool _canHealthChange = true;


		public int CurrentHealth
		{
			get { return _currentHealth; }
		}

		public bool CanHealthChange
		{
			get { return _canHealthChange; }
			set { _canHealthChange = value; }
		}

		public event Action Damaged;
		private void Start()
		{
			_currentHealth = _maxHealth;
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
			if (_canHealthChange)
			{
				int previousHealth = _currentHealth;
				_currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
				int healthLost = previousHealth - _currentHealth;

				if (healthLost > 0)
				{
					Damaged?.Invoke();
				}
			}

			Debug.Log($"Called the method ChangeHealth({amount}), _canTakeDamage is {_canHealthChange} and current health is now {_currentHealth}.", this);
		}

		/// <summary>Instruct the GameObject to self-destruct.</summary>
		/// <remarks>Bypasses CanTakeDamage flag.</remarks>
		public void Die()
		{
			Destroy(gameObject);
		}
	}
}
