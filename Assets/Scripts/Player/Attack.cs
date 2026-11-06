using UnityEngine;

namespace Assets.Scripts.Player
{
	public sealed class Attack : MonoBehaviour
	{
		[SerializeField] private GameObject _projectileTemplate;
		[SerializeField] private Transform _projectileSocket;
		[SerializeField, Min(0F)] private float _attackCooldown = 1.0F;

		private bool _isAttacking;
		private float _attackTimer;

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
			_attackTimer -= Time.deltaTime;

			if (_attackTimer <= 0.0F && _isAttacking)
			{
				FireProjectile();
			}

			_isAttacking = false;
		}

		private void FireProjectile()
		{
			if (_projectileTemplate == null || _projectileSocket == null)
			{
				return;
			}

			_attackTimer = _attackCooldown;
			Instantiate(_projectileTemplate, _projectileSocket.position, _projectileSocket.rotation);
		}

		public void TryAttack()
		{
			_isAttacking = true;
		}
	}
}
