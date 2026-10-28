using UnityEngine;

public sealed class ProjectileLogic : MonoBehaviour
{
	private const string k_StaticLevelLayer = "StaticLevel";
	private const string k_GroundLayer = "Ground";
	private const string k_EnemyTag = "Enemy";

	[SerializeField, Min(0F)] private float _speed = 20F;
	[SerializeField, Min(0F)] private float _lifeTime = 10.0F;
	[SerializeField, Min(0F)] private int _damage = 1;

	private int _layerMask;

	private void Awake()
	{
		_layerMask = LayerMask.GetMask(new string[] { k_StaticLevelLayer, k_GroundLayer });

		if (_layerMask == 0)
		{
			Debug.LogError($"Layer '{k_StaticLevelLayer}' or '{k_GroundLayer}' does not exist or is not configured.", this);
		}
	}

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

		if (Physics.Raycast(collisionRay, Time.fixedDeltaTime * _speed, _layerMask))
		{
			Destroy(gameObject);
			return true;
		}

		return false;
	}
}
