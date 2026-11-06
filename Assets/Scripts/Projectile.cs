using UnityEngine;

namespace Assets.Scripts
{
	public sealed class Projectile : MonoBehaviour
	{
		private const string k_StaticLevelLayer = "StaticLevel";
		private const string k_GroundLayer = "Ground";
		private const string k_EnemyTag = "Enemy";

		[SerializeField, Min(0F)] private float _speed = 20F;
		[SerializeField, Min(0F)] private float _lifeTime = 10.0F;
		[SerializeField, Min(0F)] private int _damage = 1;
		[SerializeField, Tooltip("Collisions with these layers will destroy this object.\nThese should be the ground, walls, etc... Not things it should deal damage to.")] private LayerMask _selfDestructLayers;

		private void Start()
		{
			Destroy(gameObject, _lifeTime);
		}

		private void FixedUpdate()
		{
			if (!IsTouchingStaticObject())
			{
				transform.position += _speed * Time.fixedDeltaTime * transform.forward;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag(k_EnemyTag) || CompareTag(other.tag))
			{
				return;
			}

			Health otherHealth = other.GetComponent<Health>();

			if (otherHealth != null)
			{
				otherHealth.ChangeHealth(-_damage);
				Destroy(gameObject);
			}
		}

		/// <remarks>
		/// Should only be called from FixedUpdate().
		/// </remarks>
		private bool IsTouchingStaticObject()
		{
			Ray collisionRay = new(transform.position, transform.forward);

			if (Physics.Raycast(collisionRay, Time.fixedDeltaTime * _speed, _selfDestructLayers))
			{
				Destroy(gameObject);
				return true;
			}

			return false;
		}
	}
}
