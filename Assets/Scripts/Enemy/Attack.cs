using UnityEngine;

namespace Assets.Scripts.Enemy
{
	public sealed class Attack : MonoBehaviour
	{
		[SerializeField, Min(0)] private int _damage = 1;
		[SerializeField, Min(0F)] private float _cooldown = 1F;

		private float _attackTimer;

		public void SetStats(int damage, float cooldown)
		{
			_damage = damage;
			_cooldown = cooldown;
		}

		public void TryAttack(Transform target)
		{
			if (target == null)
			{
				return;
			}

			_attackTimer -= Time.deltaTime;

			if (_attackTimer > 0F)
			{
				return;
			}

			_attackTimer = _cooldown;

			if (!target.TryGetComponent<Health>(out var targetHealth))
			{
				Debug.LogWarning("Target has no Health component.", this);
				return;
			}

			targetHealth.ChangeHealth(-_damage);
		}
	}
}
