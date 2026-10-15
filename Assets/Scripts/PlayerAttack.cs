using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
	[SerializeField] private GameObject _projectileTemplate;
	[SerializeField] private Transform _projectileSocket;
	[SerializeField, Min(0F)] private float _attackCooldown = 1.0F;

	private bool _triggerPulled;
	private float _fireTimer;

	private void Awake()
	{
		if (_projectileTemplate == null)
		{
			Debug.LogError("Projectile Template is not assigned.", this);
		}

		if (_projectileSocket == null)
		{
			Debug.LogError("Projectile Socket is not assigned.", this);
		}
	}

	private void Update()
	{
		if (_fireTimer > 0.0F)
		{
			_fireTimer -= Time.deltaTime;
		}

		if (_fireTimer <= 0.0F && _triggerPulled)
		{
			FireProjectile();
		}

		_triggerPulled = false;
	}

	private void FireProjectile()
	{
		if (_projectileTemplate == null || _projectileSocket == null)
		{
			return;
		}

		Instantiate(_projectileTemplate, _projectileSocket.position, _projectileSocket.rotation);
		_fireTimer += _attackCooldown;
	}

	public void Fire()
	{
		_triggerPulled = true;
	}
}
