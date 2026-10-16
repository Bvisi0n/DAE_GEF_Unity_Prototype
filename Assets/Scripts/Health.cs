using UnityEngine;

public class Health : MonoBehaviour
{
	[SerializeField, Min(1)] int _maxHealth = 10;

	private int _currentHealth;

	public int CurrentHealth
	{
		get { return _currentHealth; }
	}

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
		_currentHealth = Mathf.Clamp(_currentHealth + amount, 0, _maxHealth);
		Debug.Log($"Called the method ChangeHealth({amount}), current health is now {_currentHealth}.", this);
	}
}
